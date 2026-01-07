<template>
  <div class="drawer lg:drawer-open" data-test="main-layout">
    <!-- Drawer Toggle (Mobile) -->
    <input id="admin-drawer" type="checkbox" class="drawer-toggle" v-model="sidebarOpen" />

    <!-- Main Content Area -->
    <div class="drawer-content flex flex-col min-h-screen bg-base-200">
      <!-- Top Navbar -->
      <nav class="navbar bg-base-100 border-b border-base-300 sticky top-0 z-30">
        <!-- Mobile Menu Button -->
        <div class="flex-none lg:hidden">
          <label for="admin-drawer" class="btn btn-square btn-ghost" aria-label="Open menu">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M4 6h16M4 12h16M4 18h16"
              />
            </svg>
          </label>
        </div>

        <!-- Breadcrumb -->
        <div class="flex-1 px-2">
          <div class="breadcrumbs text-sm">
            <ul>
              <li><span class="text-base-content/60">Admin</span></li>
              <li>
                <span class="font-medium">{{ currentPageTitle }}</span>
              </li>
            </ul>
          </div>
        </div>

        <!-- Right Actions -->
        <div class="flex-none gap-2">
          <!-- Theme Toggle -->
          <div class="dropdown dropdown-end">
            <div tabindex="0" role="button" class="btn btn-ghost btn-circle">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z"
                />
              </svg>
            </div>
            <ul
              tabindex="0"
              class="dropdown-content menu bg-base-100 rounded-box z-50 w-40 p-2 shadow-lg border border-base-300"
            >
              <li>
                <button @click="setTheme('light')" :class="{ active: currentTheme === 'light' }">
                  Light
                </button>
              </li>
              <li>
                <button @click="setTheme('dark')" :class="{ active: currentTheme === 'dark' }">
                  Dark
                </button>
              </li>
              <li>
                <button
                  @click="setTheme('corporate')"
                  :class="{ active: currentTheme === 'corporate' }"
                >
                  Corporate
                </button>
              </li>
            </ul>
          </div>

          <!-- Notifications -->
          <button class="btn btn-ghost btn-circle" aria-label="Notifications">
            <div class="indicator">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9"
                />
              </svg>
              <span class="badge badge-xs badge-primary indicator-item"></span>
            </div>
          </button>

          <!-- User Dropdown -->
          <div class="dropdown dropdown-end">
            <div tabindex="0" role="button" class="btn btn-ghost btn-circle avatar placeholder">
              <div class="bg-primary text-primary-content rounded-full w-10">
                <span class="text-sm font-bold">{{ userInitial }}</span>
              </div>
            </div>
            <ul
              tabindex="0"
              class="dropdown-content menu bg-base-100 rounded-box z-50 w-52 p-2 shadow-lg border border-base-300"
            >
              <li class="menu-title">
                <span>{{ authStore.user?.email }}</span>
              </li>
              <li><a href="#">Profile</a></li>
              <li><a href="#">Settings</a></li>
              <div class="divider my-1"></div>
              <li><button @click="logout" class="text-error">Logout</button></li>
            </ul>
          </div>
        </div>
      </nav>

      <!-- Page Content -->
      <main class="flex-1 p-4 lg:p-6" data-test="main-content">
        <router-view />
      </main>
    </div>

    <!-- Sidebar -->
    <div class="drawer-side z-40">
      <label for="admin-drawer" class="drawer-overlay" aria-label="Close menu"></label>
      <aside class="bg-base-100 w-64 min-h-full border-r border-base-300" data-test="sidebar">
        <!-- Logo -->
        <div class="p-4 border-b border-base-300">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 rounded-lg bg-primary flex items-center justify-center">
              <span class="text-primary-content font-bold text-lg">B</span>
            </div>
            <div>
              <h1 class="font-bold text-base-content">B2Connect</h1>
              <p class="text-xs text-base-content/60">Admin Panel</p>
            </div>
          </div>
        </div>

        <!-- Navigation -->
        <nav class="p-4" data-test="sidebar-nav">
          <ul class="menu menu-md gap-1">
            <li v-for="item in navItems" :key="item.path">
              <router-link
                :to="item.path"
                :class="{ active: isActive(item.path) }"
                :data-test="`nav-link-${item.path}`"
              >
                <component :is="item.icon" class="w-5 h-5" />
                {{ item.label }}
              </router-link>
            </li>
          </ul>

          <div class="divider"></div>

          <!-- AI Section -->
          <ul class="menu menu-md gap-1">
            <li class="menu-title">AI Management</li>
            <li>
              <router-link to="/ai/dashboard" :class="{ active: isActive('/ai') }">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M9.75 17L9 20l-1 1h8l-1-1-.75-3M3 13h18M5 17h14a2 2 0 002-2V5a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"
                  />
                </svg>
                AI Dashboard
              </router-link>
            </li>
          </ul>
        </nav>
      </aside>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, h, type FunctionalComponent } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';

const authStore = useAuthStore();
const router = useRouter();
const route = useRoute();

const sidebarOpen = ref(false);
const currentTheme = ref('corporate');

// Icons as functional components
const DashboardIcon: FunctionalComponent = () =>
  h('svg', { class: 'w-5 h-5', fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
    h('path', {
      'stroke-linecap': 'round',
      'stroke-linejoin': 'round',
      'stroke-width': '2',
      d: 'M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6',
    }),
  ]);

const UsersIcon: FunctionalComponent = () =>
  h('svg', { class: 'w-5 h-5', fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
    h('path', {
      'stroke-linecap': 'round',
      'stroke-linejoin': 'round',
      'stroke-width': '2',
      d: 'M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z',
    }),
  ]);

const CMSIcon: FunctionalComponent = () =>
  h('svg', { class: 'w-5 h-5', fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
    h('path', {
      'stroke-linecap': 'round',
      'stroke-linejoin': 'round',
      'stroke-width': '2',
      d: 'M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10',
    }),
  ]);

const ShopIcon: FunctionalComponent = () =>
  h('svg', { class: 'w-5 h-5', fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
    h('path', {
      'stroke-linecap': 'round',
      'stroke-linejoin': 'round',
      'stroke-width': '2',
      d: 'M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z',
    }),
  ]);

const JobsIcon: FunctionalComponent = () =>
  h('svg', { class: 'w-5 h-5', fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
    h('path', {
      'stroke-linecap': 'round',
      'stroke-linejoin': 'round',
      'stroke-width': '2',
      d: 'M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15',
    }),
  ]);

const CliToolsIcon: FunctionalComponent = () =>
  h('svg', { class: 'w-5 h-5', fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
    h('path', {
      'stroke-linecap': 'round',
      'stroke-linejoin': 'round',
      'stroke-width': '2',
      d: 'M8 9l3 3-3 3m5 0h3M5 20h14a2 2 0 002-2V6a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z',
    }),
  ]);

const SeedingIcon: FunctionalComponent = () =>
  h('svg', { class: 'w-5 h-5', fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
    h('path', {
      'stroke-linecap': 'round',
      'stroke-linejoin': 'round',
      'stroke-width': '2',
      d: 'M12 6V4m0 2a2 2 0 100 4m0-4a2 2 0 110 4m-6 8a2 2 0 100-4m0 4a2 2 0 100 4m0-4v2m0-6V4m6 6v10m6-2a2 2 0 100-4m0 4a2 2 0 100 4m0-4v2m0-6V4',
    }),
  ]);

const EmailIcon: FunctionalComponent = () =>
  h('svg', { class: 'w-5 h-5', fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
    h('path', {
      'stroke-linecap': 'round',
      'stroke-linejoin': 'round',
      'stroke-width': '2',
      d: 'M3 8l7.89 4.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z',
    }),
  ]);

interface NavItem {
  path: string;
  label: string;
  icon: FunctionalComponent;
}

const navItems: NavItem[] = [
  { path: '/dashboard', label: 'Dashboard', icon: DashboardIcon },
  { path: '/users', label: 'Benutzer', icon: UsersIcon },
  { path: '/email/templates', label: 'E-Mail', icon: EmailIcon },
  { path: '/cms/pages', label: 'CMS', icon: CMSIcon },
  { path: '/shop/products', label: 'Shop', icon: ShopIcon },
  { path: '/jobs/queue', label: 'Jobs', icon: JobsIcon },
  { path: '/tools/cli', label: 'CLI Tools', icon: CliToolsIcon },
  { path: '/tools/seeding', label: 'Seeding', icon: SeedingIcon },
];

const currentPageTitle = computed(() => {
  const path = route.path;
  const item = navItems.find(i => path.startsWith(i.path));
  return item?.label || 'Dashboard';
});

const userInitial = computed(() => authStore.user?.email?.charAt(0).toUpperCase() || 'U');

const isActive = (path: string) => route.path.startsWith(path);

const setTheme = (theme: string) => {
  currentTheme.value = theme;
  document.documentElement.setAttribute('data-theme', theme);
  localStorage.setItem('admin-theme', theme);
};

const logout = async () => {
  await authStore.logout();
  router.push('/login');
};

onMounted(() => {
  const savedTheme = localStorage.getItem('admin-theme') || 'corporate';
  setTheme(savedTheme);
});
</script>
