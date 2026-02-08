// authSlice.ts
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';
import authService from '../services/authService';
import type { RootState } from '../../../store/store';

/* =======================
   TYPES
======================= */

export interface User {
  id: string;
  email: string;
  name?: string;
}

interface AuthResponse {
  user: User;
  token: string;
}

interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  loading: boolean;
  error: string | null;
}

/* =======================
   INITIAL STATE
======================= */

const initialState: AuthState = {
  user: null,
  token: null,
  isAuthenticated: false,
  loading: false,
  error: null,
};

/* =======================
   ASYNC THUNKS
======================= */

// LOGIN
export const loginUser = createAsyncThunk<
  AuthResponse,
  { email: string; password: string },
  { rejectValue: string }
>('auth/loginUser', async ({ email, password }, { rejectWithValue }) => {
  try {
    const response = await authService.login(email, password);

    if (!response.user || !response.token) {
      return rejectWithValue('Invalid login response');
    }

    return {
      user: response.user,
      token: response.token,
    };
  } catch (err: any) {
    return rejectWithValue(err?.message ?? 'Login failed');
  }
});

// REGISTER
export interface RegisterPayload {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  passportNumber: string;
}

export const registerUser = createAsyncThunk<
  AuthResponse,
  RegisterPayload,
  { rejectValue: string }
>('auth/registerUser', async (payload, { rejectWithValue }) => {
  try {
    const response = await authService.register(
      payload.firstName,
      payload.lastName,
      payload.email,
      payload.password,
      payload.passportNumber
    );

    if (!response.user || !response.token) {
      return rejectWithValue('Invalid registration response');
    }

    return {
      user: response.user,
      token: response.token,
    };
  } catch (err: any) {
    return rejectWithValue(err?.message ?? 'Registration failed');
  }
});

// RESTORE AUTH
export const restoreAuth = createAsyncThunk<
  AuthResponse,
  void,
  { rejectValue: string }
>('auth/restoreAuth', async (_, { rejectWithValue }) => {
  try {
    if (!authService.isAuthenticated()) {
      return rejectWithValue('Not authenticated');
    }

    const user = authService.getCurrentUser();
    const token = authService.getToken();

    if (!user || !token) {
      return rejectWithValue('Invalid auth data');
    }

    return { user, token };
  } catch {
    return rejectWithValue('Restore failed');
  }
});

/* =======================
   SLICE
======================= */

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    logout(state) {
      state.user = null;
      state.token = null;
      state.isAuthenticated = false;
      state.error = null;
      authService.logout();
    },
    clearError(state) {
      state.error = null;
    },
    updateUser(state, action: PayloadAction<Partial<User>>) {
      if (state.user) {
        state.user = { ...state.user, ...action.payload };
      }
    },
  },
  extraReducers: (builder) => {
    builder
      // LOGIN
      .addCase(loginUser.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(loginUser.fulfilled, (state, action) => {
        state.loading = false;
        state.user = action.payload.user;
        state.token = action.payload.token;
        state.isAuthenticated = true;
      })
      .addCase(loginUser.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload ?? 'Login failed';
        state.isAuthenticated = false;
      })

      // REGISTER
      .addCase(registerUser.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(registerUser.fulfilled, (state, action) => {
        state.loading = false;
        state.user = action.payload.user;
        state.token = action.payload.token;
        state.isAuthenticated = true;
      })
      .addCase(registerUser.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload ?? 'Registration failed';
        state.isAuthenticated = false;
      })

      // RESTORE AUTH
      .addCase(restoreAuth.pending, (state) => {
        state.loading = true;
      })
      .addCase(restoreAuth.fulfilled, (state, action) => {
        state.loading = false;
        state.user = action.payload.user;
        state.token = action.payload.token;
        state.isAuthenticated = true;
      })
      .addCase(restoreAuth.rejected, (state) => {
        state.loading = false;
        state.isAuthenticated = false;
      });
  },
});

/* =======================
   EXPORTS
======================= */

export const { logout, clearError, updateUser } = authSlice.actions;

// Selectors
export const selectUser = (state: RootState) => state.auth.user;
export const selectIsAuthenticated = (state: RootState) =>
  state.auth.isAuthenticated;
export const selectAuthLoading = (state: RootState) => state.auth.loading;
export const selectAuthError = (state: RootState) => state.auth.error;

export default authSlice.reducer;
