<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-3xl font-bold text-gray-900 dark:text-white">{{ $t('dashboard.title') }}</h1>
      <p class="mt-2 text-gray-600 dark:text-soft-300">
        {{ $t('dashboard.welcomeBack', { name: authStore.user?.firstName }) }}
      </p>
    </div>

    <!-- Quick Stats -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
      <div class="bg-white dark:bg-soft-800 rounded-lg shadow p-6">
        <div class="text-gray-600 dark:text-soft-300 text-sm font-medium">
          {{ $t('dashboard.stats.totalPages') }}
        </div>
        <div class="text-3xl font-bold mt-2 text-gray-900 dark:text-white">
          {{ cmsStore.pages.length }}
        </div>
      </div>
      <div class="bg-white dark:bg-soft-800 rounded-lg shadow p-6">
        <div class="text-gray-600 dark:text-soft-300 text-sm font-medium">
          {{ $t('dashboard.stats.totalProducts') }}
        </div>
        <div class="text-3xl font-bold mt-2 text-gray-900 dark:text-white">
          {{ shopStore.products.length }}
        </div>
      </div>
      <div class="bg-white dark:bg-soft-800 rounded-lg shadow p-6">
        <div class="text-gray-600 dark:text-soft-300 text-sm font-medium">
          {{ $t('dashboard.stats.activeJobs') }}
        </div>
        <div class="text-3xl font-bold mt-2 text-gray-900 dark:text-white">
          {{ activeJobsCount }}
        </div>
      </div>
      <div class="bg-white dark:bg-soft-800 rounded-lg shadow p-6">
        <div class="text-gray-600 dark:text-soft-300 text-sm font-medium">
          {{ $t('dashboard.stats.systemStatus') }}
        </div>
        <div class="text-lg font-semibold mt-2 text-green-600 dark:text-green-400">
          {{ $t('dashboard.stats.healthy') }}
        </div>
      </div>
    </div>

    <!-- Quick Actions -->
    <div class="bg-white dark:bg-soft-800 rounded-lg shadow p-6">
      <h2 class="text-lg font-semibold mb-4 text-gray-900 dark:text-white">
        {{ $t('dashboard.quickActions.title') }}
      </h2>
      <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
        <router-link
          to="/cms/pages"
          class="text-center p-4 bg-blue-50 dark:bg-blue-900/30 rounded-lg hover:bg-blue-100 dark:hover:bg-blue-900/50 transition text-gray-900 dark:text-white"
        >
          <div class="text-2xl mb-2">📄</div>
          <div class="text-sm font-medium">{{ $t('dashboard.quickActions.managePages') }}</div>
        </router-link>
        <router-link
          to="/shop/products"
          class="text-center p-4 bg-green-50 dark:bg-green-900/30 rounded-lg hover:bg-green-100 dark:hover:bg-green-900/50 transition text-gray-900 dark:text-white"
        >
          <div class="text-2xl mb-2">📦</div>
          <div class="text-sm font-medium">{{ $t('dashboard.quickActions.manageProducts') }}</div>
        </router-link>
        <router-link
          to="/jobs/queue"
          class="text-center p-4 bg-purple-50 dark:bg-purple-900/30 rounded-lg hover:bg-purple-100 dark:hover:bg-purple-900/50 transition text-gray-900 dark:text-white"
        >
          <div class="text-2xl mb-2">⚙️</div>
          <div class="text-sm font-medium">{{ $t('dashboard.quickActions.monitorJobs') }}</div>
        </router-link>
        <router-link
          to="/cms/media"
          class="text-center p-4 bg-yellow-50 dark:bg-yellow-900/30 rounded-lg hover:bg-yellow-100 dark:hover:bg-yellow-900/50 transition text-gray-900 dark:text-white"
        >
          <div class="text-2xl mb-2">🖼️</div>
          <div class="text-sm font-medium">{{ $t('dashboard.quickActions.mediaLibrary') }}</div>
        </router-link>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useCmsStore } from '@/stores/cms';
import { useShopStore } from '@/stores/shop';
import { useJobsStore } from '@/stores/jobs';

const authStore = useAuthStore();
const cmsStore = useCmsStore();
const shopStore = useShopStore();
const jobsStore = useJobsStore();

const activeJobsCount = computed(() => jobsStore.jobs.filter(j => j.status === 'running').length);

onMounted(async () => {
  await Promise.all([
    cmsStore.fetchPages(),
    shopStore.fetchProducts(),
    jobsStore.fetchJobs('running'),
  ]);
});
</script>
