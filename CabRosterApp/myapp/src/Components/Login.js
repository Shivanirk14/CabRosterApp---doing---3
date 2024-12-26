import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import './assets/styles/Login.css';

const Login = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setErrorMessage('');

    const loginData = { email, password };

    try {
      const response = await fetch('https://localhost:7160/api/Login/LoginForm', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(loginData),
      });

      const data = await response.json();

      if (response.ok) {
        // Save user ID and admin status in localStorage for global usage
        localStorage.setItem('userId', data.userId);
        localStorage.setItem('isAdmin', data.isAdmin);

        // Navigate to respective dashboards based on admin status
        if (data.isAdmin) {
          alert('Admin login successful!');
          navigate('/AdminDashboard');
        } else {
          alert('Login successful!');
          navigate('/UserDashboard');
        }
      } else {
        setErrorMessage(data.Error || 'Invalid credentials. Please try again.');
      }
    } catch (error) {
      setErrorMessage('An error occurred. Please try again later.');
      console.error('Login error:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-background">
      <div className="login-card">
        <h2>Login</h2>
        <form onSubmit={handleSubmit}>
          <div className="login-field">
            <label>Email</label>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="Enter your email"
              required
            />
          </div>
          <div className="login-field">
            <label>Password</label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="Enter your password"
              required
            />
          </div>
          {errorMessage && <p className="error">{errorMessage}</p>}
          <button type="submit" className="login-btn" disabled={loading}>
            {loading ? 'Logging in...' : 'Login'}
          </button>
        </form>
        <p className="register-link">
          Don't have an account? <Link to="/registeration">Register</Link>
        </p>
      </div>
    </div>
  );
};

export default Login;
