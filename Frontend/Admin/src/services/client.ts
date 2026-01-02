/**
 * HTTP Client wrapper for API requests
 * @todo Add proper typing for POST/PUT/PATCH data parameters
 */
/* eslint-disable @typescript-eslint/no-explicit-any -- Generic HTTP methods accept varied payloads */

import axios from "axios";
import type {
  AxiosInstance,
  AxiosRequestConfig,
  InternalAxiosRequestConfig,
} from "axios";
import type { ApiResponse } from "@/types/api";
import errorLogging from "./errorLogging";

// Request timing for performance tracking
interface RequestTiming {
  startTime: number;
  url: string;
  method: string;
}

class ApiClient {
  private instance: AxiosInstance;
  private requestTimings = new Map<string, RequestTiming>();

  constructor(baseURL: string) {
    this.instance = axios.create({
      baseURL,
      timeout: 30000,
      headers: {
        "Content-Type": "application/json",
      },
      withCredentials: true, // Enable httpOnly cookie handling
    });

    this.setupInterceptors();
  }

  private generateRequestId(): string {
    return `req_${Date.now()}_${Math.random().toString(36).substring(2, 9)}`;
  }

  private setupInterceptors() {
    // Request Interceptor
    this.instance.interceptors.request.use(
      (config: InternalAxiosRequestConfig) => {
        // Track request timing
        const requestId = this.generateRequestId();
        (config as any).__requestId = requestId;
        this.requestTimings.set(requestId, {
          startTime: performance.now(),
          url: config.url || "",
          method: config.method?.toUpperCase() || "GET",
        });

        // Use sessionStorage (more secure than localStorage)
        const token = sessionStorage.getItem("authToken");
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        const tenantId = sessionStorage.getItem("tenantId");
        if (tenantId) {
          config.headers["X-Tenant-ID"] = tenantId;
        }
        // Add CSRF token from cookie if available
        const csrfToken = document.cookie
          .split("; ")
          .find((row) => row.startsWith("XSRF-TOKEN="))
          ?.split("=")[1];
        if (csrfToken) {
          config.headers["X-XSRF-TOKEN"] = decodeURIComponent(csrfToken);
        }
        return config;
      }
    );

    // Response Interceptor
    this.instance.interceptors.response.use(
      (response) => {
        // Clean up timing
        const requestId = (response.config as any).__requestId;
        if (requestId) {
          this.requestTimings.delete(requestId);
        }
        return response;
      },
      (error) => {
        // Get timing info for error reporting
        const requestId = error.config?.__requestId;
        const timing = requestId ? this.requestTimings.get(requestId) : null;
        const duration = timing
          ? Math.round(performance.now() - timing.startTime)
          : undefined;

        // Clean up timing
        if (requestId) {
          this.requestTimings.delete(requestId);
        }

        // Log network errors to error logging service
        if (error.response) {
          // Server responded with error status
          const status = error.response.status;
          const url = error.config?.url || "unknown";
          const method = error.config?.method?.toUpperCase() || "GET";

          // Only log server errors (5xx) and unexpected client errors
          if (
            status >= 500 ||
            (status >= 400 &&
              status !== 401 &&
              status !== 403 &&
              status !== 404)
          ) {
            errorLogging.captureNetworkError(
              new Error(
                `HTTP ${status}: ${error.response.statusText || "Request failed"}`
              ),
              {
                url,
                method,
                status,
                duration,
              }
            );
          }
        } else if (error.request) {
          // Request made but no response (network error)
          errorLogging.captureNetworkError(
            new Error(`Network error: ${error.message}`),
            {
              url: error.config?.url || "unknown",
              method: error.config?.method?.toUpperCase() || "GET",
              duration,
            }
          );
        }

        // Handle 401 - redirect to login
        if (error.response?.status === 401) {
          sessionStorage.removeItem("authToken");
          window.location.href = "/login";
        }
        return Promise.reject(error);
      }
    );
  }

  public async get<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    const response = await this.instance.get<ApiResponse<T>>(url, config);
    return response.data.data as T;
  }

  public async post<T>(
    url: string,
    data?: any,
    config?: AxiosRequestConfig
  ): Promise<T> {
    const response = await this.instance.post<ApiResponse<T>>(
      url,
      data,
      config
    );
    return response.data.data as T;
  }

  public async put<T>(
    url: string,
    data?: any,
    config?: AxiosRequestConfig
  ): Promise<T> {
    const response = await this.instance.put<ApiResponse<T>>(url, data, config);
    return response.data.data as T;
  }

  public async patch<T>(
    url: string,
    data?: any,
    config?: AxiosRequestConfig
  ): Promise<T> {
    const response = await this.instance.patch<ApiResponse<T>>(
      url,
      data,
      config
    );
    return response.data.data as T;
  }

  public async delete<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    const response = await this.instance.delete<ApiResponse<T>>(url, config);
    return response.data.data as T;
  }

  public getAxiosInstance(): AxiosInstance {
    return this.instance;
  }
}

const baseURL = import.meta.env.VITE_ADMIN_API_URL || "http://localhost:8080";

export const apiClient = new ApiClient(baseURL);
