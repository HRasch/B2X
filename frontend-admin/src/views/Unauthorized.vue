<template>
  <div class="min-h-screen bg-gray-50 flex items-center justify-center">
    <div class="bg-white rounded-lg shadow p-8">
      <h1 class="text-2xl font-bold mb-4">Access Denied</h1>
      <p class="text-gray-600 mb-6">
        You do not have permission to access this page.
      </p>
      <router-link to="/dashboard" class="text-blue-600 hover:text-blue-800">
        Return to Dashboard
      </router-link>
    </div>
  </div>
</template>

<script setup lang="ts">
</script>
