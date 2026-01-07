<script setup lang="ts">
/**
 * TextWidget - Rich-Text Content Widget
 * Phase 1 MVP
 */
import { computed } from 'vue';
import { useSafeHtml } from '@/utils/sanitize';
import type { TextWidgetConfig, ResponsiveValue, Alignment } from '@/types/widgets';

interface Props {
  config: TextWidgetConfig;
  isEditing?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  isEditing: false,
});

const emit = defineEmits<{
  (e: 'update:config', config: Partial<TextWidgetConfig>): void;
}>();

// Resolve responsive value for current breakpoint (simplified - use CSS for real responsive)
function resolveResponsive<T>(value: ResponsiveValue<T> | T | undefined, fallback: T): T {
  if (!value) return fallback;
  if (typeof value === 'object' && 'desktop' in value) {
    return (value as ResponsiveValue<T>).desktop ?? fallback;
  }
  return value as T;
}

const alignment = computed(() => resolveResponsive(props.config.alignment, 'left' as Alignment));
const fontSize = computed(() => resolveResponsive(props.config.fontSize, '1rem'));

const containerStyle = computed(() => ({
  textAlign: alignment.value,
  fontSize: fontSize.value,
  fontWeight: props.config.fontWeight || 'normal',
  lineHeight: props.config.lineHeight || '1.6',
  color: props.config.textColor || 'inherit',
  maxWidth: props.config.maxWidth || 'none',
}));

const safeContent = useSafeHtml(props.config.content || '');

const containerClass = computed(() => [
  'widget-text',
  `text-${alignment.value}`,
  {
    'widget-text--editing': props.isEditing,
  },
]);

function handleContentUpdate(event: Event) {
  const target = event.target as HTMLElement;
  emit('update:config', { content: target.innerHTML });
}
</script>

<template>
  <div :class="containerClass" :style="containerStyle">
    <!-- Editing mode: contenteditable -->
    <div
      v-if="isEditing"
      class="widget-text__content widget-text__content--editable"
      contenteditable="true"
      v-html="safeContent"
      @blur="handleContentUpdate"
      @keydown.enter.prevent
    />

    <!-- Display mode: static HTML -->
    <div v-else class="widget-text__content" v-html="safeContent" />
  </div>
</template>

<style scoped>
.widget-text {
  width: 100%;
}

.widget-text__content {
  word-wrap: break-word;
  overflow-wrap: break-word;
}

.widget-text__content--editable {
  outline: none;
  min-height: 1.5em;
  cursor: text;
}

.widget-text__content--editable:focus {
  outline: 2px solid var(--color-primary, #3b82f6);
  outline-offset: 2px;
  border-radius: 4px;
}

/* Prose styles for rendered HTML */
.widget-text__content :deep(p) {
  margin-bottom: 1em;
}

.widget-text__content :deep(p:last-child) {
  margin-bottom: 0;
}

.widget-text__content :deep(h1),
.widget-text__content :deep(h2),
.widget-text__content :deep(h3),
.widget-text__content :deep(h4),
.widget-text__content :deep(h5),
.widget-text__content :deep(h6) {
  font-weight: 600;
  line-height: 1.3;
  margin-bottom: 0.5em;
}

.widget-text__content :deep(ul),
.widget-text__content :deep(ol) {
  padding-left: 1.5em;
  margin-bottom: 1em;
}

.widget-text__content :deep(a) {
  color: var(--color-primary, #3b82f6);
  text-decoration: underline;
}

.widget-text__content :deep(strong) {
  font-weight: 600;
}

.widget-text__content :deep(em) {
  font-style: italic;
}

/* Responsive alignment */
@media (max-width: 768px) {
  .widget-text.text-left {
    text-align: left;
  }
  .widget-text.text-center {
    text-align: center;
  }
  .widget-text.text-right {
    text-align: right;
  }
}
</style>
