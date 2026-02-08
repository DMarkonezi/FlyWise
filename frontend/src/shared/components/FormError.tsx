import React from 'react';
import type { ReactNode } from 'react'
import { AlertCircle, CheckCircle, AlertTriangle } from 'lucide-react';
import styles from './FormError.module.css';

type FormErrorType = 'error' | 'warning' | 'success';

interface FormErrorProps {
  message?: string;
  type?: FormErrorType;
}

/**
 * Displays form errors, warnings, or success messages
 */
const FormError: React.FC<FormErrorProps> = ({ message, type = 'error' }) => {
  if (!message) return null;

  const iconMap: Record<FormErrorType, ReactNode> = {
    error: <AlertCircle size={20} />,
    warning: <AlertTriangle size={20} />,
    success: <CheckCircle size={20} />,
  };

  const messageClass = (() => {
    switch (type) {
      case 'error':
        return styles.messageError;
      case 'warning':
        return styles.messageWarning;
      case 'success':
        return styles.messageSuccess;
    }
  })();

  return (
    <div className={`${styles.formMessage} ${messageClass}`}>
      {iconMap[type]}
      <p>{message}</p>
    </div>
  );
};

export default FormError;
