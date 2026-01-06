import axios, { type AxiosInstance, type AxiosRequestConfig, type AxiosResponse } from 'axios';
import { useAuthStore } from '../stores/auth';
import { toast } from 'vue3-toastify';

interface ResilienceConfig {
  retryAttempts: number;
  retryDelay: number;
  circuitBreakerThreshold: number;
  circuitBreakerTimeout: number;
  timeout: number;
}

const defaultConfig: ResilienceConfig = {
  retryAttempts: 3,
  retryDelay: 1000,
  circuitBreakerThreshold: 5,
  circuitBreakerTimeout: 30000,
  timeout: 10000,
};

// Simple circuit breaker state
type CircuitState = 'closed' | 'open' | 'half-open';

class ResilientApiClient {
  private axiosClient: AxiosInstance;
  private config: ResilienceConfig;
  private failureCount = 0;
  private lastFailureTime = 0;
  private circuitState: CircuitState = 'closed';

  constructor(config: Partial<ResilienceConfig> = {}) {
    this.config = { ...defaultConfig, ...config };
    this.axiosClient = this.createClient();
    this.setupInterceptors();
  }

  private createClient(): AxiosInstance {
    return axios.create({
      baseURL: '/api',
      timeout: this.config.timeout,
      headers: {
        'Content-Type': 'application/json',
      },
    });
  }

  private shouldRetry(error: unknown): boolean {
    if (!error || typeof error !== 'object') return false;
    const axiosError = error as { response?: { status: number } };
    return (
      !axiosError.response ||
      axiosError.response.status >= 500 ||
      [408, 429].includes(axiosError.response.status)
    );
  }

  private async executeWithRetry<T>(fn: () => Promise<T>): Promise<T> {
    // Check circuit breaker
    if (this.circuitState === 'open') {
      const timeSinceFailure = Date.now() - this.lastFailureTime;
      if (timeSinceFailure > this.config.circuitBreakerTimeout) {
        this.circuitState = 'half-open';
      } else {
        throw new Error('Circuit breaker is open');
      }
    }

    let lastError: unknown;
    for (let attempt = 0; attempt <= this.config.retryAttempts; attempt++) {
      try {
        const result = await fn();
        // Success - reset circuit breaker
        if (this.circuitState === 'half-open') {
          this.circuitState = 'closed';
          this.failureCount = 0;
        }
        return result;
      } catch (error) {
        lastError = error;
        if (!this.shouldRetry(error) || attempt === this.config.retryAttempts) {
          this.recordFailure();
          throw error;
        }
        // Exponential backoff
        await this.delay(this.config.retryDelay * Math.pow(2, attempt));
      }
    }
    throw lastError;
  }

  private recordFailure(): void {
    this.failureCount++;
    this.lastFailureTime = Date.now();
    if (this.failureCount >= this.config.circuitBreakerThreshold) {
      this.circuitState = 'open';
    }
  }

  private delay(ms: number): Promise<void> {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  private setupInterceptors() {
    // Request interceptor
    this.axiosClient.interceptors.request.use(config => {
      const authStore = useAuthStore();

      // Add authorization header
      if (authStore.accessToken) {
        config.headers.Authorization = `Bearer ${authStore.accessToken}`;
      }

      // Add tenant ID header
      if (authStore.tenantId) {
        config.headers['X-Tenant-ID'] = authStore.tenantId;
      }

      // Add request ID for tracing
      config.headers['X-Request-ID'] = this.generateRequestId();

      return config;
    });

    // Response interceptor
    this.axiosClient.interceptors.response.use(
      response => response,
      async error => {
        // Handle authentication errors
        if (error.response?.status === 401) {
          const authStore = useAuthStore();
          authStore.logout();
          window.location.href = '/login';
          return Promise.reject(error);
        }

        // Handle rate limiting
        if (error.response?.status === 429) {
          const retryAfter = error.response.headers['retry-after'];
          toast.warning(`Rate limited. Retry after ${retryAfter || 'a moment'}`);
        }

        // Handle server errors with user feedback
        if (error.response?.status >= 500) {
          toast.error('Server error occurred. Please try again later.');
        }

        // Handle network errors
        if (!error.response) {
          toast.error('Network error. Please check your connection.');
        }

        return Promise.reject(error);
      }
    );
  }

  private generateRequestId(): string {
    return `req_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
  }

  async request<T = unknown>(config: AxiosRequestConfig): Promise<AxiosResponse<T>> {
    return this.executeWithRetry(() => this.axiosClient.request<T>(config));
  }

  async get<T = unknown>(url: string, config?: AxiosRequestConfig): Promise<AxiosResponse<T>> {
    return this.request<T>({ ...config, method: 'GET', url });
  }

  async post<T = unknown>(
    url: string,
    data?: unknown,
    config?: AxiosRequestConfig
  ): Promise<AxiosResponse<T>> {
    return this.request<T>({ ...config, method: 'POST', url, data });
  }

  async put<T = unknown>(
    url: string,
    data?: unknown,
    config?: AxiosRequestConfig
  ): Promise<AxiosResponse<T>> {
    return this.request<T>({ ...config, method: 'PUT', url, data });
  }

  async patch<T = unknown>(
    url: string,
    data?: unknown,
    config?: AxiosRequestConfig
  ): Promise<AxiosResponse<T>> {
    return this.request<T>({ ...config, method: 'PATCH', url, data });
  }

  async delete<T = unknown>(url: string, config?: AxiosRequestConfig): Promise<AxiosResponse<T>> {
    return this.request<T>({ ...config, method: 'DELETE', url });
  }

  // Health check method
  async healthCheck(): Promise<boolean> {
    try {
      await this.axiosClient.get('/health', { timeout: 5000 });
      return true;
    } catch {
      return false;
    }
  }

  // Get circuit breaker state
  getCircuitBreakerState(): CircuitState {
    return this.circuitState;
  }

  // Get underlying axios client for backward compatibility
  getClient(): AxiosInstance {
    return this.axiosClient;
  }
}

// Create singleton instance
export const resilientApiClient = new ResilientApiClient();

// Export the axios instance for backward compatibility
export const api = resilientApiClient.getClient();

// Export types
export type { ResilienceConfig };
