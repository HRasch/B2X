import { defineStore } from "pinia";
import { ref, computed } from "vue";
import type { AdminUser } from "@/types/auth";
import { authApi } from "@/services/api/auth";

// Token refresh interval (5 minutes before expiry, assuming 1hr token)
const TOKEN_REFRESH_INTERVAL = 55 * 60 * 1000; // 55 minutes

export const useAuthStore = defineStore("auth", () => {
  const user = ref<AdminUser | null>(null);
  // Use sessionStorage for non-sensitive session data only
  // Actual tokens should be in httpOnly cookies (handled by backend)
  const token = ref<string | null>(sessionStorage.getItem("authToken"));
  const refreshToken = ref<string | null>(null); // Not stored client-side for security
  const loading = ref(false);
  const error = ref<string | null>(null);
  let refreshTimerId: ReturnType<typeof setTimeout> | null = null;

  const isAuthenticated = computed(
    () => user.value !== null && token.value !== null
  );

  function scheduleTokenRefresh() {
    // Clear existing timer
    if (refreshTimerId) {
      clearTimeout(refreshTimerId);
    }
    // Schedule refresh before token expires
    refreshTimerId = setTimeout(async () => {
      try {
        await performTokenRefresh();
      } catch {
        // Refresh failed, logout user
        await logout();
      }
    }, TOKEN_REFRESH_INTERVAL);
  }

  async function performTokenRefresh() {
    try {
      const response = await authApi.refreshToken();
      user.value = response.user;
      token.value = response.accessToken;
      sessionStorage.setItem("authToken", response.accessToken);
      scheduleTokenRefresh();
      return response;
    } catch (err) {
      throw err;
    }
  }

  async function login(email: string, password: string, rememberMe?: boolean) {
    loading.value = true;
    error.value = null;
    try {
      const response = await authApi.login({ email, password, rememberMe });
      user.value = response.user;
      token.value = response.accessToken;
      // Note: refreshToken is now in httpOnly cookie, not stored client-side

      // Use sessionStorage for session-scoped data (cleared on tab close)
      sessionStorage.setItem("authToken", response.accessToken);
      sessionStorage.setItem("tenantId", response.user.tenantId);

      // Schedule automatic token refresh
      scheduleTokenRefresh();

      return response;
    } catch (err: unknown) {
      const errorObj = err as {
        response?: { data?: { error?: { message?: string } } };
      };
      error.value = errorObj.response?.data?.error?.message || "Login failed";
      throw err;
    } finally {
      loading.value = false;
    }
  }

  async function logout() {
    loading.value = true;
    try {
      await authApi.logout();
    } catch (err) {
      // Don't log sensitive info
      console.error("Logout failed");
    } finally {
      // Clear refresh timer
      if (refreshTimerId) {
        clearTimeout(refreshTimerId);
        refreshTimerId = null;
      }
      user.value = null;
      token.value = null;
      refreshToken.value = null;
      sessionStorage.removeItem("authToken");
      sessionStorage.removeItem("tenantId");
      loading.value = false;
    }
  }

  async function getCurrentUser() {
    if (!token.value) return;
    loading.value = true;
    try {
      user.value = await authApi.getCurrentUser();
      // Schedule refresh if we successfully got user
      scheduleTokenRefresh();
    } catch (err: unknown) {
      const errorObj = err as { message?: string };
      error.value = errorObj.message || "Failed to get user";
      await logout();
    } finally {
      loading.value = false;
    }
  }

  function hasPermission(permission: string): boolean {
    if (!user.value) return false;
    return user.value.permissions.some((p: any) => p.name === permission);
  }

  function hasRole(role: string): boolean {
    if (!user.value) return false;
    return user.value.roles.some((r: any) => r.name === role);
  }

  function hasAnyRole(roles: string[]): boolean {
    if (!user.value) return false;
    return user.value.roles.some((r: any) => roles.includes(r.name));
  }

  async function updateProfile(data: Partial<AdminUser>) {
    loading.value = true;
    error.value = null;
    try {
      user.value = await authApi.updateProfile(data);
      return user.value;
    } catch (err: unknown) {
      const errorObj = err as { message?: string };
      error.value = errorObj.message || "Failed to update profile";
      throw err;
    } finally {
      loading.value = false;
    }
  }

  return {
    user,
    token,
    refreshToken,
    loading,
    error,
    isAuthenticated,
    login,
    logout,
    getCurrentUser,
    hasPermission,
    hasRole,
    hasAnyRole,
    updateProfile,
    performTokenRefresh,
  };
});
