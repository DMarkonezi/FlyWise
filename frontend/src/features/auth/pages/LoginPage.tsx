import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import LoginForm from '../components/LoginForm';
import styles from './LoginPage.module.css';

/**
 * Full login page with layout and branding
 */
const LoginPage: React.FC = () => {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();

  // Redirect if already logged in
  useEffect(() => {
    if (isAuthenticated) {
      navigate('/home', { replace: true });
    }
  }, [isAuthenticated, navigate]);

  const handleLoginSuccess = () => {
    navigate('/home');
  };

  return (
    <div className={styles.loginPage}>
      {/* Left Side - Branding */}
      <div className={styles.loginBranding}>
        <div className={styles.brandingContent}>
          <h1 className={styles.brandTitle}>Flight Finder</h1>
          <p className={styles.brandSubtitle}>
            Find your perfect flight with AI-powered recommendations
          </p>
          <div className={styles.featuresList}>
            <div className={styles.feature}>✈️ Smart Search</div>
            <div className={styles.feature}>⭐ Personalized Picks</div>
            <div className={styles.feature}>💰 Best Prices</div>
          </div>
        </div>
      </div>

      {/* Right Side - Form */}
      <div className={styles.loginFormContainer}>
        <div className={styles.formWrapper}>
          <LoginForm onSuccess={handleLoginSuccess} />
        </div>
      </div>
    </div>
  );
};

export default LoginPage;
