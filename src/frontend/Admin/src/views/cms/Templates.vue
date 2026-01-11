<template>
  <div class="space-y-6">
    <PageHeader :title="$t('cms.templates.title')" :subtitle="$t('cms.templates.subtitle')" />
    <CardContainer>
      <div class="text-center p-8">
        <p class="text-gray-500 dark:text-soft-300">{{ $t('cms.templates.coming_soon') }}</p>
      </div>
    </CardContainer>
  </div>
</template>

<script setup lang="ts"></script>
