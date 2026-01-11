import axios, { AxiosInstance, AxiosResponse } from 'axios';

const DEMO_MODE = import.meta.env.VITE_ENABLE_DEMO_MODE === 'true';

// Create axios instance
const apiClient: AxiosInstance = axios.create({
  baseURL: DEMO_MODE ? '' : '/api',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor to add auth token
apiClient.interceptors.request.use(
  config => {
    // Add tenant header if available
    const tenantId = localStorage.getItem('tenant-id');
    if (tenantId) {
      config.headers['X-Tenant-ID'] = tenantId;
    }

    // Add auth token if available
    const token = localStorage.getItem('auth-token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  error => {
    return Promise.reject(error);
  }
);

// Response interceptor for error handling
apiClient.interceptors.response.use(
  (response: AxiosResponse) => {
    return response;
  },
  error => {
    if (error.response?.status === 401) {
      // Handle unauthorized - redirect to login
      localStorage.removeItem('auth-token');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export { apiClient };
