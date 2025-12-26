<template>
  <!-- Dynamically render widget component -->
  <component
    :is="widgetComponent"
    :settings="widget.settings"
    :widget-id="widget.id"
    class="widget-instance"
    :class="`widget-${widget.widgetTypeId}`"
  />
</template>

<script setup lang="ts">
import { computed, defineAsyncComponent } from 'vue';
import type { WidgetInstance } from '@/types/cms';

interface Props {
  widget: WidgetInstance;
}

const props = defineProps<Props>();

/**
 * Dynamically import widget component based on componentPath
 * Example: 'widgets/HeroBanner.vue' -> loads HeroBanner component
 * Uses a mapped import for better test support
 */
const widgetComponent = computed(() => {
  try {
    // Convert the componentPath to a proper import path
    // componentPath: 'widgets/HeroBanner.vue' -> './widgets/HeroBanner.vue'
    const importPath = `./${props.widget.componentPath}`;
    return defineAsyncComponent(() =>
      import(importPath)
    );
  } catch (err) {
    console.error(`Failed to load widget: ${props.widget.widgetTypeId}`, err);
    return defineAsyncComponent(() =>
      import('@/components/cms/WidgetNotFound.vue')
    );
  }
});
</script>

<style scoped>
.widget-instance {
  margin: 0;
  padding: 0;
}
</style>
