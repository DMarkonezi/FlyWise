import React, { useState } from 'react';
import type { ForwardedRef } from 'react';
import { Eye, EyeOff } from 'lucide-react';
import styles from './FormInput.module.css';

interface FormInputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
  icon?: React.ComponentType<{ className?: string; size?: number }>;
  required?: boolean;
}

const FormInput = React.forwardRef<HTMLInputElement, FormInputProps>(
  (
    {
      label,
      type = 'text',
      placeholder,
      value,
      onChange,
      onBlur,
      error,
      icon: Icon,
      disabled = false,
      required = false,
      autoComplete,
      ...props
    },
    ref: ForwardedRef<HTMLInputElement>
  ) => {
    const [showPassword, setShowPassword] = useState(false);
    const isPassword = type === 'password';
    const displayType = isPassword && showPassword ? 'text' : type;

    return (
      <div className={styles.formInputWrapper}>
        {label && (
          <label className={styles.formLabel}>
            {label}
            {required && <span className={styles.required}>*</span>}
          </label>
        )}
        <div className={styles.inputContainer}>
          {Icon && <Icon className={styles.inputIcon} size={20} />}
          <input
            ref={ref}
            type={displayType}
            placeholder={placeholder}
            value={value}
            onChange={onChange}
            onBlur={onBlur}
            disabled={disabled}
            autoComplete={autoComplete}
            className={`${styles.formInput} ${error ? styles.inputError : ''} ${Icon ? styles.withIcon : ''}`}
            {...props}
          />
          {isPassword && (
            <button
              type="button"
              className={styles.passwordToggle}
              onClick={() => setShowPassword(!showPassword)}
              tabIndex={-1}
            >
              {showPassword ? <EyeOff size={20} /> : <Eye size={20} />}
            </button>
          )}
        </div>
        {error && <span className={styles.inputErrorMessage}>{error}</span>}
      </div>
    );
  }
);

FormInput.displayName = 'FormInput';

export default FormInput;
