<template>
  <div class="form-control" :class="wrapperClasses">
    <!-- Label -->
    <label v-if="label" :for="textareaId" class="label">
      <span class="label-text" :class="labelClasses">
        {{ label }}
        <span v-if="required" class="text-error ml-1" aria-label="required"
          >*</span
        >
      </span>
    </label>

    <!-- Textarea wrapper -->
    <div class="relative">
      <!-- Textarea -->
      <textarea
        :id="textareaId"
        :ref="textareaRef"
        :name="name"
        :value="value"
        :placeholder="placeholder"
        :disabled="disabled"
        :required="required"
        :rows="rows"
        :maxlength="maxlength"
        :aria-describedby="errorId"
        :aria-invalid="hasError"
        :class="textareaClasses"
        @input="handleInput"
        @blur="handleBlur"
        @focus="handleFocus"
      ></textarea>

      <!-- Character counter -->
      <div
        v-if="maxlength && showCounter"
        class="absolute bottom-2 right-2 text-xs text-base-content/60"
        :class="{ 'text-error': characterCount > maxlength * 0.9 }"
      >
        {{ characterCount }}/{{ maxlength }}
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
import { computed, ref } from 'vue'
interface Props {
  name?: string;
  value?: string;
  label?: string;
  placeholder?: string;
  hint?: string;
  error?: string;
  disabled?: boolean;
  required?: boolean;
  rows?: number;
  maxlength?: number;
  showCounter?: boolean;
  size?: "sm" | "md" | "lg";
}

const props = withDefaults(defineProps<Props>(), {
  rows: 3,
  size: "md",
  showCounter: false,
});

const emit = defineEmits<{
  input: [value: string];
  blur: [event: FocusEvent];
  focus: [event: FocusEvent];
}>();

const textareaRef = ref();
const textareaId = computed(
  () => `textarea-${Math.random().toString(36).substr(2, 9)}`
);
const errorId = computed(() => `${textareaId.value}-error`);

const hasError = computed(() => Boolean(props.error));
const characterCount = computed(() => (props.value || "").length);

const wrapperClasses = computed(() => ({
  "w-full": true,
}));

const labelClasses = computed(() => ({
  "text-error": hasError.value,
}));

const textareaClasses = computed(() => [
  "textarea",
  "textarea-bordered",
  `textarea-${props.size === "sm" ? "sm" : props.size === "lg" ? "lg" : ""}`,
  {
    "textarea-error": hasError.value,
    "textarea-disabled": props.disabled,
  },
]);

const handleInput = (event: Event) => {
  const target = event.target as HTMLTextAreaElement;
  const value = target.value;
  emit("input", value);
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
.textarea:focus-visible {
  @apply outline-none ring-2 ring-primary ring-offset-2;
}
</style>
