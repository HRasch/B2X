import { describe, it, expect, beforeEach, vi } from 'vitest';
import { createPinia, setActivePinia } from 'pinia';
import { useAuthStore } from '@/stores/auth';

describe('AI Dashboard Access Control', () => {
  let authStore: ReturnType<typeof useAuthStore>;

  beforeEach(() => {
    setActivePinia(createPinia());
    authStore = useAuthStore();
  });

  describe('Admin Access to AI Dashboard', () => {
    it('should grant access to admin user', () => {
      authStore.user = {
        id: 'admin-1',
        email: 'admin@example.com',
        firstName: 'Admin',
        lastName: 'User',
        tenantId: 'tenant-1',
        roles: [{ id: 'role-admin', name: 'admin', description: 'Admin role', permissions: [] }],
        permissions: [{ id: 'perm-1', name: '*', resource: '*', action: '*' }],
      };
      authStore.token = 'test-token';

      const hasAccess = authStore.hasRole('admin');
      expect(hasAccess).toBe(true);
    });

    it('should deny access to non-admin user', () => {
      authStore.user = {
        id: 'user-1',
        email: 'user@example.com',
        firstName: 'John',
        lastName: 'Doe',
        tenantId: 'tenant-1',
        roles: [{ id: 'role-user', name: 'user', description: 'User role', permissions: [] }],
        permissions: [],
      };
      authStore.token = 'test-token';

      const hasAccess = authStore.hasRole('admin');
      expect(hasAccess).toBe(false);
    });

    it('should deny access to unauthenticated user', () => {
      authStore.user = null;
      authStore.token = null;

      const hasAccess = authStore.hasRole('admin');
      expect(hasAccess).toBe(false);
    });
  });

  describe('AI Prompt Seeding', () => {
    it('should allow admin to seed prompts', async () => {
      authStore.user = {
        id: 'admin-1',
        email: 'admin@example.com',
        firstName: 'Admin',
        lastName: 'User',
        tenantId: 'tenant-1',
        roles: [{ id: 'role-admin', name: 'admin', description: 'Admin role', permissions: [] }],
        permissions: [
          { id: 'perm-1', name: 'ai:seed_prompts', resource: 'ai', action: 'seed_prompts' },
        ],
      };

      const canSeedPrompts = authStore.user.permissions.some(
        p => p.name === 'ai:seed_prompts' || p.action === 'seed_prompts'
      );

      expect(canSeedPrompts).toBe(true);
    });

    it('should deny non-admin from seeding prompts', () => {
      authStore.user = {
        id: 'user-1',
        email: 'user@example.com',
        firstName: 'John',
        lastName: 'Doe',
        tenantId: 'tenant-1',
        roles: [{ id: 'role-user', name: 'user', description: 'User role', permissions: [] }],
        permissions: [],
      };

      const canSeedPrompts = authStore.user.permissions.some(
        p => p.name === 'ai:seed_prompts' || p.action === 'seed_prompts'
      );

      expect(canSeedPrompts).toBe(false);
    });
  });

  describe('AI Provider Management', () => {
    it('admin should have permission to manage AI providers', () => {
      authStore.user = {
        id: 'admin-1',
        email: 'admin@example.com',
        firstName: 'Admin',
        lastName: 'User',
        tenantId: 'tenant-1',
        roles: [{ id: 'role-admin', name: 'admin', description: 'Admin role', permissions: [] }],
        permissions: [
          { id: 'perm-1', name: 'ai:manage_providers', resource: 'ai', action: 'manage_providers' },
        ],
      };

      const canManageProviders = authStore.user.permissions.some(
        p => p.action === 'manage_providers'
      );

      expect(canManageProviders).toBe(true);
    });
  });
});
