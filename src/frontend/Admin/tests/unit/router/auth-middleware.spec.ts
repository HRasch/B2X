import { describe, it, expect, beforeEach } from 'vitest';
import { createRouter, createMemoryHistory } from 'vue-router';
import type { Router, RouteRecordRaw } from 'vue-router';
import { createPinia, setActivePinia } from 'pinia';
import { useAuthStore } from '@/stores/auth';

describe('Authentication Middleware', () => {
  let router: Router;
  let authStore: ReturnType<typeof useAuthStore>;

  const routes: RouteRecordRaw[] = [
    {
      path: '/login',
      name: 'Login',
      component: { template: '<div>Login</div>' },
      meta: { requiresAuth: false },
    },
    {
      path: '/dashboard',
      name: 'Dashboard',
      component: { template: '<div>Dashboard</div>' },
      meta: { requiresAuth: true },
    },
    {
      path: '/cms/pages',
      name: 'CMSPages',
      component: { template: '<div>CMS Pages</div>' },
      meta: { requiresAuth: true, requiredRole: 'content_manager' },
    },
    {
      path: '/catalog/products',
      name: 'CatalogProducts',
      component: { template: '<div>Catalog Products</div>' },
      meta: { requiresAuth: true, requiredRole: 'catalog_manager' },
    },
    {
      path: '/ai/prompts',
      name: 'SystemPrompts',
      component: { template: '<div>System Prompts</div>' },
      meta: { requiresAuth: true, requiredRole: 'admin' },
    },
    {
      path: '/unauthorized',
      name: 'Unauthorized',
      component: { template: '<div>Unauthorized</div>' },
      meta: { requiresAuth: false },
    },
  ];

  beforeEach(() => {
    setActivePinia(createPinia());
    authStore = useAuthStore();

    router = createRouter({
      history: createMemoryHistory(),
      routes,
    });

    // Setup auth middleware with role checking
    router.beforeEach((to, _from, next) => {
      const isAuthenticated = authStore.isAuthenticated;
      const requiresAuth = to.meta.requiresAuth ?? true;

      // Check authentication
      if (requiresAuth && !isAuthenticated) {
        next({ name: 'Login', query: { redirect: to.fullPath } });
        return;
      }

      // Redirect authenticated user from login to dashboard
      if (!requiresAuth && isAuthenticated && to.name === 'Login') {
        next({ name: 'Dashboard' });
        return;
      }

      // Check role-based access
      const requiredRole = to.meta.requiredRole as string | undefined;
      if (requiredRole && isAuthenticated && !authStore.hasRole(requiredRole)) {
        next({ name: 'Unauthorized' });
        return;
      }

      next();
    });
  });

  describe('Unauthenticated Access', () => {
    it('should redirect to login when accessing protected route without auth', async () => {
      expect(authStore.isAuthenticated).toBe(false);

      await router.push('/dashboard');
      expect(router.currentRoute.value.name).toBe('Login');
    });

    it('should allow access to public routes without auth', async () => {
      expect(authStore.isAuthenticated).toBe(false);

      await router.push('/login');
      expect(router.currentRoute.value.name).toBe('Login');
    });

    it('should preserve redirect path in login query', async () => {
      await router.push('/dashboard');
      expect(router.currentRoute.value.query.redirect).toBe('/dashboard');
    });
  });

  describe('Authenticated Access', () => {
    it('should allow access to dashboard with authentication', async () => {
      // Mock authenticated user
      authStore.user = {
        id: 'user-1',
        email: 'user@example.com',
        firstName: 'John',
        lastName: 'Doe',
        tenantId: 'tenant-1',
        roles: [{ id: 'role-1', name: 'user', description: '', permissions: [] }],
        permissions: [],
      };
      authStore.token = 'test-token';

      await router.push('/dashboard');
      expect(router.currentRoute.value.name).toBe('Dashboard');
    });

    it('should redirect to dashboard when authenticated user goes to login', async () => {
      authStore.user = {
        id: 'user-1',
        email: 'user@example.com',
        firstName: 'John',
        lastName: 'Doe',
        tenantId: 'tenant-1',
        roles: [{ id: 'role-1', name: 'user', description: '', permissions: [] }],
        permissions: [],
      };
      authStore.token = 'test-token';

      await router.push('/login');
      expect(router.currentRoute.value.name).toBe('Dashboard');
    });
  });

  describe('Role-Based Access Control (RBAC)', () => {
    it('should deny access to content_manager route without role', async () => {
      authStore.user = {
        id: 'user-1',
        email: 'user@example.com',
        firstName: 'John',
        lastName: 'Doe',
        tenantId: 'tenant-1',
        roles: [{ id: 'role-1', name: 'user', description: '', permissions: [] }],
        permissions: [],
      };
      authStore.token = 'test-token';

      await router.push('/cms/pages');
      expect(router.currentRoute.value.name).toBe('Unauthorized');
    });

    it('should allow access to content_manager route with role', async () => {
      authStore.user = {
        id: 'user-1',
        email: 'user@example.com',
        firstName: 'John',
        lastName: 'Doe',
        tenantId: 'tenant-1',
        roles: [{ id: 'role-1', name: 'content_manager', description: '', permissions: [] }],
        permissions: [],
      };
      authStore.token = 'test-token';

      await router.push('/cms/pages');
      expect(router.currentRoute.value.name).toBe('CMSPages');
    });

    it('should deny access to catalog_manager route without role', async () => {
      authStore.user = {
        id: 'user-1',
        email: 'user@example.com',
        firstName: 'John',
        lastName: 'Doe',
        tenantId: 'tenant-1',
        roles: [{ id: 'role-1', name: 'content_manager', description: '', permissions: [] }],
        permissions: [],
      };
      authStore.token = 'test-token';

      await router.push('/catalog/products');
      expect(router.currentRoute.value.name).toBe('Unauthorized');
    });

    it('should allow access to catalog_manager route with role', async () => {
      authStore.user = {
        id: 'user-1',
        email: 'user@example.com',
        firstName: 'John',
        lastName: 'Doe',
        tenantId: 'tenant-1',
        roles: [{ id: 'role-1', name: 'catalog_manager', description: '', permissions: [] }],
        permissions: [],
      };
      authStore.token = 'test-token';

      await router.push('/catalog/products');
      expect(router.currentRoute.value.name).toBe('CatalogProducts');
    });

    it('should deny access to admin-only AI routes without admin role', async () => {
      authStore.user = {
        id: 'user-1',
        email: 'user@example.com',
        firstName: 'John',
        lastName: 'Doe',
        tenantId: 'tenant-1',
        roles: [{ id: 'role-1', name: 'content_manager', description: '', permissions: [] }],
        permissions: [],
      };
      authStore.token = 'test-token';

      await router.push('/ai/prompts');
      expect(router.currentRoute.value.name).toBe('Unauthorized');
    });

    it('should allow access to admin-only AI routes with admin role', async () => {
      authStore.user = {
        id: 'user-1',
        email: 'admin@example.com',
        firstName: 'Admin',
        lastName: 'User',
        tenantId: 'tenant-1',
        roles: [{ id: 'role-1', name: 'admin', description: '', permissions: [] }],
        permissions: [],
      };
      authStore.token = 'test-token';

      await router.push('/ai/prompts');
      expect(router.currentRoute.value.name).toBe('SystemPrompts');
    });

    it('should allow access with multiple roles when one matches', async () => {
      authStore.user = {
        id: 'user-1',
        email: 'user@example.com',
        firstName: 'John',
        lastName: 'Doe',
        tenantId: 'tenant-1',
        roles: [
          { id: 'role-1', name: 'user', description: '', permissions: [] },
          { id: 'role-2', name: 'content_manager', description: '', permissions: [] },
        ],
        permissions: [],
      };
      authStore.token = 'test-token';

      await router.push('/cms/pages');
      expect(router.currentRoute.value.name).toBe('CMSPages');
    });
  });

  describe('Auth Store Methods', () => {
    it('hasRole should return true when role exists', () => {
      authStore.user = {
        id: 'user-1',
        email: 'user@example.com',
        firstName: 'John',
        lastName: 'Doe',
        tenantId: 'tenant-1',
        roles: [{ id: 'role-1', name: 'content_manager', description: '', permissions: [] }],
        permissions: [],
      };

      expect(authStore.hasRole('content_manager')).toBe(true);
      // Admin role grants access to everything
      expect(authStore.hasRole('admin')).toBe(false); // This user only has content_manager
    });

    it('hasRole should return true for Admin user checking any role', () => {
      authStore.user = {
        id: 'user-1',
        email: 'admin@example.com',
        firstName: 'Admin',
        lastName: 'User',
        tenantId: 'tenant-1',
        roles: [{ id: 'role-1', name: 'Admin', description: '', permissions: [] }],
        permissions: [],
      };

      // Admin has access to ALL roles
      expect(authStore.hasRole('content_manager')).toBe(true);
      expect(authStore.hasRole('catalog_manager')).toBe(true);
      expect(authStore.hasRole('shop_manager')).toBe(true);
      expect(authStore.hasRole('admin')).toBe(true);
    });

    it('hasRole should be case-insensitive', () => {
      authStore.user = {
        id: 'user-1',
        email: 'user@example.com',
        firstName: 'John',
        lastName: 'Doe',
        tenantId: 'tenant-1',
        roles: [{ id: 'role-1', name: 'Content_Manager', description: '', permissions: [] }],
        permissions: [],
      };

      expect(authStore.hasRole('content_manager')).toBe(true);
      expect(authStore.hasRole('CONTENT_MANAGER')).toBe(true);
    });

    it('hasRole should return false when user is null', () => {
      authStore.user = null;

      expect(authStore.hasRole('content_manager')).toBe(false);
    });

    it('hasAnyRole should return true when any role matches', () => {
      authStore.user = {
        id: 'user-1',
        email: 'user@example.com',
        firstName: 'John',
        lastName: 'Doe',
        tenantId: 'tenant-1',
        roles: [{ id: 'role-1', name: 'content_manager', description: '', permissions: [] }],
        permissions: [],
      };

      expect(authStore.hasAnyRole(['admin', 'content_manager'])).toBe(true);
      expect(authStore.hasAnyRole(['admin', 'catalog_manager'])).toBe(false);
    });

    it('hasAnyRole should return true for Admin user regardless of required roles', () => {
      authStore.user = {
        id: 'user-1',
        email: 'admin@example.com',
        firstName: 'Admin',
        lastName: 'User',
        tenantId: 'tenant-1',
        roles: [{ id: 'role-1', name: 'Admin', description: '', permissions: [] }],
        permissions: [],
      };

      // Admin has access to all roles
      expect(authStore.hasAnyRole(['catalog_manager', 'shop_manager'])).toBe(true);
    });
  });
});
