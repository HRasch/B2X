<template>
  <div
    class="flex items-center justify-center py-8"
    role="status"
    aria-live="polite"
    aria-label="Loading content"
  >
    <div
      class="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-blue-500"
      aria-hidden="true"
    ></div>
    <span class="sr-only">Loading...</span>
  </div>
</template>

<script setup lang="ts">
// Loading spinner component
</script>

<style scoped>
@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.animate-spin {
  animation: spin 1s linear infinite;
}
</style>
