import { createRouter, createWebHistory } from "vue-router";
import type { RouteRecordRaw } from "vue-router";
import { useAuthStore } from "@/stores/auth";

const routes: RouteRecordRaw[] = [
  {
    path: "/login",
    name: "Login",
    component: () => import("@/views/Login.vue"),
    meta: { requiresAuth: false, layout: "auth" },
  },
  {
    path: "/unauthorized",
    name: "Unauthorized",
    component: () => import("@/views/Unauthorized.vue"),
    meta: { requiresAuth: false, layout: "auth" },
  },
  {
    path: "/",
    redirect: "/dashboard",
  },
  {
    path: "/dashboard",
    name: "Dashboard",
    component: () => import("@/views/Dashboard.vue"),
    meta: { requiresAuth: true, layout: "main" },
  },
  {
    path: "/cms",
    meta: {
      requiresAuth: true,
      layout: "main",
      requiredRole: "content_manager",
    },
    children: [
      {
        path: "pages",
        name: "CMSPages",
        component: () => import("@/views/cms/Pages.vue"),
      },
      {
        path: "pages/:id",
        name: "CMSPageDetail",
        component: () => import("@/views/cms/PageDetail.vue"),
      },
      {
        path: "templates",
        name: "CMSTemplates",
        component: () => import("@/views/cms/Templates.vue"),
      },
      {
        path: "media",
        name: "CMSMedia",
        component: () => import("@/views/cms/MediaLibrary.vue"),
      },
    ],
  },
  {
    path: "/catalog",
    meta: {
      requiresAuth: true,
      layout: "main",
      requiredRole: "catalog_manager",
    },
    children: [
      {
        path: "",
        redirect: "overview",
      },
      {
        path: "overview",
        name: "CatalogOverview",
        component: () => import("@/views/catalog/Overview.vue"),
      },
      {
        path: "products",
        name: "CatalogProducts",
        component: () => import("@/views/catalog/Products.vue"),
      },
      {
        path: "products/create",
        name: "CatalogProductCreate",
        component: () => import("@/views/catalog/ProductForm.vue"),
        meta: { formMode: "create" },
      },
      {
        path: "products/:id/edit",
        name: "CatalogProductEdit",
        component: () => import("@/views/catalog/ProductForm.vue"),
        meta: { formMode: "edit" },
      },
      {
        path: "categories",
        name: "CatalogCategories",
        component: () => import("@/views/catalog/Categories.vue"),
      },
      {
        path: "categories/create",
        name: "CatalogCategoryCreate",
        component: () => import("@/views/catalog/CategoryForm.vue"),
        meta: { formMode: "create" },
      },
      {
        path: "categories/:id/edit",
        name: "CatalogCategoryEdit",
        component: () => import("@/views/catalog/CategoryForm.vue"),
        meta: { formMode: "edit" },
      },
      {
        path: "brands",
        name: "CatalogBrands",
        component: () => import("@/views/catalog/Brands.vue"),
      },
      {
        path: "brands/create",
        name: "CatalogBrandCreate",
        component: () => import("@/views/catalog/BrandForm.vue"),
        meta: { formMode: "create" },
      },
      {
        path: "brands/:id/edit",
        name: "CatalogBrandEdit",
        component: () => import("@/views/catalog/BrandForm.vue"),
        meta: { formMode: "edit" },
      },
    ],
  },
  {
    path: "/shop",
    meta: { requiresAuth: true, layout: "main", requiredRole: "shop_manager" },
    children: [
      {
        path: "products",
        name: "ShopProducts",
        component: () => import("@/views/shop/Products.vue"),
      },
      {
        path: "products/:id",
        name: "ShopProductDetail",
        component: () => import("@/views/shop/ProductDetail.vue"),
      },
      {
        path: "categories",
        name: "ShopCategories",
        component: () => import("@/views/shop/Categories.vue"),
      },
      {
        path: "pricing",
        name: "ShopPricing",
        component: () => import("@/views/shop/Pricing.vue"),
      },
    ],
  },
  {
    path: "/jobs",
    meta: { requiresAuth: true, layout: "main", requiredRole: "operator" },
    children: [
      {
        path: "queue",
        name: "JobsQueue",
        component: () => import("@/views/jobs/JobQueue.vue"),
      },
      {
        path: ":id",
        name: "JobsDetail",
        component: () => import("@/views/jobs/JobDetail.vue"),
      },
      {
        path: "history",
        name: "JobsHistory",
        component: () => import("@/views/jobs/JobHistory.vue"),
      },
    ],
  },
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

// Navigation Guard für Authentication
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore();
  const requiresAuth = to.meta.requiresAuth ?? true; // Default: Auth required

  if (requiresAuth && !authStore.isAuthenticated) {
    // Nicht authentifiziert → Login
    next({ name: "Login", query: { redirect: to.fullPath } });
  } else if (
    !requiresAuth &&
    authStore.isAuthenticated &&
    to.name === "Login"
  ) {
    // Schon eingeloggt und versucht Login zu öffnen → Dashboard
    next({ name: "Dashboard" });
  } else {
    next();
  }
});

export default router;
