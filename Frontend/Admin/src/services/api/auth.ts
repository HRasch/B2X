import { apiClient } from '../client';
import axios from 'axios';
import type { AdminUser, LoginRequest, LoginResponse, Role } from '@/types/auth';

/**
 * Transform backend role format to frontend Role[] format.
 * Backend returns roles as string[] (e.g., ["Admin", "User"])
 * Frontend expects Role[] with {id, name, description, permissions}
 */
function transformRoles(roles: unknown): Role[] {
  if (!roles || !Array.isArray(roles)) {
    return [];
  }

  return roles.map((role, index) => {
    // If already a Role object, return as-is
    if (typeof role === 'object' && role !== null && 'name' in role) {
      return role as Role;
    }
    // If string, transform to Role object
    if (typeof role === 'string') {
      const roleName = role;
      return {
        id: `role-${index + 1}`,
        name: roleName,
        description: getRoleDescription(roleName),
        permissions: [],
      };
    }
    // Fallback for unknown format
    return {
      id: `role-${index + 1}`,
      name: String(role),
      description: '',
      permissions: [],
    };
  });
}

/**
 * Get human-readable description for known role names
 */
function getRoleDescription(roleName: string): string {
  const descriptions: Record<string, string> = {
    Admin: 'Administrator with full access',
    User: 'Standard user',
    Manager: 'Manager with elevated permissions',
    Viewer: 'Read-only access',
  };
  return descriptions[roleName] || `${roleName} role`;
}

// Demo mode - DISABLED for production security
// Only enable for local E2E testing via environment variable
const DEMO_MODE = import.meta.env.VITE_ENABLE_DEMO_MODE === 'true';

// Default tenant GUID for admin authentication
const DEFAULT_TENANT_ID =
  import.meta.env.VITE_DEFAULT_TENANT_ID || '00000000-0000-0000-0000-000000000001';

const baseURL = import.meta.env.VITE_ADMIN_API_URL || 'http://localhost:8080';

export const authApi = {
  async login(credentials: LoginRequest): Promise<LoginResponse> {
    // Ensure tenant ID is set before any login attempt
    if (!sessionStorage.getItem('tenantId')) {
      sessionStorage.setItem('tenantId', DEFAULT_TENANT_ID);
    }

    // Demo mode: ONLY enabled via environment variable for testing
    // WARNING: Never enable in production!
    if (
      DEMO_MODE &&
      credentials.email === 'admin@example.com' &&
      credentials.password === 'password'
    ) {
      console.warn('[AUTH] Demo mode active - NOT FOR PRODUCTION USE');
      return new Promise(resolve => {
        setTimeout(() => {
          resolve({
            user: {
              id: 'admin-001',
              email: 'admin@example.com',
              firstName: 'Admin',
              lastName: 'User',
              tenantId: DEFAULT_TENANT_ID,
              roles: [
                {
                  id: '1',
                  name: 'Admin',
                  description: 'Administrator',
                  permissions: [],
                },
              ],
              permissions: [{ id: '1', name: '*', resource: '*', action: '*' }],
            },
            accessToken: 'demo-access-token-' + Date.now(),
            refreshToken: 'demo-refresh-token-' + Date.now(),
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
              message: 'Invalid credentials',
            },
          },
        },
      });
    }

    const tenantId = sessionStorage.getItem('tenantId') || DEFAULT_TENANT_ID;

    // Use axios with credentials for httpOnly cookie support
    const response = await axios.post<LoginResponse>(`${baseURL}/api/auth/login`, credentials, {
      headers: {
        'Content-Type': 'application/json',
        'X-Tenant-ID': tenantId,
      },
      withCredentials: true, // Enable httpOnly cookie handling
    });

    // Store tenant ID from response if provided (non-sensitive)
    if (response.data.user?.tenantId) {
      sessionStorage.setItem('tenantId', response.data.user.tenantId);
    }

    // Transform backend role format (string[]) to frontend format (Role[])
    // Backend returns: roles: ["Admin", "User"]
    // Frontend expects: roles: [{id, name, description, permissions}]
    const transformedResponse = {
      ...response.data,
      user: {
        ...response.data.user,
        roles: transformRoles(response.data.user?.roles),
      },
    };

    return transformedResponse;
  },

  async verify(token: string) {
    const response = await apiClient.post<AdminUser>('/api/auth/verify', {
      token,
    });
    // Transform roles if needed
    return {
      ...response,
      data: {
        ...response.data,
        roles: transformRoles(response.data.roles),
      },
    };
  },

  async getCurrentUser() {
    const response = await apiClient.get<AdminUser>('/api/auth/me');
    // Transform roles if needed
    return {
      ...response,
      data: {
        ...response.data,
        roles: transformRoles(response.data.roles),
      },
    };
  },

  updateProfile(data: Partial<AdminUser>) {
    return apiClient.put<AdminUser>('/api/auth/profile', data);
  },

  changePassword(oldPassword: string, newPassword: string) {
    return apiClient.post<void>('/api/auth/change-password', {
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
    // Transform roles if needed
    return {
      ...response.data,
      user: {
        ...response.data.user,
        roles: transformRoles(response.data.user?.roles),
      },
    };
  },

  requestMFA() {
    return apiClient.post<{ method: string }>('/api/auth/2fa/request', {});
  },

  verifyMFA(code: string) {
    return apiClient.post<LoginResponse>('/api/auth/2fa/verify', { code });
  },

  async logout(): Promise<void> {
    // In demo mode, just clear session storage (handled by store)
    if (DEMO_MODE) {
      return Promise.resolve();
    }
    // Real backend call - server will clear httpOnly cookies
    return apiClient.post<void>('/api/auth/logout', {});
  },
};
