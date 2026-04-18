<template>
  <div class="widget-not-found bg-yellow-50 border border-yellow-200 rounded p-4 my-4">
    <p class="text-yellow-700 font-semibold">{{ $t('widgets.notFound') }}</p>
    <p class="text-yellow-600 text-sm mt-1">
      {{ $t('widgets.notFoundDescription') }}
    </p>
  </div>
</template>

<script setup lang="ts">
interface Props {
  settings?: Record<string, string>;
  widgetId?: string;
}

defineProps<Props>();
</script>

<style scoped>
.widget-not-found {
  width: 100%;
}
</style>
