<template>
  <div
    class="rounded-soft-lg bg-white shadow-soft-md p-safe border border-soft-100 overflow-hidden"
  >
    <!-- Header -->
    <div v-if="$slots.header" class="border-b border-soft-100 pb-safe">
      <slot name="header" />
    </div>

    <!-- Title + Description -->
    <div v-if="title || description" class="mb-safe">
      <h3 v-if="title" class="text-lg font-semibold text-soft-900 mb-1">
        {{ title }}
      </h3>
      <p v-if="description" class="text-soft-600 text-sm">{{ description }}</p>
    </div>

    <!-- Content -->
    <div class="space-y-4">
      <slot />
    </div>

    <!-- Footer -->
    <div v-if="$slots.footer" class="border-t border-soft-100 mt-safe pt-safe">
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
