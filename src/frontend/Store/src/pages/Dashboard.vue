<template>
  <div class="dashboard">
    <h1>{{ t('dashboard.title') }}</h1>

    <div v-if="user" class="user-info">
      <h2>{{ t('dashboard.welcome', { firstName: user.firstName, lastName: user.lastName }) }}</h2>
      <p>{{ t('dashboard.email') }}: {{ user.email }}</p>
      <p>{{ t('dashboard.tenantId') }}: {{ user.tenantId }}</p>
    </div>

    <div class="dashboard-grid">
      <div class="card">
        <h3>{{ t('dashboard.statistics.title') }}</h3>
        <p>{{ t('dashboard.statistics.description') }}</p>
      </div>

      <div class="card">
        <h3>{{ t('dashboard.recentActivity.title') }}</h3>
        <p>{{ t('dashboard.recentActivity.description') }}</p>
      </div>

      <div class="card">
        <h3>{{ t('dashboard.quickActions.title') }}</h3>
        <ul>
          <li>
            <router-link to="/tenants">{{ t('dashboard.quickActions.manageTenants') }}</router-link>
          </li>
          <li>
            <a href="#" @click.prevent="showSettings">{{
              t('dashboard.quickActions.accountSettings')
            }}</a>
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { useAuthStore } from '../stores/auth';

const { t } = useI18n();
const authStore = useAuthStore();
const user = computed(() => authStore.user);

const showSettings = () => {
  alert(t('dashboard.alerts.settingsComingSoon'));
};
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
