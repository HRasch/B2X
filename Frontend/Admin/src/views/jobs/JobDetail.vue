<template>
  <div class="space-y-6">
    <PageHeader :title="$t('jobs.detail.title')" :subtitle="$t('jobs.detail.subtitle')" />

    <CardContainer>
      <p class="text-base-content/60">{{ $t('jobs.detail.coming_soon') }}</p>
    </CardContainer>
  </div>
</template>

<script setup lang="ts">
import { useRoute } from 'vue-router';

const route = useRoute();
const jobId = route.params.id;

console.log('Job ID:', jobId);
</script>
