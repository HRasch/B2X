import type { Router } from 'vue-router';
import { useAuthStore } from '@/stores/auth';

export function setupAuthMiddleware(router: Router) {
  router.beforeEach(async (to, _from, next) => {
    const authStore = useAuthStore();

    // Check if route requires auth
    const requiresAuth = to.matched.some(record => record.meta.requiresAuth);

    // Check if user is authenticated
    const isAuthenticated = authStore.isAuthenticated;

    // Redirect to login if auth required but not authenticated
    if (requiresAuth && !isAuthenticated) {
      next({ name: 'Login', query: { redirect: to.fullPath } });
      return;
    }

    // If going to login and already authenticated, redirect to dashboard
    if (to.name === 'Login' && isAuthenticated) {
      next({ name: 'Dashboard' });
      return;
    }

    // Check role-based access
    const requiredRole = to.meta.requiredRole as string | undefined;
    if (requiredRole && !authStore.hasRole(requiredRole)) {
      next({ name: 'Unauthorized' });
      return;
    }

    // Check permission-based access
    const requiredPermission = to.meta.requiredPermission as string | undefined;
    if (requiredPermission && !authStore.hasPermission(requiredPermission)) {
      next({ name: 'Unauthorized' });
      return;
    }

    next();
  });
}
