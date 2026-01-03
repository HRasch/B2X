<script setup lang="ts">
/**
 * WidgetRenderer - Dynamically renders widgets based on type
 * Handles selection, hover states, and editing mode
 */
import { computed, inject } from 'vue';
import { storeToRefs } from 'pinia';
import { usePageBuilderStore } from '@/stores/pageBuilder';
import { getWidgetComponent } from './widgets';
import type { WidgetBase } from '@/types/widgets';

interface Props {
  widget: WidgetBase;
  depth?: number;
}

const props = withDefaults(defineProps<Props>(), {
  depth: 0,
});

const store = usePageBuilderStore();
const { selectedWidgetId, hoveredWidgetId, isPreviewMode } = storeToRefs(store);

// Get the component for this widget type
const WidgetComponent = computed(() => getWidgetComponent(props.widget.type));

// Computed states
const isSelected = computed(() => selectedWidgetId.value === props.widget.id);
const isHovered = computed(() => hoveredWidgetId.value === props.widget.id && !isSelected.value);
const isEditing = computed(() => !isPreviewMode.value);

// Container classes
const containerClass = computed(() => [
  'widget-renderer',
  `widget-renderer--${props.widget.type}`,
  `widget-renderer--depth-${props.depth}`,
  {
    'widget-renderer--selected': isSelected.value,
    'widget-renderer--hovered': isHovered.value,
    'widget-renderer--editing': isEditing.value,
    'widget-renderer--preview': isPreviewMode.value,
  },
]);

// Apply widget styles
const containerStyle = computed(() => {
  const style = props.widget.style || {};
  return {
    margin: typeof style.margin === 'object' ? style.margin.desktop : style.margin,
    padding: typeof style.padding === 'object' ? style.padding.desktop : style.padding,
    backgroundColor: style.backgroundColor,
    borderRadius: style.borderRadius,
    boxShadow: style.boxShadow ? getBoxShadow(style.boxShadow) : undefined,
  };
});

function getBoxShadow(size: string): string {
  const shadows: Record<string, string> = {
    none: 'none',
    sm: '0 1px 2px 0 rgb(0 0 0 / 0.05)',
    md: '0 4px 6px -1px rgb(0 0 0 / 0.1)',
    lg: '0 10px 15px -3px rgb(0 0 0 / 0.1)',
    xl: '0 20px 25px -5px rgb(0 0 0 / 0.1)',
  };
  return shadows[size] || 'none';
}

// Event handlers
function handleClick(event: MouseEvent) {
  if (isPreviewMode.value) return;
  event.stopPropagation();
  store.selectWidget(props.widget.id);
}

function handleMouseEnter() {
  if (isPreviewMode.value) return;
  store.hoverWidget(props.widget.id);
}

function handleMouseLeave() {
  if (isPreviewMode.value) return;
  store.hoverWidget(null);
}

function handleConfigUpdate(config: Partial<typeof props.widget.config>) {
  store.updateWidget(props.widget.id, config);
}

// Check if widget has children
const hasChildren = computed(() => {
  return 'children' in props.widget.config && Array.isArray(props.widget.config.children);
});

const children = computed(() => {
  if (hasChildren.value) {
    return props.widget.config.children as WidgetBase[];
  }
  return [];
});
</script>

<template>
  <div
    :class="containerClass"
    :style="containerStyle"
    :data-widget-id="widget.id"
    :data-widget-type="widget.type"
    @click="handleClick"
    @mouseenter="handleMouseEnter"
    @mouseleave="handleMouseLeave"
  >
    <!-- Selection indicator -->
    <div v-if="isSelected && isEditing" class="widget-renderer__selection-ring" />

    <!-- Hover indicator -->
    <div v-if="isHovered && isEditing" class="widget-renderer__hover-ring" />

    <!-- Widget toolbar (when selected) -->
    <div v-if="isSelected && isEditing" class="widget-renderer__toolbar">
      <span class="widget-renderer__type-label">{{ widget.type }}</span>
      <div class="widget-renderer__actions">
        <button
          class="widget-renderer__action-btn"
          title="Duplizieren"
          @click.stop="store.duplicateWidget(widget.id)"
        >
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <rect x="9" y="9" width="13" height="13" rx="2" />
            <path d="M5 15H4a2 2 0 01-2-2V4a2 2 0 012-2h9a2 2 0 012 2v1" />
          </svg>
        </button>
        <button
          class="widget-renderer__action-btn"
          title="Kopieren"
          @click.stop="store.copyWidget(widget.id)"
        >
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <rect x="9" y="9" width="13" height="13" rx="2" />
            <path d="M5 15H4a2 2 0 01-2-2V4a2 2 0 012-2h9a2 2 0 012 2v1" />
          </svg>
        </button>
        <button
          class="widget-renderer__action-btn widget-renderer__action-btn--danger"
          title="Löschen"
          @click.stop="store.removeWidget(widget.id)"
        >
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path
              d="M3 6h18M19 6v14a2 2 0 01-2 2H7a2 2 0 01-2-2V6m3 0V4a2 2 0 012-2h4a2 2 0 012 2v2"
            />
          </svg>
        </button>
      </div>
    </div>

    <!-- Render the actual widget component -->
    <component
      v-if="WidgetComponent"
      :is="WidgetComponent"
      :config="widget.config"
      :is-editing="isEditing"
      @update:config="handleConfigUpdate"
    >
      <!-- Slot for nested widgets -->
      <template v-if="hasChildren" #widget="{ widget: childWidget, index }">
        <WidgetRenderer :widget="childWidget" :depth="depth + 1" />
      </template>
    </component>

    <!-- Fallback for unknown widget types -->
    <div v-else class="widget-renderer__unknown">
      <span>Unknown widget type: {{ widget.type }}</span>
    </div>
  </div>
</template>

<style scoped>
.widget-renderer {
  position: relative;
  min-height: 20px;
}

/* Selection ring */
.widget-renderer__selection-ring {
  position: absolute;
  inset: -2px;
  border: 2px solid var(--color-primary, #3b82f6);
  border-radius: 4px;
  pointer-events: none;
  z-index: 10;
}

/* Hover ring */
.widget-renderer__hover-ring {
  position: absolute;
  inset: -2px;
  border: 2px dashed var(--color-primary, #3b82f6);
  border-radius: 4px;
  pointer-events: none;
  z-index: 9;
  opacity: 0.5;
}

/* Toolbar */
.widget-renderer__toolbar {
  position: absolute;
  top: -32px;
  left: 0;
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 4px 8px;
  background-color: var(--color-primary, #3b82f6);
  color: white;
  border-radius: 4px 4px 0 0;
  font-size: 12px;
  z-index: 20;
  box-shadow: 0 -2px 4px rgba(0, 0, 0, 0.1);
}

.widget-renderer__type-label {
  font-weight: 500;
  text-transform: capitalize;
}

.widget-renderer__actions {
  display: flex;
  gap: 4px;
  margin-left: 8px;
  padding-left: 8px;
  border-left: 1px solid rgba(255, 255, 255, 0.3);
}

.widget-renderer__action-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 20px;
  height: 20px;
  padding: 2px;
  background: transparent;
  border: none;
  border-radius: 3px;
  cursor: pointer;
  color: white;
  transition: background-color 0.2s;
}

.widget-renderer__action-btn:hover {
  background-color: rgba(255, 255, 255, 0.2);
}

.widget-renderer__action-btn--danger:hover {
  background-color: #ef4444;
}

.widget-renderer__action-btn svg {
  width: 14px;
  height: 14px;
}

/* Unknown widget fallback */
.widget-renderer__unknown {
  padding: 1rem;
  background-color: #fef3c7;
  border: 1px solid #f59e0b;
  border-radius: 4px;
  color: #92400e;
  font-size: 0.875rem;
  text-align: center;
}

/* Editing mode cursor */
.widget-renderer--editing {
  cursor: pointer;
}

/* Preview mode - no interactions */
.widget-renderer--preview {
  cursor: default;
}

/* Depth indication (subtle) */
.widget-renderer--depth-1 {
  /* Nested widget styles if needed */
}
</style>
