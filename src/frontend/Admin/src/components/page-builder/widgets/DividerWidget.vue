<script setup lang="ts">
/**
 * DividerWidget - Horizontal Divider Widget
 * Phase 1 MVP
 */
import { computed } from 'vue';
import type { DividerWidgetConfig } from '@/types/widgets';

interface Props {
  config: DividerWidgetConfig;
  isEditing?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  isEditing: false,
});

const dividerStyle = computed(() => ({
  borderTopWidth: props.config.thickness || '1px',
  borderTopStyle: props.config.style || 'solid',
  borderTopColor: props.config.color || '#e5e7eb',
  width: props.config.width || '100%',
}));

const containerClass = computed(() => [
  'widget-divider',
  `widget-divider--${props.config.alignment || 'center'}`,
  {
    'widget-divider--editing': props.isEditing,
  },
]);
</script>

<template>
  <div :class="containerClass">
    <hr class="widget-divider__line" :style="dividerStyle" />
  </div>
</template>

<style scoped>
.widget-divider {
  width: 100%;
  display: flex;
  padding: 0.5rem 0;
}

/* Alignment */
.widget-divider--left {
  justify-content: flex-start;
}

.widget-divider--center {
  justify-content: center;
}

.widget-divider--right {
  justify-content: flex-end;
}

.widget-divider__line {
  border: none;
  margin: 0;
}

/* Editing mode */
.widget-divider--editing {
  position: relative;
  padding: 1rem 0;
  cursor: pointer;
}

.widget-divider--editing:hover {
  background-color: rgba(59, 130, 246, 0.05);
  border-radius: 4px;
}

.widget-divider--editing:hover::before {
  content: 'Divider';
  position: absolute;
  top: 0;
  left: 50%;
  transform: translateX(-50%);
  background-color: white;
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 0.75rem;
  color: #6b7280;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
}
</style>
