import {
  createRouter,
  createWebHistory,
  type RouteRecordRaw,
} from "vue-router";
import { useAuthStore } from "../stores/auth";

const routes: RouteRecordRaw[] = [
  {
    path: "/",
    name: "Home",
    component: () => import("../views/Home.vue"),
  },
  {
    path: "/register/customer-type",
    name: "CustomerTypeSelection",
    component: () => import("../views/CustomerTypeSelection.vue"),
    meta: { requiresAuth: false },
  },
  {
    path: "/register/private",
    name: "PrivateCustomerRegistration",
    component: () => import("../views/PrivateCustomerRegistration.vue"),
    meta: { requiresAuth: false, title: "Private Customer Registration" },
  },
  {
    path: "/login",
    name: "Login",
    component: () => import("../views/Login.vue"),
    meta: { requiresAuth: false },
  },
  {
    path: "/dashboard",
    name: "Dashboard",
    component: () => import("../views/Dashboard.vue"),
    meta: { requiresAuth: true },
  },
  {
    path: "/tenants",
    name: "Tenants",
    component: () => import("../views/Tenants.vue"),
    meta: { requiresAuth: true },
  },
  {
    path: "/shop",
    name: "Store",
    component: () => import("../views/Store.vue"),
    meta: { requiresAuth: false },
  },
  {
    path: "/cart",
    name: "Cart",
    component: () => import("../views/Cart.vue"),
    meta: { requiresAuth: false },
  },
  {
    path: "/checkout",
    name: "Checkout",
    component: () => import("../views/Checkout.vue"),
    meta: { requiresAuth: false },
  },
  {
    path: "/:pathMatch(.*)*",
    name: "NotFound",
    component: () => import("../views/NotFound.vue"),
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore();
  const requiresAuth = to.meta?.requiresAuth === true;

  if (requiresAuth && !authStore.isAuthenticated) {
    // Redirect to login if authentication is required but user is not authenticated
    next({ name: "Login", query: { redirect: to.fullPath } });
  } else if (to.path === "/login" && authStore.isAuthenticated) {
    // Redirect to dashboard if already authenticated and trying to access login
    next({ name: "Dashboard" });
  } else {
    // Allow navigation
    next();
  }
});

export default router;
