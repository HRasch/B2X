import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router';
import { useAuthStore } from '../stores/auth';
import { storeVisibilityService } from '../services/storeVisibilityService';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'Home',
    component: () => import('../views/Home.vue'),
  },
  {
    path: '/register/customer-type',
    name: 'CustomerTypeSelection',
    component: () => import('../views/CustomerTypeSelection.vue'),
    meta: { requiresAuth: false },
  },
  {
    path: '/register/private',
    name: 'PrivateCustomerRegistration',
    component: () => import('../views/PrivateCustomerRegistration.vue'),
    meta: { requiresAuth: false, title: 'Private Customer Registration' },
  },
  {
    path: '/login',
    name: 'Login',
    component: () => import('../views/Login.vue'),
    meta: { requiresAuth: false },
  },
  {
    path: '/dashboard',
    name: 'Dashboard',
    component: () => import('../views/Dashboard.vue'),
    meta: { requiresAuth: true },
  },
  {
    path: '/tenants',
    name: 'Tenants',
    component: () => import('../views/Tenants.vue'),
    meta: { requiresAuth: true },
  },
  {
    path: '/shop',
    name: 'Store',
    component: () => import('../views/Store.vue'),
    meta: { requiresAuth: false },
  },
  {
    path: '/products',
    name: 'Products',
    component: () => import('../views/Store.vue'),
    meta: { requiresAuth: false },
  },
  {
    path: '/products/:id',
    name: 'ProductDetail',
    component: () => import('../views/ProductDetail.vue'),
    meta: { requiresAuth: false },
  },
  {
    path: '/categories',
    name: 'Categories',
    component: () => import('../views/Categories.vue'),
    meta: { requiresAuth: false },
  },
  {
    path: '/categories/:slug',
    name: 'CategoryDetail',
    component: () => import('../views/CategoryDetail.vue'),
    meta: { requiresAuth: false },
  },
  {
    path: '/cart',
    name: 'Cart',
    component: () => import('../views/Cart.vue'),
    meta: { requiresAuth: false },
  },
  {
    path: '/checkout',
    name: 'Checkout',
    component: () => import('../views/Checkout.vue'),
    meta: { requiresAuth: false },
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'NotFound',
    component: () => import('../views/NotFound.vue'),
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore();
  const requiresAuth = to.meta?.requiresAuth === true;

  // Check if store requires authentication (closed shop mode)
  // Skip check for login/register routes to avoid infinite redirect
  const isAuthRoute = ['Login', 'CustomerTypeSelection', 'PrivateCustomerRegistration'].includes(
    to.name as string
  );

  if (!isAuthRoute && !authStore.isAuthenticated) {
    try {
      const storeRequiresAuth = await storeVisibilityService.requiresAuthentication();
      if (storeRequiresAuth) {
        // Closed shop: redirect to login
        next({ name: 'Login', query: { redirect: to.fullPath } });
        return;
      }
    } catch (error) {
      // On error, default to allowing access (fail-open for public stores)
      console.warn('Failed to check store visibility:', error);
    }
  }

  if (requiresAuth && !authStore.isAuthenticated) {
    // Redirect to login if authentication is required but user is not authenticated
    next({ name: 'Login', query: { redirect: to.fullPath } });
  } else if (to.path === '/login' && authStore.isAuthenticated) {
    // Redirect to dashboard if already authenticated and trying to access login
    next({ name: 'Dashboard' });
  } else {
    // Allow navigation
    next();
  }
});

export default router;
