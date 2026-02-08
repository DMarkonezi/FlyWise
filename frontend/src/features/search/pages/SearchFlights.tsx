import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import styles from './SearchFlights.module.css';

interface Route {
  routeId: number;
  departure: string;
  arrival: string;
  from: string;
  to: string;
}

interface City {
  id: number;
  cityName: string;
  countryName: string;
}

interface Country {
  id: number;
  name: string;
}

const SearchFlights: React.FC = () => {
  const navigate = useNavigate(); // 👈 DODAJEMO OVO
  
  const [searchMode, setSearchMode] = useState<'city' | 'country'>('city');
  const [cities, setCities] = useState<City[]>([]);
  const [countries, setCountries] = useState<Country[]>([]);
  const [results, setResults] = useState<Route[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Form states
  const [fromCity, setFromCity] = useState('');
  const [toCity, setToCity] = useState('');
  const [fromCountry, setFromCountry] = useState('');
  const [toCountry, setToCountry] = useState('');

  const API_BASE = 'http://localhost:5117/api';

  // Fetch cities on component mount
  useEffect(() => {
    fetchCities();
    fetchCountries();
  }, []);

  const fetchCities = async () => {
    try {
      const response = await fetch(`${API_BASE}/City`);
      if (!response.ok) throw new Error('Failed to fetch cities');
      const data = await response.json();
      setCities(data);
    } catch (err) {
      console.error('Error fetching cities:', err);
    }
  };

  const fetchCountries = async () => {
    try {
      const response = await fetch(`${API_BASE}/Country`);
      if (!response.ok) throw new Error('Failed to fetch countries');
      const data = await response.json();
      setCountries(data);
    } catch (err) {
      console.error('Error fetching countries:', err);
    }
  };

  const searchByCity = async () => {
    if (!fromCity || !toCity) {
      setError('Please select both departure and arrival cities');
      return;
    }

    setLoading(true);
    setError(null);
    try {
      const response = await fetch(
        `${API_BASE}/Filter/cities?from=${fromCity}&to=${toCity}`
      );
      if (!response.ok) throw new Error('Failed to search flights');
      const data = await response.json();
      setResults(data);
      if (data.length === 0) {
        setError('No flights found for this route');
      }
    } catch (err) {
      setError('Error searching for flights');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const searchByCountry = async () => {
    if (!fromCountry || !toCountry) {
      setError('Please select both departure and arrival countries');
      return;
    }

    setLoading(true);
    setError(null);
    try {
      const response = await fetch(
        `${API_BASE}/Filter/countries?from=${fromCountry}&to=${toCountry}`
      );
      if (!response.ok) throw new Error('Failed to search flights');
      const data = await response.json();
      setResults(data);
      if (data.length === 0) {
        setError('No flights found for this route');
      }
    } catch (err) {
      setError('Error searching for flights');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = () => {
    if (searchMode === 'city') {
      searchByCity();
    } else {
      searchByCountry();
    }
  };

  // 👇 DODAJEMO OVU FUNKCIJU
  const handleBookFlight = (routeId: number) => {
    navigate(`/booking/${routeId}`);
  };

  const formatDateTime = (dateTimeString: string) => {
    try {
      const date = new Date(dateTimeString);
      return date.toLocaleString('en-US', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
      });
    } catch {
      return dateTimeString;
    }
  };

  const handleSwapLocations = () => {
    if (searchMode === 'city') {
      const temp = fromCity;
      setFromCity(toCity);
      setToCity(temp);
    } else {
      const temp = fromCountry;
      setFromCountry(toCountry);
      setToCountry(temp);
    }
  };

  return (
    <div className={styles.page}>
      <div className={styles.header}>
        <h1>FlyWise - Search Flights</h1>
        <p>Find and book your perfect flight</p>
      </div>

      <div className={styles.container}>
        {/* Search Mode Selector */}
        <div className={styles.modeSelector}>
          <label className={styles.modeOption}>
            <input
              type="radio"
              value="city"
              checked={searchMode === 'city'}
              onChange={(e) => {
                setSearchMode(e.target.value as 'city' | 'country');
                setResults([]);
                setError(null);
              }}
            />
            <span>Search by City</span>
          </label>
          <label className={styles.modeOption}>
            <input
              type="radio"
              value="country"
              checked={searchMode === 'country'}
              onChange={(e) => {
                setSearchMode(e.target.value as 'city' | 'country');
                setResults([]);
                setError(null);
              }}
            />
            <span>Search by Country</span>
          </label>
        </div>

        {/* Search Form */}
        <div className={styles.searchForm}>
          {searchMode === 'city' ? (
            <>
              <div className={styles.formGroup}>
                <label htmlFor="fromCity">Departure City</label>
                <select
                  id="fromCity"
                  value={fromCity}
                  onChange={(e) => setFromCity(e.target.value)}
                  className={styles.select}
                >
                  <option value="">Select a city</option>
                  {cities.map((city) => (
                    <option key={city.id} value={city.id}>
                      {city.cityName}, {city.countryName}
                    </option>
                  ))}
                </select>
              </div>

              <button
                className={styles.swapButton}
                onClick={handleSwapLocations}
                title="Swap locations"
              >
                ⇅
              </button>

              <div className={styles.formGroup}>
                <label htmlFor="toCity">Arrival City</label>
                <select
                  id="toCity"
                  value={toCity}
                  onChange={(e) => setToCity(e.target.value)}
                  className={styles.select}
                >
                  <option value="">Select a city</option>
                  {cities.map((city) => (
                    <option key={city.id} value={city.id}>
                      {city.cityName}, {city.countryName}
                    </option>
                  ))}
                </select>
              </div>
            </>
          ) : (
            <>
              <div className={styles.formGroup}>
                <label htmlFor="fromCountry">Departure Country</label>
                <select
                  id="fromCountry"
                  value={fromCountry}
                  onChange={(e) => setFromCountry(e.target.value)}
                  className={styles.select}
                >
                  <option value="">Select a country</option>
                  {countries.map((country) => (
                    <option key={country.id} value={country.id}>
                      {country.name}
                    </option>
                  ))}
                </select>
              </div>

              <button
                className={styles.swapButton}
                onClick={handleSwapLocations}
                title="Swap locations"
              >
                ⇅
              </button>

              <div className={styles.formGroup}>
                <label htmlFor="toCountry">Arrival Country</label>
                <select
                  id="toCountry"
                  value={toCountry}
                  onChange={(e) => setToCountry(e.target.value)}
                  className={styles.select}
                >
                  <option value="">Select a country</option>
                  {countries.map((country) => (
                    <option key={country.id} value={country.id}>
                      {country.name}
                    </option>
                  ))}
                </select>
              </div>
            </>
          )}

          <button
            className={styles.searchButton}
            onClick={handleSearch}
            disabled={loading}
          >
            {loading ? 'Searching...' : 'Search Flights'}
          </button>
        </div>

        {/* Error Message */}
        {error && <div className={styles.error}>{error}</div>}

        {/* Results */}
        <div className={styles.results}>
          {results.length > 0 && (
            <>
              <h2>Available Flights ({results.length})</h2>
              <div className={styles.flightsList}>
                {results.map((route) => (
                  <div key={route.routeId} className={styles.flightCard}>
                    <div className={styles.routeInfo}>
                      <div className={styles.locations}>
                        <div className={styles.location}>
                          <h3>{route.from}</h3>
                          <p className={styles.time}>
                            {formatDateTime(route.departure)}
                          </p>
                        </div>
                        <div className={styles.arrow}>→</div>
                        <div className={styles.location}>
                          <h3>{route.to}</h3>
                          <p className={styles.time}>
                            {formatDateTime(route.arrival)}
                          </p>
                        </div>
                      </div>
                    </div>
                    <div className={styles.duration}>
                      <p>Route ID: {route.routeId}</p>
                    </div>
                    {/* 👇 MENJAMO DUGME */}
                    <button 
                      className={styles.bookButton}
                      onClick={() => handleBookFlight(route.routeId)}
                    >
                      📖 Rezerviši Let
                    </button>
                  </div>
                ))}
              </div>
            </>
          )}
        </div>
      </div>
    </div>
  );
};

export default SearchFlights;