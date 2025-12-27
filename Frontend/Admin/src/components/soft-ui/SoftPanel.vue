<template>
  <div
    class="rounded-soft-lg bg-white dark:bg-soft-800 shadow-soft-md p-safe border border-soft-100 dark:border-soft-700 overflow-hidden"
  >
    <!-- Header -->
    <div
      v-if="$slots.header"
      class="border-b border-soft-100 dark:border-soft-700 pb-safe"
    >
      <slot name="header" />
    </div>

    <!-- Title + Description -->
    <div v-if="title || description" class="mb-safe">
      <h3
        v-if="title"
        class="text-lg font-semibold text-soft-900 dark:text-soft-100 mb-1"
      >
        {{ title }}
      </h3>
      <p v-if="description" class="text-soft-600 dark:text-soft-400 text-sm">
        {{ description }}
      </p>
    </div>

    <!-- Content -->
    <div class="space-y-4">
      <slot />
    </div>

    <!-- Footer -->
    <div
      v-if="$slots.footer"
      class="border-t border-soft-100 dark:border-soft-700 mt-safe pt-safe"
    >
      <slot name="footer" />
    </div>
  </div>
</template>

<script setup lang="ts">
interface Props {
  title?: string;
  description?: string;
}

withDefaults(defineProps<Props>(), {
  title: undefined,
  description: undefined,
});
</script>
