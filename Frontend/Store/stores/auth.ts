import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { api } from '~/services/api';
import type { UserDto, AuthResponse } from '~/types';

export const useAuthStore = defineStore('auth', () => {
  const user = ref<UserDto | null>(null);
  const accessToken = ref<string | null>(null);
  const refreshToken = ref<string | null>(null);
  const tenantId = ref<string | null>(null);

  // Initialize from cookies on client-side
  if (process.client) {
    const cookies = useCookie('auth');
    if (cookies.value) {
      const authData = cookies.value as any;
      accessToken.value = authData.accessToken;
      refreshToken.value = authData.refreshToken;
      tenantId.value = authData.tenantId;
      user.value = authData.user;
    }
  }

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

      // Store in cookies for SSR
      const authCookie = useCookie('auth', { maxAge: 60 * 60 * 24 * 7 }); // 7 days
      authCookie.value = JSON.stringify({
        accessToken: token,
        refreshToken: refresh,
        tenantId: userData.tenantId,
        user: userData,
      });

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

    // Clear cookie
    const authCookie = useCookie('auth');
    authCookie.value = null;
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
