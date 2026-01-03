<template>
  <div class="app-container">
    <nav class="navbar">
      <div class="nav-brand">
        <h1>B2Connect Tenant Management</h1>
      </div>
      <div v-if="authStore.isAuthenticated" class="nav-menu">
        <router-link to="/dashboard">Dashboard</router-link>
        <router-link to="/stores">Stores</router-link>
        <router-link to="/admins">Administrators</router-link>
        <router-link to="/email-monitoring">Email Monitoring</router-link>
        <router-link to="/email-messages">Email Messages</router-link>
        <router-link to="/settings">Settings</router-link>
        <button @click="logout" class="btn-logout">Logout</button>
      </div>
    </nav>

    <main class="main-content">
      <router-view />
    </main>

    <footer class="footer">
      <p>&copy; 2025 B2Connect. All rights reserved.</p>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/authStore';
import { useRouter } from 'vue-router';

const authStore = useAuthStore();
const router = useRouter();

const logout = () => {
  authStore.logout();
  router.push('/login');
};
</script>

<style scoped>
.app-container {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}

.navbar {
  background-color: #1a1a2e;
  padding: 1rem 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.nav-brand h1 {
  margin: 0;
  font-size: 1.5rem;
  color: #00d4ff;
}

.nav-menu {
  display: flex;
  gap: 2rem;
  align-items: center;
}

.nav-menu a {
  color: #ffffff;
  text-decoration: none;
  transition: color 0.3s;
}

.nav-menu a:hover {
  color: #00d4ff;
}

.btn-logout {
  background-color: #ff6b6b;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.3s;
}

.btn-logout:hover {
  background-color: #ff5252;
}

.main-content {
  flex: 1;
  padding: 2rem;
  max-width: 1400px;
  width: 100%;
  margin: 0 auto;
}

.footer {
  background-color: #1a1a2e;
  padding: 1rem;
  text-align: center;
  color: #888;
  margin-top: 2rem;
}
</style>
