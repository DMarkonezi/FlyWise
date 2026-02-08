// import axios from 'axios';
// import store from '../store/store';
// import { logout } from '../features/auth/store/authSlice';

// const API_BASE = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

// /**
//  * Create axios instance
//  */
// const apiClient = axios.create({
//   baseURL: API_BASE,
//   timeout: 10000,
//   headers: {
//     'Content-Type': 'application/json',
//   },
// });

// /**
//  * Request interceptor: Add auth token
//  */
// apiClient.interceptors.request.use(
//   (config) => {
//     const token = localStorage.getItem('authToken');
//     if (token) {
//       config.headers.Authorization = `Bearer ${token}`;
//     }
//     return config;
//   },
//   (error) => Promise.reject(error)
// );

// /**
//  * Response interceptor: Handle errors globally
//  */
// apiClient.interceptors.response.use(
//   (response) => response,
//   (error) => {
//     // 401 Unauthorized - logout user
//     if (error.response?.status === 401) {
//       store.dispatch(logout());
//       window.location.href = '/login';
//     }

//     // 429 Rate Limited
//     if (error.response?.status === 429) {
//       console.warn('Rate limited. Please try again later.');
//     }

//     // 500+ Server Error
//     if (error.response?.status >= 500) {
//       console.error('Server error:', error.response.data);
//     }

//     return Promise.reject(error);
//   }
// );

// export default apiClient;

import axios from 'axios';

const API_BASE = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

const apiClient = axios.create({
  baseURL: API_BASE,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor: Add auth token
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor: samo prosledi grešku
apiClient.interceptors.response.use(
  (response) => response,
  (error) => Promise.reject(error)
);

export default apiClient;
