<template>
  <div :class="`region region-${region.name}`" :data-region-id="region.id">
    <!-- Apply region settings (classes, styles) -->
    <div class="region-content" :style="regionStyles">
      <!-- Render each widget in the region -->
      <div v-for="widget in region.widgets" :key="widget.id" class="widget-wrapper">
        <WidgetRenderer :widget="widget" />
      </div>

      <!-- Empty state (dev/admin mode) -->
      <div
        v-if="region.widgets.length === 0 && isDevelopment"
        class="empty-region border-2 border-dashed border-gray-300 rounded p-4 text-center text-gray-500"
      >
        <p>Region "{{ region.name }}" is empty</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type { PageRegion } from '@/types/cms';
import WidgetRenderer from '@/components/cms/WidgetRenderer.vue';

interface Props {
  region: PageRegion;
}

const props = defineProps<Props>();

const isDevelopment = import.meta.env.DEV;

/**
 * Build CSS classes/styles from region settings
 */
const regionStyles = computed(() => {
  const settings = props.region.settings;
  const styles: Record<string, any> = {};

  if (settings.backgroundColor) {
    styles.backgroundColor = settings.backgroundColor;
  }

  if (settings.padding) {
    styles.padding = settings.padding;
  }

  if (settings.maxWidth) {
    styles.maxWidth = settings.maxWidth;
    styles.marginLeft = 'auto';
    styles.marginRight = 'auto';
  }

  return styles;
});
</script>

<style scoped>
.region {
  width: 100%;
}

.region-content {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.widget-wrapper {
  width: 100%;
}

.empty-region {
  min-height: 200px;
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>
