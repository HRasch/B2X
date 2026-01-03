<script setup lang="ts">
/**
 * SectionWidget - Full-width Section with Background
 * Phase 1 MVP
 */
import { computed, provide, readonly } from 'vue'
import type { SectionWidgetConfig, ResponsiveValue, WidgetBase } from '@/types/widgets'

interface Props {
  config: SectionWidgetConfig
  isEditing?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  isEditing: false
})

const emit = defineEmits<{
  (e: 'update:config', config: Partial<SectionWidgetConfig>): void
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

const paddingTop = computed(() => resolveResponsive(props.config.paddingTop, '4rem'))
const paddingBottom = computed(() => resolveResponsive(props.config.paddingBottom, '4rem'))

const sectionStyle = computed(() => {
  const style: Record<string, string> = {
    paddingTop: paddingTop.value,
    paddingBottom: paddingBottom.value
  }

  if (props.config.backgroundColor) {
    style.backgroundColor = props.config.backgroundColor
  }

  if (props.config.backgroundImage) {
    style.backgroundImage = `url(${props.config.backgroundImage})`
    style.backgroundSize = 'cover'
    style.backgroundPosition = 'center'
  }

  return style
})

const overlayStyle = computed(() => {
  if (!props.config.backgroundOverlay) return null
  return {
    backgroundColor: props.config.backgroundOverlay
  }
})

const containerClass = computed(() => [
  'widget-section',
  {
    'widget-section--full-width': props.config.fullWidth,
    'widget-section--editing': props.isEditing,
    'widget-section--has-bg-image': !!props.config.backgroundImage,
    'widget-section--empty': !props.config.children?.length
  }
])

// Responsive CSS custom properties
const responsiveVars = computed(() => ({
  '--section-pt-mobile': typeof props.config.paddingTop === 'object'
    ? props.config.paddingTop.mobile || '2rem'
    : '2rem',
  '--section-pt-tablet': typeof props.config.paddingTop === 'object'
    ? props.config.paddingTop.tablet || '3rem'
    : '3rem',
  '--section-pt-desktop': paddingTop.value,
  '--section-pb-mobile': typeof props.config.paddingBottom === 'object'
    ? props.config.paddingBottom.mobile || '2rem'
    : '2rem',
  '--section-pb-tablet': typeof props.config.paddingBottom === 'object'
    ? props.config.paddingBottom.tablet || '3rem'
    : '3rem',
  '--section-pb-desktop': paddingBottom.value
}))

function handleChildSelect(widget: WidgetBase) {
  emit('select-child', widget.id)
}

function handleAddWidget(index: number) {
  emit('add-widget', index)
}
</script>

<template>
  <section :class="containerClass" :style="{ ...sectionStyle, ...responsiveVars }">
    <!-- Background overlay -->
    <div v-if="overlayStyle" class="widget-section__overlay" :style="overlayStyle" />

    <!-- Content -->
    <div class="widget-section__content">
      <!-- Render children -->
      <template v-for="(child, index) in config.children" :key="child.id">
        <div 
          class="widget-section__item"
          @click.stop="handleChildSelect(child)"
        >
          <slot name="widget" :widget="child" :index="index">
            <div class="widget-section__placeholder">
              Widget: {{ child.type }}
            </div>
          </slot>
        </div>
      </template>

      <!-- Empty state for editing -->
      <div
        v-if="isEditing && (!config.children || config.children.length === 0)"
        class="widget-section__empty"
        @click="handleAddWidget(0)"
      >
        <svg class="widget-section__empty-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
          <path d="M12 5v14m-7-7h14" />
        </svg>
        <span class="widget-section__empty-text">Widget zur Section hinzufügen</span>
      </div>
    </div>
  </section>
</template>

<style scoped>
.widget-section {
  position: relative;
  width: 100%;
}

.widget-section--full-width {
  width: 100vw;
  margin-left: calc(-50vw + 50%);
}

.widget-section__overlay {
  position: absolute;
  inset: 0;
  pointer-events: none;
}

.widget-section__content {
  position: relative;
  z-index: 1;
}

.widget-section__item {
  margin-bottom: 1.5rem;
}

.widget-section__item:last-child {
  margin-bottom: 0;
}

/* Empty state */
.widget-section__empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 200px;
  background-color: rgba(249, 250, 251, 0.8);
  border: 2px dashed #d1d5db;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
}

.widget-section__empty:hover {
  border-color: var(--color-primary, #3b82f6);
  background-color: rgba(240, 249, 255, 0.9);
}

.widget-section__empty-icon {
  width: 40px;
  height: 40px;
  color: #9ca3af;
  margin-bottom: 8px;
}

.widget-section__empty-text {
  color: #6b7280;
  font-size: 0.875rem;
}

/* Placeholder */
.widget-section__placeholder {
  padding: 1rem;
  background-color: #f3f4f6;
  border: 1px solid #e5e7eb;
  border-radius: 4px;
  text-align: center;
  color: #6b7280;
  font-size: 0.875rem;
}

/* Editing mode */
.widget-section--editing {
  outline: 2px dashed transparent;
  outline-offset: 4px;
  transition: outline-color 0.2s;
}

.widget-section--editing:hover {
  outline-color: var(--color-primary, #3b82f6);
}

.widget-section--editing .widget-section__item {
  position: relative;
  cursor: pointer;
}

.widget-section--editing .widget-section__item:hover::after {
  content: '';
  position: absolute;
  inset: -4px;
  border: 2px solid var(--color-primary, #3b82f6);
  border-radius: 4px;
  pointer-events: none;
}

/* Responsive padding */
@media (max-width: 640px) {
  .widget-section {
    padding-top: var(--section-pt-mobile, 2rem) !important;
    padding-bottom: var(--section-pb-mobile, 2rem) !important;
  }
}

@media (min-width: 641px) and (max-width: 1024px) {
  .widget-section {
    padding-top: var(--section-pt-tablet, 3rem) !important;
    padding-bottom: var(--section-pb-tablet, 3rem) !important;
  }
}

@media (min-width: 1025px) {
  .widget-section {
    padding-top: var(--section-pt-desktop, 4rem) !important;
    padding-bottom: var(--section-pb-desktop, 4rem) !important;
  }
}
</style>
