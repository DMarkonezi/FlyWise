import React from 'react';
import { useNavigate } from 'react-router-dom';
import Header from '../../../shared/components/Header';
import styles from './HomePage.module.css';

const HomePage: React.FC = () => {
  const navigate = useNavigate();

  return (
    <div className={styles.homePage}>
      {/* Header */}
      <Header appName="FlyWise" />

      {/* Sadržaj */}
      <div className={styles.homeContainer}>
        <h1 className={styles.title}>What are you going next?</h1>

        <div className={styles.cardsGrid}>

          {/* Pretraga Letova */}
          <div className={styles.card} onClick={() => navigate('/search')}>
            <div className={styles.cardIcon}>🔍</div>
            <h2 className={styles.cardTitle}>Search Flights</h2>
            <p className={styles.cardDesc}>Find a way to your favourite destinations</p>
          </div>

          {/* Moje Rezervacije */}
          <div className={styles.card} onClick={() => navigate('/reservations')}>
            <div className={styles.cardIcon}>🎫</div>
            <h2 className={styles.cardTitle}>My Reservations</h2>
            <p className={styles.cardDesc}>Have a look on all your flights</p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default HomePage;
