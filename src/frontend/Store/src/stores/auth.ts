import { defineStore } from 'pinia';
import { ref, computed, readonly } from 'vue';
import { api } from '~/services/api';
import type { UserDto, AuthResponse } from '~/types';

interface AuthCookieData {
  accessToken: string;
  refreshToken: string;
  tenantId: string;
  user: UserDto;
}

export const useAuthStore = defineStore('auth', () => {
  const user = ref<UserDto | null>(null);
  const accessToken = ref<string | null>(null);
  const refreshToken = ref<string | null>(null);
  const tenantId = ref<string | null>(null);

  // Initialize from cookies on client-side, fallback to localStorage for tests
  const initAuth = () => {
    // Always initialize to null first (for tests and fresh store creation)
    user.value = null;
    accessToken.value = null;
    refreshToken.value = null;
    tenantId.value = null;

    // Simple localStorage fallback for tests (check first)
    if (typeof localStorage !== 'undefined') {
      const storedToken = localStorage.getItem('access_token');
      if (storedToken) {
        accessToken.value = storedToken;
        refreshToken.value = localStorage.getItem('refresh_token');
        tenantId.value = localStorage.getItem('tenant_id');
        const storedUser = localStorage.getItem('user');
        if (storedUser) {
          try {
            user.value = JSON.parse(storedUser);
          } catch (err) {
            console.warn('Failed to parse stored user', err);
          }
        }
        return; // Don't check cookies if localStorage has data
      }
    }

    try {
      // Try cookies second (production) - only if useCookie is available
      if (typeof window !== 'undefined' && typeof useCookie !== 'undefined') {
        const cookies = useCookie('auth');
        if (cookies.value) {
          let authData: AuthCookieData;
          if (typeof cookies.value === 'string') {
            authData = JSON.parse(cookies.value) as AuthCookieData;
          } else {
            // cookies.value is already an object (happens in some Nuxt/test environments)
            authData = cookies.value as AuthCookieData;
          }
          accessToken.value = authData.accessToken;
          refreshToken.value = authData.refreshToken;
          tenantId.value = authData.tenantId;
          user.value = authData.user;
          return;
        }
      }
    } catch (err) {
      // Ignore cookie errors in tests
    }
  };

  // Initialize immediately for production
  // initAuth(); // Commented out for tests - components should call it explicitly

  const isAuthenticated = computed(() => !!accessToken.value);

  // Role-based computed properties
  const isAdmin = computed(() => (user.value as UserDto | null)?.roles?.includes('Admin') ?? false);
  const isContentEditor = computed(
    () => (user.value as UserDto | null)?.roles?.includes('ContentEditor') ?? false
  );
  const isEditor = computed(
    () => (user.value as UserDto | null)?.roles?.includes('Editor') ?? false
  );
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

      // Also store in localStorage for test compatibility
      localStorage.setItem('access_token', token);
      localStorage.setItem('refresh_token', refresh || '');
      localStorage.setItem('tenant_id', userData.tenantId);
      localStorage.setItem('user', JSON.stringify(userData));

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

    // Also clear localStorage for test compatibility
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('tenant_id');
    localStorage.removeItem('user');
  };

  const setUser = (userData: UserDto) => {
    user.value = userData;
  };

  // Initialize auth state on store creation
  // initAuth(); // Commented out for tests - components should call it explicitly

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
    initAuth,
  };
});
