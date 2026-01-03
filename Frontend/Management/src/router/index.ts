import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '@/stores/authStore';
import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    redirect: '/dashboard',
  },
  {
    path: '/login',
    component: () => import('@/pages/LoginPage.vue'),
    meta: { requiresAuth: false },
  },
  {
    path: '/dashboard',
    component: () => import('@/pages/DashboardPage.vue'),
    meta: { requiresAuth: true },
  },
  {
    path: '/stores',
    component: () => import('@/pages/StoresPage.vue'),
    meta: { requiresAuth: true },
  },
  {
    path: '/stores/:id',
    component: () => import('@/pages/StoreDetailPage.vue'),
    meta: { requiresAuth: true },
  },
  {
    path: '/admins',
    component: () => import('@/pages/AdminsPage.vue'),
    meta: { requiresAuth: true },
  },
  {
    path: '/admins/:id',
    component: () => import('@/pages/AdminDetailPage.vue'),
    meta: { requiresAuth: true },
  },
  {
    path: '/settings',
    component: () => import('@/pages/SettingsPage.vue'),
    meta: { requiresAuth: true },
  },
  {
    path: '/email-monitoring',
    component: () => import('@/pages/EmailMonitoringPage.vue'),
    meta: { requiresAuth: true },
  },
  {
    path: '/email-messages',
    component: () => import('@/pages/EmailMessagesPage.vue'),
    meta: { requiresAuth: true },
  },
  {
    path: '/:pathMatch(.*)*',
    component: () => import('@/pages/NotFoundPage.vue'),
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach((to, _from, next) => {
  const authStore = useAuthStore();
  const requiresAuth = to.meta.requiresAuth !== false;

  if (requiresAuth && !authStore.isAuthenticated) {
    next('/login');
  } else if (to.path === '/login' && authStore.isAuthenticated) {
    next('/dashboard');
  } else {
    next();
  }
});

export default router;
