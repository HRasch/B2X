<template>
  <div id="app" class="min-h-screen flex flex-col bg-base-100">
    <!-- Skip to main content link for accessibility -->
    <a
      href="#main-content"
      class="sr-only focus:not-sr-only focus:absolute focus:top-4 focus:left-4 bg-primary text-primary-content px-4 py-2 rounded z-50"
    >
      {{ t('common.skipToMain') }}
    </a>

    <!-- Navbar -->
    <nav class="navbar bg-base-200 shadow-lg sticky top-0 z-50">
      <div class="flex-1">
        <router-link to="/" class="btn btn-ghost normal-case text-xl text-primary font-bold">
          {{ t('common.brand') }}
        </router-link>
      </div>
      <div class="flex-none gap-2">
        <!-- Desktop Navigation -->
        <div class="navbar-center hidden lg:flex">
          <ul class="menu menu-horizontal px-1 gap-2">
            <li>
              <router-link to="/" class="btn btn-ghost">{{ t('navigation.home') }}</router-link>
            </li>
            <li>
              <router-link to="/shop" class="btn btn-ghost">{{ t('navigation.shop') }}</router-link>
            </li>
            <li>
              <router-link to="/cart" class="btn btn-ghost gap-2">
                {{ t('navigation.cart') }}
                <span v-if="cartStore.items.length" class="badge badge-primary">
                  {{ cartStore.items.length }}
                </span>
              </router-link>
            </li>
            <li>
              <router-link to="/dashboard" class="btn btn-ghost">{{
                t('navigation.dashboard')
              }}</router-link>
            </li>
            <li>
              <router-link to="/tenants" class="btn btn-ghost">{{
                t('navigation.tenants')
              }}</router-link>
            </li>
            <li v-if="authStore.isAuthenticated">
              <button @click="logout" class="btn btn-ghost">{{ t('navigation.logout') }}</button>
            </li>
            <li v-else>
              <router-link to="/login" class="btn btn-ghost">{{
                t('navigation.login')
              }}</router-link>
            </li>
            <!-- Admin Mode Toggle -->
            <li v-if="authStore.hasAdminAccess">
              <button
                @click="toggleAdminMode"
                class="btn btn-ghost"
                :class="{ 'btn-active': adminStore.isActive }"
                title="Admin-Modus umschalten"
              >
                <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                  <path
                    d="M10.894 2.553a1 1 0 00-1.788 0l-7 14a1 1 0 001.169 1.409l5-1.429A1 1 0 009 15.571V11a1 1 0 112 0v4.571a1 1 0 00.725.962l5 1.428a1 1 0 001.17-1.409l-7-14z"
                  />
                </svg>
                {{ t('app.admin') }}
              </button>
            </li>
            <li>
              <LanguageSwitcher />
            </li>
          </ul>
        </div>

        <!-- Mobile Dropdown -->
        <div class="dropdown dropdown-end lg:hidden">
          <label tabindex="0" class="btn btn-ghost btn-circle">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-5 w-5"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M4 6h16M4 12h16M4 18h7"
              />
            </svg>
          </label>
          <ul tabindex="0" class="dropdown-content menu p-2 shadow bg-base-100 rounded-box w-52">
            <li>
              <router-link to="/">{{ t('navigation.home') }}</router-link>
            </li>
            <li>
              <router-link to="/shop">{{ t('navigation.shop') }}</router-link>
            </li>
            <li>
              <router-link to="/cart" class="gap-2">
                {{ t('navigation.cart') }}
                <span v-if="cartStore.items.length" class="badge badge-primary">
                  {{ cartStore.items.length }}
                </span>
              </router-link>
            </li>
            <li>
              <router-link to="/dashboard">{{ t('navigation.dashboard') }}</router-link>
            </li>
            <li>
              <router-link to="/tenants">{{ t('navigation.tenants') }}</router-link>
            </li>
            <li v-if="authStore.isAuthenticated">
              <a @click="logout">{{ t('navigation.logout') }}</a>
            </li>
            <li v-else>
              <router-link to="/login">{{ t('navigation.login') }}</router-link>
            </li>
            <li>
              <LanguageSwitcher />
            </li>
          </ul>
        </div>
      </div>
    </nav>

    <!-- Main Content -->
    <main id="main-content" tabindex="-1" class="flex-1 container mx-auto py-8 px-4">
      <router-view />
    </main>

    <!-- Footer -->
    <footer class="footer bg-base-200 text-base-content p-10 mt-auto">
      <nav>
        <header class="footer-title">{{ t('app.services') }}</header>
        <a class="link link-hover text-base-content hover:text-primary-focus" href="#">{{
          t('app.branding')
        }}</a>
        <a class="link link-hover text-base-content hover:text-primary-focus" href="#">{{
          t('app.design')
        }}</a>
        <a class="link link-hover text-base-content hover:text-primary-focus" href="#">{{
          t('app.marketing')
        }}</a>
        <a class="link link-hover text-base-content hover:text-primary-focus" href="#">{{
          t('app.advertisement')
        }}</a>
      </nav>
      <nav>
        <header class="footer-title">{{ t('app.company') }}</header>
        <a class="link link-hover text-base-content hover:text-primary-focus" href="#">{{
          t('app.aboutUs')
        }}</a>
        <a class="link link-hover text-base-content hover:text-primary-focus" href="#">{{
          t('app.contact')
        }}</a>
        <a class="link link-hover text-base-content hover:text-primary-focus" href="#">{{
          t('app.jobs')
        }}</a>
        <a class="link link-hover text-base-content hover:text-primary-focus" href="#">{{
          t('app.pressKit')
        }}</a>
      </nav>
      <nav>
        <header class="footer-title">{{ t('app.legal') }}</header>
        <a class="link link-hover text-base-content hover:text-primary-focus" href="#">{{
          t('app.termsOfUse')
        }}</a>
        <a class="link link-hover text-base-content hover:text-primary-focus" href="#">{{
          t('app.privacyPolicy')
        }}</a>
        <a class="link link-hover text-base-content hover:text-primary-focus" href="#">{{
          t('app.cookiePolicy')
        }}</a>
      </nav>
      <form>
        <header class="footer-title">{{ t('app.newsletter') }}</header>
        <fieldset class="form-control w-80">
          <label class="label" for="newsletter-email">
            <span class="label-text">{{ t('app.enterEmail') }}</span>
          </label>
          <div class="join">
            <input
              id="newsletter-email"
              type="email"
              placeholder="username@site.com"
              class="input input-bordered join-item"
              aria-label="Email address for newsletter subscription"
            />
            <button type="submit" class="btn btn-primary join-item">
              {{ t('app.subscribe') }}
            </button>
          </div>
        </fieldset>
      </form>
    </footer>

    <!-- Admin FAB -->
    <AdminFab />
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from './stores/auth';
import { useCartStore } from './stores/cart';
import { useAdminStore } from './stores/admin';
import { useRouter } from 'vue-router';
import LanguageSwitcher from './components/common/LanguageSwitcher.vue';
import AdminFab from './components/AdminFab.vue';

const authStore = useAuthStore();
const cartStore = useCartStore();
const adminStore = useAdminStore();
const router = useRouter();

/**
 * Handle user logout and redirect to login page.
 */
const logout = async (): Promise<void> => {
  adminStore.clearEditableElements(); // Clean up admin mode
  authStore.logout();
  await router.push('/login');
};

/**
 * Toggle admin mode for users with admin access.
 */
const toggleAdminMode = (): void => {
  adminStore.toggleMode();
};
</script>

<style scoped>
.navbar {
  background-color: #ffffff;
  border-bottom: 1px solid #e0e0e0;
  padding: 0 1rem;
}

.navbar-container {
  display: flex;
  justify-content: space-between;
  align-items: center;
  max-width: 1200px;
  margin: 0 auto;
  padding: 1rem 0;
}

.navbar-logo {
  font-size: 1.5rem;
  font-weight: bold;
  color: #333;
  text-decoration: none;
}

.navbar-menu {
  display: flex;
  list-style: none;
  gap: 1.5rem;
  margin: 0;
  padding: 0;
  align-items: center;
}

.navbar-menu a {
  color: #333;
  text-decoration: none;
  transition: color 0.3s;
}

.navbar-menu a:hover {
  color: #007bff;
}

.navbar-menu button {
  padding: 0.5rem 1rem;
  background-color: #dc3545;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.3s;
}

.navbar-menu button:hover {
  background-color: #c82333;
}

.cart-link {
  position: relative;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
}

.cart-badge {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-width: 1.5rem;
  height: 1.5rem;
  background-color: #dc3545;
  color: white;
  border-radius: 50%;
  font-size: 0.75rem;
  font-weight: bold;
}

.language-switcher-container {
  margin-left: 0.5rem;
}

.main-content {
  max-width: 1200px;
  margin: 2rem auto;
  padding: 0 1rem;
}

/* Screen reader only - visible only when focused */
.sr-only {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0, 0, 0, 0);
  white-space: nowrap;
  border: 0;
}

.sr-only.focus:not-sr-only {
  position: static;
  width: auto;
  height: auto;
  padding: inherit;
  margin: inherit;
  overflow: visible;
  clip: auto;
  white-space: normal;
}
</style>
