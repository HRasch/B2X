<script setup lang="ts">
/**
 * ButtonWidget - CTA Button Widget
 * Phase 1 MVP
 */
import { computed } from 'vue';
import type { ButtonWidgetConfig, ResponsiveValue } from '@/types/widgets';

interface Props {
  config: ButtonWidgetConfig;
  isEditing?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  isEditing: false,
});

// Resolve responsive value
function resolveResponsive<T>(value: ResponsiveValue<T> | T | undefined, fallback: T): T {
  if (!value) return fallback;
  if (typeof value === 'object' && 'desktop' in value) {
    return (value as ResponsiveValue<T>).desktop ?? fallback;
  }
  return value as T;
}

const isFullWidth = computed(() => resolveResponsive(props.config.fullWidth, false));

const containerClass = computed(() => [
  'widget-button',
  `widget-button--align-${props.config.alignment || 'left'}`,
  {
    'widget-button--editing': props.isEditing,
  },
]);

const buttonClass = computed(() => [
  'widget-button__btn',
  `widget-button__btn--${props.config.variant || 'primary'}`,
  `widget-button__btn--${props.config.size || 'md'}`,
  {
    'widget-button__btn--full-width': isFullWidth.value,
    'widget-button__btn--has-icon': !!props.config.icon,
  },
]);

function handleClick(event: MouseEvent) {
  if (props.isEditing) {
    event.preventDefault();
    // In editing mode, select the button for configuration
  }
}
</script>

<template>
  <div :class="containerClass">
    <component
      :is="isEditing ? 'button' : 'a'"
      :href="isEditing ? undefined : config.link"
      :target="isEditing ? undefined : config.linkTarget"
      :rel="config.linkTarget === '_blank' ? 'noopener noreferrer' : undefined"
      :class="buttonClass"
      @click="handleClick"
    >
      <!-- Icon left -->
      <span
        v-if="config.icon && config.iconPosition === 'left'"
        class="widget-button__icon widget-button__icon--left"
      >
        <!-- Icon placeholder - integrate with icon library -->
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M5 12h14M12 5l7 7-7 7" />
        </svg>
      </span>

      <!-- Button text -->
      <span class="widget-button__text">{{ config.text }}</span>

      <!-- Icon right -->
      <span
        v-if="config.icon && config.iconPosition === 'right'"
        class="widget-button__icon widget-button__icon--right"
      >
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M5 12h14M12 5l7 7-7 7" />
        </svg>
      </span>
    </component>
  </div>
</template>

<style scoped>
.widget-button {
  width: 100%;
}

/* Alignment */
.widget-button--align-left {
  text-align: left;
}

.widget-button--align-center {
  text-align: center;
}

.widget-button--align-right {
  text-align: right;
}

/* Base button styles */
.widget-button__btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  font-weight: 500;
  text-decoration: none;
  border: none;
  cursor: pointer;
  transition: all 0.2s ease;
  white-space: nowrap;
}

/* Size variants */
.widget-button__btn--sm {
  padding: 0.5rem 1rem;
  font-size: 0.875rem;
  border-radius: 0.375rem;
}

.widget-button__btn--md {
  padding: 0.625rem 1.25rem;
  font-size: 1rem;
  border-radius: 0.5rem;
}

.widget-button__btn--lg {
  padding: 0.75rem 1.5rem;
  font-size: 1.125rem;
  border-radius: 0.5rem;
}

/* Full width */
.widget-button__btn--full-width {
  width: 100%;
}

/* Variant: Primary */
.widget-button__btn--primary {
  background-color: var(--color-primary, #3b82f6);
  color: white;
}

.widget-button__btn--primary:hover {
  background-color: var(--color-primary-dark, #2563eb);
}

.widget-button__btn--primary:active {
  background-color: var(--color-primary-darker, #1d4ed8);
}

/* Variant: Secondary */
.widget-button__btn--secondary {
  background-color: var(--color-secondary, #6b7280);
  color: white;
}

.widget-button__btn--secondary:hover {
  background-color: var(--color-secondary-dark, #4b5563);
}

/* Variant: Outline */
.widget-button__btn--outline {
  background-color: transparent;
  color: var(--color-primary, #3b82f6);
  border: 2px solid var(--color-primary, #3b82f6);
}

.widget-button__btn--outline:hover {
  background-color: var(--color-primary, #3b82f6);
  color: white;
}

/* Variant: Ghost */
.widget-button__btn--ghost {
  background-color: transparent;
  color: var(--color-primary, #3b82f6);
}

.widget-button__btn--ghost:hover {
  background-color: rgba(59, 130, 246, 0.1);
}

/* Variant: Link */
.widget-button__btn--link {
  background-color: transparent;
  color: var(--color-primary, #3b82f6);
  padding-left: 0;
  padding-right: 0;
  text-decoration: underline;
}

.widget-button__btn--link:hover {
  color: var(--color-primary-dark, #2563eb);
}

/* Icon styles */
.widget-button__icon {
  display: inline-flex;
  align-items: center;
  justify-content: center;
}

.widget-button__icon svg {
  width: 1em;
  height: 1em;
}

/* Focus state */
.widget-button__btn:focus {
  outline: 2px solid var(--color-primary, #3b82f6);
  outline-offset: 2px;
}

/* Disabled state */
.widget-button__btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Editing mode */
.widget-button--editing .widget-button__btn {
  cursor: pointer;
}

.widget-button--editing .widget-button__btn:hover {
  box-shadow: 0 0 0 2px var(--color-primary, #3b82f6);
}
</style>
