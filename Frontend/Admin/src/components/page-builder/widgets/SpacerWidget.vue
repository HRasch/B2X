<script setup lang="ts">
/**
 * SpacerWidget - Vertical Spacing Widget
 * Phase 1 MVP
 */
import { computed } from 'vue'
import type { SpacerWidgetConfig, ResponsiveValue } from '@/types/widgets'

interface Props {
  config: SpacerWidgetConfig
  isEditing?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  isEditing: false
})

// Resolve responsive value
function resolveResponsive<T>(value: ResponsiveValue<T> | T | undefined, fallback: T): T {
  if (!value) return fallback
  if (typeof value === 'object' && 'desktop' in value) {
    return (value as ResponsiveValue<T>).desktop ?? fallback
  }
  return value as T
}

const height = computed(() => resolveResponsive(props.config.height, '2rem'))

const spacerStyle = computed(() => ({
  height: height.value
}))

// Responsive CSS custom properties
const responsiveVars = computed(() => ({
  '--spacer-height-mobile': typeof props.config.height === 'object'
    ? props.config.height.mobile || '1rem'
    : '1rem',
  '--spacer-height-tablet': typeof props.config.height === 'object'
    ? props.config.height.tablet || '1.5rem'
    : '1.5rem',
  '--spacer-height-desktop': height.value
}))

const containerClass = computed(() => [
  'widget-spacer',
  {
    'widget-spacer--editing': props.isEditing
  }
])
</script>

<template>
  <div :class="containerClass" :style="{ ...spacerStyle, ...responsiveVars }">
    <!-- Visual indicator in editing mode -->
    <div v-if="isEditing" class="widget-spacer__indicator">
      <span class="widget-spacer__label">{{ height }}</span>
    </div>
  </div>
</template>

<style scoped>
.widget-spacer {
  width: 100%;
}

/* Editing mode */
.widget-spacer--editing {
  position: relative;
  background: repeating-linear-gradient(
    45deg,
    transparent,
    transparent 5px,
    rgba(59, 130, 246, 0.05) 5px,
    rgba(59, 130, 246, 0.05) 10px
  );
  border: 1px dashed #d1d5db;
  border-radius: 4px;
  min-height: 20px;
}

.widget-spacer__indicator {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  display: flex;
  align-items: center;
  justify-content: center;
}

.widget-spacer__label {
  background-color: white;
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 0.75rem;
  color: #6b7280;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
}

/* Responsive height */
@media (max-width: 640px) {
  .widget-spacer {
    height: var(--spacer-height-mobile, 1rem) !important;
  }
}

@media (min-width: 641px) and (max-width: 1024px) {
  .widget-spacer {
    height: var(--spacer-height-tablet, 1.5rem) !important;
  }
}

@media (min-width: 1025px) {
  .widget-spacer {
    height: var(--spacer-height-desktop, 2rem) !important;
  }
}
</style>
