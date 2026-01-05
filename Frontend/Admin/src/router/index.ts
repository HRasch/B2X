import { createRouter, createWebHistory } from 'vue-router';
import type { RouteRecordRaw } from 'vue-router';
import { useAuthStore } from '@/stores/auth';

const routes: RouteRecordRaw[] = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/Login.vue'),
    meta: { requiresAuth: false, layout: 'auth' },
  },
  {
    path: '/unauthorized',
    name: 'Unauthorized',
    component: () => import('@/views/Unauthorized.vue'),
    meta: { requiresAuth: false, layout: 'auth' },
  },
  {
    path: '/',
    redirect: '/dashboard',
  },
  {
    path: '/dashboard',
    name: 'Dashboard',
    component: () => import('@/views/Dashboard.vue'),
    meta: { requiresAuth: true, layout: 'main' },
  },
  {
    path: '/cms',
    meta: {
      requiresAuth: true,
      layout: 'main',
      requiredRole: 'content_manager',
    },
    children: [
      {
        path: 'pages',
        name: 'CMSPages',
        component: () => import('@/views/cms/Pages.vue'),
      },
      {
        path: 'pages/:id',
        name: 'CMSPageDetail',
        component: () => import('@/views/cms/PageDetail.vue'),
      },
      {
        path: 'pages/:id/edit',
        name: 'PageBuilder',
        component: () => import('@/views/PageBuilderView.vue'),
        meta: { layout: 'fullscreen' },
      },
      {
        path: 'templates',
        name: 'CMSTemplates',
        component: () => import('@/views/cms/Templates.vue'),
      },
      {
        path: 'media',
        name: 'CMSMedia',
        component: () => import('@/views/cms/MediaLibrary.vue'),
      },
    ],
  },
  {
    path: '/catalog',
    meta: {
      requiresAuth: true,
      layout: 'main',
      requiredRole: 'catalog_manager',
    },
    children: [
      {
        path: '',
        redirect: 'overview',
      },
      {
        path: 'overview',
        name: 'CatalogOverview',
        component: () => import('@/views/catalog/Overview.vue'),
      },
      {
        path: 'products',
        name: 'CatalogProducts',
        component: () => import('@/views/catalog/Products.vue'),
      },
      {
        path: 'products/create',
        name: 'CatalogProductCreate',
        component: () => import('@/views/catalog/ProductForm.vue'),
        meta: { formMode: 'create' },
      },
      {
        path: 'products/:id/edit',
        name: 'CatalogProductEdit',
        component: () => import('@/views/catalog/ProductForm.vue'),
        meta: { formMode: 'edit' },
      },
      {
        path: 'categories',
        name: 'CatalogCategories',
        component: () => import('@/views/catalog/Categories.vue'),
      },
      {
        path: 'categories/create',
        name: 'CatalogCategoryCreate',
        component: () => import('@/views/catalog/CategoryForm.vue'),
        meta: { formMode: 'create' },
      },
      {
        path: 'categories/:id/edit',
        name: 'CatalogCategoryEdit',
        component: () => import('@/views/catalog/CategoryForm.vue'),
        meta: { formMode: 'edit' },
      },
      {
        path: 'brands',
        name: 'CatalogBrands',
        component: () => import('@/views/catalog/Brands.vue'),
      },
      {
        path: 'brands/create',
        name: 'CatalogBrandCreate',
        component: () => import('@/views/catalog/BrandForm.vue'),
        meta: { formMode: 'create' },
      },
      {
        path: 'brands/:id/edit',
        name: 'CatalogBrandEdit',
        component: () => import('@/views/catalog/BrandForm.vue'),
        meta: { formMode: 'edit' },
      },
    ],
  },
  {
    path: '/shop',
    meta: { requiresAuth: true, layout: 'main', requiredRole: 'shop_manager' },
    children: [
      {
        path: 'products',
        name: 'ShopProducts',
        component: () => import('@/views/shop/Products.vue'),
      },
      {
        path: 'products/:id',
        name: 'ShopProductDetail',
        component: () => import('@/views/shop/ProductDetail.vue'),
      },
      {
        path: 'categories',
        name: 'ShopCategories',
        component: () => import('@/views/shop/Categories.vue'),
      },
      {
        path: 'pricing',
        name: 'ShopPricing',
        component: () => import('@/views/shop/Pricing.vue'),
      },
    ],
  },
  {
    path: '/jobs',
    meta: { requiresAuth: true, layout: 'main', requiredRole: 'operator' },
    children: [
      {
        path: 'queue',
        name: 'JobsQueue',
        component: () => import('@/views/jobs/JobQueue.vue'),
      },
      {
        path: ':id',
        name: 'JobsDetail',
        component: () => import('@/views/jobs/JobDetail.vue'),
      },
      {
        path: 'history',
        name: 'JobsHistory',
        component: () => import('@/views/jobs/JobHistory.vue'),
      },
    ],
  },
  {
    path: '/users',
    meta: { requiresAuth: true, layout: 'main', requiredRole: 'admin' },
    children: [
      {
        path: '',
        name: 'UserList',
        component: () => import('@/views/users/UserList.vue'),
      },
      {
        path: 'create',
        name: 'UserCreate',
        component: () => import('@/views/users/UserForm.vue'),
        meta: { formMode: 'create' },
      },
      {
        path: ':id',
        name: 'UserDetail',
        component: () => import('@/views/users/UserDetail.vue'),
      },
      {
        path: ':id/edit',
        name: 'UserEdit',
        component: () => import('@/views/users/UserForm.vue'),
        meta: { formMode: 'edit' },
      },
    ],
  },
  {
    path: '/tools',
    meta: {
      requiresAuth: true,
      layout: 'main',
      requiredRole: 'admin',
    },
    children: [
      {
        path: 'cli',
        name: 'CliTools',
        component: () => import('@/views/tools/CliToolsView.vue'),
      },
    ],
  },
  {
    path: '/ai',
    meta: {
      requiresAuth: true,
      layout: 'main',
      requiredRole: 'admin',
    },
    children: [
      {
        path: '',
        name: 'AIDashboard',
        component: () => import('@/views/ai/Dashboard.vue'),
      },
      {
        path: 'prompts',
        name: 'SystemPrompts',
        component: () => import('@/views/ai/SystemPrompts.vue'),
      },
      {
        path: 'prompts/:toolType/:key',
        name: 'SystemPromptDetail',
        component: () => import('@/views/ai/SystemPromptDetail.vue'),
      },
      {
        path: 'providers',
        name: 'AIProviders',
        component: () => import('@/views/ai/Providers.vue'),
      },
      {
        path: 'consumption',
        name: 'AIConsumption',
        component: () => import('@/views/ai/Consumption.vue'),
      },
      {
        path: 'mcp-status',
        name: 'MCPStatus',
        component: () => import('@/views/ai/Status.vue'),
      },
    ],
  },
  {
    path: '/email',
    meta: {
      requiresAuth: true,
      layout: 'main',
      requiredRole: 'content_manager',
    },
    children: [
      {
        path: 'templates',
        name: 'EmailTemplates',
        component: () => import('@/views/email/EmailTemplatesList.vue'),
      },
      {
        path: 'templates/create',
        name: 'EmailTemplateCreate',
        component: () => import('@/views/email/EmailTemplateCreate.vue'),
      },
      {
        path: 'templates/:id/edit',
        name: 'EmailTemplateEdit',
        component: () => import('@/views/email/EmailTemplateEdit.vue'),
      },
    ],
  },
  {
    path: '/tools',
    meta: {
      requiresAuth: true,
      layout: 'main',
      requiredRole: 'admin',
    },
    children: [
      {
        path: 'cli',
        name: 'CliTools',
        component: () => import('@/views/tools/CliToolsView.vue'),
      },
    ],
  },
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

// Navigation Guard für Authentication und Authorization
router.beforeEach((to, _from, next) => {
  const authStore = useAuthStore();
  const requiresAuth = to.meta.requiresAuth ?? true; // Default: Auth required

  // Check authentication
  if (requiresAuth && !authStore.isAuthenticated) {
    // Not authenticated → Redirect to login
    next({ name: 'Login', query: { redirect: to.fullPath } });
    return;
  }

  // Redirect authenticated user from login to dashboard
  if (!requiresAuth && authStore.isAuthenticated && to.name === 'Login') {
    // Already logged in and trying to access login → Go to dashboard
    next({ name: 'Dashboard' });
    return;
  }

  // Check role-based access
  const requiredRole = to.meta.requiredRole as string | undefined;
  if (requiredRole && authStore.isAuthenticated && !authStore.hasRole(requiredRole)) {
    // User lacks required role → Unauthorized
    next({ name: 'Unauthorized' });
    return;
  }

  // Check permission-based access
  const requiredPermission = to.meta.requiredPermission as string | undefined;
  if (
    requiredPermission &&
    authStore.isAuthenticated &&
    !authStore.hasPermission(requiredPermission)
  ) {
    // User lacks required permission → Unauthorized
    next({ name: 'Unauthorized' });
    return;
  }

  next();
});

export default router;
