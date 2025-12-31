<template>
  <div id="app" class="min-h-screen flex flex-col bg-base-100">
    <!-- Skip Links for Accessibility -->
    <a
      href="#main-content"
      class="sr-only focus:not-sr-only focus:absolute focus:top-4 focus:left-4 bg-primary text-primary-content px-4 py-2 rounded z-50"
    >
      Skip to main content
    </a>

    <!-- Navbar -->
    <nav class="navbar bg-base-200 shadow-lg sticky top-0 z-50" role="banner" aria-label="Main navigation">
      <div class="flex-1">
        <router-link
          to="/"
          class="btn btn-ghost normal-case text-xl text-primary font-bold"
        >
          B2Connect
        </router-link>
      </div>
      <div class="flex-none gap-2">
        <!-- Desktop Navigation -->
        <div class="navbar-center hidden lg:flex">
          <ul class="menu menu-horizontal px-1 gap-2">
            <li>
              <router-link to="/" class="btn btn-ghost">Home</router-link>
            </li>
            <li>
              <router-link to="/shop" class="btn btn-ghost">Shop</router-link>
            </li>
            <li>
              <router-link to="/cart" class="btn btn-ghost gap-2">
                Cart
                <span v-if="cartStore.items.length" class="badge badge-primary">
                  {{ cartStore.items.length }}
                </span>
              </router-link>
            </li>
            <li>
              <router-link to="/dashboard" class="btn btn-ghost"
                >Dashboard</router-link
              >
            </li>
            <li>
              <router-link to="/tenants" class="btn btn-ghost"
                >Tenants</router-link
              >
            </li>
            <li v-if="authStore.isAuthenticated">
              <button @click="logout" class="btn btn-ghost">Logout</button>
            </li>
            <li v-else>
              <router-link to="/login" class="btn btn-ghost">Login</router-link>
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
          <ul
            tabindex="0"
            class="dropdown-content menu p-2 shadow bg-base-100 rounded-box w-52"
          >
            <li><router-link to="/">Home</router-link></li>
            <li><router-link to="/shop">Shop</router-link></li>
            <li>
              <router-link to="/cart" class="gap-2">
                Cart
                <span v-if="cartStore.items.length" class="badge badge-primary">
                  {{ cartStore.items.length }}
                </span>
              </router-link>
            </li>
            <li><router-link to="/dashboard">Dashboard</router-link></li>
            <li><router-link to="/tenants">Tenants</router-link></li>
            <li v-if="authStore.isAuthenticated">
              <a @click="logout">Logout</a>
            </li>
            <li v-else>
              <router-link to="/login">Login</router-link>
            </li>
            <li>
              <LanguageSwitcher />
            </li>
          </ul>
        </div>
      </div>
    </nav>

    <!-- Main Content -->
    <main id="main-content" class="flex-1 container mx-auto py-8 px-4" role="main">
      <router-view />
    </main>

    <!-- Footer -->
    <footer class="footer bg-base-200 text-base-content p-10 mt-auto" role="contentinfo" aria-label="Site footer">
      <nav>
        <header class="footer-title">Services</header>
        <a class="link link-hover">Branding</a>
        <a class="link link-hover">Design</a>
        <a class="link link-hover">Marketing</a>
        <a class="link link-hover">Advertisement</a>
      </nav>
      <nav>
        <header class="footer-title">Company</header>
        <a class="link link-hover">About us</a>
        <a class="link link-hover">Contact</a>
        <a class="link link-hover">Jobs</a>
        <a class="link link-hover">Press kit</a>
      </nav>
      <nav>
        <header class="footer-title">Legal</header>
        <a class="link link-hover">Terms of use</a>
        <a class="link link-hover">Privacy policy</a>
        <a class="link link-hover">Cookie policy</a>
      </nav>
      <form>
        <header class="footer-title">Newsletter</header>
        <fieldset class="form-control w-80">
          <label class="label">
            <span class="label-text">Enter your email address</span>
          </label>
          <div class="join">
            <input
              type="email"
              placeholder="username@site.com"
              class="input input-bordered join-item"
            />
            <button type="submit" class="btn btn-primary join-item">
              Subscribe
            </button>
          </div>
        </fieldset>
      </form>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from "./stores/auth";
import { useCartStore } from "./stores/cart";
import { useRouter } from "vue-router";
import LanguageSwitcher from "./components/common/LanguageSwitcher.vue";

const authStore = useAuthStore();
const cartStore = useCartStore();
const router = useRouter();

/**
 * Handle user logout and redirect to login page.
 */
const logout = async (): Promise<void> => {
  authStore.logout();
  await router.push("/login");
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
</style>
