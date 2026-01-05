<template>
  <div class="space-y-6">
    <div class="flex items-center gap-4">
      <router-link to="/jobs/queue" class="btn btn-ghost btn-sm"> ← Back </router-link>
      <h1 class="text-2xl font-bold text-base-content">Job Details</h1>
    </div>

    <div class="card bg-base-100 shadow">
      <div class="card-body">
        <p class="text-base-content/60">Job details coming soon...</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRoute } from 'vue-router';

const route = useRoute();
const jobId = route.params.id;

console.log('Job ID:', jobId);
</script>
