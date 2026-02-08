import React from 'react';
import FormInput from '../../../shared/components/FormInput';
import FormError from '../../../shared/components/FormError';
import Button from '../../../shared/components/Button';
import useRegister from '../hooks/useRegister';
import styles from './RegisterForm.module.css';

interface RegisterFormProps {
  onSuccess: () => void;
}

/**
 * Register form component
 * Handles registration input and submission
 */
const RegisterForm: React.FC<RegisterFormProps> = ({ onSuccess }) => {
  const { formData, errors, loading, generalError, handleChange, handleRegister } =
    useRegister(onSuccess);

  return (
    <form className={styles.registerForm} onSubmit={handleRegister}>
      <h2 className={styles.formTitle}>Create Account</h2>
      <p className={styles.formSubtitle}>Join FlyWise and start booking flights</p>

      {/* General Error */}
      <FormError message={generalError ?? undefined} type="error" />

      {/* First Name Field */}
      <FormInput
        label="First Name"
        type="text"
        placeholder="John"
        name="firstName"
        value={formData.firstName || ''}
        onChange={handleChange}
        error={errors.firstName}
        required
        autoComplete="given-name"
      />

      {/* Last Name Field */}
      <FormInput
        label="Last Name"
        type="text"
        placeholder="Doe"
        name="lastName"
        value={formData.lastName || ''}
        onChange={handleChange}
        error={errors.lastName}
        required
        autoComplete="family-name"
      />

      {/* Email Field */}
      <FormInput
        label="Email Address"
        type="email"
        placeholder="you@example.com"
        name="email"
        value={formData.email}
        onChange={handleChange}
        error={errors.email}
        required
        autoComplete="email"
      />

      {/* Passport Field */}
      <FormInput
        label="Passport Number"
        type="text"
        placeholder="ABC123456"
        name="passportNumber"
        value={formData.passportNumber || ''}
        onChange={handleChange}
        error={errors.passportNumber}
        required
        autoComplete="off"
      />

      {/* Password Field */}
      <FormInput
        label="Password"
        type="password"
        placeholder="Create a strong password"
        name="password"
        value={formData.password}
        onChange={handleChange}
        error={errors.password}
        required
        autoComplete="new-password"
      />

      {/* Confirm Password Field */}
      <FormInput
        label="Confirm Password"
        type="password"
        placeholder="Confirm your password"
        name="confirmPassword"
        value={formData.confirmPassword}
        onChange={handleChange}
        error={errors.confirmPassword}
        required
        autoComplete="new-password"
      />

      {/* Submit Button */}
      <Button type="submit" fullWidth loading={loading} disabled={loading}>
        Create Account
      </Button>

      {/* Login Link */}
      <p className={styles.loginLink}>
        Already have an account? <a href="/login">Sign In</a>
      </p>

      {/* Terms */}
      <p className={styles.termsText}>
        By creating an account, you agree to our{' '}
        <a href="/terms">Terms of Service</a> and{' '}
        <a href="/privacy">Privacy Policy</a>
      </p>
    </form>
  );
};

export default RegisterForm;