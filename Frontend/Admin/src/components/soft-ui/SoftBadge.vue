<template>
  <div
    :class="['rounded-soft px-4 py-3 text-sm font-medium flex items-center gap-3', badgeClasses]"
  >
    <div
      :class="[
        'w-2.5 h-2.5 rounded-full flex-shrink-0',
        {
          'bg-success-500': variant === 'success',
          'bg-warning-500': variant === 'warning',
          'bg-danger-500': variant === 'danger',
          'bg-info-500': variant === 'info',
          'bg-primary-500': variant === 'default',
        },
      ]"
    />
    <slot />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

interface Props {
  variant?: 'success' | 'warning' | 'danger' | 'info' | 'default';
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'default',
});

const badgeClasses = computed(() => {
  const variants = {
    success: 'bg-success-50 text-success-700 border border-success-100',
    warning: 'bg-warning-50 text-warning-700 border border-warning-100',
    danger: 'bg-danger-50 text-danger-700 border border-danger-100',
    info: 'bg-info-50 text-info-700 border border-info-100',
    default: 'bg-primary-50 text-primary-700 border border-primary-100',
  };

  return variants[props.variant];
});
</script>
