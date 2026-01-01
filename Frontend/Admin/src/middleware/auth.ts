import type { Router } from "vue-router";
import { useAuthStore } from "@/stores/auth";

export function setupAuthMiddleware(router: Router) {
  router.beforeEach(async (to, _from, next) => {
    const authStore = useAuthStore();

    // Try to restore session if we have a token but no user
    if (authStore.token && !authStore.user) {
      try {
        await authStore.getCurrentUser();
      } catch (error) {
        // Token is invalid, clear it
        console.error("Failed to restore session:", error);
        await authStore.logout();
      }
    }

    // Check if route requires auth
    const requiresAuth = to.matched.some((record) => record.meta.requiresAuth);

    // Check if user is authenticated
    const isAuthenticated = authStore.isAuthenticated;

    // Redirect to login if auth required but not authenticated
    if (requiresAuth && !isAuthenticated) {
      next({ name: "Login", query: { redirect: to.fullPath } });
      return;
    }

    // If going to login and already authenticated, redirect to dashboard
    if (to.name === "Login" && isAuthenticated) {
      next({ name: "Dashboard" });
      return;
    }

    // Check role-based access
    // Admin and tenant-admin roles have access to all routes
    // tenant-admin is scoped to their tenant (enforced by backend)
    const requiredRole = to.meta.requiredRole as string | undefined;
    const hasAdminAccess =
      authStore.hasRole("admin") || authStore.hasRole("tenant-admin");
    if (requiredRole && !authStore.hasRole(requiredRole) && !hasAdminAccess) {
      next({ name: "Unauthorized" });
      return;
    }

    // Check permission-based access
    // Admin and tenant-admin roles bypass permission checks
    const requiredPermission = to.meta.requiredPermission as string | undefined;
    if (
      requiredPermission &&
      !authStore.hasPermission(requiredPermission) &&
      !hasAdminAccess
    ) {
      next({ name: "Unauthorized" });
      return;
    }

    next();
  });
}
