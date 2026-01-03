import axios, { type AxiosInstance, type AxiosRequestConfig } from 'axios';
import { useAuthStore } from '../stores/auth';

const api: AxiosInstance = axios.create({
  baseURL: '/api',
  timeout: 10000,
});

// Request interceptor to inject tenant ID and auth token
api.interceptors.request.use(config => {
  const authStore = useAuthStore();

  // Add authorization header
  if (authStore.accessToken) {
    config.headers.Authorization = `Bearer ${authStore.accessToken}`;
  }

  // Add tenant ID header
  if (authStore.tenantId) {
    config.headers['X-Tenant-ID'] = authStore.tenantId;
  }

  return config;
});

// Response interceptor to handle auth errors
api.interceptors.response.use(
  response => response,
  async error => {
    if (error.response?.status === 401) {
      const authStore = useAuthStore();
      authStore.logout();
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export { api };
