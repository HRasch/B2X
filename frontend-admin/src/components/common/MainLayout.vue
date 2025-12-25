<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Navigation -->
    <nav class="bg-white shadow">
      <div class="mx-auto px-4 sm:px-6 lg:px-8 py-4">
        <div class="flex justify-between items-center">
          <div class="flex items-center space-x-8">
            <h1 class="text-2xl font-bold text-blue-600">B2Connect Admin</h1>
            <div v-if="authStore.isAuthenticated" class="hidden md:flex space-x-6">
              <router-link to="/dashboard" class="text-gray-700 hover:text-blue-600">
                Dashboard
              </router-link>
              <router-link to="/cms/pages" class="text-gray-700 hover:text-blue-600">
                CMS
              </router-link>
              <router-link to="/shop/products" class="text-gray-700 hover:text-blue-600">
                Shop
              </router-link>
              <router-link to="/jobs/queue" class="text-gray-700 hover:text-blue-600">
                Jobs
              </router-link>
            </div>
          </div>
          <div v-if="authStore.isAuthenticated" class="flex items-center space-x-4">
            <span class="text-gray-700">{{ authStore.user?.email }}</span>
            <button
              @click="logout"
              class="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700"
            >
              Logout
            </button>
          </div>
        </div>
      </div>
    </nav>

    <!-- Main Content -->
    <main class="mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <router-view />
    </main>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'

const authStore = useAuthStore()
const router = useRouter()

const logout = async () => {
  await authStore.logout()
  router.push('/login')
}
</script>

<style scoped>
/* Add your styles here */
</style>
