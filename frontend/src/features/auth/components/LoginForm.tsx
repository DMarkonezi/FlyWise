import React from 'react';
import { Mail, Lock } from 'lucide-react';
import FormInput from '../../../shared/components/FormInput';
import FormError from '../../../shared/components/FormError';
import Button from '../../../shared/components/Button';
import useLogin from '../hooks/useLogin';
import styles from './LoginForm.module.css';

interface LoginFormProps {
  onSuccess: () => void;
}

/**
 * Login form component
 * Handles email/password input and submission
 */
const LoginForm: React.FC<LoginFormProps> = ({ onSuccess }) => {
  const { formData, errors, loading, generalError, handleChange, handleLogin } =
    useLogin(onSuccess);

  return (
    <form className={styles.loginForm} onSubmit={handleLogin}>
      <h2 className={styles.formTitle}>Welcome Back</h2>
      <p className={styles.formSubtitle}>Sign in to your account</p>

      {/* General Error */}
      <FormError message={generalError ?? undefined} type="error" />

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

      {/* Password Field */}
      <FormInput
        label="Password"
        type="password"
        placeholder="Enter your password"
        name="password"
        value={formData.password}
        onChange={handleChange}
        error={errors.password}
        required
        autoComplete="current-password"
      />

      {/* Remember Me & Forgot Password */}
      <div className={styles.formOptions}>
        <label className={styles.rememberMe}>
          <input type="checkbox" />
          <span>Remember me</span>
        </label>
        <a href="/forgot-password" className={styles.forgotLink}>
          Forgot password?
        </a>
      </div>

      {/* Submit Button */}
      <Button type="submit" fullWidth loading={loading} disabled={loading}>
        Sign In
      </Button>

      {/* Register Link */}
      <p className={styles.registerLink}>
        Don't have an account? <a href="/register">Create one</a>
      </p>
    </form>
  );
};

export default LoginForm;
