import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../Services/AuthService';
import Card from '../Common/Card';
import InputField from '../Common/InputField';
import Button from '../Common/Button';

const AdminLogin = () => {
  const [formData, setFormData] = useState({
    username: '',
    password: ''
  });
  const [errors, setErrors] = useState({});
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');

  const { adminLogin } = useAuth();
  const navigate = useNavigate();

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
    // Clear error when user starts typing
    if (errors[name]) {
      setErrors(prev => ({
        ...prev,
        [name]: ''
      }));
    }
    // Clear login error message when user starts typing
    if (message) {
      setMessage('');
    }
  };

  const validateForm = () => {
    const newErrors = {};

    if (!formData.username.trim()) {
      newErrors.username = 'Username is required';
    }

    if (!formData.password) {
      newErrors.password = 'Password is required';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!validateForm()) {
      return;
    }

    setLoading(true);
    setMessage('');

    try {
      const result = await adminLogin(formData.username, formData.password);
      
      if (result.success) {
        // Add a small delay to show success state
        setTimeout(() => {
          navigate('/admin-dashboard');
        }, 500);
      } else {
        // Display specific error messages
        if (result.error.includes('Invalid admin credentials')) {
          setMessage('❌ Invalid admin credentials. Please check your username and password.');
        } else if (result.error.includes('Admin not found')) {
          setMessage('❌ No admin account found with this username.');
        } else if (result.error.includes('Access denied')) {
          setMessage('🔒 Access denied. You do not have admin privileges.');
        } else {
          setMessage(`❌ ${result.error}`);
        }
        
        // Keep error message visible for at least 3 seconds
        setTimeout(() => {
          setLoading(false);
        }, 3000);
        return; // Don't set loading to false immediately
      }
    } catch (error) {
      console.error('Admin login error:', error);
      if (error.response?.status === 401) {
        setMessage('❌ Invalid admin credentials. Please check your username and password.');
      } else if (error.response?.status === 403) {
        setMessage('🔒 Access denied. You do not have admin privileges.');
      } else if (error.response?.status === 404) {
        setMessage('❌ No admin account found with this username.');
      } else if (error.response?.status === 500) {
        setMessage('⚠️ Server error. Please try again later.');
      } else {
        setMessage('❌ An error occurred during admin login. Please check your internet connection and try again.');
      }
      
      // Keep error message visible for at least 3 seconds
      setTimeout(() => {
        setLoading(false);
      }, 3000);
      return; // Don't set loading to false immediately
    }
    
    setLoading(false);
  };

  return (
    <div className="row justify-content-center">
      <div className="col-md-6 col-lg-4">
        <Card title="Admin Login" className="mt-5">
          <div className="alert alert-info" role="alert">
            <strong>Default Admin Credentials:</strong><br />
            Username: <code>admin</code> or <code>admin@contactmanager.com</code><br />
            Password: <code>admin123</code>
          </div>

          <form onSubmit={handleSubmit}>
            {message && (
              <div className="alert alert-danger d-flex align-items-center" role="alert">
                <i className="fas fa-exclamation-triangle me-2"></i>
                <div className="flex-grow-1">
                  <strong>Admin Login Failed</strong><br />
                  <small>{message}</small>
                </div>
                <button 
                  type="button" 
                  className="btn-close" 
                  aria-label="Close"
                  onClick={() => setMessage('')}
                ></button>
              </div>
            )}

            <InputField
              type="text"
              name="username"
              label="Username"
              value={formData.username}
              onChange={handleChange}
              placeholder="Enter admin username"
              error={errors.username}
              required
            />

            <InputField
              type="password"
              name="password"
              label="Password"
              value={formData.password}
              onChange={handleChange}
              placeholder="Enter admin password"
              error={errors.password}
              required
            />

            <div className="d-grid">
              <Button
                type="submit"
                variant="primary"
                size="lg"
                loading={loading}
                disabled={loading}
              >
                {loading ? (
                  <>
                    <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                    Signing In...
                  </>
                ) : (
                  'Admin Sign In'
                )}
              </Button>
            </div>
            
            {loading && (
              <div className="text-center mt-2">
                <small className="text-muted">
                  <i className="fas fa-info-circle me-1"></i>
                  Please wait while we verify your admin credentials...
                </small>
              </div>
            )}
          </form>

          {/* <div className="text-center mt-3">
            <p className="mb-2">
              <Link to="/login" className="text-decoration-none">
                User Login
              </Link>
            </p>
            <p className="mb-2">
              <Link to="/register" className="text-decoration-none">
                User Registration
              </Link>
            </p>
            <p className="mb-2">
              <Link to="/admin-debug" className="text-decoration-none text-warning">
                Debug Admin Login
              </Link>
            </p>
            <p className="mb-0">
              <Link to="/simple-test" className="text-decoration-none text-info">
                Simple Admin Test
              </Link>
            </p>
          </div> */}
        </Card>
      </div>
    </div>
  );
};

export default AdminLogin;
