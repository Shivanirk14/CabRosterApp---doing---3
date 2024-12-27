import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Home from './Components/Home';
import Login from './Components/Login';
import Registration from './Components/Registration';
import UserDashboard from './Components/UserDashboard';
import AdminDashboard from './Components/AdminDashboard'; // Add AdminDashboard import
import CabBookingPage from './Components/CabBookingPage';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/login" element={<Login />} />
        <Route path="/registeration" element={<Registration />} />
        <Route path="/UserDashboard" element={<UserDashboard />} />
        <Route path="/AdminDashboard" element={<AdminDashboard />} /> {/* Add AdminDashboard route */}
        <Route path="/CabBooking" element={<CabBookingPage />} />
      </Routes>
    </Router>
  );
}

export default App;
