/**
 * API Client Configuration
 * Axios instance with base configuration
 */

import axios, { type AxiosInstance, type AxiosError } from 'axios';

// API Base URL from Vite config or default
const API_BASE_URL = '/api';

/**
 * Create Axios instance with default configuration
 */
export const apiClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30000, // 30 seconds
  headers: {
    'Content-Type': 'application/json',
  },
});

/**
 * Request Interceptor
 * Add authentication token or other headers
 */
apiClient.interceptors.request.use(
  (config) => {
    // Add auth token if available
    const token = localStorage.getItem('auth_token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

/**
 * Response Interceptor
 * Handle errors globally
 */
apiClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    // Handle specific error codes
    if (error.response) {
      const status = error.response.status;

      switch (status) {
        case 401:
          console.error('Unauthorized: Please login');
          // Redirect to login or handle auth error
          break;
        case 403:
          console.error('Forbidden: Access denied');
          break;
        case 404:
          console.error('Not found:', error.config?.url);
          break;
        case 500:
          console.error('Server error:', error.message);
          break;
        default:
          console.error(`API Error ${status}:`, error.message);
      }
    } else if (error.request) {
      console.error('Network error: No response from server');
    } else {
      console.error('Request error:', error.message);
    }

    return Promise.reject(error);
  }
);

/**
 * API Error Type
 */
export interface ApiError {
  message: string;
  status?: number;
  code?: string;
}

/**
 * Extract error message from Axios error
 */
export function getApiErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const axiosError = error as AxiosError<{ message?: string }>;
    return axiosError.response?.data?.message || axiosError.message || 'Unknown error';
  }

  if (error instanceof Error) {
    return error.message;
  }

  return 'Unknown error occurred';
}

export default apiClient;
