import { apiClient } from "../client";
import axios from "axios";
import type { AdminUser, LoginRequest, LoginResponse } from "@/types/auth";

// Demo mode - DISABLED for production security
// Only enable for local E2E testing via environment variable
const DEMO_MODE = import.meta.env.VITE_ENABLE_DEMO_MODE === "true";

// Default tenant GUID for admin authentication
const DEFAULT_TENANT_ID =
  import.meta.env.VITE_DEFAULT_TENANT_ID ||
  "00000000-0000-0000-0000-000000000001";

const baseURL = import.meta.env.VITE_ADMIN_API_URL || "http://localhost:8080";

export const authApi = {
  async login(credentials: LoginRequest): Promise<LoginResponse> {
    // Ensure tenant ID is set before any login attempt
    if (!sessionStorage.getItem("tenantId")) {
      sessionStorage.setItem("tenantId", DEFAULT_TENANT_ID);
    }

    // Demo mode: ONLY enabled via environment variable for testing
    // WARNING: Never enable in production!
    if (
      DEMO_MODE &&
      credentials.email === "admin@example.com" &&
      credentials.password === "password"
    ) {
      console.warn("[AUTH] Demo mode active - NOT FOR PRODUCTION USE");
      return new Promise((resolve) => {
        setTimeout(() => {
          resolve({
            user: {
              id: "admin-001",
              email: "admin@example.com",
              firstName: "Admin",
              lastName: "User",
              tenantId: DEFAULT_TENANT_ID,
              roles: [
                {
                  id: "1",
                  name: "Admin",
                  description: "Administrator",
                  permissions: [],
                },
              ],
              permissions: [{ id: "1", name: "*", resource: "*", action: "*" }],
            },
            accessToken: "demo-access-token-" + Date.now(),
            refreshToken: "demo-refresh-token-" + Date.now(),
            expiresIn: 3600,
          } as LoginResponse);
        }, 500);
      });
    }

    if (DEMO_MODE) {
      return Promise.reject({
        response: {
          data: {
            error: {
              message: "Invalid credentials",
            },
          },
        },
      });
    }

    const tenantId = sessionStorage.getItem("tenantId") || DEFAULT_TENANT_ID;

    // Use axios with credentials for httpOnly cookie support
    const response = await axios.post<LoginResponse>(
      `${baseURL}/api/auth/login`,
      credentials,
      {
        headers: {
          "Content-Type": "application/json",
          "X-Tenant-ID": tenantId,
        },
        withCredentials: true, // Enable httpOnly cookie handling
      }
    );

    // Store tenant ID from response if provided (non-sensitive)
    if (response.data.user?.tenantId) {
      sessionStorage.setItem("tenantId", response.data.user.tenantId);
    }

    return response.data;
  },

  verify(token: string) {
    return apiClient.post<AdminUser>("/api/auth/verify", { token });
  },

  getCurrentUser() {
    return apiClient.get<AdminUser>("/api/auth/me");
  },

  updateProfile(data: Partial<AdminUser>) {
    return apiClient.put<AdminUser>("/api/auth/profile", data);
  },

  changePassword(oldPassword: string, newPassword: string) {
    return apiClient.post<void>("/api/auth/change-password", {
      oldPassword,
      newPassword,
    });
  },

  async refreshToken(): Promise<LoginResponse> {
    // Token refresh uses httpOnly cookies - no token needed in request
    const response = await axios.post<LoginResponse>(
      `${baseURL}/api/auth/refresh`,
      {},
      {
        withCredentials: true, // Send httpOnly refresh cookie
      }
    );
    return response.data;
  },

  requestMFA() {
    return apiClient.post<{ method: string }>("/api/auth/2fa/request", {});
  },

  verifyMFA(code: string) {
    return apiClient.post<LoginResponse>("/api/auth/2fa/verify", { code });
  },

  async logout(): Promise<void> {
    // In demo mode, just clear session storage (handled by store)
    if (DEMO_MODE) {
      return Promise.resolve();
    }
    // Real backend call - server will clear httpOnly cookies
    return apiClient.post<void>("/api/auth/logout", {});
  },
};
