import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import './assets/styles/AdminDashboard.css';

const AdminDashboard = () => {
  const [pendingUsers, setPendingUsers] = useState([]);
  const [approvedUsers, setApprovedUsers] = useState([]);
  const [rejectedUsers, setRejectedUsers] = useState([]);
  const [errorMessage, setErrorMessage] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    const isAdmin = localStorage.getItem('isAdmin');

    if (!isAdmin) {
      navigate('/login'); // Redirect if not an admin
    } else {
      const fetchUsers = async () => {
        try {
          const response = await fetch('https://localhost:7160/api/Admin/get-all-users');
          const data = await response.json();

          if (response.ok) {
            setPendingUsers(data.pendingUsers || []);
            setApprovedUsers(data.approvedUsers || []);
            setRejectedUsers(data.rejectedUsers || []);
          } else {
            setErrorMessage(data.Message || 'Failed to fetch users.');
          }
        } catch (error) {
          setErrorMessage('An error occurred while fetching users.');
          console.error('Error fetching users:', error);
        }
      };

      fetchUsers();
    }
  }, [navigate]);

  const approveUser = async (userId) => {
    try {
      const response = await fetch(`https://localhost:7160/api/Admin/approve-user/${userId}`, {
        method: 'POST',
      });

      if (response.ok) {
        alert('User approved successfully!');
        const updatedPending = pendingUsers.filter((user) => user.id !== userId);
        setPendingUsers(updatedPending);

        const approvedUser = pendingUsers.find((user) => user.id === userId);
        setApprovedUsers((prev) => [...prev, approvedUser]);
      } else {
        setErrorMessage('Failed to approve the user.');
      }
    } catch (error) {
      setErrorMessage('An error occurred while approving the user.');
    }
  };

  const rejectUser = async (userId) => {
    try {
      const response = await fetch(`https://localhost:7160/api/Admin/reject-user/${userId}`, {
        method: 'POST',
      });

      if (response.ok) {
        alert('User rejected successfully!');
        const updatedPending = pendingUsers.filter((user) => user.id !== userId);
        setPendingUsers(updatedPending);

        const rejectedUser = pendingUsers.find((user) => user.id === userId);
        setRejectedUsers((prev) => [...prev, rejectedUser]);
      } else {
        setErrorMessage('Failed to reject the user.');
      }
    } catch (error) {
      setErrorMessage('An error occurred while rejecting the user.');
    }
  };

  const handleLogout = () => {
    localStorage.removeItem('isAdmin'); // Remove admin status
    navigate('/login'); // Redirect to login page
  };

  return (
    <div className="admin-dashboard">
      <header className="dashboard-header">
        <h2>Admin Dashboard</h2>
        <div className="button-container">
          <button className="logout-btn" onClick={handleLogout}>Logout</button>
          <button className="cab-booking-btn" onClick={() => navigate('/CabBooking')}>Go to Cab Booking</button>
        </div>
      </header>

      {errorMessage && <div className="error-message">{errorMessage}</div>}

      {/* Pending Users */}
      <div className="user-list">
        <h3>Pending Users</h3>
        <table>
          <thead>
            <tr>
              <th>Name</th>
              <th>Mobile Number</th>
              <th>Email</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            {pendingUsers.length > 0 ? (
              pendingUsers.map((user) => (
                <tr key={user.id}>
                  <td>{user.name}</td>
                  <td>{user.mobileNumber || 'Not provided'}</td>
                  <td>{user.email}</td>
                  <td>
                    <button onClick={() => approveUser(user.id)} className="approve-btn">
                      Approve
                    </button>
                    <button onClick={() => rejectUser(user.id)} className="reject-btn">
                      Reject
                    </button>
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan="4">No pending users to display.</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      {/* Approved Users */}
      <div className="user-list">
        <h3>Approved Users</h3>
        <table>
          <thead>
            <tr>
              <th>Name</th>
              <th>Mobile Number</th>
              <th>Email</th>
            </tr>
          </thead>
          <tbody>
            {approvedUsers.length > 0 ? (
              approvedUsers.map((user) => (
                <tr key={user.id}>
                  <td>{user.name}</td>
                  <td>{user.mobileNumber || 'Not provided'}</td>
                  <td>{user.email}</td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan="3">No approved users to display.</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      {/* Rejected Users */}
      <div className="user-list">
        <h3>Rejected Users</h3>
        <table>
          <thead>
            <tr>
              <th>Name</th>
              <th>Mobile Number</th>
              <th>Email</th>
            </tr>
          </thead>
          <tbody>
            {rejectedUsers.length > 0 ? (
              rejectedUsers.map((user) => (
                <tr key={user.id}>
                  <td>{user.name}</td>
                  <td>{user.mobileNumber || 'Not provided'}</td>
                  <td>{user.email}</td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan="3">No rejected users to display.</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default AdminDashboard;
