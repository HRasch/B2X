<template>
  <div id="app">
    <!-- Auth Layout (for login page) -->
    <template v-if="$route.meta.layout === 'auth'">
      <router-view />
    </template>

    <!-- Main Layout (for authenticated pages) -->
    <template v-else-if="$route.meta.layout === 'main' || isAuthenticated">
      <MainLayout />
    </template>

    <!-- Default -->
    <template v-else>
      <router-view />
    </template>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useThemeStore } from '@/stores/theme';
import MainLayout from '@/components/common/MainLayout.vue';

const authStore = useAuthStore();
const themeStore = useThemeStore();
const isAuthenticated = computed(() => authStore.isAuthenticated);

// Initialize theme and auth on app load
onMounted(async () => {
  // Initialize theme
  themeStore.initializeTheme();

  // Initialize auth
  const token = localStorage.getItem('authToken');
  if (token && !authStore.user) {
    try {
      await authStore.getCurrentUser();
    } catch (error) {
      console.error('Failed to load user:', error);
    }
  }
});
</script>

<style scoped>
#app {
  width: 100%;
  min-height: 100vh;
}
</style>
