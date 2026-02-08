import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import RegisterForm from '../components/RegisterForm'
import styles from './RegisterPage.module.css';

/**
 * Full registration page with layout and branding
 */
const RegisterPage: React.FC = () => {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();

  // Redirect if already logged in
  useEffect(() => {
    if (isAuthenticated) {
      navigate('/home', { replace: true });
    }
  }, [isAuthenticated, navigate]);

  const handleRegisterSuccess = () => {
    navigate('/login', { replace: true });
  };

  return (
    <div className={styles.registerPage}>
      {/* Left Side - Branding */}
      <div className={styles.registerBranding}>
        <div className={styles.brandingContent}>
          <h1 className={styles.brandTitle}>FlyWise</h1>
          <p className={styles.brandSubtitle}>
            Start your journey to discover amazing flights
          </p>
          <div className={styles.benefitsList}>
            <div className={styles.benefit}>✈️ Easy Booking</div>
            <div className={styles.benefit}>🔒 Secure & Safe</div>
            <div className={styles.benefit}>💰 Best Prices</div>
          </div>
        </div>
      </div>

      {/* Right Side - Form */}
      <div className={styles.registerFormContainer}>
        <div className={styles.formWrapper}>
          <RegisterForm onSuccess={handleRegisterSuccess} />
        </div>
      </div>
    </div>
  );
};

export default RegisterPage;