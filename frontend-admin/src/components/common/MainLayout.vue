<template>
  <div class="min-h-screen bg-soft-50">
    <!-- Sidebar Navigation -->
    <aside
      :class="[
        'fixed inset-y-0 left-0 w-64 bg-white border-r border-soft-100 shadow-soft-sm transition-transform duration-300 z-50',
        { '-translate-x-full md:translate-x-0': !sidebarOpen },
      ]"
    >
      <!-- Logo -->
      <div
        class="flex items-center justify-between p-safe border-b border-soft-100"
      >
        <div class="flex items-center gap-2">
          <div
            class="w-10 h-10 rounded-soft-lg bg-gradient-soft-blue flex items-center justify-center"
          >
            <span class="text-white font-bold text-lg">B</span>
          </div>
          <div>
            <h1 class="text-base font-bold text-soft-900">B2Connect</h1>
            <p class="text-xs text-soft-500">Admin</p>
          </div>
        </div>
        <button
          @click="sidebarOpen = false"
          class="md:hidden text-soft-400 hover:text-soft-600"
        >
          <svg
            class="w-5 h-5"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M6 18L18 6M6 6l12 12"
            />
          </svg>
        </button>
      </div>

      <!-- Navigation Menu -->
      <nav class="p-4 space-y-1">
        <router-link
          v-for="item in navItems"
          :key="item.path"
          :to="item.path"
          :class="[
            'flex items-center gap-3 px-4 py-3 rounded-soft text-sm font-medium transition-all duration-200',
            'hover:bg-soft-100',
            {
              'bg-primary-50 text-primary-600 shadow-soft-sm': isActive(
                item.path
              ),
              'text-soft-700': !isActive(item.path),
            },
          ]"
        >
          <svg
            class="w-5 h-5"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
          <span>{{ item.label }}</span>
        </router-link>
      </nav>

      <!-- Divider -->
      <div class="mx-4 my-6 border-t border-soft-100" />

      <!-- Settings Section -->
      <div class="p-4 space-y-1">
        <button
          @click="toggleDarkMode"
          class="w-full flex items-center gap-3 px-4 py-3 rounded-soft text-sm font-medium text-soft-700 hover:bg-soft-100 transition-all duration-200"
        >
          <svg
            class="w-5 h-5"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M20.354 15.354A9 9 0 0 0 8.646 3.646 9.003 9.003 0 0 0 12 21a9.003 9.003 0 0 0 8.354-5.646z"
            />
          </svg>
          <span>{{ isDark ? "Light" : "Dark" }} Mode</span>
        </button>
      </div>
    </aside>

    <!-- Top Navigation Bar -->
    <nav
      class="fixed top-0 right-0 left-0 md:left-64 bg-white border-b border-soft-100 shadow-soft-sm z-40"
    >
      <div class="px-safe py-4 flex items-center justify-between">
        <!-- Left: Menu Toggle + Breadcrumb -->
        <div class="flex items-center gap-4">
          <button
            @click="sidebarOpen = !sidebarOpen"
            class="md:hidden text-soft-600 hover:text-soft-900 transition-colors"
          >
            <svg
              class="w-6 h-6"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M4 6h16M4 12h16M4 18h16"
              />
            </svg>
          </button>
          <!-- Breadcrumb -->
          <div class="hidden md:flex items-center gap-2 text-sm text-soft-600">
            <span>Dashboard</span>
            <svg
              class="w-4 h-4"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M9 5l7 7-7 7"
              />
            </svg>
          </div>
        </div>

        <!-- Right: User Menu -->
        <div class="flex items-center gap-4">
          <!-- Notifications -->
          <button
            class="relative text-soft-600 hover:text-soft-900 transition-colors"
          >
            <svg
              class="w-5 h-5"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M15 17h5l-1.405-1.405A2.032 2.032 0 0 1 18 14.158V11a6.002 6.002 0 0 0-4-5.659V5a2 2 0 1 0-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 1 1-6 0v-1m6 0H9"
              />
            </svg>
            <span
              class="absolute top-0 right-0 block h-2 w-2 rounded-full bg-danger-500"
            />
          </button>

          <!-- User Dropdown -->
          <div class="relative" @click="userMenuOpen = !userMenuOpen">
            <button
              class="flex items-center gap-3 p-2 rounded-soft hover:bg-soft-100 transition-colors"
            >
              <div
                class="w-8 h-8 rounded-soft-lg bg-gradient-soft-purple flex items-center justify-center text-white text-sm font-semibold"
              >
                {{ authStore.user?.email?.charAt(0).toUpperCase() || "U" }}
              </div>
              <div class="hidden sm:block text-left">
                <p class="text-sm font-medium text-soft-900">
                  {{ authStore.user?.email?.split("@")[0] }}
                </p>
                <p class="text-xs text-soft-500">Admin</p>
              </div>
              <svg
                :class="[
                  'w-4 h-4 transition-transform duration-200',
                  { 'rotate-180': userMenuOpen },
                ]"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M19 14l-7 7m0 0l-7-7m7 7V3"
                />
              </svg>
            </button>

            <!-- Dropdown Menu -->
            <div
              v-if="userMenuOpen"
              @click:outside="userMenuOpen = false"
              class="absolute right-0 mt-2 w-48 bg-white rounded-soft-lg shadow-soft-lg border border-soft-100 z-50"
            >
              <a
                href="#"
                class="block px-4 py-3 text-sm text-soft-700 hover:bg-soft-50 first:rounded-t-soft-lg transition-colors"
              >
                Profile
              </a>
              <a
                href="#"
                class="block px-4 py-3 text-sm text-soft-700 hover:bg-soft-50 transition-colors"
              >
                Settings
              </a>
              <div class="border-t border-soft-100" />
              <button
                @click="logout"
                class="w-full text-left px-4 py-3 text-sm text-danger-600 hover:bg-danger-50 last:rounded-b-soft-lg transition-colors font-medium"
              >
                Logout
              </button>
            </div>
          </div>
        </div>
      </div>
    </nav>

    <!-- Main Content -->
    <main class="pt-20 md:pt-20 md:ml-64 min-h-screen bg-soft-50">
      <div class="p-safe">
        <router-view />
      </div>
    </main>

    <!-- Overlay for Mobile -->
    <div
      v-if="sidebarOpen"
      @click="sidebarOpen = false"
      class="fixed inset-0 bg-black/30 md:hidden z-40"
    />
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useRoute } from "vue-router";
import { useAuthStore } from "@/stores/auth";
import { useRouter } from "vue-router";

const authStore = useAuthStore();
const router = useRouter();
const route = useRoute();

const sidebarOpen = ref(false);
const userMenuOpen = ref(false);
const isDark = ref(false);

interface NavItem {
  path: string;
  label: string;
}

const navItems: NavItem[] = [
  {
    path: "/dashboard",
    label: "Dashboard",
  },
  {
    path: "/cms/pages",
    label: "CMS",
  },
  {
    path: "/shop/products",
    label: "Shop",
  },
  {
    path: "/jobs/queue",
    label: "Jobs",
  },
];

const isActive = (path: string) => {
  return route.path.startsWith(path);
};

const toggleDarkMode = () => {
  isDark.value = !isDark.value;
  document.documentElement.classList.toggle("dark", isDark.value);
};

const logout = async () => {
  await authStore.logout();
  router.push("/login");
};
</script>

<style scoped>
/* Smooth transitions */
:deep(.router-link-active) {
  @apply text-primary-600;
}
</style>
