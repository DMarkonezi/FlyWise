// import { useState, useCallback } from 'react';
// import { registerUser } from '../store/authSlice';
// import { useAppDispatch } from '../hooks/hooks';

// interface RegisterFormData {
//   firstName: string;
//   lastName: string;
//   email: string;
//   passportNumber: string;
//   password: string;
//   confirmPassword: string;
// }

// interface RegisterFormErrors {
//   firstName?: string;
//   lastName?: string;
//   email?: string;
//   passportNumber?: string;
//   password?: string;
//   confirmPassword?: string;
// }

// type UseRegisterReturn = {
//   formData: RegisterFormData;
//   errors: RegisterFormErrors;
//   loading: boolean;
//   generalError: string | undefined;
//   handleChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
//   handleRegister: (e: React.FormEvent<HTMLFormElement>) => Promise<void>;
//   setGeneralError: React.Dispatch<React.SetStateAction<string | undefined>>;
// };

// /**
//  * Custom hook for registration form logic
//  * Handles form state, validation, and API calls
//  */
// export function useRegister(onSuccess?: (result?: any) => void): UseRegisterReturn {
//   const dispatch = useAppDispatch();
//   const [formData, setFormData] = useState<RegisterFormData>({
//     firstName: '',
//     lastName: '',
//     email: '',
//     passportNumber: '',
//     password: '',
//     confirmPassword: '',
//   });
//   const [errors, setErrors] = useState<RegisterFormErrors>({});
//   const [loading, setLoading] = useState(false);
//   const [generalError, setGeneralError] = useState<string | undefined>(undefined);

//   /** Validate email format */
//   const validateEmail = (email: string) => {
//     const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
//     return regex.test(email);
//   };

//   /** Validate passport number format (ABC123456) */
//   const validatePassport = (passport: string) => {
//     const regex = /^[A-Z]{3}\d{6}$/;
//     return regex.test(passport.trim().toUpperCase());
//   };

//   /** Validate password strength */
//   const validatePasswordStrength = (password: string) => {
//     const minLength = password.length >= 8;
//     const hasNumber = /[0-9]/.test(password);
//     const hasUpperCase = /[A-Z]/.test(password);
//     const hasLowerCase = /[a-z]/.test(password);

//     return minLength && hasNumber && hasUpperCase && hasLowerCase;
//   };

//   /** Validate entire form */
//   const validateForm = useCallback(() => {
//     const newErrors: RegisterFormErrors = {};

//     // First Name validation
//     if (!formData.firstName.trim()) {
//       newErrors.firstName = 'First name is required';
//     } else if (formData.firstName.trim().length < 2) {
//       newErrors.firstName = 'First name must be at least 2 characters';
//     }

//     // Last Name validation
//     if (!formData.lastName.trim()) {
//       newErrors.lastName = 'Last name is required';
//     } else if (formData.lastName.trim().length < 2) {
//       newErrors.lastName = 'Last name must be at least 2 characters';
//     }

//     // Email validation
//     if (!formData.email.trim()) {
//       newErrors.email = 'Email is required';
//     } else if (!validateEmail(formData.email)) {
//       newErrors.email = 'Please enter a valid email';
//     }

//     // Passport validation
//     if (!formData.passportNumber.trim()) {
//       newErrors.passportNumber = 'Passport number is required';
//     } else if (!validatePassport(formData.passportNumber)) {
//       newErrors.passportNumber = 'Invalid format (ABC123456)';
//     }

//     // Password validation
//     if (!formData.password) {
//       newErrors.password = 'Password is required';
//     } else if (!validatePasswordStrength(formData.password)) {
//       newErrors.password =
//         'Password must have 8+ chars, number, uppercase and lowercase';
//     }

//     // Confirm password validation
//     if (formData.password !== formData.confirmPassword) {
//       newErrors.confirmPassword = 'Passwords do not match';
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
//       if (errors[name as keyof RegisterFormErrors]) {
//         setErrors(prev => ({ ...prev, [name]: '' }));
//       }
//     },
//     [errors]
//   );

//   /** Handle form submission */
//   const handleRegister = useCallback(
//     async (e: React.FormEvent<HTMLFormElement>) => {
//       e.preventDefault();
//       setGeneralError(undefined);

//       if (!validateForm()) return;

//       setLoading(true);
//       try {
//         // Construct name from first and last name
//         const name = `${formData.firstName.trim()} ${formData.lastName.trim()}`;

//         const result = await dispatch(
//           registerUser({
//             email: formData.email.trim().toLowerCase(),
//             password: formData.password,
//             name,
//           })
//         ).unwrap();

//         onSuccess?.(result);
//       } catch (error: any) {
//         setGeneralError(
//           error?.message ?? 'Registration failed. Please try again.'
//         );
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
//     handleRegister,
//     setGeneralError,
//   };
// }

// export default useRegister;

import { useState, useCallback } from 'react';
import { registerUser } from '../store/authSlice';
import { useAppDispatch } from '../hooks/hooks';

interface RegisterFormData {
  firstName: string;
  lastName: string;
  email: string;
  passportNumber: string;
  password: string;
  confirmPassword: string;
}

interface RegisterFormErrors {
  firstName?: string;
  lastName?: string;
  email?: string;
  passportNumber?: string;
  password?: string;
  confirmPassword?: string;
}

type UseRegisterReturn = {
  formData: RegisterFormData;
  errors: RegisterFormErrors;
  loading: boolean;
  generalError: string | undefined;
  handleChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  handleRegister: (e: React.FormEvent<HTMLFormElement>) => Promise<void>;
  setGeneralError: React.Dispatch<React.SetStateAction<string | undefined>>;
};

/**
 * Custom hook for registration form logic
 */
export function useRegister(onSuccess?: (result?: any) => void): UseRegisterReturn {
  const dispatch = useAppDispatch();
  const [formData, setFormData] = useState<RegisterFormData>({
    firstName: '',
    lastName: '',
    email: '',
    passportNumber: '',
    password: '',
    confirmPassword: '',
  });
  const [errors, setErrors] = useState<RegisterFormErrors>({});
  const [loading, setLoading] = useState(false);
  const [generalError, setGeneralError] = useState<string | undefined>(undefined);

  /** Validate email format */
  const validateEmail = (email: string) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);

  /** Validate passport number format (ABC123456) */
  const validatePassport = (passport: string) =>
    /^[A-Z]{3}\d{6}$/.test(passport.trim().toUpperCase());

  /** Validate password strength */
  const validatePasswordStrength = (password: string) => {
    const minLength = password.length >= 8;
    const hasNumber = /[0-9]/.test(password);
    const hasUpperCase = /[A-Z]/.test(password);
    const hasLowerCase = /[a-z]/.test(password);
    return minLength && hasNumber && hasUpperCase && hasLowerCase;
  };

  /** Validate entire form */
  const validateForm = useCallback(() => {
    const newErrors: RegisterFormErrors = {};

    if (!formData.firstName.trim()) newErrors.firstName = 'First name is required';
    else if (formData.firstName.trim().length < 2)
      newErrors.firstName = 'First name must be at least 2 characters';

    if (!formData.lastName.trim()) newErrors.lastName = 'Last name is required';
    else if (formData.lastName.trim().length < 2)
      newErrors.lastName = 'Last name must be at least 2 characters';

    if (!formData.email.trim()) newErrors.email = 'Email is required';
    else if (!validateEmail(formData.email)) newErrors.email = 'Please enter a valid email';

    if (!formData.passportNumber.trim()) newErrors.passportNumber = 'Passport number is required';
    else if (!validatePassport(formData.passportNumber))
      newErrors.passportNumber = 'Invalid format (ABC123456)';

    if (!formData.password) newErrors.password = 'Password is required';
    else if (!validatePasswordStrength(formData.password))
      newErrors.password = 'Password must have 8+ chars, number, uppercase and lowercase';

    if (formData.password !== formData.confirmPassword)
      newErrors.confirmPassword = 'Passwords do not match';

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  }, [formData]);

  /** Handle input change */
  const handleChange = useCallback(
    (e: React.ChangeEvent<HTMLInputElement>) => {
      const { name, value } = e.target;
      setFormData(prev => ({ ...prev, [name]: value }));

      // Clear error for this field
      if (errors[name as keyof RegisterFormErrors]) {
        setErrors(prev => ({ ...prev, [name]: '' }));
      }
    },
    [errors]
  );

  /** Handle form submission */
  const handleRegister = useCallback(
    async (e: React.FormEvent<HTMLFormElement>) => {
      e.preventDefault();
      setGeneralError(undefined);

      if (!validateForm()) return;

      setLoading(true);
      try {
        const result = await dispatch(
          registerUser({
            firstName: formData.firstName.trim(),
            lastName: formData.lastName.trim(),
            email: formData.email.trim().toLowerCase(),
            password: formData.password,
            passportNumber: formData.passportNumber.trim(),
          })
        ).unwrap();

        onSuccess?.(result);
      } catch (error: any) {
        setGeneralError(error?.message ?? 'Registration failed. Please try again.');
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
    handleRegister,
    setGeneralError,
  };
}

export default useRegister;
