import { describe, it, expect, beforeEach, vi } from "vitest";
import { setActivePinia, createPinia } from "pinia";
import { useAuthStore } from "@/stores/auth";

// Mock API
vi.mock("@/services/api", () => ({
  api: {
    post: vi.fn(),
  },
}));

describe("Auth Store - Comprehensive Tests", () => {
  beforeEach(() => {
    setActivePinia(createPinia());
    localStorage.clear();
  });

  describe("Initialization", () => {
    it("should initialize with null user when no tokens in localStorage", () => {
      const authStore = useAuthStore();
      expect(authStore.user).toBeNull();
      expect(authStore.accessToken).toBeNull();
      expect(authStore.refreshToken).toBeNull();
      expect(authStore.isAuthenticated).toBe(false);
    });

    it("should initialize with existing token from localStorage", () => {
      localStorage.setItem("access_token", "test-token");
      localStorage.setItem("tenant_id", "test-tenant-id");

      const authStore = useAuthStore();
      expect(authStore.accessToken).toBe("test-token");
      expect(authStore.tenantId).toBe("test-tenant-id");
      expect(authStore.isAuthenticated).toBe(true);
    });
  });

  describe("Login", () => {
    it("should successfully login and store tokens", async () => {
      const { api } = await import("@/services/api");
      const mockResponse = {
        data: {
          accessToken: "new-access-token",
          refreshToken: "new-refresh-token",
          user: {
            id: "user-123",
            tenantId: "tenant-123",
            email: "test@example.com",
            firstName: "Test",
            lastName: "User",
            status: "active",
            lastLoginAt: new Date(),
            emailConfirmed: true,
          },
        },
      };

      vi.mocked(api.post).mockResolvedValueOnce(mockResponse);

      const authStore = useAuthStore();
      const result = await authStore.login("test@example.com", "password123");

      expect(result).toBe(true);
      expect(authStore.accessToken).toBe("new-access-token");
      expect(authStore.refreshToken).toBe("new-refresh-token");
      expect(authStore.user).toEqual(mockResponse.data.user);
      expect(authStore.isAuthenticated).toBe(true);

      // Check localStorage
      expect(localStorage.getItem("access_token")).toBe("new-access-token");
      expect(localStorage.getItem("refresh_token")).toBe("new-refresh-token");
      expect(localStorage.getItem("tenant_id")).toBe("tenant-123");
    });

    it("should handle login failure gracefully", async () => {
      const { api } = await import("@/services/api");
      vi.mocked(api.post).mockRejectedValueOnce(
        new Error("Invalid credentials")
      );

      const authStore = useAuthStore();
      const result = await authStore.login(
        "test@example.com",
        "wrong-password"
      );

      expect(result).toBe(false);
      expect(authStore.accessToken).toBeNull();
      expect(authStore.user).toBeNull();
      expect(authStore.isAuthenticated).toBe(false);
    });
  });

  describe("Logout", () => {
    it("should clear all auth data on logout", () => {
      localStorage.setItem("access_token", "test-token");
      localStorage.setItem("refresh_token", "test-refresh");
      localStorage.setItem("tenant_id", "test-tenant");

      const authStore = useAuthStore();
      authStore.setUser({
        id: "user-123",
        tenantId: "tenant-123",
        email: "test@example.com",
        firstName: "Test",
        lastName: "User",
        status: "active",
        lastLoginAt: new Date(),
        emailConfirmed: true,
      });

      authStore.logout();

      expect(authStore.user).toBeNull();
      expect(authStore.accessToken).toBeNull();
      expect(authStore.refreshToken).toBeNull();
      expect(authStore.tenantId).toBeNull();
      expect(authStore.isAuthenticated).toBe(false);

      // Check localStorage cleared
      expect(localStorage.getItem("access_token")).toBeNull();
      expect(localStorage.getItem("refresh_token")).toBeNull();
      expect(localStorage.getItem("tenant_id")).toBeNull();
    });
  });

  describe("isAuthenticated", () => {
    it("should return true when access token exists", () => {
      const authStore = useAuthStore();
      authStore.accessToken = "test-token";
      expect(authStore.isAuthenticated).toBe(true);
    });

    it("should return false when no access token", () => {
      const authStore = useAuthStore();
      expect(authStore.isAuthenticated).toBe(false);
    });
  });
});
