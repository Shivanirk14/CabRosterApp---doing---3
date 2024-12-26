import React from 'react';
import './assets/styles/Home.css';  // Import your updated styles

const Home = () => {
  return (
    <div className="app-container">
      {/* Navigation Bar with links */}
      <div className="navbar">
        <div className="navbar-left">SmartRoute</div>
        <div className="navbar-right">
          <a href="/" className="navbar-link">Home</a>
          <a href="/login" className="navbar-link">Login</a>
          <a href="/registeration" className="navbar-link">Register</a>
          <a href="/contact" className="navbar-link">Contact</a>
          <a href="/about" className="navbar-link">About Us</a>
        </div>
      </div>

      {/* Home Section */}
      <div className="home-section">
        <h1 className="home-title">Welcome to SmartRoute</h1>
        <p className="home-description">
          Find your desired service in just a few steps.
        </p>
      </div>
    </div>
  );
};

export default Home;
