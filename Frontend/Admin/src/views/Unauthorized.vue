<template>
  <div class="min-h-screen bg-gray-50 dark:bg-soft-900 flex items-center justify-center">
    <div class="bg-white dark:bg-soft-800 rounded-lg shadow p-8">
      <h1 class="text-2xl font-bold mb-4 text-gray-900 dark:text-white">Access Denied</h1>
      <p class="text-gray-600 dark:text-soft-300 mb-6">
        You do not have permission to access this page.
      </p>
      <router-link
        to="/dashboard"
        class="text-blue-600 dark:text-blue-400 hover:text-blue-800 dark:hover:text-blue-300"
      >
        Return to Dashboard
      </router-link>
    </div>
  </div>
</template>

<script setup lang="ts"></script>
