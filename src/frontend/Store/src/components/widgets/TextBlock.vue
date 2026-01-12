<template>
  <section class="text-block-widget py-8" :style="{ maxWidth: settings.maxWidth + 'px' }">
    <div class="prose max-w-none" v-html="settings.content"></div>
  </section>
</template>

<script setup lang="ts">
interface TextBlockSettings {
  content?: string;
  maxWidth?: number;
}

interface Props {
  settings: TextBlockSettings;
  widgetId: string;
}

defineProps<Props>();
</script>

<style scoped>
.text-block-widget {
  width: 100%;
  margin-left: auto;
  margin-right: auto;
}

:deep(.prose) {
  font-family: inherit;
}

:deep(.prose p) {
  margin-bottom: 1rem;
  line-height: 1.6;
}

:deep(.prose h1),
:deep(.prose h2),
:deep(.prose h3) {
  margin-top: 1.5rem;
  margin-bottom: 0.5rem;
  font-weight: 600;
}

:deep(.prose a) {
  color: #2563eb;
  text-decoration: underline;
}
</style>
