import { configureStore } from '@reduxjs/toolkit';
import authReducer from '../features/auth/store/authSlice';
// import flightReducer from '../features/flights/store/flightSlice';
// import recommendationReducer from '../features/recommendations/store/recommendationSlice';

export const store = configureStore({
  reducer: {
    auth: authReducer,
    //flights: flightReducer,
    //recommendations: recommendationReducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: {
        ignoredActions: ['auth/loginUser/rejected', 'auth/registerUser/rejected'],
        ignoredPaths: ['auth.error'],
      },
    }),
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

export default store;