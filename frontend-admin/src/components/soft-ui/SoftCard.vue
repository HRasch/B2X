<template>
  <div
    :class="[
      'rounded-soft-lg px-safe py-safe shadow-soft-md transition-all duration-200',
      'bg-white hover:shadow-soft-lg',
      'border border-soft-100',
      {
        'bg-gradient-to-br from-soft-50': variant === 'gradient',
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
