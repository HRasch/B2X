<template>
  <div class="tooltip" :class="tooltipClasses" :data-tip="content">
    <slot />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
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
}

const props = withDefaults(defineProps<Props>(), {
  position: "top",
  variant: "primary",
  alwaysVisible: false,
});

const tooltipClasses = computed(() => [
  `tooltip-${props.position}`,
  props.variant !== "primary" ? `tooltip-${props.variant}` : "",
  {
    "tooltip-open": props.alwaysVisible,
  },
]);
</script>

<style scoped>
/* Custom tooltip styles for better accessibility */
.tooltip {
  position: relative;
}

.tooltip::before,
.tooltip::after {
  @apply opacity-0 transition-opacity duration-200;
}

.tooltip:hover::before,
.tooltip:hover::after,
.tooltip:focus::before,
.tooltip:focus::after,
.tooltip.tooltip-open::before,
.tooltip.tooltip-open::after {
  @apply opacity-100;
}

/* Focus styles */
.tooltip:focus-visible {
  @apply outline-none ring-2 ring-primary ring-offset-2 rounded;
}
</style>
