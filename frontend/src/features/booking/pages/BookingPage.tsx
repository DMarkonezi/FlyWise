import React, { useState, useEffect } from 'react';
import Header from '../../../shared/components/Header';
import styles from './BookingPage.module.css';
import { useParams } from 'react-router-dom';

// Fixed prices for all flights
const FIXED_PRICES = {
  SEAT_PRICE: 150, // RSD
  SUITCASE_PRICE_PER_KG: 5, // RSD per kg
  MAX_SUITCASES: 3,
  SUITCASE_WEIGHTS: [20, 15, 10], // kg per suitcase
};

interface RouteData {
  routeId: number;
  from: string;
  to: string;
  departure: string;
  arrival: string;
}

interface PassengerForm {
  firstName: string;
  lastName: string;
  email: string;
  passportNumber: string;
  phoneNumber: string;
}

interface BookingState {
  selectedRoute: RouteData | null;
  passenger: PassengerForm;
  seat: string;
  suitcases: {
    count: number;
    weights: number[];
  };
  loading: boolean;
  error: string | null;
  success: string | null;
}

const BookingPage: React.FC = () => {
  const { routeId } = useParams<{ routeId: string }>();
  const numericRouteId = routeId ? Number(routeId) : null;
  const [state, setState] = useState<BookingState>({
    selectedRoute: null,
    passenger: {
      firstName: '',
      lastName: '',
      email: '',
      passportNumber: '',
      phoneNumber: '',
    },
    seat: '1A',
    suitcases: {
      count: 0,
      weights: [0, 0, 0],
    },
    loading: false,
    error: null,
    success: null,
  });

  const API_BASE = import.meta.env.VITE_API_URL;

  // Fetch only route data, without passengers
  useEffect(() => {
    if (numericRouteId) {
      fetchRoute(numericRouteId);
    }
  }, [numericRouteId]);

  const fetchRoute = async (id: number) => {
    setState((prev) => ({ ...prev, loading: true }));
    try {
      const response = await fetch(`${API_BASE}/Route/detailed/${id}`);
      if (!response.ok) throw new Error('Nije moguće učitati rutu');
      const data = await response.json();

      setState((prev) => ({
        ...prev,
        selectedRoute: {
          routeId: data.routeId || id,
          from: data.fromCity || 'Unknown',
          to: data.toCity || 'Unknown',
          departure: data.departure || new Date().toISOString(),
          arrival: data.arrival || new Date().toISOString(),
        },
        loading: false,
      }));
    } catch (error) {
      setState((prev) => ({
        ...prev,
        error: 'Greška pri učitavanju rute: ' + (error instanceof Error ? error.message : 'Unknown error'),
        loading: false,
      }));
    }
  };

  // Passenger form handlers
  const handlePassengerChange = (field: keyof PassengerForm, value: string) => {
    setState((prev) => ({
      ...prev,
      passenger: { ...prev.passenger, [field]: value },
    }));
  };

  // Seat selection
  const handleSeatChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setState((prev) => ({ ...prev, seat: e.target.value }));
  };

  // Suitcase handlers
  const handleSuitcaseCountChange = (count: number) => {
    const newWeights = [...state.suitcases.weights];
    if (count < state.suitcases.count) {
      newWeights.splice(count);
    } else {
      for (let i = state.suitcases.count; i < count; i++) {
        newWeights[i] = FIXED_PRICES.SUITCASE_WEIGHTS[i] || 20;
      }
    }
    setState((prev) => ({ ...prev, suitcases: { count, weights: newWeights } }));
  };

  const handleWeightChange = (index: number, weight: number) => {
    setState((prev) => ({
      ...prev,
      suitcases: {
        ...prev.suitcases,
        weights: prev.suitcases.weights.map((w, i) => (i === index ? weight : w)),
      },
    }));
  };

  // Price calculation
  const calculatePrice = () => {
    const seatCost = FIXED_PRICES.SEAT_PRICE;
    const suitcaseCost = state.suitcases.weights
      .slice(0, state.suitcases.count)
      .reduce((sum, weight) => sum + weight * FIXED_PRICES.SUITCASE_PRICE_PER_KG, 0);
    return { seat: seatCost, suitcase: suitcaseCost, total: seatCost + suitcaseCost };
  };

  // Form validation
  const validateForm = (): boolean => {
    const { firstName, lastName, email, passportNumber, phoneNumber } = state.passenger;

    if (!firstName.trim()) {
      setState((prev) => ({ ...prev, error: 'Unesite ime' }));
      return false;
    }

    if (!lastName.trim()) {
      setState((prev) => ({ ...prev, error: 'Unesite prezime' }));
      return false;
    }

    if (!email.includes('@')) {
      setState((prev) => ({ ...prev, error: 'Unesite validnu email adresu' }));
      return false;
    }

    if (!passportNumber.trim()) {
      setState((prev) => ({ ...prev, error: 'Unesite broj pasoša' }));
      return false;
    }

    if (!phoneNumber.trim()) {
      setState((prev) => ({ ...prev, error: 'Unesite telefonski broj' }));
      return false;
    }

    if (!state.selectedRoute) {
      setState((prev) => ({ ...prev, error: 'Odaberite rutu' }));
      return false;
    }

    if (!state.seat) {
      setState((prev) => ({ ...prev, error: 'Odaberite sedište' }));
      return false;
    }

    return true;
  };

  // Submit booking
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    setState((prev) => ({ ...prev, error: null, success: null }));

    if (!validateForm()) return;

    setState((prev) => ({ ...prev, loading: true }));

    try {
      const prices = calculatePrice();

      const response = await fetch(`${API_BASE}/Booking/book-flight`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          firstName: state.passenger.firstName,
          lastName: state.passenger.lastName,
          email: state.passenger.email,
          passportNumber: state.passenger.passportNumber,
          phoneNumber: state.passenger.phoneNumber,
          routeId: state.selectedRoute!.routeId,
          seat: state.seat,
          suitcases: state.suitcases.weights.slice(0, state.suitcases.count),
          seatPrice: prices.seat,
          suitcasePrice: prices.suitcase,
          totalPrice: prices.total,
        }),
      });

      if (!response.ok) {
        const text = await response.text();
        throw new Error(text || 'Greška pri kreiranju rezervacije');
      }

      const data = await response.json();

      setState((prev) => ({
        ...prev,
        success: `✅ Rezervacija uspešna! Ticket ID: ${data.ticketId}`,
        loading: false,
      }));

      setTimeout(() => {
        setState({
          selectedRoute: null,
          passenger: { firstName: '', lastName: '', email: '', passportNumber: '', phoneNumber: '' },
          seat: '1A',
          suitcases: { count: 0, weights: [0, 0, 0] },
          loading: false,
          error: null,
          success: null,
        });
      }, 2000);

    } catch (error) {
      setState((prev) => ({
        ...prev,
        error: `❌ ${error instanceof Error ? error.message : 'Greška pri kreiranju rezervacije'}`,
        loading: false,
      }));
    }
  };

  const prices = calculatePrice();
  const route = state.selectedRoute;

  return (
    <div className={styles.page}>
      <Header appName="FlyWise" />
      <div className={styles.container}>
        <div className={styles.header}>
          <h1>Rezerviši Let</h1>
          <p>Popuni formu da rezervišeš tvoj let</p>
        </div>

        {state.error && <div className={styles.errorMessage}>{state.error}</div>}
        {state.success && <div className={styles.successMessage}>{state.success}</div>}

        <div className={styles.contentWrapper}>
          {/* Route Info */}
          <div className={styles.routeSection}>
            <div className={styles.sectionTitle}>✈️ Odabrana Ruta</div>
            {route ? (
              <div className={styles.routeCard}>
                <div className={styles.route}>
                  <div className={styles.city}>
                    <div className={styles.cityCode}>{route.from.substring(0, 3).toUpperCase()}</div>
                    <div className={styles.cityName}>{route.from}</div>
                  </div>
                  <div className={styles.arrow}>→</div>
                  <div className={styles.city}>
                    <div className={styles.cityCode}>{route.to.substring(0, 3).toUpperCase()}</div>
                    <div className={styles.cityName}>{route.to}</div>
                  </div>
                </div>
                <div className={styles.routeDetails}>
                  <div className={styles.detailRow}>
                    <span className={styles.label}>Polazak:</span>
                    <span className={styles.value}>{new Date(route.departure).toLocaleDateString('sr-RS')}</span>
                  </div>
                  <div className={styles.detailRow}>
                    <span className={styles.label}>Vreme:</span>
                    <span className={styles.value}>{new Date(route.departure).toLocaleTimeString('sr-RS')}</span>
                  </div>
                </div>
              </div>
            ) : (
              <div className={styles.noRoute}>
                <p>{state.loading ? '⏳ Učitavanje rute...' : '❌ Ruta nije učitana. Odaberite rutu iz pretrage.'}</p>
              </div>
            )}
          </div>

          {/* Booking Form */}
          <div className={styles.formSection}>
            <form onSubmit={handleSubmit} className={styles.form}>
              {/* Passenger Info */}
              <fieldset className={styles.fieldset}>
                <legend className={styles.legend}>👤 Podaci Putnika</legend>
                {['firstName','lastName','email','passportNumber','phoneNumber'].map((field) => (
                  <div key={field} className={styles.formGroup}>
                    <label htmlFor={field}>{field === 'firstName' ? 'Ime *' :
                                               field === 'lastName' ? 'Prezime *' :
                                               field === 'email' ? 'Email *' :
                                               field === 'passportNumber' ? 'Broj Pasoša *' :
                                               'Telefonski Broj *'}</label>
                    <input
                      id={field}
                      type={field === 'email' ? 'email' : field === 'phoneNumber' ? 'tel' : 'text'}
                      placeholder={`Unesite ${field}`}
                      value={(state.passenger as any)[field]}
                      onChange={(e) => handlePassengerChange(field as keyof PassengerForm, e.target.value)}
                      required
                    />
                  </div>
                ))}
              </fieldset>

              {/* Seat Selection */}
              <fieldset className={styles.fieldset}>
                <legend className={styles.legend}>💺 Odaberite Sedište</legend>
                <div className={styles.seatGrid}>
                  {['1A','1B','1C','2A','2B','2C','3A','3B','3C'].map(seat => (
                    <label key={seat} className={styles.seatLabel}>
                      <input type="radio" name="seat" value={seat} checked={state.seat === seat} onChange={handleSeatChange} />
                      <span className={styles.seatButton}>{seat}</span>
                    </label>
                  ))}
                </div>
              </fieldset>

              {/* Suitcases */}
              <fieldset className={styles.fieldset}>
                <legend className={styles.legend}>🧳 Prtljag</legend>
                <div className={styles.suitcaseCount}>
                  <label>Broj prtljaga:</label>
                  <div className={styles.countButtons}>
                    {[0,1,2,3].map(count => (
                      <button key={count} type="button" className={`${styles.countBtn} ${state.suitcases.count===count?styles.active:''}`} onClick={()=>handleSuitcaseCountChange(count)}>
                        {count}
                      </button>
                    ))}
                  </div>
                </div>
                {state.suitcases.count>0 && (
                  <div className={styles.suitcaseWeights}>
                    {Array.from({length: state.suitcases.count}).map((_, i)=>(
                      <div key={i} className={styles.weightInput}>
                        <label>Prtljag {i+1} - Težina (kg):</label>
                        <input type="number" min={1} max={50} value={state.suitcases.weights[i] || FIXED_PRICES.SUITCASE_WEIGHTS[i]} onChange={e=>handleWeightChange(i, parseFloat(e.target.value))} />
                        <span className={styles.weightInfo}>{state.suitcases.weights[i]*FIXED_PRICES.SUITCASE_PRICE_PER_KG} RSD</span>
                      </div>
                    ))}
                  </div>
                )}
              </fieldset>

              {/* Price Summary */}
              <div className={styles.priceSummary}>
                <h3>💰 Pregled Cene</h3>
                <div className={styles.priceRow}><span>Sedište:</span><span>{prices.seat} RSD</span></div>
                {state.suitcases.count>0 && <div className={styles.priceRow}><span>Prtljag:</span><span>{prices.suitcase} RSD</span></div>}
                <div className={`${styles.priceRow} ${styles.total}`}><span>UKUPNO:</span><span>{prices.total} RSD</span></div>
              </div>

              <button type="submit" className={styles.submitBtn} disabled={state.loading || !route}>
                {state.loading ? '⏳ Učitavanje...' : '✅ Rezerviši Let'}
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
};

export default BookingPage;
