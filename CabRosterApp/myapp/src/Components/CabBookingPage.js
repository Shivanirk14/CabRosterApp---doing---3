import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import * as XLSX from 'xlsx'; // Import XLSX library for Excel export
import "./assets/styles/CabBookingPage.css";

const CabBookingPage = () => {
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [filteredBookings, setFilteredBookings] = useState([]);
  const navigate = useNavigate();

  // Helper function to format date
  const formatDate = (dateString) => {
    const date = new Date(dateString);
    if (!isNaN(date.getTime())) {
      return date.toLocaleDateString(); // Return formatted date
    } else {
      return "Invalid date"; // Return error message if invalid
    }
  };

  // Fetching the list of bookings
  useEffect(() => {
    fetchBookings();
  }, []);

  useEffect(() => {
    // Filter bookings based on search term (User)
    const results = bookings.filter((booking) =>
      booking.user.toLowerCase().includes(searchTerm.toLowerCase())
    );
    setFilteredBookings(results); // Update filtered list
  }, [searchTerm, bookings]);

  const fetchBookings = async () => {
    try {
      const response = await fetch("https://localhost:7160/api/CabBooking/list");
      if (!response.ok) {
        throw new Error("Failed to fetch bookings");
      }
      const data = await response.json();
      setBookings(data);
      setFilteredBookings(data); // Default to show all bookings
      setLoading(false);
    } catch (error) {
      console.error("Error fetching bookings:", error);
      setLoading(false);
    }
  };

  const updateBookingStatus = async (bookingId, status) => {
    try {
      const response = await fetch(
        `https://localhost:7160/api/CabBooking/update-status/${bookingId}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ status: status }),
        }
      );

      if (!response.ok) {
        throw new Error("Failed to update status");
      }
      fetchBookings(); // Refresh bookings after status update
    } catch (error) {
      console.error("Error updating booking status:", error);
    }
  };

  const handleApprove = (bookingId) => {
    updateBookingStatus(bookingId, "Approved");
  };

  const handleReject = (bookingId) => {
    updateBookingStatus(bookingId, "Rejected");
  };

  const handleSearch = (e) => {
    e.preventDefault();
    // Search logic is handled by useEffect, as it's based on searchTerm.
  };

  const handleExport = () => {
    const ws = XLSX.utils.json_to_sheet(filteredBookings); // Convert filtered bookings to sheet
    const wb = XLSX.utils.book_new(); // Create a new workbook
    XLSX.utils.book_append_sheet(wb, ws, "Bookings"); // Add sheet to workbook
    XLSX.writeFile(wb, "CabBookings.xlsx"); // Export the Excel file
  };

  const handleLogout = () => {
    navigate("/login");
  };

  return (
    <div className="cab-booking-page">
      <div className="header">
        <button className="btn-dashboard" onClick={() => navigate('/AdminDashboard')}>
          Dashboard
        </button>
        <form onSubmit={handleSearch} className="search-form">
          <input
            type="text"
            placeholder="Search by user"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)} // Update search term
          />
          <button type="submit" className="btn-search">Search</button>
        </form>
        <button className="btn-logout" onClick={handleLogout}>Logout</button>
      </div>

      <h1 className="page-title">Cab Booking Management</h1>

      {loading ? (
        <div className="loading-message"><p>Loading bookings...</p></div>
      ) : (
        <div className="booking-list-container">
          <table className="booking-list-table">
            <thead>
              <tr>
                <th>Booking ID</th>
                <th>Booking Date</th>
                <th>Shift</th>
                <th>User</th>
                <th>Nodal Point</th>
                <th>Status</th>
                <th>Action</th>
              </tr>
            </thead>
            <tbody>
              {filteredBookings.map((booking) => (
                <tr key={booking.id}>
                  <td>{booking.id}</td>
                  <td>
                    {booking.startDate && booking.endDate
                      ? `${formatDate(booking.startDate)} - ${formatDate(booking.endDate)}`
                      : 'Invalid dates'}
                  </td>
                  <td>{booking.shift}</td>
                  <td>{booking.user}</td>
                  <td>{booking.nodalPoint ? booking.nodalPoint : 'Not provided'}</td>
                  <td className={`status ${booking.status.toLowerCase()}`}>{booking.status}</td>
                  <td>
                    {booking.status === "Booked" ? (
                      <div className="action-buttons">
                        <button onClick={() => handleApprove(booking.id)} className="approve-btn">
                          Approve
                        </button>
                        <button onClick={() => handleReject(booking.id)} className="reject-btn">
                          Reject
                        </button>
                      </div>
                    ) : (
                      <span>{booking.status}</span>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* Export Button */}
      <button className="export-btn" onClick={handleExport}>Export to Excel</button>
    </div>
  );
};

export default CabBookingPage;
