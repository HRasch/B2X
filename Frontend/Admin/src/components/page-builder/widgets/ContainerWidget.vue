<script setup lang="ts">
/**
 * ContainerWidget - Centered Container with Max-Width
 * Phase 1 MVP
 */
import { computed, provide, readonly } from 'vue'
import type { ContainerWidgetConfig, WidgetBase } from '@/types/widgets'

interface Props {
  config: ContainerWidgetConfig
  isEditing?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  isEditing: false
})

const emit = defineEmits<{
  (e: 'update:config', config: Partial<ContainerWidgetConfig>): void
  (e: 'select-child', widgetId: string): void
  (e: 'add-widget', index: number): void
}>()

// Provide editing context to children
provide('isEditing', readonly(props.isEditing))

const maxWidthMap: Record<string, string> = {
  sm: '640px',
  md: '768px',
  lg: '1024px',
  xl: '1280px',
  '2xl': '1536px',
  full: '100%'
}

const containerStyle = computed(() => ({
  maxWidth: maxWidthMap[props.config.maxWidth] || maxWidthMap.xl,
  marginLeft: props.config.centered ? 'auto' : undefined,
  marginRight: props.config.centered ? 'auto' : undefined,
  paddingLeft: '1rem',
  paddingRight: '1rem'
}))

const containerClass = computed(() => [
  'widget-container',
  `widget-container--${props.config.maxWidth || 'xl'}`,
  {
    'widget-container--centered': props.config.centered,
    'widget-container--editing': props.isEditing,
    'widget-container--empty': !props.config.children?.length
  }
])

function handleChildSelect(widget: WidgetBase) {
  emit('select-child', widget.id)
}

function handleAddWidget(index: number) {
  emit('add-widget', index)
}
</script>

<template>
  <div :class="containerClass" :style="containerStyle">
    <!-- Render children -->
    <template v-for="(child, index) in config.children" :key="child.id">
      <div 
        class="widget-container__item"
        @click.stop="handleChildSelect(child)"
      >
        <slot name="widget" :widget="child" :index="index">
          <div class="widget-container__placeholder">
            Widget: {{ child.type }}
          </div>
        </slot>
      </div>
    </template>

    <!-- Empty state for editing -->
    <div
      v-if="isEditing && (!config.children || config.children.length === 0)"
      class="widget-container__empty"
      @click="handleAddWidget(0)"
    >
      <svg class="widget-container__empty-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
        <path d="M12 5v14m-7-7h14" />
      </svg>
      <span class="widget-container__empty-text">Widget hinzufügen</span>
    </div>
  </div>
</template>

<style scoped>
.widget-container {
  width: 100%;
}

.widget-container__item {
  margin-bottom: 1.5rem;
}

.widget-container__item:last-child {
  margin-bottom: 0;
}

/* Empty state */
.widget-container__empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 120px;
  background-color: #f9fafb;
  border: 2px dashed #d1d5db;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
}

.widget-container__empty:hover {
  border-color: var(--color-primary, #3b82f6);
  background-color: #f0f9ff;
}

.widget-container__empty-icon {
  width: 28px;
  height: 28px;
  color: #9ca3af;
  margin-bottom: 8px;
}

.widget-container__empty-text {
  color: #6b7280;
  font-size: 0.875rem;
}

/* Placeholder */
.widget-container__placeholder {
  padding: 1rem;
  background-color: #f3f4f6;
  border: 1px solid #e5e7eb;
  border-radius: 4px;
  text-align: center;
  color: #6b7280;
  font-size: 0.875rem;
}

/* Editing mode */
.widget-container--editing {
  outline: 2px dashed transparent;
  outline-offset: 2px;
  transition: outline-color 0.2s;
}

.widget-container--editing:hover {
  outline-color: var(--color-primary, #3b82f6);
}

.widget-container--editing .widget-container__item {
  position: relative;
  cursor: pointer;
}

.widget-container--editing .widget-container__item:hover::after {
  content: '';
  position: absolute;
  inset: -4px;
  border: 2px solid var(--color-primary, #3b82f6);
  border-radius: 4px;
  pointer-events: none;
}

/* Responsive padding */
@media (max-width: 640px) {
  .widget-container {
    padding-left: 1rem;
    padding-right: 1rem;
  }
}

@media (min-width: 641px) {
  .widget-container {
    padding-left: 1.5rem;
    padding-right: 1.5rem;
  }
}

@media (min-width: 1025px) {
  .widget-container {
    padding-left: 2rem;
    padding-right: 2rem;
  }
}
</style>
