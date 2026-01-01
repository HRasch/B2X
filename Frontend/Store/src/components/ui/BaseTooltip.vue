<template>
  <div class="relative inline-block">
    <!-- Trigger element -->
    <div
      :id="triggerId"
      :aria-describedby="tooltipId"
      :tabindex="interactive ? 0 : undefined"
      class="tooltip-trigger"
      @mouseenter="showTooltip"
      @mouseleave="hideTooltip"
      @focus="showTooltip"
      @blur="hideTooltip"
      @keydown="handleKeydown"
    >
      <slot />
    </div>

    <!-- Tooltip content -->
    <div
      v-show="isVisible"
      :id="tooltipId"
      :class="tooltipClasses"
      role="tooltip"
      :aria-hidden="!isVisible"
    >
      {{ content }}
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
export type TooltipPosition = "top" | "bottom" | "left" | "right";

interface Props {
  content: string;
  position?: TooltipPosition;
  variant?:
    | "primary"
    | "secondary"
    | "accent"
    | "info"
    | "success"
    | "warning"
    | "error";
  alwaysVisible?: boolean;
  interactive?: boolean;
  delay?: number;
}

const props = withDefaults(defineProps<Props>(), {
  position: "top",
  variant: "primary",
  alwaysVisible: false,
  interactive: false,
  delay: 300,
});

// Generate unique IDs for ARIA relationships
const triggerId = `tooltip-trigger-${Math.random().toString(36).substr(2, 9)}`;
const tooltipId = `tooltip-${Math.random().toString(36).substr(2, 9)}`;

const isVisible = ref(props.alwaysVisible);

let showTimeout: number | null = null;
let hideTimeout: number | null = null;

const showTooltip = () => {
  if (hideTimeout) {
    clearTimeout(hideTimeout);
    hideTimeout = null;
  }
  if (!isVisible.value) {
    showTimeout = window.setTimeout(() => {
      isVisible.value = true;
    }, props.delay);
  }
};

const hideTooltip = () => {
  if (showTimeout) {
    clearTimeout(showTimeout);
    showTimeout = null;
  }
  if (isVisible.value && !props.alwaysVisible) {
    hideTimeout = window.setTimeout(() => {
      isVisible.value = false;
    }, 150);
  }
};

const handleKeydown = (event: KeyboardEvent) => {
  if (event.key === "Escape" && isVisible.value) {
    isVisible.value = false;
  }
};

// Cleanup timeouts on unmount
import { onUnmounted } from "vue";
onUnmounted(() => {
  if (showTimeout) clearTimeout(showTimeout);
  if (hideTimeout) clearTimeout(hideTimeout);
});

const tooltipClasses = computed(() => [
  "tooltip-content",
  `tooltip-${props.position}`,
  `tooltip-${props.variant}`,
  {
    "tooltip-visible": isVisible.value,
  },
]);
</script>

<style scoped>
.tooltip-trigger {
  @apply inline-block;
}

.tooltip-content {
  @apply absolute z-50 px-2 py-1 text-sm rounded shadow-lg pointer-events-none;
  @apply transform -translate-x-1/2;
  @apply transition-opacity duration-200 ease-in-out;
  @apply opacity-0;
  min-width: 120px;
  max-width: 300px;
  word-wrap: break-word;
}

/* Position variants */
.tooltip-top {
  @apply bottom-full left-1/2 mb-2;
  transform: translateX(-50%) translateY(4px);
}

.tooltip-bottom {
  @apply top-full left-1/2 mt-2;
  transform: translateX(-50%) translateY(-4px);
}

.tooltip-left {
  @apply right-full top-1/2 mr-2;
  transform: translateY(-50%) translateX(4px);
}

.tooltip-right {
  @apply left-full top-1/2 ml-2;
  transform: translateY(-50%) translateX(-4px);
}

/* Variant colors */
.tooltip-primary {
  @apply bg-gray-900 text-white;
}

.tooltip-secondary {
  @apply bg-gray-700 text-white;
}

.tooltip-accent {
  @apply bg-blue-500 text-white;
}

.tooltip-info {
  @apply bg-blue-500 text-white;
}

.tooltip-success {
  @apply bg-green-500 text-white;
}

.tooltip-warning {
  @apply bg-yellow-500 text-black;
}

.tooltip-error {
  @apply bg-red-500 text-white;
}

/* Visible state */
.tooltip-visible {
  @apply opacity-100;
}

/* Arrow styles */
.tooltip-content::after {
  content: "";
  @apply absolute w-0 h-0;
  border-style: solid;
}

.tooltip-top::after {
  @apply bottom-0 left-1/2 transform -translate-x-1/2 translate-y-full;
  border-width: 4px 4px 0 4px;
  border-color: currentColor transparent transparent transparent;
}

.tooltip-bottom::after {
  @apply top-0 left-1/2 transform -translate-x-1/2 -translate-y-full;
  border-width: 0 4px 4px 4px;
  border-color: transparent transparent currentColor transparent;
}

.tooltip-left::after {
  @apply right-0 top-1/2 transform translate-x-full -translate-y-1/2;
  border-width: 4px 0 4px 4px;
  border-color: transparent transparent transparent currentColor;
}

.tooltip-right::after {
  @apply left-0 top-1/2 transform -translate-x-full -translate-y-1/2;
  border-width: 4px 4px 4px 0;
  border-color: transparent currentColor transparent transparent;
}

/* Focus styles for accessibility */
.tooltip-trigger:focus-visible {
  @apply outline-none ring-2 ring-blue-500 ring-offset-2 rounded;
}
</style>
