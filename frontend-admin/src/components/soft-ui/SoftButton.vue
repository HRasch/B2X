<template>
  <button
    :class="[
      'inline-flex items-center justify-center',
      'px-5 py-2.5 rounded-soft',
      'font-medium text-sm',
      'transition-all duration-200',
      'focus:outline-none focus:ring-2 focus:ring-offset-2',
      'disabled:opacity-50 disabled:cursor-not-allowed',
      buttonClasses,
    ]"
    :disabled="disabled || loading"
  >
    <svg
      v-if="loading"
      class="animate-spin -ml-1 mr-2 h-4 w-4"
      xmlns="http://www.w3.org/2000/svg"
      fill="none"
      viewBox="0 0 24 24"
    >
      <circle
        class="opacity-25"
        cx="12"
        cy="12"
        r="10"
        stroke="currentColor"
        stroke-width="4"
      />
      <path
        class="opacity-75"
        fill="currentColor"
        d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
      />
    </svg>

    <slot />
  </button>
</template>

<script setup lang="ts">
import { computed } from "vue";

interface Props {
  variant?: "primary" | "secondary" | "danger" | "success" | "ghost";
  size?: "sm" | "md" | "lg";
  disabled?: boolean;
  loading?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  variant: "primary",
  size: "md",
  disabled: false,
  loading: false,
});

const buttonClasses = computed(() => {
  const base = "transform hover:scale-105 active:scale-95";

  const variants = {
    primary:
      "bg-gradient-soft-blue text-white shadow-soft-md hover:shadow-soft-lg focus:ring-primary-600",
    secondary:
      "bg-soft-100 text-soft-700 hover:bg-soft-200 shadow-soft-sm focus:ring-soft-400",
    danger:
      "bg-danger-500 text-white shadow-soft-md hover:shadow-soft-lg focus:ring-danger-600",
    success:
      "bg-success-500 text-white shadow-soft-md hover:shadow-soft-lg focus:ring-success-600",
    ghost: "bg-transparent text-soft-700 hover:bg-soft-100 focus:ring-soft-400",
  };

  const sizes = {
    sm: "px-3 py-1.5 text-xs",
    md: "px-5 py-2.5 text-sm",
    lg: "px-6 py-3 text-base",
  };

  return `${base} ${variants[props.variant]} ${sizes[props.size]}`;
});
</script>
