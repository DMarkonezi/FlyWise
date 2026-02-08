// import apiClient from '../../../services/api';

// interface LoginResponse {
//   token?: string;
//   user?: any;
//   expiresIn?: number;
// }

// interface RegisterResponse {
//   token?: string;
//   user?: any;
// }

// // Koristimo tipove za parametre
// const authService = {
//   login: async (email: string, password: string): Promise<LoginResponse> => {
//     try {
//       const response = await apiClient.post<LoginResponse>('/auth/login', {
//         email,
//         password,
//       });

//       if (response.data.token) {
//         localStorage.setItem('authToken', response.data.token);
//         localStorage.setItem('user', JSON.stringify(response.data.user));
//         if (response.data.expiresIn) {
//           const expiryTime = new Date().getTime() + response.data.expiresIn * 1000;
//           localStorage.setItem('tokenExpiry', expiryTime.toString()); // broj → string
//         }
//       }

//       return response.data;
//     } catch (error: unknown) {
//       console.error('Login failed:', error);
//       // @ts-ignore: TypeScript ne zna tip error.response
//       throw (error as any)?.response?.data || (error as Error).message;
//     }
//   },

//   register: async (email: string, password: string, name: string): Promise<RegisterResponse> => {
//     try {
//       const response = await apiClient.post<RegisterResponse>('/auth/register', {
//         email,
//         password,
//         name,
//       });

//       if (response.data.token) {
//         localStorage.setItem('authToken', response.data.token);
//         localStorage.setItem('user', JSON.stringify(response.data.user));
//       }

//       return response.data;
//     } catch (error: unknown) {
//       console.error('Registration failed:', error);
//       throw (error as any)?.response?.data || (error as Error).message;
//     }
//   },

//   logout: () => {
//     localStorage.removeItem('authToken');
//     localStorage.removeItem('user');
//     localStorage.removeItem('tokenExpiry');
//   },

//   getCurrentUser: () => {
//     const userStr = localStorage.getItem('user');
//     return userStr ? JSON.parse(userStr) : null;
//   },

//   isAuthenticated: () => {
//     const token = localStorage.getItem('authToken');
//     if (!token) return false;

//     const expiry = localStorage.getItem('tokenExpiry');
//     if (expiry && new Date().getTime() > parseInt(expiry)) {
//       authService.logout();
//       return false;
//     }

//     return true;
//   },

//   getToken: () => localStorage.getItem('authToken'),

//   refreshToken: async (): Promise<string | undefined> => {
//     try {
//       const response = await apiClient.post<{ token: string }>('/auth/refresh-token');
//       if (response.data.token) {
//         localStorage.setItem('authToken', response.data.token);
//         return response.data.token;
//       }
//     } catch (error: unknown) {
//       console.error('Token refresh failed:', error);
//       authService.logout();
//       throw error;
//     }
//   },

//   validateToken: async (): Promise<boolean> => {
//     try {
//       const response = await apiClient.get<{ valid: boolean }>('/auth/validate-token');
//       return response.data.valid;
//     } catch {
//       return false;
//     }
//   },

//   getProfile: async (): Promise<any> => {
//     try {
//       const response = await apiClient.get('/users/profile');
//       localStorage.setItem('user', JSON.stringify(response.data));
//       return response.data;
//     } catch (error: unknown) {
//       console.error('Failed to get profile:', error);
//       throw error;
//     }
//   },
// };

// export default authService;

import apiClient from '../../../services/api';

interface LoginResponse {
  token?: string;
  user?: any;
  expiresIn?: number;
}

interface RegisterResponse {
  token?: string;
  user?: any;
  id?: string;
  firstName?: string;
  lastName?: string;
  email?: string;
  passportNumber?: string;
}

/**
 * Authentication service for API communication
 */
const authService = {
  /**
   * Login user
   */
  login: async (email: string, password: string): Promise<LoginResponse> => {
    try {
      const response = await apiClient.post<LoginResponse>('/auth/login', {
        email,
        password,
      });

      if (response.data.token) {
        localStorage.setItem('authToken', response.data.token);
        localStorage.setItem('user', JSON.stringify(response.data.user));
        if (response.data.expiresIn) {
          const expiryTime = new Date().getTime() + response.data.expiresIn * 1000;
          localStorage.setItem('tokenExpiry', expiryTime.toString());
        }
      }

      return response.data;
    } catch (error: unknown) {
      console.error('Login failed:', error);
      throw (error as any)?.response?.data || (error as Error).message;
    }
  },

  /**
   * Register new user
   */
  register: async (
    firstName: string,
    lastName: string,
    email: string,
    password: string,
    passportNumber: string
  ): Promise<RegisterResponse> => {
    if (!firstName || !lastName || !email || !password || !passportNumber) {
      throw new Error('All fields are required for registration');
    }

    try {
      console.log("Sending registration payload:", {
        FirstName: firstName,
        LastName: lastName,
        Email: email,
        Password: password,
        PassportNumber: passportNumber
      });

      const response = await apiClient.post<RegisterResponse>('/user/register', {
        FirstName: firstName,
        LastName: lastName,
        Email: email,
        Password: password,
        PassportNumber: passportNumber
      });

      if (response.data.token) {
        localStorage.setItem('authToken', response.data.token);
        localStorage.setItem('user', JSON.stringify(response.data.user));
      }

      return response.data;
    } catch (error: unknown) {
      console.error('Registration failed:', error);
      throw (error as any)?.response?.data || (error as Error).message;
    }
  },

  // ...ostale metode (login, logout, getCurrentUser, itd.)
  /**
   * Logout user
   */
  logout: () => {
    localStorage.removeItem('authToken');
    localStorage.removeItem('user');
    localStorage.removeItem('tokenExpiry');
  },

  /**
   * Get current user from localStorage
   */
  getCurrentUser: () => {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  },

  /**
   * Check if user is authenticated
   */
  isAuthenticated: () => {
    const token = localStorage.getItem('authToken');
    if (!token) return false;

    const expiry = localStorage.getItem('tokenExpiry');
    if (expiry && new Date().getTime() > parseInt(expiry)) {
      authService.logout();
      return false;
    }

    return true;
  },

  /**
   * Get auth token
   */
  getToken: () => localStorage.getItem('authToken'),

  /**
   * Refresh auth token
   */
  refreshToken: async (): Promise<string | undefined> => {
    try {
      const response = await apiClient.post<{ token: string }>('/user/refresh-token');
      if (response.data.token) {
        localStorage.setItem('authToken', response.data.token);
        return response.data.token;
      }
    } catch (error: unknown) {
      console.error('Token refresh failed:', error);
      authService.logout();
      throw error;
    }
  },

  /**
   * Validate token
   */
  validateToken: async (): Promise<boolean> => {
    try {
      const response = await apiClient.get<{ valid: boolean }>('/user/validate-token');
      return response.data.valid;
    } catch {
      return false;
    }
  },

  /**
   * Get user profile
   */
  getProfile: async (): Promise<any> => {
    try {
      const response = await apiClient.get('/user/profile');
      localStorage.setItem('user', JSON.stringify(response.data));
      return response.data;
    } catch (error: unknown) {
      console.error('Failed to get profile:', error);
      throw error;
    }
  },
};

export default authService;
