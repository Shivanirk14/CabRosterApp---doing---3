import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom"; // Import useNavigate for navigation
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import { addDays, isSaturday, isSunday, startOfWeek, endOfWeek, format } from "date-fns";
import { toZonedTime, fromZonedTime } from 'date-fns-tz'; // Correct import
import axios from "axios";
import "./assets/styles/UserDashboard.css";

const UserDashboard = () => {
  const navigate = useNavigate();
  const today = new Date();

  // Get next Monday date and set it for the week
  const nextMonday = addDays(today, (8 - today.getDay()) % 7);
  const nextWeekStart = startOfWeek(nextMonday, { weekStartsOn: 1 });
  const nextWeekEnd = endOfWeek(nextMonday, { weekStartsOn: 1 });

  const [startDate, setStartDate] = useState(null);
  const [endDate, setEndDate] = useState(null);
  const [shifts, setShifts] = useState([]);
  const [nodalPoints, setNodalPoints] = useState([]);
  const [selectedShift, setSelectedShift] = useState(null);
  const [selectedNodalPoint, setSelectedNodalPoint] = useState(null);
  const [confirmationMessage, setConfirmationMessage] = useState(null);
  const [bookingDetails, setBookingDetails] = useState(null);

  // Fetch shifts and nodal points data
  useEffect(() => {
    const fetchData = async () => {
      try {
        const [shiftsRes, nodalPointsRes] = await Promise.all([
          axios.get("https://localhost:7160/api/Shifts/list"),
          axios.get("https://localhost:7160/api/NodalPoint/get-nodal-points"),
        ]);
        setShifts(shiftsRes.data);
        setNodalPoints(nodalPointsRes.data);
      } catch (error) {
        console.error("Error fetching data:", error);
        setConfirmationMessage("Failed to load shifts or nodal points.");
      }
    };

    fetchData();
  }, []);

  // Date filtering to enable Monday to Friday, disable Saturday and Sunday
  const filterAvailableDates = (date) => {
    return !isSaturday(date) && !isSunday(date) && date >= nextWeekStart && date <= nextWeekEnd;
  };

  const handleDateChange = (date) => {
    console.log('Selected Start Date:', format(date, 'yyyy-MM-dd')); // Log the start date
    setStartDate(date);
    if (endDate && date >= endDate) {
      setEndDate(null);
    }
  };

  const handleEndDateChange = (date) => {
    console.log('Selected End Date:', format(date, 'yyyy-MM-dd')); // Log the end date
    if (startDate && date >= startDate) {
      setEndDate(date);
    }
  };

  const handleShiftSelect = (shift) => {
    console.log('Selected Shift:', shift.shiftTime); // Log the selected shift
    setSelectedShift(shift);
  };

  const handleNodalPointSelect = (e) => {
    console.log('Selected Nodal Point:', e.target.value); // Log the selected nodal point
    setSelectedNodalPoint(e.target.value);
  };

  const handleBooking = async () => {
    const userId = sessionStorage.getItem("userId") || localStorage.getItem("userId");

    if (!userId) {
      setConfirmationMessage("Please log in to book a cab.");
      return;
    }

    if (!startDate || !endDate || !selectedShift || !selectedNodalPoint) {
      setConfirmationMessage("Please fill in all the required fields.");
      return;
    }

    if (startDate >= endDate) {
      setConfirmationMessage("End Date must be after Start Date.");
      return;
    }

    const userTimezone = Intl.DateTimeFormat().resolvedOptions().timeZone;
    const startDateInUserTimezone = toZonedTime(startDate, userTimezone);
    const endDateInUserTimezone = toZonedTime(endDate, userTimezone);
    const startDateUtc = fromZonedTime(startDateInUserTimezone, 'UTC');
    const endDateUtc = fromZonedTime(endDateInUserTimezone, 'UTC');

    const payload = {
      userId,
      startDate: startDateUtc.toISOString(),
      endDate: endDateUtc.toISOString(),
      shiftId: selectedShift.id,
      nodalPointId: parseInt(selectedNodalPoint, 10),
    };

    try {
      const response = await axios.post("https://localhost:7160/api/CabBooking/book-date-range", payload);

      // Format date without time part (removes 00:00:00)
      const formattedStartDate = format(startDateInUserTimezone, 'yyyy-MM-dd');
      const formattedEndDate = format(endDateInUserTimezone, 'yyyy-MM-dd');

      const details = {
        userId,
        startDate: formattedStartDate,
        endDate: formattedEndDate,
        shift: selectedShift.shiftTime,
        nodalPoint: nodalPoints.find((point) => point.id === parseInt(selectedNodalPoint, 10)).locationName
      };
      setBookingDetails(details);
      setConfirmationMessage(response.data.message);
    } catch (error) {
      setConfirmationMessage("Booking failed. Please try again.");
      console.error("Error during booking:", error);
    }
  };

  const handleLogout = () => {
    sessionStorage.removeItem("userId");
    localStorage.removeItem("userId");
    navigate("/");
  };

  return (
    <div className="dashboard-container">
      <h2>Book Your Cab for Next Week</h2>

      {/* Logout Button */}
      <button onClick={handleLogout} className="logout-btn">Logout</button>

      <div className="calendar-range">
        <div>
          <label>Start Date</label>
          <DatePicker
            selected={startDate}
            onChange={handleDateChange}
            minDate={nextWeekStart}
            maxDate={nextWeekEnd}
            filterDate={filterAvailableDates}
            selectsStart
            inline
          />
          {startDate && <p>Start Date Selected: {format(startDate, 'yyyy-MM-dd')}</p>}
        </div>
        <div>
          <label>End Date</label>
          <DatePicker
            selected={endDate}
            onChange={handleEndDateChange}
            minDate={startDate || nextWeekStart}
            maxDate={nextWeekEnd}
            filterDate={filterAvailableDates}
            selectsEnd
            inline
          />
          {endDate && <p>End Date Selected: {format(endDate, 'yyyy-MM-dd')}</p>}
        </div>
      </div>

      <div className="shifts-container">
        <h3>Available Shifts</h3>
        <div className="shifts-list">
          {shifts.map((shift) => (
            <button
              key={shift.id}
              className={`shift-card ${selectedShift?.id === shift.id ? "selected" : ""}`}
              onClick={() => handleShiftSelect(shift)}
            >
              {shift.shiftTime}
            </button>
          ))}
        </div>
      </div>

      <div className="nodal-point-container">
        <label>Select Nodal Point</label>
        <select onChange={handleNodalPointSelect} className="dropdown">
          <option value="">Choose Nodal Point</option>
          {nodalPoints.map((point) => (
            <option key={point.id} value={point.id}>
              {point.locationName}
            </option>
          ))}
        </select>
      </div>

      <button
        onClick={handleBooking}
        className="book-btn"
        disabled={!startDate || !endDate || !selectedShift || !selectedNodalPoint}
      >
        Book Cab
      </button>

      {/* Displaying Confirmation Message */}
      {confirmationMessage && <div className="confirmation-message">{confirmationMessage}</div>}

      {bookingDetails && (
        <div className="booking-details">
          <h3>Booking Details:</h3>
          <p><strong>Start Date:</strong> {bookingDetails.startDate}</p>
          <p><strong>End Date:</strong> {bookingDetails.endDate}</p>
          <p><strong>Shift:</strong> {bookingDetails.shift}</p>
          <p><strong>Nodal Point:</strong> {bookingDetails.nodalPoint}</p>
        </div>
      )}
    </div>
  );
};

export default UserDashboard;
