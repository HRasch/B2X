<template>
  <div class="space-y-6">
    <PageHeader
      :title="$t('jobs.history.title')"
      :subtitle="$t('jobs.history.subtitle')"
    />

    <CardContainer
      :title="$t('jobs.history.content_title')"
      :subtitle="$t('jobs.history.coming_soon')"
    >
      <div class="text-center py-12">
        <p class="text-gray-500 dark:text-gray-400">{{ $t('jobs.history.coming_soon') }}</p>
      </div>
    </CardContainer>
  </div>
</template>

<script setup lang="ts">
// Job history component - placeholder
</script>
