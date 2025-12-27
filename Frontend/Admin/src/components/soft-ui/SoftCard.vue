<template>
  <div
    :class="[
      'rounded-soft-lg px-safe py-safe shadow-soft-md transition-all duration-200',
      'bg-white dark:bg-soft-800 hover:shadow-soft-lg',
      'border border-soft-100 dark:border-soft-700',
      {
        'bg-gradient-to-br from-soft-50 dark:from-soft-700':
          variant === 'gradient',
      },
      className,
    ]"
  >
    <slot />
  </div>
</template>

<script setup lang="ts">
withDefaults(
  defineProps<{
    variant?: "default" | "gradient";
    className?: string;
  }>(),
  {
    variant: "default",
    className: "",
  }
);
</script>
