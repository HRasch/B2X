<template>
  <div class="dashboard">
    <h1>Dashboard</h1>

    <div v-if="user" class="user-info">
      <h2>Welcome, {{ user.firstName }} {{ user.lastName }}!</h2>
      <p>Email: {{ user.email }}</p>
      <p>Tenant ID: {{ user.tenantId }}</p>
    </div>

    <div class="dashboard-grid">
      <div class="card">
        <h3>Statistics</h3>
        <p>Your dashboard statistics will appear here.</p>
      </div>

      <div class="card">
        <h3>Recent Activity</h3>
        <p>Recent activities will be displayed here.</p>
      </div>

      <div class="card">
        <h3>Quick Actions</h3>
        <ul>
          <li><router-link to="/tenants">Manage Tenants</router-link></li>
          <li><a href="#" @click.prevent="showSettings">Account Settings</a></li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useAuthStore } from '../stores/auth'

const authStore = useAuthStore()
const user = computed(() => authStore.user)

const showSettings = () => {
  alert('Settings will be implemented soon')
}
</script>

<style scoped>
.dashboard {
  padding: 1rem 0;
}

h1 {
  color: #333;
  margin-bottom: 2rem;
}

.user-info {
  background-color: #f0f8ff;
  padding: 1.5rem;
  border-radius: 8px;
  margin-bottom: 2rem;
}

.user-info h2 {
  margin: 0 0 1rem 0;
  color: #333;
}

.user-info p {
  margin: 0.5rem 0;
  color: #666;
}

.dashboard-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 1.5rem;
}

.card {
  padding: 1.5rem;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  background-color: #ffffff;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.card h3 {
  margin: 0 0 1rem 0;
  color: #333;
}

.card ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.card li {
  margin: 0.5rem 0;
}

.card a {
  color: #007bff;
  text-decoration: none;
  transition: color 0.3s;
}

.card a:hover {
  color: #0056b3;
  text-decoration: underline;
}
</style>
