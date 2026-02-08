import React from 'react';
import { useNavigate } from 'react-router-dom';
import styles from './Header.module.css';

interface HeaderProps {
  appName: string;
}

const Header: React.FC<HeaderProps> = ({ appName }) => {
  const navigate = useNavigate();

  return (
    <header className={styles.header}>
      <div className={styles.left}>{appName}</div>
      <button
        className={styles.profileButton}
        onClick={() => navigate('/profile')}
      >
        Profile
      </button>
    </header>
  );
};

export default Header;
