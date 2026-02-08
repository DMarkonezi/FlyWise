// import { useState, useCallback } from 'react';
// import { useDispatch } from 'react-redux';
// import { useAppDispatch } from '../hooks/hooks.ts'

// import { loginUser } from '../store/authSlice.ts';

// interface LoginFormData {
//   email: string;
//   password: string;
// }

// interface LoginFormErrors {
//   email?: string;
//   password?: string;
// }

// type UseLoginReturn = {
//   formData: LoginFormData;
//   errors: LoginFormErrors;
//   loading: boolean;
//   generalError: string | null;
//   handleChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
//   handleLogin: (e: React.FormEvent<HTMLFormElement>) => Promise<void>;
//   setGeneralError: React.Dispatch<React.SetStateAction<string | null>>;
// };

// /**
//  * Custom hook for login form logic
//  * Handles form state, validation, and API calls
//  */
// export function useLogin(onSuccess?: (result?: any) => void): UseLoginReturn {
//   const dispatch = useDispatch();
//   const [formData, setFormData] = useState<LoginFormData>({
//     email: '',
//     password: '',
//   });
//   const [errors, setErrors] = useState<LoginFormErrors>({});
//   const [loading, setLoading] = useState(false);
//   const [generalError, setGeneralError] = useState<string | null>(null);

//   /** Validate email format */
//   const validateEmail = (email: string) => {
//     const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
//     return regex.test(email);
//   };

//   /** Validate entire form */
//   const validateForm = useCallback(() => {
//     const newErrors: LoginFormErrors = {};

//     if (!formData.email.trim()) {
//       newErrors.email = 'Email is required';
//     } else if (!validateEmail(formData.email)) {
//       newErrors.email = 'Please enter a valid email';
//     }

//     if (!formData.password) {
//       newErrors.password = 'Password is required';
//     } else if (formData.password.length < 6) {
//       newErrors.password = 'Password must be at least 6 characters';
//     }

//     setErrors(newErrors);
//     return Object.keys(newErrors).length === 0;
//   }, [formData]);

//   /** Handle input change */
//   const handleChange = useCallback(
//     (e: React.ChangeEvent<HTMLInputElement>) => {
//       const { name, value } = e.target;

//       setFormData(prev => ({ ...prev, [name]: value }));

//       // Clear error for this field
//       if (errors[name as keyof LoginFormErrors]) {
//         setErrors(prev => ({ ...prev, [name]: '' }));
//       }
//     },
//     [errors]
//   );

//   /** Handle form submission */
//   const handleLogin = useCallback(
//     async (e: React.FormEvent<HTMLFormElement>) => {
//       e.preventDefault();
//       setGeneralError(null);

//       if (!validateForm()) return;

//       setLoading(true);
//       try {
//         const result = await dispatch(
//           loginUser({
//             email: formData.email,
//             password: formData.password!,
//           })
//         ).unwrap();

//         onSuccess?.(result);
//       } catch (error: any) {
//         setGeneralError(error?.message || 'Login failed. Please try again.');
//       } finally {
//         setLoading(false);
//       }
//     },
//     [dispatch, formData, onSuccess, validateForm]
//   );

//   return {
//     formData,
//     errors,
//     loading,
//     generalError,
//     handleChange,
//     handleLogin,
//     setGeneralError,
//   };
// }

// export default useLogin;

import { useState, useCallback } from 'react';
import { loginUser } from '../store/authSlice';
import { useAppDispatch } from '../hooks/hooks'; // tip-safe dispatch

interface LoginFormData {
  email: string;
  password: string;
}

interface LoginFormErrors {
  email?: string;
  password?: string;
}

type UseLoginReturn = {
  formData: LoginFormData;
  errors: LoginFormErrors;
  loading: boolean;
  generalError: string | undefined;
  handleChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  handleLogin: (e: React.FormEvent<HTMLFormElement>) => Promise<void>;
  setGeneralError: React.Dispatch<React.SetStateAction<string | undefined>>;
};

/**
 * Custom hook for login form logic
 * Handles form state, validation, and API calls
 */
export function useLogin(onSuccess?: (result?: any) => void): UseLoginReturn {
  const dispatch = useAppDispatch(); // koristimo tip-safe dispatch
  const [formData, setFormData] = useState<LoginFormData>({
    email: '',
    password: '',
  });
  const [errors, setErrors] = useState<LoginFormErrors>({});
  const [loading, setLoading] = useState(false);
  const [generalError, setGeneralError] = useState<string | undefined>(undefined);

  /** Validate email format */
  const validateEmail = (email: string) => {
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
  };

  /** Validate entire form */
  const validateForm = useCallback(() => {
    const newErrors: LoginFormErrors = {};

    if (!formData.email.trim()) {
      newErrors.email = 'Email is required';
    } else if (!validateEmail(formData.email)) {
      newErrors.email = 'Please enter a valid email';
    }

    if (!formData.password) {
      newErrors.password = 'Password is required';
    } else if (formData.password.length < 6) {
      newErrors.password = 'Password must be at least 6 characters';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  }, [formData]);

  /** Handle input change */
  const handleChange = useCallback(
    (e: React.ChangeEvent<HTMLInputElement>) => {
      const { name, value } = e.target;
      setFormData(prev => ({ ...prev, [name]: value }));

      // Clear error for this field
      if (errors[name as keyof LoginFormErrors]) {
        setErrors(prev => ({ ...prev, [name]: '' }));
      }
    },
    [errors]
  );

  /** Handle form submission */
  const handleLogin = useCallback(
    async (e: React.FormEvent<HTMLFormElement>) => {
      e.preventDefault();
      setGeneralError(undefined);

      if (!validateForm()) return;

      setLoading(true);
      try {
        const result = await dispatch(
          loginUser({ email: formData.email, password: formData.password })
        ).unwrap(); // unwrap daje tip AuthResponse

        onSuccess?.(result);
      } catch (error: any) {
        setGeneralError(error?.message ?? 'Login failed. Please try again.');
      } finally {
        setLoading(false);
      }
    },
    [dispatch, formData, onSuccess, validateForm]
  );

  return {
    formData,
    errors,
    loading,
    generalError,
    handleChange,
    handleLogin,
    setGeneralError,
  };
}

export default useLogin;
