<script setup lang="ts">
/**
 * GridWidget - Flexible Grid Layout Widget
 * Phase 1 MVP
 */
import { computed, provide, readonly } from 'vue'
import type { GridWidgetConfig, ResponsiveValue, WidgetBase } from '@/types/widgets'

interface Props {
  config: GridWidgetConfig
  isEditing?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  isEditing: false
})

const emit = defineEmits<{
  (e: 'update:config', config: Partial<GridWidgetConfig>): void
  (e: 'select-child', widgetId: string): void
  (e: 'add-widget', index: number): void
}>()

// Provide editing context to children
provide('isEditing', readonly(props.isEditing))

// Resolve responsive value
function resolveResponsive<T>(value: ResponsiveValue<T> | T | undefined, fallback: T): T {
  if (!value) return fallback
  if (typeof value === 'object' && 'desktop' in value) {
    return (value as ResponsiveValue<T>).desktop ?? fallback
  }
  return value as T
}

const columns = computed(() => resolveResponsive(props.config.columns, 3))
const gap = computed(() => resolveResponsive(props.config.gap, '1.5rem'))

const gridStyle = computed(() => ({
  display: 'grid',
  gridTemplateColumns: `repeat(${columns.value}, minmax(0, 1fr))`,
  gap: gap.value,
  alignItems: props.config.alignItems || 'stretch',
  justifyContent: props.config.justifyContent || 'start'
}))

const containerClass = computed(() => [
  'widget-grid',
  {
    'widget-grid--editing': props.isEditing,
    'widget-grid--empty': !props.config.children?.length
  }
])

// Responsive CSS custom properties
const responsiveVars = computed(() => ({
  '--grid-cols-mobile': resolveResponsive(props.config.columns, 1),
  '--grid-cols-tablet': typeof props.config.columns === 'object' 
    ? props.config.columns.tablet || 2 
    : props.config.columns || 2,
  '--grid-cols-desktop': columns.value,
  '--grid-gap-mobile': typeof props.config.gap === 'object'
    ? props.config.gap.mobile || '1rem'
    : '1rem',
  '--grid-gap-tablet': typeof props.config.gap === 'object'
    ? props.config.gap.tablet || '1.5rem'
    : '1.5rem',
  '--grid-gap-desktop': gap.value
}))

function handleChildSelect(widget: WidgetBase) {
  emit('select-child', widget.id)
}

function handleAddWidget(index: number) {
  emit('add-widget', index)
}
</script>

<template>
  <div :class="containerClass" :style="responsiveVars">
    <div class="widget-grid__container" :style="gridStyle">
      <!-- Render children -->
      <template v-for="(child, index) in config.children" :key="child.id">
        <div 
          class="widget-grid__item"
          @click.stop="handleChildSelect(child)"
        >
          <!-- Child widget slot - parent will render appropriate component -->
          <slot name="widget" :widget="child" :index="index">
            <div class="widget-grid__placeholder">
              Widget: {{ child.type }}
            </div>
          </slot>
        </div>
      </template>

      <!-- Empty state for editing -->
      <div
        v-if="isEditing && (!config.children || config.children.length === 0)"
        class="widget-grid__empty"
        @click="handleAddWidget(0)"
      >
        <svg class="widget-grid__empty-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
          <path d="M12 5v14m-7-7h14" />
        </svg>
        <span class="widget-grid__empty-text">Widget hinzufügen</span>
      </div>

      <!-- Add button between items in editing mode -->
      <button
        v-if="isEditing && config.children?.length"
        class="widget-grid__add-btn"
        @click="handleAddWidget(config.children.length)"
      >
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M12 5v14m-7-7h14" />
        </svg>
      </button>
    </div>
  </div>
</template>

<style scoped>
.widget-grid {
  width: 100%;
}

.widget-grid__container {
  width: 100%;
}

.widget-grid__item {
  min-width: 0; /* Prevent overflow in grid */
}

/* Empty state */
.widget-grid__empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 150px;
  background-color: #f9fafb;
  border: 2px dashed #d1d5db;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
  grid-column: 1 / -1; /* Span all columns */
}

.widget-grid__empty:hover {
  border-color: var(--color-primary, #3b82f6);
  background-color: #f0f9ff;
}

.widget-grid__empty-icon {
  width: 32px;
  height: 32px;
  color: #9ca3af;
  margin-bottom: 8px;
}

.widget-grid__empty-text {
  color: #6b7280;
  font-size: 0.875rem;
}

/* Add button */
.widget-grid__add-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 80px;
  background-color: transparent;
  border: 2px dashed #e5e7eb;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
}

.widget-grid__add-btn:hover {
  border-color: var(--color-primary, #3b82f6);
  background-color: #f0f9ff;
}

.widget-grid__add-btn svg {
  width: 24px;
  height: 24px;
  color: #9ca3af;
}

.widget-grid__add-btn:hover svg {
  color: var(--color-primary, #3b82f6);
}

/* Placeholder for unimplemented widgets */
.widget-grid__placeholder {
  padding: 1rem;
  background-color: #f3f4f6;
  border: 1px solid #e5e7eb;
  border-radius: 4px;
  text-align: center;
  color: #6b7280;
  font-size: 0.875rem;
}

/* Editing mode */
.widget-grid--editing .widget-grid__item {
  position: relative;
  cursor: pointer;
}

.widget-grid--editing .widget-grid__item:hover::after {
  content: '';
  position: absolute;
  inset: -4px;
  border: 2px solid var(--color-primary, #3b82f6);
  border-radius: 4px;
  pointer-events: none;
}

/* Responsive grid using CSS custom properties */
@media (max-width: 640px) {
  .widget-grid__container {
    grid-template-columns: repeat(var(--grid-cols-mobile, 1), minmax(0, 1fr)) !important;
    gap: var(--grid-gap-mobile, 1rem) !important;
  }
}

@media (min-width: 641px) and (max-width: 1024px) {
  .widget-grid__container {
    grid-template-columns: repeat(var(--grid-cols-tablet, 2), minmax(0, 1fr)) !important;
    gap: var(--grid-gap-tablet, 1.5rem) !important;
  }
}

@media (min-width: 1025px) {
  .widget-grid__container {
    grid-template-columns: repeat(var(--grid-cols-desktop, 3), minmax(0, 1fr)) !important;
    gap: var(--grid-gap-desktop, 2rem) !important;
  }
}
</style>
