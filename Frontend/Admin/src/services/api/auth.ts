import { apiClient } from "../client";
import axios from "axios";
import type { AdminUser, LoginRequest, LoginResponse } from "@/types/auth";

// Demo mode - set to false when backend is running
const DEMO_MODE = false;

// Default tenant GUID for admin authentication
const DEFAULT_TENANT_ID = import.meta.env.VITE_DEFAULT_TENANT_ID || "00000000-0000-0000-0000-000000000001";

const baseURL = import.meta.env.VITE_ADMIN_API_URL || "/api";

export const authApi = {
  async login(credentials: LoginRequest): Promise<LoginResponse> {
    // Demo mode: accept admin@example.com / password
    if (
      DEMO_MODE &&
      credentials.email === "admin@example.com" &&
      credentials.password === "password"
    ) {
      return new Promise((resolve) => {
        setTimeout(() => {
          resolve({
            user: {
              id: "admin-001",
              email: "admin@example.com",
              firstName: "Admin",
              lastName: "User",
              tenantId: DEFAULT_TENANT_ID,
              roles: ["Admin"],
              permissions: ["*"],
            },
            accessToken: "demo-access-token",
            refreshToken: "demo-refresh-token",
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

    // Use stored tenant ID or default GUID for login
    const tenantId = localStorage.getItem("tenantId") || DEFAULT_TENANT_ID;

    // Direkt axios verwenden da Backend kein "data"-Wrapper hat
    const response = await axios.post<LoginResponse>(
      `${baseURL}/api/auth/login`,
      credentials,
      {
        headers: {
          "Content-Type": "application/json",
          "X-Tenant-ID": tenantId,
        },
      }
    );

    // Store tenant ID from response if provided
    if (response.data.user?.tenantId) {
      localStorage.setItem("tenantId", response.data.user.tenantId);
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

  changePassword(oldPassword: String, newPassword: string) {
    return apiClient.post<void>("/api/auth/change-password", {
      oldPassword,
      newPassword,
    });
  },

  requestMFA() {
    return apiClient.post<{ method: string }>("/api/auth/2fa/request", {});
  },

  verifyMFA(code: string) {
    return apiClient.post<LoginResponse>("/api/auth/2fa/verify", { code });
  },
};
