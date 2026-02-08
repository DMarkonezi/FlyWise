import { useDispatch, useSelector } from 'react-redux';
import { useCallback } from 'react';
import {
  logout,
  selectUser,
  selectIsAuthenticated,
  selectAuthLoading,
  selectAuthError,
  clearError,
} from '../store/authSlice.ts';

/**
 * Custom hook for accessing auth state and actions
 * Abstracts Redux complexity from components
 */
export function useAuth() {
  const dispatch = useDispatch();
  const user = useSelector(selectUser);
  const isAuthenticated = useSelector(selectIsAuthenticated);
  const loading = useSelector(selectAuthLoading);
  const error = useSelector(selectAuthError);

  const handleLogout = useCallback(() => {
    dispatch(logout());
  }, [dispatch]);

  const handleClearError = useCallback(() => {
    dispatch(clearError());
  }, [dispatch]);

  return {
    user,
    isAuthenticated,
    loading,
    error,
    logout: handleLogout,
    clearError: handleClearError,
  };
}

export default useAuth;