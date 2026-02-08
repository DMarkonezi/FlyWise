import { Routes, Route, Navigate } from 'react-router-dom';
import LoginPage from './features/auth/pages/LoginPage';
import HomePage from './features/home/pages/HomePage';
import SearchFlights from './features/search/pages/SearchFlights';
import MyTickets from './features/reservations/pages/MyTickets';
import BookingPage from './features/booking/pages/BookingPage';
import RegisterPage from './features/auth/pages/RegisterPage';

function App() {
  return (
    <Routes>
      {/* Auth */}
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />

      {/* Home + protected pages */}
      <Route path="/home" element={<HomePage />} />
      <Route path="/search" element={<SearchFlights />} />
      <Route path="/reservations" element={<MyTickets />} />
      <Route path="/booking/:routeId" element={<BookingPage />} />

      {/* default redirect */}
      <Route path="/" element={<Navigate to="/login" replace />} />
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
}

export default App;
