<template>
  <div class="relative">
    <label
      v-if="label"
      :for="id"
      class="block text-sm font-medium text-soft-700 dark:text-soft-300 mb-2"
    >
      {{ label }}
      <span v-if="required" class="text-danger-500">*</span>
    </label>

    <input
      :id="id"
      :type="type"
      :value="modelValue"
      :placeholder="placeholder"
      :required="required"
      :disabled="disabled"
      @input="
        $emit('update:modelValue', ($event.target as HTMLInputElement).value)
      "
      :class="[
        'w-full px-4 py-3 rounded-soft',
        'border transition-all duration-200',
        'bg-white dark:bg-soft-700 dark:text-soft-100',
        'focus:outline-none focus:ring-2 focus:ring-offset-0',
        'disabled:bg-soft-50 dark:disabled:bg-soft-600 disabled:cursor-not-allowed',
        {
          'border-soft-200 dark:border-soft-600 focus:border-primary-400 focus:ring-primary-200':
            !error,
          'border-danger-300 dark:border-danger-600 focus:border-danger-400 focus:ring-danger-200':
            error,
        },
      ]"
    />

    <p v-if="error" class="mt-2 text-sm text-danger-600 dark:text-danger-400">
      {{ error }}
    </p>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";

interface Props {
  modelValue: string | number;
  type?: string;
  label?: string;
  placeholder?: string;
  error?: string;
  required?: boolean;
  disabled?: boolean;
}

withDefaults(defineProps<Props>(), {
  type: "text",
  label: undefined,
  placeholder: "",
  error: undefined,
  required: false,
  disabled: false,
});

defineEmits<{
  "update:modelValue": [value: string | number];
}>();

const id = computed(() => `input-${Math.random().toString(36).substr(2, 9)}`);
</script>
