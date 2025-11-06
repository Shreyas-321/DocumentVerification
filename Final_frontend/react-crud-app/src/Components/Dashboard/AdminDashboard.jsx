import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../Services/AuthService';
import userService from '../../Services/UserService';
import documentService from '../../Services/DocumentService';
import auditLogsService from '../../Services/AuditLogsService';
import Card from '../Common/Card';
import Button from '../Common/Button';
import Navigation from '../Layout/Navigation';
import VerificationResultsModal from './VerificationResultsModal';

const AdminDashboard = () => {
  const { logout } = useAuth();
  const navigate = useNavigate();
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState('');
  const [selectedUser, setSelectedUser] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [search, setSearch] = useState('');
  const [documentSearch, setDocumentSearch] = useState('');
  const [editForm, setEditForm] = useState({
    fullName: '',
    email: '',
    phone: '',
    city: '',
    state: '',
    pincode: '',
    role: 'User'
  });

   const handleGetGeoLocation = async (id) => {
    try {
      const data = await documentService.geolocation(id);
      const { latitude, longitude } = data;

      // 👉 Open Google Maps
      const mapsUrl = `https://www.google.com/maps?q=${latitude},${longitude}`;
      window.open(mapsUrl, "_blank");

      // 👉 Or if you prefer OpenStreetMap, use:
      // const osmUrl = `https://www.openstreetmap.org/?mlat=${latitude}&mlon=${longitude}#map=18/${latitude}/${longitude}`;
      // window.open(osmUrl, "_blank");
    } catch (err) {
      alert(`❌ Failed to get location: Land Record Not Found`);
      console.log(`❌ Failed to get location: ${err}`);
    }
  };
//   const handleGetGeoLocation = async (applicationNumber) => {
//   try {
//     const data = await documentService.geolocationByApplication(applicationNumber);
//     const { latitude, longitude } = data;

//     const mapsUrl = `https://www.google.com/maps?q=${latitude},${longitude}`;
//     window.open(mapsUrl, "_blank");
//   } catch (err) {
//     alert(`❌ Failed to get location: ${err}`);
//   }
// };
  // New states for document management
  const [documents, setDocuments] = useState([]);
  const [documentsLoading, setDocumentsLoading] = useState(false);
  const [analytics, setAnalytics] = useState(null);
  const [activityLogs, setActivityLogs] = useState([]);
  const [activeTab, setActiveTab] = useState('users');
  const [selectedDocument, setSelectedDocument] = useState(null);
  const [extractedData, setExtractedData] = useState(null);
  const [showExtractedDataModal, setShowExtractedDataModal] = useState(false);

  // Audit logs states
  const [auditLogs, setAuditLogs] = useState([]);
  const [auditLogsLoading, setAuditLogsLoading] = useState(false);

  // User documents states
  const [userDocuments, setUserDocuments] = useState([]);
  const [showUserDocumentsModal, setShowUserDocumentsModal] = useState(false);
  const [selectedUserForDocuments, setSelectedUserForDocuments] = useState(null);

  // Verification results modal state
  const [showVerificationResultsModal, setShowVerificationResultsModal] = useState(false);
  const [selectedDocumentForVerification, setSelectedDocumentForVerification] = useState(null);

  useEffect(() => {
    fetchUsers();
    fetchAnalytics();
    fetchDocuments();
    fetchActivityLogs();
    fetchAuditLogs();
  }, []);



  const fetchUsers = async () => {
    try {
      setLoading(true);
      const usersData = await userService.getAllUsers();
      setUsers(usersData);
    } catch (error) {
      console.error('Error fetching users:', error);
      setMessage('Failed to load users. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const fetchDocuments = async (forceRefresh = false) => {
    // Prevent multiple simultaneous calls
    if (documentsLoading && !forceRefresh) {
      return;
    }

    try {
      setDocumentsLoading(true);
      const documentsData = await documentService.getAllDocuments();
      setDocuments(documentsData || []);
    } catch (error) {
      console.error('Failed to load documents:', error);
      setMessage('Failed to load documents. Please try again.');
    } finally {
      setDocumentsLoading(false);
    }
  };

  const fetchAnalytics = async () => {
    try {
      const analyticsData = await documentService.getAnalytics();
      setAnalytics(analyticsData);
    } catch (error) {
      // Analytics loading failed silently
    }
  };

  const fetchActivityLogs = async () => {
    try {
      // Fetch user document activities specifically
      const response = await auditLogsService.getAllAuditLogs(1, 50);
      // Filter for document-related activities only
      const documentActivities = (response.logs || []).filter(log =>
        log.activity && (
          log.activity.toLowerCase().includes('document') ||
          log.activity.toLowerCase().includes('upload') ||
          log.activity.toLowerCase().includes('verification') ||
          log.activity.toLowerCase().includes('extract') ||
          log.activity.toLowerCase().includes('process')
        )
      );
      setActivityLogs(documentActivities);
    } catch (error) {
      // Activity logs loading failed silently
    }
  };

  const fetchAuditLogs = async () => {
    try {
      setAuditLogsLoading(true);
      const response = await auditLogsService.getAllAuditLogs(1, 20); // Get more audit logs
      // Show all system events (no filtering)
      setAuditLogs(response.logs || []);
    } catch (error) {
      // Audit logs loading failed silently
    } finally {
      setAuditLogsLoading(false);
    }
  };

  // const handleTriggerVerification = async (documentId) => {
  //   try {
  //     setMessage('Verifying documents...');
  //     const result = await documentService.triggerVerification(documentId);
  //     setMessage(`Verification completed! Status: ${result.isVerified ? 'VERIFIED' : 'FAILED'}.`);
  //     fetchDocuments(true);
  //   } catch (error) {
  //     console.error('Verification error:', error);
  //     setMessage('Failed to trigger verification: ' + (error.response?.data?.message || error.message));
  //   }
  // };

  const handleTriggerVerification = async (documentId) => {
  try {
    setMessage('Verifying documents...');
    const result = await documentService.triggerVerification(documentId);
    setMessage(`Verification completed!`);

    // ✅ Force immutable refresh
    setDocuments((prev) =>
      prev.map((d) =>
        d.id === documentId
          ? {
              ...d,
              verificationStatus: result.verificationStatus || 'Verified',
              riskScore: result.riskScore ?? 0,
            }
          : d
      )
    );
  } catch (error) {
    console.error('Verification error:', error);
    setMessage('Failed to trigger verification');
  }
};


  // Removed handleAdminManualVerification - now using single verification method

  const [extractingDocuments, setExtractingDocuments] = useState(new Set());

  const handleExtractDocuments = async (documentId) => {
    try {
      setExtractingDocuments(prev => new Set(prev).add(documentId));
      setMessage('Extracting documents...');
      const result = await documentService.extractDocumentsAdmin(documentId);
      setMessage(`Extraction completed! ${result.message}`);
      fetchDocuments(true); // Force refresh the documents list
    } catch (error) {
      console.error('Error extracting documents:', error);
      setMessage('Failed to extract documents');
    } finally {
      setExtractingDocuments(prev => {
        const newSet = new Set(prev);
        newSet.delete(documentId);
        return newSet;
      });
    }
  };

  const handleViewExtractedData = async (documentId) => {
    try {
      const data = await documentService.getDocumentExtractedData(documentId);
      setExtractedData(data);
      setShowExtractedDataModal(true);
    } catch (error) {
      console.error('Error fetching extracted data:', error);
      setMessage('Failed to fetch extracted data');
    }
  };

  const handleViewUserDocuments = async (user) => {
    try {
      const data = await documentService.getUserDocuments(user.id);
      setUserDocuments(data.documents || []);
      setSelectedUserForDocuments(data.user);
      setShowUserDocumentsModal(true);
    } catch (error) {
      console.error('Error fetching user documents:', error);
      setMessage('Failed to fetch user documents');
    }
  };

  const handleDownloadDocument = async (documentId, documentType) => {
    try {
      const blob = await documentService.downloadDocument(documentId, documentType);
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `${documentType}_${documentId}.pdf`;
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      document.body.removeChild(a);
    } catch (error) {
      setMessage('Failed to download document');
    }
  };


  const handleViewVerificationResults = (documentId) => {
    setSelectedDocumentForVerification(documentId);
    setShowVerificationResultsModal(true);
  };

  const handleDeleteUser = async (userId) => {
    if (window.confirm('Are you sure you want to delete this user? This action cannot be undone.')) {
      try {
        await userService.deleteUser(userId);
        setMessage('User deleted successfully.');
        fetchUsers(); // Refresh the list
      } catch (error) {
        console.error('Delete user error:', error);
        setMessage('Failed to delete user. Please try again.');
      }
    }
  };

  const handleViewUser = (user) => {
    setSelectedUser(user);
    setShowModal(true);
  };

  const handleEditUser = (user) => {
    setSelectedUser(user);
    setEditForm({
      fullName: user.fullName || '',
      email: user.email || '',
      phone: user.phone || '',
      city: user.city || '',
      state: user.state || '',
      pincode: user.pincode || '',
      role: user.role || 'User'
    });
    setShowEditModal(true);
  };

  const handleEditFormChange = (e) => {
    const { name, value } = e.target;
    setEditForm(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleUpdateUser = async (e) => {
    e.preventDefault();
    try {
      await userService.updateUser(selectedUser.id, editForm);
      setMessage('User updated successfully.');
      setShowEditModal(false);
      fetchUsers(); // Refresh the list
    } catch (error) {
      console.error('Update user error:', error);
      setMessage('Failed to update user. Please try again.');
    }
  };

  const getInitials = (name) => {
    return name
      .split(' ')
      .map(word => word.charAt(0))
      .join('')
      .toUpperCase()
      .slice(0, 2);
  };

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  // 🔹 Filter users based on search input
  const filteredUsers = users.filter(user =>
    user.fullName?.toLowerCase().includes(search.toLowerCase()) ||
    user.email?.toLowerCase().includes(search.toLowerCase())
  );

  // 🔹 Filter documents based on search input
  const filteredDocuments = documents.filter(doc =>
    doc.userName?.toLowerCase().includes(documentSearch.toLowerCase()) ||
    doc.userEmail?.toLowerCase().includes(documentSearch.toLowerCase())
  );

  if (loading) {
    return (
      <div className="row justify-content-center">
        <div className="col-md-8">
          <Card className="mt-5 text-center">
            <div className="card-body">
              <div className="spinner-border text-primary" role="status">
                <span className="visually-hidden">Loading...</span>
              </div>
              <p className="mt-3">Loading users...</p>
            </div>
          </Card>
        </div>
      </div>
    );
  }

  return (
    <div className="container-fluid py-4">
      <div className="row">
        <div className="col-12">
          <div className="d-flex justify-content-between align-items-center mb-4">
            <h1 className="h2 mb-0">Admin Dashboard</h1>
            <div className="d-flex gap-2">
              <Button variant="primary" onClick={fetchUsers}>
                Refresh Users
              </Button>
              <Button
                variant="outline-danger"
                onClick={() => {
                  logout();
                  navigate('/');
                }}
              >
                <span className="me-1">🚪</span>
                Sign Out
              </Button>
            </div>
          </div>
        </div>
      </div>

      {/* 🔹 Enhanced Search bar */}
      <div className="row mb-4">
        <div className="col-12">
          <div className="position-relative">
            <div className="input-group">
              <span className="input-group-text bg-primary text-white">
                🔍
              </span>
              <input
                type="text"
                className="form-control form-control-lg"
                placeholder="Search users by name or email..."
                value={search}
                onChange={e => setSearch(e.target.value)}
                style={{
                  borderLeft: 'none',
                  boxShadow: '0 2px 10px rgba(0,0,0,0.1)',
                  transition: 'all 0.3s ease'
                }}
              />
              {search && (
                <button
                  className="btn btn-outline-secondary"
                  onClick={() => setSearch('')}
                  style={{ borderLeft: 'none' }}
                >
                  ✕
                </button>
              )}
            </div>
            {search && (
              <div className="position-absolute top-100 start-0 w-100 mt-1">
                <div className="bg-white border rounded shadow-sm p-2">
                  <small className="text-muted">
                    Found {filteredUsers.length} user{filteredUsers.length !== 1 ? 's' : ''} matching "{search}"
                  </small>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>

      {message && (
        <div className="row">
          <div className="col-12">
            <div className={`alert ${message.includes('successfully') ? 'alert-success' : 'alert-warning'}`} role="alert">
              {message}
            </div>
          </div>
        </div>
      )}

      {/* Navigation Tabs */}
      <div className="row mb-4">
        <div className="col-12">
          <ul className="nav nav-tabs">
            <li className="nav-item">
              <button
                className={`nav-link ${activeTab === 'users' ? 'active' : ''}`}
                onClick={() => setActiveTab('users')}
              >
                Users Management
              </button>
            </li>
            <li className="nav-item">
              <button
                className={`nav-link ${activeTab === 'documents' ? 'active' : ''}`}
                onClick={() => setActiveTab('documents')}
              >
                Document Monitoring
              </button>
            </li>
            <li className="nav-item">
              <button
                className={`nav-link ${activeTab === 'analytics' ? 'active' : ''}`}
                onClick={() => setActiveTab('analytics')}
              >
                Analytics & Reports
              </button>
            </li>
            <li className="nav-item">
              <button
                className={`nav-link ${activeTab === 'activity' ? 'active' : ''}`}
                onClick={() => setActiveTab('activity')}
              >
                Document Activities
              </button>
            </li>
            <li className="nav-item">
              <button
                className={`nav-link ${activeTab === 'audit' ? 'active' : ''}`}
                onClick={() => setActiveTab('audit')}
              >
                System Audit Logs
              </button>
            </li>
          </ul>
        </div>
      </div>

      {/* Analytics Dashboard */}
      {activeTab === 'analytics' && analytics && (
        <div>
          {/* Main Statistics Row */}
          <div className="row mb-4">
            <div className="col-md-3">
              <Card title="Total Users" className="text-center">
                <h3 className="text-primary">{analytics.userStats?.totalUsers || 0}</h3>
                <small className="text-muted">Regular: {analytics.userStats?.totalRegularUsers || 0} | Admin: {analytics.userStats?.totalAdmins || 0}</small>
              </Card>
            </div>
            <div className="col-md-3">
              <Card title="Document Uploads" className="text-center">
                <h3 className="text-info">{analytics.documentStats?.totalUploads || 0}</h3>
                <small className="text-muted">Total Documents Submitted</small>
              </Card>
            </div>
            <div className="col-md-3">
              <Card title="Verification Status" className="text-center">
                <h3 className="text-success">{analytics.documentStats?.verifiedUploads || 0}</h3>
                <small className="text-muted">Verified Documents</small>
              </Card>
            </div>
            <div className="col-md-3">
              <Card title="Pending Review" className="text-center">
                <h3 className="text-warning">{analytics.documentStats?.pendingVerification || 0}</h3>
                <small className="text-muted">Awaiting Verification</small>
              </Card>
            </div>
          </div>

          {/* Status Distribution Row */}
          <div className="row mb-4">
            <div className="col-md-4">
              <Card title="Verification Status Distribution" className="text-center">
                <div className="row">
                  <div className="col-4">
                    <div className="text-success">
                      <h4>{analytics.verificationStats?.verifiedCount || 0}</h4>
                      <small>Verified</small>
                    </div>
                  </div>
                  <div className="col-4">
                    <div className="text-warning">
                      <h4>{analytics.verificationStats?.pendingCount || 0}</h4>
                      <small>Pending</small>
                    </div>
                  </div>
                  <div className="col-4">
                    <div className="text-danger">
                      <h4>{analytics.verificationStats?.rejectedCount || 0}</h4>
                      <small>Rejected</small>
                    </div>
                  </div>
                </div>
              </Card>
            </div>
            <div className="col-md-4">
              <Card title="Risk Score Distribution" className="text-center">
                <div className="row">
                  <div className="col-4">
                    <div className="text-danger">
                      <h4>{analytics.riskDistribution?.highRisk || 0}</h4>
                      <small>High Risk</small>
                    </div>
                  </div>
                  <div className="col-4">
                    <div className="text-warning">
                      <h4>{analytics.riskDistribution?.mediumRisk || 0}</h4>
                      <small>Medium Risk</small>
                    </div>
                  </div>
                  <div className="col-4">
                    <div className="text-success">
                      <h4>{analytics.riskDistribution?.lowRisk || 0}</h4>
                      <small>Low Risk</small>
                    </div>
                  </div>
                </div>
              </Card>
            </div>
            <div className="col-md-4">
              <Card title="Recent Activity (30 Days)" className="text-center">
                <div className="row">
                  <div className="col-6">
                    <div className="text-info">
                      <h4>{analytics.recentActivity?.recentUploads || 0}</h4>
                      <small>New Uploads</small>
                    </div>
                  </div>
                  <div className="col-6">
                    <div className="text-success">
                      <h4>{analytics.recentActivity?.recentVerifications || 0}</h4>
                      <small>Verifications</small>
                    </div>
                  </div>
                </div>
              </Card>
            </div>
          </div>

          {/* Summary Statistics */}
          <div className="row">
            <div className="col-12">
              <Card title="System Summary">
                <div className="row text-center">
                  <div className="col-md-3">
                    <h5 className="text-primary">Total System Users</h5>
                    <p className="h3">{analytics.userStats?.totalUsers || 0}</p>
                  </div>
                  <div className="col-md-3">
                    <h5 className="text-info">Total Documents</h5>
                    <p className="h3">{analytics.documentStats?.totalUploads || 0}</p>
                  </div>
                  <div className="col-md-3">
                    <h5 className="text-success">Verification Rate</h5>
                    <p className="h3">
                      {analytics.documentStats?.totalUploads > 0
                        ? Math.round((analytics.documentStats?.verifiedUploads / analytics.documentStats?.totalUploads) * 100)
                        : 0}%
                    </p>
                  </div>
                  <div className="col-md-3">
                    <h5 className="text-warning">Pending Rate</h5>
                    <p className="h3">
                      {analytics.documentStats?.totalUploads > 0
                        ? Math.round((analytics.documentStats?.pendingVerification / analytics.documentStats?.totalUploads) * 100)
                        : 0}%
                    </p>
                  </div>
                </div>
              </Card>
            </div>
          </div>
        </div>
      )}

      {/* Document Monitoring */}
      {activeTab === 'documents' && (
        <div className="row mb-4">
          <div className="col-12">
            <Card title="Document Monitoring" className="dashboard-card">
              <div className="card-header d-flex justify-content-between align-items-center">
                <h6 className="mb-0">User Document Status & Actions</h6>
                <Button
                  variant="outline-primary"
                  size="sm"
                  onClick={() => fetchDocuments(true)}
                  disabled={documentsLoading}
                >
                  {documentsLoading ? (
                    <>
                      <span className="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span>
                      Loading...
                    </>
                  ) : (
                    'Refresh Documents'
                  )}
                </Button>
              </div>

              {/* 🔹 Enhanced Document Search bar */}
              <div className="p-3 border-bottom">
                <div className="position-relative">
                  <div className="input-group">
                    <span className="input-group-text bg-info text-white">
                      🔍
                    </span>
                    <input
                      type="text"
                      className="form-control"
                      placeholder="Search documents by user name or email..."
                      value={documentSearch}
                      onChange={e => setDocumentSearch(e.target.value)}
                      style={{
                        borderLeft: 'none',
                        boxShadow: '0 2px 10px rgba(0,0,0,0.1)',
                        transition: 'all 0.3s ease'
                      }}
                    />
                    {documentSearch && (
                      <button
                        className="btn btn-outline-secondary"
                        onClick={() => setDocumentSearch('')}
                        style={{ borderLeft: 'none' }}
                      >
                        ✕
                      </button>
                    )}
                  </div>
                  {documentSearch && (
                    <div className="position-absolute top-100 start-0 w-100 mt-1">
                      <div className="bg-white border rounded shadow-sm p-2">
                        <small className="text-muted">
                          Found {filteredDocuments.length} document{filteredDocuments.length !== 1 ? 's' : ''} matching "{documentSearch}"
                        </small>
                      </div>
                    </div>
                  )}
                </div>
              </div>

              <div className="table-responsive">
                <table className="table table-hover">
                  <thead className="table-dark">
                    <tr>
                      <th>User</th>
                      <th>Email</th>
                      <th>Documents Uploaded</th>
                      <th>Status</th>
                      <th>Risk Score</th>
                      <th>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {documentsLoading ? (
                      <tr>
                        <td colSpan="6" className="text-center py-4">
                          <div className="d-flex justify-content-center align-items-center">
                            <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                            <span>Loading documents...</span>
                          </div>
                        </td>
                      </tr>
                    ) : filteredDocuments.length === 0 ? (
                      <tr>
                        <td colSpan="6" className="text-center py-4">
                          <p className="text-muted">
                            {documents.length === 0 ? 'No documents found.' : 'No documents match your search.'}
                          </p>
                          {documents.length === 0 ? (
                            <Button variant="primary" size="sm" onClick={() => fetchDocuments(true)}>
                              Load Documents
                            </Button>
                          ) : (
                            <Button variant="outline-secondary" size="sm" onClick={() => setDocumentSearch('')}>
                              Clear Search
                            </Button>
                          )}
                        </td>
                      </tr>
                    ) : (
                      filteredDocuments.map((doc) => (
                        <tr key={doc.id}>
                          <td>
                            <div>
                              <strong>{doc.userName || 'Unknown User'}</strong>
                              <br />
                              <small className="text-muted">ID: {doc.id}</small>
                            </div>
                          </td>
                          <td>
                            <span className="text-primary">{doc.userEmail || 'N/A'}</span>
                          </td>
                          <td>
                            <div className="d-flex flex-wrap gap-1">
                              <span className={`badge ${doc.ecPath ? 'bg-success' : 'bg-secondary'}`}>
                                EC {doc.ecPath ? '✓' : '✗'}
                              </span>
                              <span className={`badge ${doc.aadhaarPath ? 'bg-success' : 'bg-secondary'}`}>
                                Aadhaar {doc.aadhaarPath ? '✓' : '✗'}
                              </span>
                              <span className={`badge ${doc.panPath ? 'bg-success' : 'bg-secondary'}`}>
                                PAN {doc.panPath ? '✓' : '✗'}
                              </span>
                            </div>
                          </td>
                          <td>
                            <span className={`badge ${doc.verificationStatus === 'Verified' ? 'bg-success' :
                                doc.verificationStatus === 'Rejected' ? 'bg-danger' :
                                  'bg-warning'
                              }`}>
                              {doc.verificationStatus || 'Pending'}
                            </span>
                            {doc.isVerified && (
                              <>
                                <br />
                                <small className="text-success">✓ Verified</small>
                              </>
                            )}
                          </td>
                          <td>
                            <div className="text-center">
                              <span className={`badge ${doc.riskScore >= 80 ? 'bg-danger' :
                                  doc.riskScore >= 50 ? 'bg-warning' :
                                    'bg-success'
                                }`}>
                                {doc.riskScore ? `${doc.riskScore}%` : 'N/A'}
                              </span>
                              <br />
                              <small className="text-muted">
                                {doc.riskScore >= 80 ? 'High Risk' :
                                  doc.riskScore >= 50 ? 'Medium Risk' :
                                    'Low Risk'}
                              </small>
                            </div>
                          </td>
                          <td>
                            <div className="d-flex flex-wrap gap-1">
                              <Button
                                variant="outline-success"
                                size="sm"
                                onClick={() => handleExtractDocuments(doc.id)}
                                disabled={doc.hasExtractedData || extractingDocuments.has(doc.id)}
                                title={doc.hasExtractedData ? "Already extracted" : extractingDocuments.has(doc.id) ? "Extracting..." : "Extract data from documents"}
                              >
                                {extractingDocuments.has(doc.id) ? (
                                  <>
                                    <span className="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span>
                                    Extracting...
                                  </>
                                ) : doc.hasExtractedData ? "Extracted" : "Extract"}
                              </Button>
                              {doc.IsVerified ? (
                                <Button
                                  variant="success"
                                  size="sm"
                                  disabled
                                  title="Already verified"
                                >
                                  ✅ Verified
                                </Button>
                              ) : doc.VerificationStatus === 'Rejected' ? (
                                <Button
                                  variant="danger"
                                  size="sm"
                                  disabled
                                  title="Verification rejected"
                                >
                                  ❌ Rejected
                                </Button>
                              ) : doc.VerificationStatus === 'Under Review' ? (
                                <Button
                                  variant="warning"
                                  size="sm"
                                  disabled
                                  title="Under review"
                                >
                                  🔍 Under Review
                                </Button>
                              ) : (
                                <Button
                                  variant="outline-primary"
                                  size="sm"
                                  onClick={() => handleTriggerVerification(doc.id)}
                                  title="Verify document authenticity"
                                >
                                  🔍 Verify
                                </Button>
                              )}
                              <Button
                                variant="outline-info"
                                size="sm"
                                onClick={() => handleViewExtractedData(doc.id)}
                                title="View extracted document data"
                              >
                                View
                              </Button>
                               <Button 
                                variant="outline-info" 
                                size="sm"
                                onClick={() => handleGetGeoLocation(doc.id)}
                                style={{
                                  borderRadius: '15px',
                                  fontSize: '12px'
                                }}
                              >
                               📍 Get Location
                              </Button>
                              <Button
                                variant="outline-info"
                                size="sm"
                                onClick={() => handleViewVerificationResults(doc.id)}
                                title="View verification results"
                              >
                                📊 Results
                              </Button>
                            </div>
                          </td>
                        </tr>
                      ))
                    )}
                  </tbody>
                </table>
              </div>
              <div className="card-footer text-center">
                <small className="text-muted">
                  Showing {documents.length} document records
                </small>
              </div>
            </Card>
          </div>
        </div>
      )}

      {/* Activity Logs */}
      {activeTab === 'activity' && (
        <div className="row mb-4">
          <div className="col-12">
            <Card title="Document Activities" className="dashboard-card">
              <div className="card-header d-flex justify-content-between align-items-center">
                <h6 className="mb-0">Recent User Activities</h6>
                <Button variant="outline-primary" size="sm" onClick={fetchActivityLogs}>
                  Refresh
                </Button>
              </div>
              <div className="table-responsive">
                <table className="table table-hover">
                  <thead>
                    <tr>
                      <th>User</th>
                      <th>Activity Type</th>
                      <th>Description</th>
                      <th>Result</th>
                      <th>IP Address</th>
                      <th>Timestamp</th>
                    </tr>
                  </thead>
                  <tbody>
                    {activityLogs.length === 0 ? (
                      <tr>
                        <td colSpan="6" className="text-center py-4">
                          <p className="text-muted">No activity logs found.</p>
                          <Button variant="primary" size="sm" onClick={fetchActivityLogs}>
                            Load Activities
                          </Button>
                        </td>
                      </tr>
                    ) : (
                      activityLogs.map((log) => (
                        <tr key={log.id}>
                          <td>
                            <div>
                              <strong>{log.userName || 'Unknown'}</strong>
                              <br />
                              <small className="text-muted">{log.userEmail || 'N/A'}</small>
                            </div>
                          </td>
                          <td>
                            <span className={`badge ${auditLogsService.getActivityBadgeColor(log.activity)}`}>
                              {log.activity}
                            </span>
                          </td>
                          <td>
                            <div style={{ maxWidth: '300px' }}>
                              <div className="text-truncate" title={log.description}>
                                {log.description}
                              </div>
                              {log.relatedEntityType && (
                                <small className="text-muted">
                                  Related: {log.relatedEntityType} #{log.relatedEntityId}
                                </small>
                              )}
                            </div>
                          </td>
                          <td>
                            {log.actionResult && (
                              <span className={`badge ${auditLogsService.getActionResultBadgeColor(log.actionResult)}`}>
                                {log.actionResult}
                              </span>
                            )}
                          </td>
                          <td>
                            <small className="text-muted">{log.ipAddress || 'N/A'}</small>
                          </td>
                          <td>
                            <small>{auditLogsService.formatDateForDisplay(log.timestamp)}</small>
                          </td>
                        </tr>
                      ))
                    )}
                  </tbody>
                </table>
              </div>
              <div className="card-footer text-center">
                <small className="text-muted">
                  Showing {activityLogs.length} recent user activities
                </small>
              </div>
            </Card>
          </div>
        </div>
      )}

      {/* Audit Logs */}
      {activeTab === 'audit' && (
        <div className="row mb-4">
          <div className="col-12">
            <Card title="System Audit Logs" className="dashboard-card">
              {auditLogsLoading ? (
                <div className="text-center py-4">
                  <div className="spinner-border text-primary" role="status">
                    <span className="visually-hidden">Loading...</span>
                  </div>
                  <p className="mt-3">Loading audit logs...</p>
                </div>
              ) : auditLogs.length === 0 ? (
                <div className="text-center py-4">
                  <p className="text-muted">No audit logs found.</p>
                  <Button variant="primary" onClick={fetchAuditLogs}>
                    Refresh
                  </Button>
                </div>
              ) : (
                <div className="table-responsive">
                  <table className="table table-hover">
                    <thead>
                      <tr>
                        <th>User</th>
                        <th>Activity</th>
                        <th>Description</th>
                        <th>Result</th>
                        <th>IP Address</th>
                        <th>Timestamp</th>
                      </tr>
                    </thead>
                    <tbody>
                      {auditLogs.map((log) => (
                        <tr key={log.id}>
                          <td>
                            <div>
                              <strong>{log.userName || 'Unknown'}</strong>
                              <br />
                              <small className="text-muted">{log.userEmail || 'N/A'}</small>
                            </div>
                          </td>
                          <td>
                            <span className={`badge ${auditLogsService.getActivityBadgeColor(log.activity)}`}>
                              {log.activity}
                            </span>
                          </td>
                          <td>
                            <div style={{ maxWidth: '300px' }}>
                              <div className="text-truncate" title={log.description}>
                                {log.description}
                              </div>
                              {log.relatedEntityType && (
                                <small className="text-muted">
                                  Related: {log.relatedEntityType} #{log.relatedEntityId}
                                </small>
                              )}
                            </div>
                          </td>
                          <td>
                            {log.actionResult && (
                              <span className={`badge ${auditLogsService.getActionResultBadgeColor(log.actionResult)}`}>
                                {log.actionResult}
                              </span>
                            )}
                          </td>
                          <td>
                            <small className="text-muted">{log.ipAddress || 'N/A'}</small>
                          </td>
                          <td>
                            <small>{auditLogsService.formatDateForDisplay(log.timestamp)}</small>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
              <div className="card-footer text-center">
                <Button variant="outline-primary" onClick={fetchAuditLogs}>
                  Refresh Audit Logs
                </Button>
                <small className="text-muted ms-3">
                  Showing recent {auditLogs.length} audit logs
                </small>
              </div>
            </Card>
          </div>
        </div>
      )}

      {/* Users Management */}
      {activeTab === 'users' && (
        <div className="row">
          <div className="col-12">
            <Card title={`All Users (${filteredUsers.length})`} className="dashboard-card">
              {filteredUsers.length === 0 ? (
                <div className="text-center py-4">
                  <p className="text-muted">No users found.</p>
                  <Button variant="primary" onClick={fetchUsers}>
                    Refresh
                  </Button>
                </div>
              ) : (
                <div className="row">
                  {filteredUsers.map((user) => (
                    <div key={user.id} className="col-md-6 col-lg-4 mb-3">
                      <Card
                        className="h-100 user-card"
                        style={{
                          transition: 'all 0.3s ease',
                          cursor: 'pointer',
                          border: '1px solid #e9ecef',
                          boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
                        }}
                        onMouseEnter={(e) => {
                          e.currentTarget.style.transform = 'translateY(-5px)';
                          e.currentTarget.style.boxShadow = '0 8px 25px rgba(0,0,0,0.15)';
                        }}
                        onMouseLeave={(e) => {
                          e.currentTarget.style.transform = 'translateY(0)';
                          e.currentTarget.style.boxShadow = '0 2px 4px rgba(0,0,0,0.1)';
                        }}
                      >
                        <div className="card-body">
                          <div className="d-flex align-items-center mb-3">
                            <div
                              className="user-avatar me-3"
                              style={{
                                width: '50px',
                                height: '50px',
                                borderRadius: '50%',
                                background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                color: 'white',
                                fontWeight: 'bold',
                                fontSize: '18px'
                              }}
                            >
                              {getInitials(user.fullName)}
                            </div>
                            <div className="flex-grow-1">
                              <h6 className="card-title mb-1 fw-bold">{user.fullName}</h6>
                              <small className="text-muted">{user.email}</small>
                              {user.role === 'Admin' && (
                                <span className="badge bg-danger ms-2">Admin</span>
                              )}
                            </div>
                          </div>

                          <div className="mb-3">
                            <div className="row">
                              <div className="col-6">
                                <small className="text-muted">Phone</small>
                                <p className="mb-1">{user.phone}</p>
                              </div>
                              <div className="col-6">
                                <small className="text-muted">Role</small>
                                <p className="mb-1">
                                  <span className={`badge ${user.role === 'Admin' ? 'bg-danger' : 'bg-primary'}`}>
                                    {user.role}
                                  </span>
                                </p>
                              </div>
                            </div>
                            <div className="row">
                              <div className="col-6">
                                <small className="text-muted">City</small>
                                <p className="mb-1">{user.city}</p>
                              </div>
                              <div className="col-6">
                                <small className="text-muted">State</small>
                                <p className="mb-1">{user.state}</p>
                              </div>
                            </div>
                            <div className="row">
                              <div className="col-6">
                                <small className="text-muted">Pincode</small>
                                <p className="mb-1">{user.pincode}</p>
                              </div>
                              <div className="col-6">
                                <small className="text-muted">Joined</small>
                                <p className="mb-1">{formatDate(user.createdAt)}</p>
                              </div>
                            </div>
                          </div>

                          <Navigation
                            onEdit={() => handleEditUser(user)}
                            onView={() => handleViewUserDocuments(user)}
                            onDelete={() => handleDeleteUser(user.id)}
                            onDocuments={() => handleViewUserDocuments(user)}
                            showDocuments={false}
                            viewLabel="Documents"
                          />
                        </div>
                      </Card>
                    </div>
                  ))}
                </div>
              )}
            </Card>
          </div>
        </div>
      )}

      {/* User Details Modal */}
      {showModal && selectedUser && (
        <div className="modal show d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
          <div className="modal-dialog modal-lg">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">User Details</h5>
                <button
                  type="button"
                  className="btn-close"
                  onClick={() => setShowModal(false)}
                ></button>
              </div>
              <div className="modal-body">
                <div className="row">
                  <div className="col-md-4 text-center">
                    <div className="user-avatar mx-auto mb-3" style={{ width: '80px', height: '80px', fontSize: '2rem' }}>
                      {getInitials(selectedUser.fullName)}
                    </div>
                    <h5>{selectedUser.fullName}</h5>
                    <span className={`badge ${selectedUser.role === 'Admin' ? 'bg-danger' : 'bg-primary'}`}>
                      {selectedUser.role}
                    </span>
                  </div>
                  <div className="col-md-8">
                    <div className="row">
                      <div className="col-sm-6">
                        <h6 className="text-muted">Email</h6>
                        <p>{selectedUser.email}</p>
                      </div>
                      <div className="col-sm-6">
                        <h6 className="text-muted">Phone</h6>
                        <p>{selectedUser.phone}</p>
                      </div>
                      <div className="col-sm-6">
                        <h6 className="text-muted">City</h6>
                        <p>{selectedUser.city}</p>
                      </div>
                      <div className="col-sm-6">
                        <h6 className="text-muted">State</h6>
                        <p>{selectedUser.state}</p>
                      </div>
                      <div className="col-sm-6">
                        <h6 className="text-muted">Pincode</h6>
                        <p>{selectedUser.pincode}</p>
                      </div>
                      <div className="col-sm-6">
                        <h6 className="text-muted">Member Since</h6>
                        <p>{formatDate(selectedUser.createdAt)}</p>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
              <div className="modal-footer">
                <Button variant="secondary" onClick={() => setShowModal(false)}>
                  Close
                </Button>
                <Button variant="warning" onClick={() => {
                  setShowModal(false);
                  handleEditUser(selectedUser);
                }}>
                  Edit User
                </Button>
                <Button variant="danger" onClick={() => {
                  setShowModal(false);
                  handleDeleteUser(selectedUser.id);
                }}>
                  Delete User
                </Button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Edit User Modal */}
      {showEditModal && selectedUser && (
        <div className="modal show d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
          <div className="modal-dialog modal-lg">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">Edit User: {selectedUser.fullName}</h5>
                <button
                  type="button"
                  className="btn-close"
                  onClick={() => setShowEditModal(false)}
                ></button>
              </div>
              <form onSubmit={handleUpdateUser}>
                <div className="modal-body">
                  <div className="row">
                    <div className="col-md-6">
                      <div className="mb-3">
                        <label className="form-label">Full Name</label>
                        <input
                          type="text"
                          className="form-control"
                          name="fullName"
                          value={editForm.fullName}
                          onChange={handleEditFormChange}
                          required
                        />
                      </div>
                    </div>
                    <div className="col-md-6">
                      <div className="mb-3">
                        <label className="form-label">Email</label>
                        <input
                          type="email"
                          className="form-control"
                          name="email"
                          value={editForm.email}
                          onChange={handleEditFormChange}
                          required
                        />
                      </div>
                    </div>
                    <div className="col-md-6">
                      <div className="mb-3">
                        <label className="form-label">Phone</label>
                        <input
                          type="tel"
                          className="form-control"
                          name="phone"
                          value={editForm.phone}
                          onChange={handleEditFormChange}
                          required
                        />
                      </div>
                    </div>
                    <div className="col-md-6">
                      <div className="mb-3">
                        <label className="form-label">Role</label>
                        <select
                          className="form-select"
                          name="role"
                          value={editForm.role}
                          onChange={handleEditFormChange}
                          required
                        >
                          <option value="User">User</option>
                          <option value="Admin">Admin</option>
                        </select>
                      </div>
                    </div>
                    <div className="col-md-4">
                      <div className="mb-3">
                        <label className="form-label">City</label>
                        <input
                          type="text"
                          className="form-control"
                          name="city"
                          value={editForm.city}
                          onChange={handleEditFormChange}
                          required
                        />
                      </div>
                    </div>
                    <div className="col-md-4">
                      <div className="mb-3">
                        <label className="form-label">State</label>
                        <input
                          type="text"
                          className="form-control"
                          name="state"
                          value={editForm.state}
                          onChange={handleEditFormChange}
                          required
                        />
                      </div>
                    </div>
                    <div className="col-md-4">
                      <div className="mb-3">
                        <label className="form-label">Pincode</label>
                        <input
                          type="text"
                          className="form-control"
                          name="pincode"
                          value={editForm.pincode}
                          onChange={handleEditFormChange}
                          required
                        />
                      </div>
                    </div>
                  </div>
                </div>
                <div className="modal-footer">
                  <Button
                    type="button"
                    variant="secondary"
                    onClick={() => setShowEditModal(false)}
                  >
                    Cancel
                  </Button>
                  <Button type="submit" variant="primary">
                    Update User
                  </Button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}

      {/* Extracted Data Modal */}
      {showExtractedDataModal && extractedData && (
        <div className="modal show d-block" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
          <div className="modal-dialog modal-xl">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">Extracted Document Data</h5>
                <button
                  type="button"
                  className="btn-close"
                  onClick={() => setShowExtractedDataModal(false)}
                ></button>
              </div>
              <div className="modal-body">
                <div className="row mb-3">
                  <div className="col-12">
                    <div className="alert alert-info">
                      <h6 className="mb-2"><strong>Document Information</strong></h6>
                      <p className="mb-1"><strong>User:</strong> {extractedData?.userName || 'N/A'}</p>
                      <p className="mb-1"><strong>Email:</strong> {extractedData?.userEmail || 'N/A'}</p>
                      <p className="mb-1"><strong>Uploaded:</strong> {extractedData?.uploadedAt ? new Date(extractedData.uploadedAt).toLocaleString() : 'N/A'}</p>
                      <p className="mb-0"><strong>Extracted:</strong> {extractedData?.extractedAt ? new Date(extractedData.extractedAt).toLocaleString() : 'N/A'}</p>
                    </div>
                  </div>
                </div>

                <div className="row">
                  <div className="col-md-6">
                    <Card title="Aadhaar Data" className="mb-3">
                      {extractedData?.extractedData?.aadhaarData?.hasData ? (
                        <div>
                          <p><strong>Name:</strong> {extractedData.extractedData.aadhaarData.name || 'N/A'}</p>
                          <p><strong>Number:</strong> {extractedData.extractedData.aadhaarData.number || 'N/A'}</p>
                          <p><strong>DOB:</strong> {extractedData.extractedData.aadhaarData.dob || 'N/A'}</p>
                        </div>
                      ) : (
                        <p className="text-muted">No Aadhaar data extracted</p>
                      )}
                    </Card>
                  </div>

                  <div className="col-md-6">
                    <Card title="PAN Data" className="mb-3">
                      {extractedData?.extractedData?.panData?.hasData ? (
                        <div>
                          <p><strong>Name:</strong> {extractedData.extractedData.panData.name || 'N/A'}</p>
                          <p><strong>Number:</strong> {extractedData.extractedData.panData.number || 'N/A'}</p>
                          <p><strong>DOB:</strong> {extractedData.extractedData.panData.pandob || 'N/A'}</p>
                        </div>
                      ) : (
                        <p className="text-muted">No PAN data extracted</p>
                      )}
                    </Card>
                  </div>

                  <div className="col-md-6">
                    <Card title="Application Data" className="mb-3">
                      {extractedData?.extractedData?.applicationData?.hasData ? (
                        <div>
                          <p><strong>Application Number:</strong> {extractedData.extractedData.applicationData.applicationNumber || 'N/A'}</p>
                          <p><strong>Applicant Name:</strong> {extractedData.extractedData.applicationData.applicantName || 'N/A'}</p>
                          <p><strong>Address:</strong> {extractedData.extractedData.applicationData.applicantAddress || 'N/A'}</p>
                        </div>
                      ) : (
                        <p className="text-muted">No application data extracted</p>
                      )}
                    </Card>
                  </div>

                  <div className="col-md-6">
                    <Card title="Survey Data" className="mb-3">
                      {extractedData?.extractedData?.surveyData?.hasData ? (
                        <div>
                          <p><strong>Survey No:</strong> {extractedData.extractedData.surveyData.surveyNo || 'N/A'}</p>
                          <p><strong>Measuring Area:</strong> {extractedData.extractedData.surveyData.measuringArea || 'N/A'}</p>
                          <p><strong>Village:</strong> {extractedData.extractedData.surveyData.village || 'N/A'}</p>
                          <p><strong>Hobli:</strong> {extractedData.extractedData.surveyData.hobli || 'N/A'}</p>
                          <p><strong>Taluk:</strong> {extractedData.extractedData.surveyData.taluk || 'N/A'}</p>
                          <p><strong>District:</strong> {extractedData.extractedData.surveyData.district || 'N/A'}</p>
                        </div>
                      ) : (
                        <p className="text-muted">No survey data extracted</p>
                      )}
                    </Card>
                  </div>
                </div>
              </div>
              <div className="modal-footer">
                <Button
                  variant="secondary"
                  onClick={() => setShowExtractedDataModal(false)}
                >
                  Close
                </Button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* User Documents Modal */}
      {showUserDocumentsModal && selectedUserForDocuments && (
        <div className="modal show d-block" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
          <div className="modal-dialog modal-xl">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">
                  📄 Documents for {selectedUserForDocuments.fullName}
                </h5>
                <button
                  type="button"
                  className="btn-close"
                  onClick={() => setShowUserDocumentsModal(false)}
                ></button>
              </div>
              <div className="modal-body">
                <div className="row mb-3">
                  <div className="col-12">
                    <div className="alert alert-info">
                      <h6 className="mb-2"><strong>User Information</strong></h6>
                      <p className="mb-1"><strong>Name:</strong> {selectedUserForDocuments.fullName}</p>
                      <p className="mb-1"><strong>Email:</strong> {selectedUserForDocuments.email}</p>
                      <p className="mb-0"><strong>Role:</strong> {selectedUserForDocuments.role}</p>
                    </div>
                  </div>
                </div>

                {userDocuments.length === 0 ? (
                  <div className="text-center py-4">
                    <p className="text-muted">No documents uploaded by this user.</p>
                  </div>
                ) : (
                  <div className="row">
                    {userDocuments.map((doc) => (
                      <div key={doc.id} className="col-md-6 col-lg-4 mb-3">
                        <Card
                          className="h-100 document-card"
                          style={{
                            transition: 'all 0.3s ease',
                            cursor: 'pointer',
                            border: '1px solid #e9ecef',
                            boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
                          }}
                          onMouseEnter={(e) => {
                            e.currentTarget.style.transform = 'translateY(-3px)';
                            e.currentTarget.style.boxShadow = '0 6px 20px rgba(0,0,0,0.15)';
                          }}
                          onMouseLeave={(e) => {
                            e.currentTarget.style.transform = 'translateY(0)';
                            e.currentTarget.style.boxShadow = '0 2px 4px rgba(0,0,0,0.1)';
                          }}
                        >
                          <div className="card-body">
                            <div className="d-flex align-items-center mb-3">
                              <div
                                className="me-3"
                                style={{
                                  width: '40px',
                                  height: '40px',
                                  borderRadius: '50%',
                                  background: 'linear-gradient(135deg, #28a745 0%, #20c997 100%)',
                                  display: 'flex',
                                  alignItems: 'center',
                                  justifyContent: 'center',
                                  color: 'white',
                                  fontWeight: 'bold',
                                  fontSize: '16px'
                                }}
                              >
                                📄
                              </div>
                              <div className="flex-grow-1">
                                <h6 className="card-title mb-1 fw-bold">Document #{doc.id}</h6>
                                <small className="text-muted">
                                  Uploaded: {new Date(doc.createdAt).toLocaleDateString()}
                                </small>
                              </div>
                            </div>

                            <div className="mb-3">
                              <div className="row">
                                <div className="col-4">
                                  <small className="text-muted">EC Document</small>
                                  <p className="mb-1">
                                    <span className={`badge ${doc.ecPath ? 'bg-success' : 'bg-secondary'}`}>
                                      {doc.ecPath ? '✓ Available' : '✗ Not Uploaded'}
                                    </span>
                                  </p>
                                </div>
                                <div className="col-4">
                                  <small className="text-muted">Aadhaar</small>
                                  <p className="mb-1">
                                    <span className={`badge ${doc.aadhaarPath ? 'bg-success' : 'bg-secondary'}`}>
                                      {doc.aadhaarPath ? '✓ Available' : '✗ Not Uploaded'}
                                    </span>
                                  </p>
                                </div>
                                <div className="col-4">
                                  <small className="text-muted">PAN</small>
                                  <p className="mb-1">
                                    <span className={`badge ${doc.panPath ? 'bg-success' : 'bg-secondary'}`}>
                                      {doc.panPath ? '✓ Available' : '✗ Not Uploaded'}
                                    </span>
                                  </p>
                                </div>
                              </div>
                            </div>

                            {/* <div className="mb-3">
                              <div className="row">
                                <div className="col-6">
                                  <small className="text-muted">Status</small>
                                  <p className="mb-1">
                                    <span className={`badge ${
                                      doc.verificationStatus === 'Verified' ? 'bg-success' : 
                                      doc.verificationStatus === 'Rejected' ? 'bg-danger' : 
                                      'bg-warning'
                                    }`}>
                                      {doc.verificationStatus || 'Pending'}
                                    </span>
                                  </p>
                                </div>
                                <div className="col-6">
                                  <small className="text-muted">Risk Score</small>
                                  <p className="mb-1">
                                    <span className={`badge ${
                                      doc.riskScore >= 80 ? 'bg-danger' : 
                                      doc.riskScore >= 50 ? 'bg-warning' : 
                                      'bg-success'
                                    }`}>
                                      {doc.riskScore ? `${doc.riskScore}%` : 'N/A'}
                                    </span>
                                  </p>
                                </div>
                              </div>
                            </div> */}
                            <div className="mb-3">
                              <div className="row">
                                {/* ✅ STATUS */}
                                <div className="col-6">
                                  <small className="text-muted">Status</small>
                                  <p className="mb-1">
                                    <span
                                      className={`badge ${(doc.verificationStatus || doc.verification_status) === 'Verified'
                                          ? 'bg-success'
                                          : (doc.verificationStatus || doc.verification_status) === 'Rejected'
                                            ? 'bg-danger'
                                            : 'bg-warning'
                                        }`}
                                    >
                                      {doc.verificationStatus ||
                                        doc.verification_status ||
                                        'Pending'}
                                    </span>
                                  </p>
                                </div>

                                {/* ✅ RISK SCORE */}
                                <div className="col-6">
                                  <small className="text-muted">Risk Score</small>
                                  <p className="mb-1">
                                    {(() => {
                                      // ✅ Either take from backend or calculate if fields data available
                                      let riskScore = doc.riskScore ?? doc.risk_score;

                                      // If not provided, calculate it from fieldMatches
                                      if (!riskScore && doc.fieldMatches) {
                                        const totalFields = 11;
                                        const matchedCount = Object.values(doc.fieldMatches).filter(v => v === true).length;
                                        riskScore = Math.round((matchedCount / totalFields) * 100);
                                      }

                                      // Handle display
                                      if (typeof riskScore === 'number' && !isNaN(riskScore)) {
                                        const badgeClass =
                                          riskScore >= 80
                                            ? 'bg-danger'
                                            : riskScore >= 50
                                              ? 'bg-warning text-dark'
                                              : 'bg-success';
                                        return <span className={`badge ${badgeClass}`}>{riskScore}%</span>;
                                      } else {
                                        return <span className="badge bg-secondary">N/A</span>;
                                      }
                                    })()}
                                  </p>
                                </div>

                              </div>
                            </div>




                            <div className="d-flex gap-2 justify-content-end">
                              {/* <Button 
                                variant="outline-primary" 
                                size="sm"
                                onClick={() => handleExtractDocuments(doc.id)}
                                disabled={doc.hasExtractedData || extractingDocuments.has(doc.id)}
                                title={doc.hasExtractedData ? "Already extracted" : extractingDocuments.has(doc.id) ? "Extracting..." : "Extract data from documents"}
                                style={{
                                  borderRadius: '15px',
                                  fontSize: '12px'
                                }}
                              >
                                {extractingDocuments.has(doc.id) ? (
                                  <>
                                    <span className="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span>
                                    Extracting...
                                  </>
                                ) : doc.hasExtractedData ? "✅ Extracted" : "🔍 Extract"}
                              </Button> */}
                              <Button
                                variant="outline-success"
                                size="sm"
                                style={{
                                  borderRadius: '15px',
                                  fontSize: '12px'
                                }}
                                onClick={() => handleExtractDocuments(doc.id)}
                                title="Extract data from documents"
                              >
                                Extract
                              </Button>
                              {doc.IsVerified ? (
                                <Button
                                  variant="success"
                                  size="sm"
                                  disabled
                                  style={{
                                    borderRadius: '15px',
                                    fontSize: '12px'
                                  }}
                                  title="Already verified"
                                >
                                  ✅ Verified
                                </Button>
                              ) : doc.VerificationStatus === 'Rejected' ? (
                                <Button
                                  variant="danger"
                                  size="sm"
                                  disabled
                                  style={{
                                    borderRadius: '15px',
                                    fontSize: '12px'
                                  }}
                                  title="Verification rejected"
                                >
                                  ❌ Rejected
                                </Button>
                              ) : doc.VerificationStatus === 'Under Review' ? (
                                <Button
                                  variant="warning"
                                  size="sm"
                                  disabled
                                  style={{
                                    borderRadius: '15px',
                                    fontSize: '12px'
                                  }}
                                  title="Under review"
                                >
                                  🔍 Under Review
                                </Button>
                              ) : (
                                <Button
                                  variant="outline-success"
                                  size="sm"
                                  onClick={() => handleTriggerVerification(doc.id)}
                                  style={{
                                    borderRadius: '15px',
                                    fontSize: '12px'
                                  }}
                                  title="Verify document authenticity"
                                >
                                  🔍 Verify
                                </Button>
                              )}
                              <Button
                                variant="outline-info"
                                size="sm"
                                onClick={() => handleViewExtractedData(doc.id)}
                                style={{
                                  borderRadius: '15px',
                                  fontSize: '12px'
                                }}
                              >
                                👁️ View
                              </Button>
                               <Button 
                                variant="outline-info" 
                                size="sm"
                                onClick={() => handleGetGeoLocation(doc.id)}
                                style={{
                                  borderRadius: '15px',
                                  fontSize: '12px'
                                }}
                              >
                               📍 Get Location
                              </Button>
                              <Button
                                variant="outline-info"
                                size="sm"
                                onClick={() => handleViewVerificationResults(doc.id)}
                                title="View verification results"
                                style={{
                                  borderRadius: '15px',
                                  fontSize: '12px'
                                }}
                              >
                                📊 Results
                              </Button>
                            </div>
                          </div>
                        </Card>
                      </div>
                    ))}
                  </div>
                )}
              </div>
              <div className="modal-footer">
                <Button
                  variant="secondary"
                  onClick={() => setShowUserDocumentsModal(false)}
                >
                  Close
                </Button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Verification Results Modal */}
      <VerificationResultsModal
        uploadId={selectedDocumentForVerification}
        isOpen={showVerificationResultsModal}
        onClose={() => {
          setShowVerificationResultsModal(false);
          setSelectedDocumentForVerification(null);
        }}
      />
    </div>
  );
};

export default AdminDashboard;