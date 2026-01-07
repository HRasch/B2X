<template>
  <div class="dashboard-page">
    <h1>{{ $t('dashboard.title') }}</h1>

    <div class="welcome-section">
      <h2>{{ $t('dashboard.welcome', { email: authStore.email }) }}</h2>
      <p>{{ $t('dashboard.subtitle') }}</p>
    </div>

    <div class="stats-grid">
      <div class="stat-card">
        <h3>{{ $t('dashboard.stats.storeInstances') }}</h3>
        <p class="stat-value">{{ storeStore.storeCount }}</p>
        <router-link to="/stores" class="link">{{ $t('dashboard.links.viewStores') }}</router-link>
      </div>

      <div class="stat-card">
        <h3>{{ $t('dashboard.stats.administrators') }}</h3>
        <p class="stat-value">{{ adminStore.admins.length }}</p>
        <router-link to="/admins" class="link">{{ $t('dashboard.links.manageAdmins') }}</router-link>
      </div>

      <div class="stat-card">
        <h3>{{ $t('dashboard.stats.emailMonitoring') }}</h3>
        <p class="stat-value">Monitor</p>
        <router-link to="/email-monitoring" class="link">{{ $t('dashboard.links.viewEmailStatus') }}</router-link>
      </div>

      <div class="stat-card">
        <h3>{{ $t('dashboard.stats.quickActions') }}</h3>
        <router-link to="/stores" class="action-btn">{{ $t('dashboard.links.createStore') }}</router-link>
        <router-link to="/admins" class="action-btn">{{ $t('dashboard.links.addAdministrator') }}</router-link>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/authStore';
import { useStoreStore } from '@/stores/storeStore';
import { useAdminStore } from '@/stores/adminStore';

const authStore = useAuthStore();
const storeStore = useStoreStore();
const adminStore = useAdminStore();
</script>

<style scoped>
.dashboard-page {
  padding: 2rem 0;
}

.dashboard-page h1 {
  margin-bottom: 2rem;
  color: #333;
}

.welcome-section {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 2rem;
  border-radius: 8px;
  margin-bottom: 2rem;
}

.welcome-section h2 {
  margin: 0 0 0.5rem 0;
}

.welcome-section p {
  margin: 0;
  opacity: 0.9;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 2rem;
  margin-top: 2rem;
}

.stat-card {
  background: white;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 2rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  transition:
    transform 0.3s,
    box-shadow 0.3s;
}

.stat-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.stat-card h3 {
  margin: 0 0 1rem 0;
  color: #667eea;
  font-size: 0.9rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.stat-value {
  font-size: 2.5rem;
  font-weight: bold;
  color: #333;
  margin: 0 0 1rem 0;
}

.link {
  color: #667eea;
  text-decoration: none;
  font-weight: 600;
  transition: color 0.3s;
}

.link:hover {
  color: #764ba2;
}

.action-btn {
  display: inline-block;
  margin-top: 0.5rem;
  padding: 0.5rem 1rem;
  background-color: #667eea;
  color: white;
  text-decoration: none;
  border-radius: 4px;
  font-size: 0.9rem;
  transition: background-color 0.3s;
}

.action-btn:hover {
  background-color: #5568d3;
}
</style>
