import { defineStore } from "pinia";
import { ref, computed } from "vue";
import type { AdminUser } from "@/types/auth";
import { authApi } from "@/services/api/auth";

export const useAuthStore = defineStore("auth", () => {
  const user = ref<AdminUser | null>(null);
  const token = ref<string | null>(localStorage.getItem("authToken"));
  const refreshToken = ref<string | null>(localStorage.getItem("refreshToken"));
  const loading = ref(false);
  const error = ref<string | null>(null);

  const isAuthenticated = computed(
    () => user.value !== null && token.value !== null
  );

  async function login(email: string, password: string, rememberMe?: boolean) {
    loading.value = true;
    error.value = null;
    try {
      const response = await authApi.login({ email, password, rememberMe });
      user.value = response.user;
      token.value = response.accessToken;
      refreshToken.value = response.refreshToken;

      localStorage.setItem("authToken", response.accessToken);
      localStorage.setItem("refreshToken", response.refreshToken);
      localStorage.setItem("tenantId", response.user.tenantId);

      return response;
    } catch (err: any) {
      error.value = err.response?.data?.error?.message || "Login failed";
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
      console.error("Logout error:", err);
    } finally {
      user.value = null;
      token.value = null;
      refreshToken.value = null;
      localStorage.removeItem("authToken");
      localStorage.removeItem("refreshToken");
      localStorage.removeItem("tenantId");
      loading.value = false;
    }
  }

  async function getCurrentUser() {
    if (!token.value) return;
    loading.value = true;
    try {
      user.value = await authApi.getCurrentUser();
    } catch (err: any) {
      error.value = err.message;
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

  // Check if user is a tenant admin (has access to all pages within their tenant)
  function isTenantAdmin(): boolean {
    return hasRole("tenant-admin");
  }

  // Check if user has full admin access (admin or tenant-admin)
  function hasAdminAccess(): boolean {
    return hasRole("admin") || hasRole("tenant-admin");
  }

  // Get current tenant ID for filtering
  function getCurrentTenantId(): string | null {
    return user.value?.tenantId || localStorage.getItem("tenantId");
  }

  async function updateProfile(data: Partial<AdminUser>) {
    loading.value = true;
    error.value = null;
    try {
      user.value = await authApi.updateProfile(data);
      return user.value;
    } catch (err: any) {
      error.value = err.message;
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
    isTenantAdmin,
    hasAdminAccess,
    getCurrentTenantId,
    updateProfile,
  };
});
