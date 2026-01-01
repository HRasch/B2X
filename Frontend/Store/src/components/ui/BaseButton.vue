<template>
  <component
    :is="tag"
    :class="buttonClasses"
    :disabled="disabled || loading"
    :aria-label="ariaLabel"
    :aria-describedby="errorId"
    @click="handleClick"
  >
    <!-- Loading spinner -->
    <span
      v-if="loading"
      class="loading loading-spinner loading-sm"
      aria-hidden="true"
    ></span>

    <!-- Icon (left) -->
    <span
      v-if="icon && iconPosition === 'left'"
      class="shrink-0"
      aria-hidden="true"
    >
      <slot name="icon">
        <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
          <path :d="icon" />
        </svg>
      </slot>
    </span>

    <!-- Button content -->
    <span v-if="loading" class="sr-only">{{ displayLoadingText }}</span>
    <span v-else>
      <slot>{{ text }}</slot>
    </span>

    <!-- Icon (right) -->
    <span
      v-if="icon && iconPosition === 'right'"
      class="shrink-0"
      aria-hidden="true"
    >
      <slot name="icon-right">
        <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
          <path :d="icon" />
        </svg>
      </slot>
    </span>
  </component>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
export type ButtonVariant =
  | "primary"
  | "secondary"
  | "outline"
  | "ghost"
  | "success"
  | "warning"
  | "error";
export type ButtonSize = "xs" | "sm" | "md" | "lg" | "xl";
export type IconPosition = "left" | "right";

interface Props {
  variant?: ButtonVariant;
  size?: ButtonSize;
  disabled?: boolean;
  loading?: boolean;
  loadingText?: string;
  icon?: string;
  iconPosition?: IconPosition;
  text?: string;
  ariaLabel?: string;
  errorId?: string;
  tag?: "button" | "a" | "router-link";
  href?: string;
  to?: string;
}

const { t } = useI18n();

const props = withDefaults(defineProps<Props>(), {
  variant: "primary",
  size: "md",
  disabled: false,
  loading: false,
  loadingText: "", // Will use i18n
  iconPosition: "left",
  tag: "button",
});

const emit = defineEmits<{
  click: [event: Event];
}>();

const handleClick = (event: Event) => {
  if (props.disabled || props.loading) {
    event.preventDefault();
    return;
  }
  emit("click", event);
};

const displayLoadingText = computed(() => {
  return props.loadingText || t("ui.loading");
});

const buttonClasses = computed(() => {
  const classes = [
    "btn",
    // Size classes
    props.size !== "md" ? `btn-${props.size}` : "",
    // Variant classes
    getVariantClass(props.variant),
    // State classes
    {
      "btn-disabled": props.disabled,
      "cursor-not-allowed": props.disabled || props.loading,
      "pointer-events-none": props.disabled || props.loading,
    },
  ];

  return classes.filter(Boolean).join(" ");
});

const getVariantClass = (variant: ButtonVariant): string => {
  const variantMap = {
    primary: "btn-primary",
    secondary: "btn-secondary",
    outline: "btn-outline",
    ghost: "btn-ghost",
    success: "btn-success",
    warning: "btn-warning",
    error: "btn-error",
  };
  return variantMap[variant] || "btn-primary";
};
</script>

<style scoped>
/* Custom focus styles for better accessibility - WCAG AA compliant */
.btn:focus-visible {
  @apply outline-none ring-2 ring-offset-2;
}

.btn-primary:focus-visible {
  @apply ring-blue-500;
}

.btn-secondary:focus-visible {
  @apply ring-gray-500;
}

.btn-outline:focus-visible {
  @apply ring-blue-500;
}

.btn-ghost:focus-visible {
  @apply ring-blue-500;
}

.btn-success:focus-visible {
  @apply ring-green-500;
}

.btn-warning:focus-visible {
  @apply ring-yellow-500;
}

.btn-error:focus-visible {
  @apply ring-red-500;
}

/* Screen reader only text */
.sr-only {
  @apply absolute w-px h-px p-0 -m-px overflow-hidden whitespace-nowrap border-0;
  clip: rect(0, 0, 0, 0);
}
</style>
