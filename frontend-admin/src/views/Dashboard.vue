<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-3xl font-bold text-gray-900">Dashboard</h1>
      <p class="mt-2 text-gray-600">Welcome back, {{ authStore.user?.firstName }}!</p>
    </div>

    <!-- Quick Stats -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
      <div class="bg-white rounded-lg shadow p-6">
        <div class="text-gray-600 text-sm font-medium">Total Pages</div>
        <div class="text-3xl font-bold mt-2">{{ cmsStore.pages.length }}</div>
      </div>
      <div class="bg-white rounded-lg shadow p-6">
        <div class="text-gray-600 text-sm font-medium">Total Products</div>
        <div class="text-3xl font-bold mt-2">{{ shopStore.products.length }}</div>
      </div>
      <div class="bg-white rounded-lg shadow p-6">
        <div class="text-gray-600 text-sm font-medium">Active Jobs</div>
        <div class="text-3xl font-bold mt-2">{{ activeJobsCount }}</div>
      </div>
      <div class="bg-white rounded-lg shadow p-6">
        <div class="text-gray-600 text-sm font-medium">System Status</div>
        <div class="text-lg font-semibold mt-2 text-green-600">Healthy</div>
      </div>
    </div>

    <!-- Quick Actions -->
    <div class="bg-white rounded-lg shadow p-6">
      <h2 class="text-lg font-semibold mb-4">Quick Actions</h2>
      <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
        <router-link
          to="/cms/pages"
          class="text-center p-4 bg-blue-50 rounded-lg hover:bg-blue-100 transition"
        >
          <div class="text-2xl mb-2">📄</div>
          <div class="text-sm font-medium">Manage Pages</div>
        </router-link>
        <router-link
          to="/shop/products"
          class="text-center p-4 bg-green-50 rounded-lg hover:bg-green-100 transition"
        >
          <div class="text-2xl mb-2">📦</div>
          <div class="text-sm font-medium">Manage Products</div>
        </router-link>
        <router-link
          to="/jobs/queue"
          class="text-center p-4 bg-purple-50 rounded-lg hover:bg-purple-100 transition"
        >
          <div class="text-2xl mb-2">⚙️</div>
          <div class="text-sm font-medium">Monitor Jobs</div>
        </router-link>
        <router-link
          to="/cms/media"
          class="text-center p-4 bg-yellow-50 rounded-lg hover:bg-yellow-100 transition"
        >
          <div class="text-2xl mb-2">🖼️</div>
          <div class="text-sm font-medium">Media Library</div>
        </router-link>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useCmsStore } from '@/stores/cms'
import { useShopStore } from '@/stores/shop'
import { useJobsStore } from '@/stores/jobs'

const authStore = useAuthStore()
const cmsStore = useCmsStore()
const shopStore = useShopStore()
const jobsStore = useJobsStore()

const activeJobsCount = computed(() => jobsStore.jobs.filter(j => j.status === 'running').length)

onMounted(async () => {
  await Promise.all([
    cmsStore.fetchPages(),
    shopStore.fetchProducts(),
    jobsStore.fetchJobs('running'),
  ])
})
</script>
