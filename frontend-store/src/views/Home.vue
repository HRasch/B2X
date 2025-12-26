<template>
  <div class="home">
    <h1>Welcome to B2Connect</h1>
    <p class="subtitle">A modern multitenant SaaS platform for seamless business connectivity</p>

    <div v-if="!isAuthenticated" class="hero">
      <router-link to="/login" class="btn btn-primary btn-large">Get Started</router-link>
    </div>

    <div v-else class="features">
      <h2>Your Dashboard</h2>
      <p>You are logged in. Navigate to your dashboard to get started.</p>
      <router-link to="/dashboard" class="btn btn-primary">Go to Dashboard</router-link>
    </div>

    <div class="features-grid">
      <div class="feature">
        <h3>Multitenant Architecture</h3>
        <p>Secure data isolation for multiple organizations on a single platform.</p>
      </div>
      <div class="feature">
        <h3>Microservices</h3>
        <p>Scalable microservices designed for high performance and reliability.</p>
      </div>
      <div class="feature">
        <h3>Real-time Updates</h3>
        <p>Event-driven architecture with Wolverine for instant data synchronization.</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useAuthStore } from '../stores/auth'

const authStore = useAuthStore()
const isAuthenticated = computed(() => authStore.isAuthenticated)
</script>

<style scoped>
.home {
  text-align: center;
}

h1 {
  font-size: 2.5rem;
  margin-bottom: 0.5rem;
  color: #333;
}

.subtitle {
  font-size: 1.2rem;
  color: #666;
  margin-bottom: 2rem;
}

.hero {
  margin: 2rem 0;
}

.btn {
  display: inline-block;
  padding: 0.75rem 1.5rem;
  border-radius: 4px;
  text-decoration: none;
  transition: all 0.3s;
  border: none;
  cursor: pointer;
  font-size: 1rem;
}

.btn-primary {
  background-color: #007bff;
  color: white;
}

.btn-primary:hover {
  background-color: #0056b3;
}

.btn-large {
  padding: 1rem 2rem;
  font-size: 1.1rem;
}

.features-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 2rem;
  margin-top: 3rem;
}

.feature {
  padding: 2rem;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  background-color: #f9f9f9;
}

.feature h3 {
  color: #333;
  margin-bottom: 0.5rem;
}

.feature p {
  color: #666;
  line-height: 1.6;
}
</style>
