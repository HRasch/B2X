import { describe, it, expect, beforeEach, vi, afterEach } from 'vitest';
import { setActivePinia, createPinia } from 'pinia';
import { useAuthStore } from '@/stores/auth';
import { authApi } from '@/services/api/auth';
import type { LoginResponse } from '@/types/auth';

// Mock the auth API
vi.mock('@/services/api/auth', () => ({
  authApi: {
    login: vi.fn(),
    logout: vi.fn(),
    getCurrentUser: vi.fn(),
    updateProfile: vi.fn(),
  },
}));

describe('Auth Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia());
    localStorage.clear();
  });

  afterEach(() => {
    vi.clearAllMocks();
  });

  describe('Initial State', () => {
    it('should initialize with null user and no token', () => {
      const store = useAuthStore();
      expect(store.user).toBeNull();
      expect(store.token).toBeNull();
      expect(store.isAuthenticated).toBe(false);
    });

    it('should restore token from localStorage', () => {
      localStorage.setItem('authToken', 'test-token-123');
      const store = useAuthStore();
      expect(store.token).toBe('test-token-123');
    });
  });

  describe('Login', () => {
    it('should login successfully', async () => {
      const mockResponse: LoginResponse = {
        accessToken: 'access-token-123',
        refreshToken: 'refresh-token-456',
        user: {
          id: '1',
          email: 'admin@example.com',
          firstName: 'Admin',
          lastName: 'User',
          roles: [{ id: '1', name: 'super_admin', description: 'Super Admin', permissions: [] }],
          permissions: [],
          tenantId: 'tenant-1',
          isActive: true,
        },
      };

      vi.mocked(authApi.login).mockResolvedValue(mockResponse);

      const store = useAuthStore();
      const result = await store.login('admin@example.com', 'password123');

      expect(store.user).toEqual(mockResponse.user);
      expect(store.token).toBe('access-token-123');
      expect(store.isAuthenticated).toBe(true);
      expect(localStorage.getItem('authToken')).toBe('access-token-123');
      expect(result).toEqual(mockResponse);
    });

    it('should handle login error', async () => {
      const error = new Error('Invalid credentials');
      vi.mocked(authApi.login).mockRejectedValue(error);

      const store = useAuthStore();
      await expect(store.login('admin@example.com', 'wrongpassword')).rejects.toThrow(
        'Invalid credentials'
      );
      expect(store.user).toBeNull();
      expect(store.isAuthenticated).toBe(false);
    });

    it('should set loading state during login', async () => {
      const mockResponse: LoginResponse = {
        accessToken: 'token',
        refreshToken: 'refresh',
        user: {
          id: '1',
          email: 'admin@example.com',
          firstName: 'Admin',
          lastName: 'User',
          roles: [],
          permissions: [],
          tenantId: 'tenant-1',
          isActive: true,
        },
      };

      vi.mocked(authApi.login).mockImplementation(
        () => new Promise(resolve => setTimeout(() => resolve(mockResponse), 100))
      );

      const store = useAuthStore();
      const promise = store.login('admin@example.com', 'password');
      expect(store.loading).toBe(true);
      await promise;
      expect(store.loading).toBe(false);
    });
  });

  describe('Logout', () => {
    it('should logout and clear state', async () => {
      const store = useAuthStore();
      store.user = {
        id: '1',
        email: 'admin@example.com',
        firstName: 'Admin',
        lastName: 'User',
        roles: [],
        permissions: [],
        tenantId: 'tenant-1',
        isActive: true,
      };
      store.token = 'token-123';
      localStorage.setItem('authToken', 'token-123');

      await store.logout();

      expect(store.user).toBeNull();
      expect(store.token).toBeNull();
      expect(store.isAuthenticated).toBe(false);
      expect(localStorage.getItem('authToken')).toBeNull();
    });

    it('should handle logout error gracefully', async () => {
      vi.mocked(authApi.logout).mockRejectedValue(new Error('Network error'));

      const store = useAuthStore();
      store.user = {
        id: '1',
        email: 'admin@example.com',
        firstName: 'Admin',
        lastName: 'User',
        roles: [],
        permissions: [],
        tenantId: 'tenant-1',
        isActive: true,
      };

      await expect(store.logout()).resolves.not.toThrow();
      expect(store.user).toBeNull();
    });
  });

  describe('Permissions', () => {
    it('should check if user has permission', () => {
      const store = useAuthStore();
      store.user = {
        id: '1',
        email: 'admin@example.com',
        firstName: 'Admin',
        lastName: 'User',
        roles: [],
        permissions: [
          { id: '1', name: 'cms:edit', resource: 'cms', action: 'edit' },
          { id: '2', name: 'shop:view', resource: 'shop', action: 'view' },
        ],
        tenantId: 'tenant-1',
        isActive: true,
      };

      expect(store.hasPermission('cms:edit')).toBe(true);
      expect(store.hasPermission('shop:delete')).toBe(false);
    });

    it('should return false if no user', () => {
      const store = useAuthStore();
      expect(store.hasPermission('cms:edit')).toBe(false);
    });
  });

  describe('Roles', () => {
    it('should check if user has role', () => {
      const store = useAuthStore();
      store.user = {
        id: '1',
        email: 'admin@example.com',
        firstName: 'Admin',
        lastName: 'User',
        roles: [
          { id: '1', name: 'super_admin', description: 'Super Admin', permissions: [] },
          { id: '2', name: 'content_manager', description: 'Content Manager', permissions: [] },
        ],
        permissions: [],
        tenantId: 'tenant-1',
        isActive: true,
      };

      expect(store.hasRole('super_admin')).toBe(true);
      expect(store.hasRole('content_manager')).toBe(true);
      expect(store.hasRole('shop_manager')).toBe(false);
    });

    it('should check multiple roles with hasAnyRole', () => {
      const store = useAuthStore();
      store.user = {
        id: '1',
        email: 'admin@example.com',
        firstName: 'Admin',
        lastName: 'User',
        roles: [{ id: '1', name: 'super_admin', description: 'Super Admin', permissions: [] }],
        permissions: [],
        tenantId: 'tenant-1',
        isActive: true,
      };

      expect(store.hasAnyRole(['super_admin', 'admin'])).toBe(true);
      expect(store.hasAnyRole(['shop_manager', 'operator'])).toBe(false);
    });
  });

  describe('Get Current User', () => {
    it('should fetch current user', async () => {
      const mockUser = {
        id: '1',
        email: 'admin@example.com',
        firstName: 'Admin',
        lastName: 'User',
        roles: [],
        permissions: [],
        tenantId: 'tenant-1',
        isActive: true,
      };

      vi.mocked(authApi.getCurrentUser).mockResolvedValue(mockUser);

      const store = useAuthStore();
      store.token = 'token-123';

      await store.getCurrentUser();

      expect(store.user).toEqual(mockUser);
    });

    it('should logout if no token', async () => {
      const store = useAuthStore();
      store.token = null;

      await store.getCurrentUser();

      expect(store.user).toBeNull();
    });
  });

  describe('Update Profile', () => {
    it('should update user profile', async () => {
      const updatedUser = {
        id: '1',
        email: 'admin@example.com',
        firstName: 'UpdatedAdmin',
        lastName: 'User',
        roles: [],
        permissions: [],
        tenantId: 'tenant-1',
        isActive: true,
      };

      vi.mocked(authApi.updateProfile).mockResolvedValue(updatedUser);

      const store = useAuthStore();
      const result = await store.updateProfile({ firstName: 'UpdatedAdmin' });

      expect(store.user).toEqual(updatedUser);
      expect(result).toEqual(updatedUser);
    });

    it('should handle update error', async () => {
      const error = new Error('Update failed');
      vi.mocked(authApi.updateProfile).mockRejectedValue(error);

      const store = useAuthStore();
      await expect(store.updateProfile({ firstName: 'New' })).rejects.toThrow('Update failed');
    });
  });
});
