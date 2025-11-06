import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../Services/AuthService';
import Card from '../Common/Card';
import Button from '../Common/Button';

const SignOut = () => {
  const { logout, isAuthenticated } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    // Auto logout after 3 seconds
    const timer = setTimeout(() => {
      handleLogout();
    }, 3000);

    return () => clearTimeout(timer);
  }, []);

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  const handleCancel = () => {
    navigate(-1); // Go back to previous page
  };

  if (!isAuthenticated) {
    navigate('/');
    return null;
  }

  return (
    <div className="row justify-content-center">
      <div className="col-md-6 col-lg-4">
        <Card title="Signing Out..." className="mt-5 text-center">
          <div className="card-body">
            <div className="mb-4">
              <div className="spinner-border text-primary" role="status">
                <span className="visually-hidden">Signing out...</span>
              </div>
            </div>
            
            <h5 className="card-title">Are you sure you want to sign out?</h5>
            <p className="card-text text-muted">
              You will be automatically signed out in 3 seconds...
            </p>
            
            <div className="d-grid gap-2">
              <Button 
                variant="danger" 
                onClick={handleLogout}
                className="mb-2"
              >
                Yes, Sign Out
              </Button>
              <Button 
                variant="outline-secondary" 
                onClick={handleCancel}
              >
                Cancel
              </Button>
            </div>
          </div>
        </Card>
      </div>
    </div>
  );
};

export default SignOut;
