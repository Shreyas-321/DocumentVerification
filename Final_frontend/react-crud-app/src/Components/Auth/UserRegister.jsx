import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../Services/AuthService';
import Card from '../Common/Card';
import InputField from '../Common/InputField';
import Button from '../Common/Button';

const UserRegister = () => {
  const [formData, setFormData] = useState({
    fullName: '',
    email: '',
    phone: '',
    city: '',
    state: '',
    pincode: '',
    password: '',
    confirmPassword: ''
  });
  const [errors, setErrors] = useState({});
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');

  const { register } = useAuth();
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
  };

  const validateForm = () => {
    const newErrors = {};

    if (!formData.fullName.trim()) {
      newErrors.fullName = 'Full name is required';
    }

    if (!formData.email) {
      newErrors.email = 'Email is required';
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = 'Email is invalid';
    }

    if (!formData.phone) {
      newErrors.phone = 'Phone number is required';
    } else if (!/^\d{10}$/.test(formData.phone)) {
      newErrors.phone = 'Phone number must be 10 digits';
    }

    if (!formData.city.trim()) {
      newErrors.city = 'City is required';
    }

    if (!formData.state.trim()) {
      newErrors.state = 'State is required';
    }

    if (!formData.pincode) {
      newErrors.pincode = 'Pincode is required';
    } else if (!/^\d{6}$/.test(formData.pincode)) {
      newErrors.pincode = 'Pincode must be 6 digits';
    }

    if (!formData.password) {
      newErrors.password = 'Password is required';
    } else if (formData.password.length < 6) {
      newErrors.password = 'Password must be at least 6 characters';
    }

    if (!formData.confirmPassword) {
      newErrors.confirmPassword = 'Please confirm your password';
    } else if (formData.password !== formData.confirmPassword) {
      newErrors.confirmPassword = 'Passwords do not match';
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
      const { confirmPassword, ...userData } = formData;
      const result = await register(userData);
      
      if (result.success) {
        setMessage('Registration successful! Please login to continue.');
        setTimeout(() => {
          navigate('/login');
        }, 2000);
      } else {
        setMessage(result.error);
      }
    } catch (error) {
      setMessage('An error occurred during registration. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="row justify-content-center">
      <div className="col-md-8 col-lg-6">
        <Card title="User Registration" className="mt-5">
          <form onSubmit={handleSubmit}>
            {message && (
              <div className={`alert ${message.includes('successful') ? 'alert-success' : 'alert-danger'}`} role="alert">
                {message}
              </div>
            )}

            <div className="row">
              <div className="col-md-6">
                <InputField
                  type="text"
                  name="fullName"
                  label="Full Name"
                  value={formData.fullName}
                  onChange={handleChange}
                  placeholder="Enter your full name"
                  error={errors.fullName}
                  required
                />
              </div>
              <div className="col-md-6">
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
              </div>
            </div>

            <div className="row">
              <div className="col-md-6">
                <InputField
                  type="tel"
                  name="phone"
                  label="Phone Number"
                  value={formData.phone}
                  onChange={handleChange}
                  placeholder="Enter your phone number"
                  error={errors.phone}
                  required
                />
              </div>
              <div className="col-md-6">
                <InputField
                  type="text"
                  name="city"
                  label="City"
                  value={formData.city}
                  onChange={handleChange}
                  placeholder="Enter your city"
                  error={errors.city}
                  required
                />
              </div>
            </div>

            <div className="row">
              <div className="col-md-6">
                <InputField
                  type="text"
                  name="state"
                  label="State"
                  value={formData.state}
                  onChange={handleChange}
                  placeholder="Enter your state"
                  error={errors.state}
                  required
                />
              </div>
              <div className="col-md-6">
                <InputField
                  type="text"
                  name="pincode"
                  label="Pincode"
                  value={formData.pincode}
                  onChange={handleChange}
                  placeholder="Enter your pincode"
                  error={errors.pincode}
                  required
                />
              </div>
            </div>

            <div className="row">
              <div className="col-md-6">
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
              </div>
              <div className="col-md-6">
                <InputField
                  type="password"
                  name="confirmPassword"
                  label="Confirm Password"
                  value={formData.confirmPassword}
                  onChange={handleChange}
                  placeholder="Confirm your password"
                  error={errors.confirmPassword}
                  required
                />
              </div>
            </div>

            <div className="d-grid">
              <Button
                type="submit"
                variant="primary"
                size="lg"
                loading={loading}
                disabled={loading}
              >
                {loading ? 'Creating Account...' : 'Create Account'}
              </Button>
            </div>
          </form>

          <div className="text-center mt-3">
            <p className="mb-0">
              Already have an account?{' '}
              <Link to="/login" className="text-decoration-none">
                Login here
              </Link>
            </p>
          </div>
        </Card>
      </div>
    </div>
  );
};

export default UserRegister;
