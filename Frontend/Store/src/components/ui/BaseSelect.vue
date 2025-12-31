<template>
  <div class="form-control" :class="wrapperClasses">
    <!-- Label -->
    <label
      v-if="label"
      :for="selectId"
      class="label"
    >
      <span class="label-text" :class="labelClasses">
        {{ label }}
        <span v-if="required" class="text-error ml-1" aria-label="required">*</span>
      </span>
    </label>

    <!-- Select wrapper -->
    <div class="relative">
      <!-- Prefix -->
      <div
        v-if="$slots.prefix"
        class="absolute left-3 top-1/2 -translate-y-1/2 text-base-content/60 z-10"
      >
        <slot name="prefix" />
      </div>

      <!-- Select -->
      <select
        :id="selectId"
        :ref="selectRef"
        :name="name"
        :value="value"
        :disabled="disabled"
        :required="required"
        :aria-describedby="errorId"
        :aria-invalid="hasError"
        :class="selectClasses"
        @change="handleChange"
        @blur="handleBlur"
        @focus="handleFocus"
      >
        <option v-if="placeholder" value="" disabled>
          {{ placeholder }}
        </option>
        <option
          v-for="option in options"
          :key="option.value"
          :value="option.value"
          :disabled="option.disabled"
        >
          {{ option.label }}
        </option>
      </select>

      <!-- Custom dropdown arrow -->
      <div class="absolute right-3 top-1/2 -translate-y-1/2 pointer-events-none">
        <svg
          class="w-4 h-4 text-base-content/60"
          fill="currentColor"
          viewBox="0 0 20 20"
        >
          <path fill-rule="evenodd" d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 010-1.414L5.293 7.293a1 1 0 01-1.414-1.414z" clip-rule="evenodd" />
        </svg>
      </div>
    </div>

    <!-- Hint text -->
    <div v-if="hint && !hasError" class="label">
      <span class="label-text-alt text-base-content/60">{{ hint }}</span>
    </div>

    <!-- Error message -->
    <div v-if="hasError" class="label">
      <span :id="errorId" class="label-text-alt text-error" role="alert">
        {{ error }}
      </span>
    </div>
  </div>
</template>

<script setup lang="ts">
export interface SelectOption {
  value: string | number;
  label: string;
  disabled?: boolean;
}

interface Props {
  name?: string;
  value?: string | number;
  options: SelectOption[];
  label?: string;
  placeholder?: string;
  hint?: string;
  error?: string;
  disabled?: boolean;
  required?: boolean;
  size?: "sm" | "md" | "lg";
}

const props = withDefaults(defineProps<Props>(), {
  size: "md",
  options: () => [],
});

const emit = defineEmits<{
  change: [value: string | number];
  blur: [event: FocusEvent];
  focus: [event: FocusEvent];
}>();

const selectRef = ref<HTMLSelectElement>();
const selectId = computed(
  () => `select-${Math.random().toString(36).substr(2, 9)}`
);
const errorId = computed(() => `${selectId.value}-error`);

const hasError = computed(() => Boolean(props.error));

const wrapperClasses = computed(() => ({
  "w-full": true,
}));

const labelClasses = computed(() => ({
  "text-error": hasError.value,
}));

const selectClasses = computed(() => [
  "select",
  "select-bordered",
  `select-${props.size === "sm" ? "sm" : props.size === "lg" ? "lg" : ""}`,
  {
    "select-error": hasError.value,
    "select-disabled": props.disabled,
    "pl-10": Boolean($slots.prefix),
  },
]);

const handleChange = (event: Event) => {
  const target = event.target as HTMLSelectElement;
  const value = target.value;
  emit("change", value);
};

const handleBlur = (event: FocusEvent) => {
  emit("blur", event);
};

const handleFocus = (event: FocusEvent) => {
  emit("focus", event);
};
</script>

<style scoped>
/* Custom focus styles for better accessibility */
.select:focus-visible {
  @apply outline-none ring-2 ring-primary ring-offset-2;
}
</style>