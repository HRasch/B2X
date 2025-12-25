<template>
  <div id="app">
    <nav class="navbar">
      <div class="navbar-container">
        <router-link to="/" class="navbar-logo">B2Connect</router-link>
        <ul class="navbar-menu">
          <li><router-link to="/">Home</router-link></li>
          <li><router-link to="/shop">Shop</router-link></li>
          <li>
            <router-link to="/cart" class="cart-link">
              Cart <span v-if="cartStore.items.length" class="cart-badge">{{ cartStore.items.length }}</span>
            </router-link>
          </li>
          <li><router-link to="/dashboard">Dashboard</router-link></li>
          <li><router-link to="/tenants">Tenants</router-link></li>
          <li v-if="authStore.isAuthenticated">
            <button @click="logout">Logout</button>
          </li>
          <li v-else>
            <router-link to="/login">Login</router-link>
          </li>
          <li class="language-switcher-container">
            <LanguageSwitcher />
          </li>
        </ul>
      </div>
    </nav>

    <main class="main-content">
      <router-view />
    </main>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from './stores/auth'
import { useCartStore } from './stores/cart'
import { useRouter } from 'vue-router'
import LanguageSwitcher from './components/common/LanguageSwitcher.vue'

const authStore = useAuthStore()
const cartStore = useCartStore()
const router = useRouter()

const logout = () => {
  authStore.logout()
  router.push('/login')
}
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
