import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import './assets/styles/AdminDashboard.css';

const AdminDashboard = () => {
  const [pendingUsers, setPendingUsers] = useState([]);
  const [errorMessage, setErrorMessage] = useState('');
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const isAdmin = localStorage.getItem('isAdmin');
    
    if (!isAdmin) {
      navigate('/login'); // Redirect to login if not an admin
    } else {
      const fetchPendingUsers = async () => {
        try {
          const response = await fetch('https://localhost:7160/api/Admin/get-all-users');
          const data = await response.json();

          if (response.ok) {
            setPendingUsers(data.PendingUsers || []);
          } else {
            setErrorMessage(data.Message || 'Failed to fetch pending users.');
          }
        } catch (error) {
          setErrorMessage('An error occurred while fetching users.');
          console.error('Fetch error:', error);
        } finally {
          setLoading(false);
        }
      };

      fetchPendingUsers();
    }
  }, [navigate]);

  const handleUserAction = async (userId, action) => {
    const url = `https://localhost:7160/api/Admin/${action}-user/${userId}`;

    try {
      const response = await fetch(url, { method: 'POST' });
      
      if (response.ok) {
        setPendingUsers((prevUsers) => prevUsers.filter(user => user.id !== userId));
      } else {
        const errorData = await response.json();
        setErrorMessage(errorData.Error || `Failed to ${action} the user.`);
      }
    } catch (error) {
      setErrorMessage(`An error occurred while trying to ${action} the user.`);
      console.error(`${action} error:`, error);
    }
  };

  return (
    <div className="admin-dashboard">
      <h2>Admin Dashboard</h2>

      {errorMessage && <div className="error-message">{errorMessage}</div>}
      
      <div className="user-list">
        <h3>Pending User Approvals</h3>
        
        {loading ? (
          <p>Loading...</p>
        ) : (
          <table className="user-table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Email</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {pendingUsers.length > 0 ? (
                pendingUsers.map((user) => (
                  <tr key={user.id}>
                    <td>{user.name}</td>
                    <td>{user.email}</td>
                    <td>
                      <button
                        onClick={() => handleUserAction(user.id, 'approve')}
                        className="approve-btn"
                      >
                        Approve
                      </button>
                      <button
                        onClick={() => handleUserAction(user.id, 'reject')}
                        className="reject-btn"
                      >
                        Reject
                      </button>
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan="3">No pending users to display.</td>
                </tr>
              )}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
};

export default AdminDashboard;
