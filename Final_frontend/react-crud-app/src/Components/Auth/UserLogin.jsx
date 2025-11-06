import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../Services/AuthService';
import Card from '../Common/Card';
import InputField from '../Common/InputField';
import Button from '../Common/Button';

const UserLogin = () => {
  const [formData, setFormData] = useState({
    email: '',
    password: ''
  });
  const [errors, setErrors] = useState({});
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');

  const { login } = useAuth();
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

    if (!formData.email) {
      newErrors.email = 'Email is required';
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = 'Email is invalid';
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
      const result = await login(formData.email, formData.password);
      
      if (result.success) {
        // Add a small delay to show success state
        setTimeout(() => {
          navigate('/dashboard');
        }, 500);
      } else {
        // Display specific error messages
        if (result.error.includes('Invalid email or password')) {
          setMessage('❌ Invalid email or password. Please check your credentials and try again.');
        } else if (result.error.includes('User not found')) {
          setMessage('❌ No account found with this email address. Please register first.');
        } else if (result.error.includes('Account locked')) {
          setMessage('🔒 Your account has been temporarily locked. Please contact support.');
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
      console.error('Login error:', error);
      if (error.response?.status === 401) {
        setMessage('❌ Invalid email or password. Please check your credentials and try again.');
      } else if (error.response?.status === 404) {
        setMessage('❌ No account found with this email address. Please register first.');
      } else if (error.response?.status === 500) {
        setMessage('⚠️ Server error. Please try again later.');
      } else {
        setMessage('❌ An error occurred during login. Please check your internet connection and try again.');
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
        <Card title="User Login" className="mt-5">
          <form onSubmit={handleSubmit}>
            {message && (
              <div className="alert alert-danger d-flex align-items-center" role="alert">
                <i className="fas fa-exclamation-triangle me-2"></i>
                <div className="flex-grow-1">
                  <strong>Login Failed</strong><br />
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
              type="email"
              name="email"
              label="Email Address"
              value={formData.email}
              onChange={handleChange}
              placeholder="Enter your email"
              error={errors.email}
              required
            />

            <InputField
              type="password"
              name="password"
              label="Password"
              value={formData.password}
              onChange={handleChange}
              placeholder="Enter your password"
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
                  'Sign In'
                )}
              </Button>
            </div>
            
            {loading && (
              <div className="text-center mt-2">
                <small className="text-muted">
                  <i className="fas fa-info-circle me-1"></i>
                  Please wait while we verify your credentials...
                </small>
              </div>
            )}
          </form>

          <div className="text-center mt-3">
            <p className="mb-2">
              Don't have an account?{' '}
              <Link to="/register" className="text-decoration-none">
                Register here
              </Link>
            </p>
            <p className="mb-0">
              <Link to="/admin-login" className="text-decoration-none">
                Admin Login
              </Link>
            </p>
          </div>
        </Card>
      </div>
    </div>
  );
};

export default UserLogin;
