<script setup lang="ts">
/**
 * ImageWidget - Image Content Widget
 * Phase 1 MVP
 */
import { computed, ref } from 'vue';
import type { ImageWidgetConfig, ResponsiveValue } from '@/types/widgets';

interface Props {
  config: ImageWidgetConfig;
  isEditing?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  isEditing: false,
});

const isLoaded = ref(false);
const hasError = ref(false);

// Resolve responsive value
function resolveResponsive<T>(value: ResponsiveValue<T> | T | undefined, fallback: T): T {
  if (!value) return fallback;
  if (typeof value === 'object' && 'desktop' in value) {
    return (value as ResponsiveValue<T>).desktop ?? fallback;
  }
  return value as T;
}

const imageStyle = computed(() => ({
  width: resolveResponsive(props.config.width, '100%'),
  height: resolveResponsive(props.config.height, 'auto'),
  objectFit: props.config.objectFit || 'cover',
  objectPosition: props.config.objectPosition || 'center',
}));

const containerClass = computed(() => [
  'widget-image',
  `widget-image--rounded-${props.config.rounded || 'none'}`,
  {
    'widget-image--editing': props.isEditing,
    'widget-image--loading': !isLoaded.value && !hasError.value,
    'widget-image--error': hasError.value,
  },
]);

const roundedClass = computed(() => {
  const rounded = props.config.rounded || 'none';
  return {
    none: 'rounded-none',
    sm: 'rounded-sm',
    md: 'rounded-md',
    lg: 'rounded-lg',
    full: 'rounded-full',
  }[rounded];
});

function handleLoad() {
  isLoaded.value = true;
  hasError.value = false;
}

function handleError() {
  hasError.value = true;
  isLoaded.value = true;
}

function handleImageSelect() {
  // In editing mode, open media picker
  if (props.isEditing) {
    // Emit event to open media picker
    // This will be handled by parent component
  }
}
</script>

<template>
  <div :class="containerClass">
    <!-- Link wrapper if configured -->
    <component
      :is="config.link ? 'a' : 'div'"
      :href="config.link || undefined"
      :target="config.link ? config.linkTarget : undefined"
      :rel="config.linkTarget === '_blank' ? 'noopener noreferrer' : undefined"
      class="widget-image__wrapper"
    >
      <!-- Placeholder when no image -->
      <div
        v-if="!config.src && isEditing"
        class="widget-image__placeholder"
        @click="handleImageSelect"
      >
        <svg
          class="widget-image__placeholder-icon"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          stroke-width="1.5"
        >
          <rect x="3" y="3" width="18" height="18" rx="2" />
          <circle cx="8.5" cy="8.5" r="1.5" />
          <path d="M21 15l-5-5L5 21" />
        </svg>
        <span class="widget-image__placeholder-text">{{
          $t('pageBuilder.image.selectImage')
        }}</span>
      </div>

      <!-- Error state -->
      <div v-else-if="hasError" class="widget-image__error">
        <svg
          class="widget-image__error-icon"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          stroke-width="1.5"
        >
          <circle cx="12" cy="12" r="10" />
          <path d="M12 8v4m0 4h.01" />
        </svg>
        <span class="widget-image__error-text">{{ $t('pageBuilder.image.loadError') }}</span>
      </div>

      <!-- Actual image -->
      <img
        v-else
        :src="config.src"
        :alt="config.alt"
        :loading="config.lazyLoad ? 'lazy' : 'eager'"
        :style="imageStyle"
        :class="['widget-image__img', roundedClass]"
        @load="handleLoad"
        @error="handleError"
      />

      <!-- Loading skeleton -->
      <div
        v-if="config.src && !isLoaded && !hasError"
        class="widget-image__skeleton"
        :class="roundedClass"
      />
    </component>

    <!-- Caption -->
    <figcaption v-if="config.caption" class="widget-image__caption">
      {{ config.caption }}
    </figcaption>
  </div>
</template>

<style scoped>
.widget-image {
  position: relative;
  width: 100%;
}

.widget-image__wrapper {
  display: block;
  position: relative;
}

.widget-image__img {
  display: block;
  max-width: 100%;
  height: auto;
}

/* Loading skeleton */
.widget-image__skeleton {
  position: absolute;
  inset: 0;
  background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
}

@keyframes shimmer {
  0% {
    background-position: 200% 0;
  }
  100% {
    background-position: -200% 0;
  }
}

/* Placeholder for editing */
.widget-image__placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 200px;
  background-color: #f5f5f5;
  border: 2px dashed #d1d5db;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
}

.widget-image__placeholder:hover {
  border-color: var(--color-primary, #3b82f6);
  background-color: #f0f9ff;
}

.widget-image__placeholder-icon {
  width: 48px;
  height: 48px;
  color: #9ca3af;
  margin-bottom: 8px;
}

.widget-image__placeholder-text {
  color: #6b7280;
  font-size: 0.875rem;
}

/* Error state */
.widget-image__error {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 200px;
  background-color: #fef2f2;
  border: 1px solid #fecaca;
  border-radius: 8px;
}

.widget-image__error-icon {
  width: 48px;
  height: 48px;
  color: #ef4444;
  margin-bottom: 8px;
}

.widget-image__error-text {
  color: #dc2626;
  font-size: 0.875rem;
}

/* Caption */
.widget-image__caption {
  margin-top: 0.5rem;
  font-size: 0.875rem;
  color: #6b7280;
  text-align: center;
}

/* Rounded variants */
.rounded-none {
  border-radius: 0;
}
.rounded-sm {
  border-radius: 0.125rem;
}
.rounded-md {
  border-radius: 0.375rem;
}
.rounded-lg {
  border-radius: 0.5rem;
}
.rounded-full {
  border-radius: 9999px;
}

/* Editing mode */
.widget-image--editing {
  cursor: pointer;
}

.widget-image--editing:hover::after {
  content: '';
  position: absolute;
  inset: 0;
  background: rgba(0, 0, 0, 0.1);
  border-radius: inherit;
  pointer-events: none;
}
</style>
