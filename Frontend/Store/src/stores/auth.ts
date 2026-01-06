import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { api } from '../services/api';
import type { UserDto, AuthResponse } from '../types';

export const useAuthStore = defineStore('auth', () => {
  const user = ref<UserDto | null>(null);
  const accessToken = ref<string | null>(localStorage.getItem('access_token'));
  const refreshToken = ref<string | null>(localStorage.getItem('refresh_token'));
  const tenantId = ref<string | null>(localStorage.getItem('tenant_id'));

  const isAuthenticated = computed(() => !!accessToken.value);

  // Role-based computed properties
  const isAdmin = computed(() => user.value?.roles?.includes('Admin') ?? false);
  const isContentEditor = computed(() => user.value?.roles?.includes('ContentEditor') ?? false);
  const isEditor = computed(() => user.value?.roles?.includes('Editor') ?? false);
  const hasAdminAccess = computed(() => isAdmin.value || isContentEditor.value || isEditor.value);

  const login = async (email: string, password: string, selectedTenantId?: string) => {
    try {
      const response = await api.post<AuthResponse>('/auth/login', {
        email,
        password,
        tenantId: selectedTenantId,
      });

      const { accessToken: token, refreshToken: refresh, user: userData } = response.data;

      accessToken.value = token;
      refreshToken.value = refresh;
      user.value = userData;
      tenantId.value = userData.tenantId;

      localStorage.setItem('access_token', token);
      localStorage.setItem('refresh_token', refresh);
      localStorage.setItem('tenant_id', userData.tenantId);

      return true;
    } catch (error) {
      console.error('Login failed:', error);
      return false;
    }
  };

  const logout = () => {
    user.value = null;
    accessToken.value = null;
    refreshToken.value = null;
    tenantId.value = null;

    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('tenant_id');
  };

  const setUser = (userData: UserDto) => {
    user.value = userData;
  };

  return {
    user,
    accessToken,
    refreshToken,
    tenantId,
    isAuthenticated,
    isAdmin,
    isContentEditor,
    isEditor,
    hasAdminAccess,
    login,
    logout,
    setUser,
  };
});
