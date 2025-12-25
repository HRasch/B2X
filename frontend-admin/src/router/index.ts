import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'

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
    meta: { requiresAuth: true, layout: 'main', requiredRole: 'content_manager' },
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
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

export default router
