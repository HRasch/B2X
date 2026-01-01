<template>
  <div class="form-control" :class="wrapperClasses">
    <!-- Label -->
    <label
      v-if="label"
      :for="inputId"
      class="label"
      :class="{ 'cursor-pointer': type === 'checkbox' || type === 'radio' }"
    >
      <span class="label-text" :class="labelClasses">
        {{ label }}
        <span
          v-if="required"
          class="text-error ml-1"
          :aria-label="t('ui.required')"
          >*</span
        >
      </span>
    </label>

    <!-- Input wrapper for different types -->
    <div
      v-if="type === 'checkbox' || type === 'radio'"
      class="flex items-center gap-2"
    >
      <input
        :id="inputId"
        :ref="inputRef"
        :type="type"
        :name="name"
        :value="value"
        :checked="checked"
        :disabled="disabled"
        :required="required"
        :aria-describedby="errorId"
        :aria-invalid="hasError"
        :class="inputClasses"
        @input="handleInput"
        @blur="handleBlur"
        @focus="handleFocus"
      />
      <slot name="input-content" />
    </div>

    <!-- Text input wrapper -->
    <div v-else class="relative">
      <!-- Prefix -->
      <div
        v-if="$slots.prefix"
        class="absolute left-3 top-1/2 -translate-y-1/2 text-base-content/60"
      >
        <slot name="prefix" />
      </div>

      <!-- Input -->
      <input
        :id="inputId"
        :ref="inputRef"
        :type="inputType"
        :name="name"
        :value="value"
        :placeholder="placeholder"
        :disabled="disabled"
        :required="required"
        :minlength="minlength"
        :maxlength="maxlength"
        :pattern="pattern"
        :aria-describedby="errorId"
        :aria-invalid="hasError"
        :class="inputClasses"
        @input="handleInput"
        @blur="handleBlur"
        @focus="handleFocus"
      />

      <!-- Suffix -->
      <div
        v-if="$slots.suffix"
        class="absolute right-3 top-1/2 -translate-y-1/2 text-base-content/60"
      >
        <slot name="suffix" />
      </div>
    </div>

    <!-- Character counter -->
    <div v-if="showCharacterCounter" class="flex justify-end mt-1">
      <span class="text-xs" :class="characterCounterClasses">
        {{ characterCount }}/{{ maxlength }}
      </span>
    </div>

    <!-- Hint text -->
    <div v-if="hint && !hasError" class="label">
      <span class="label-text-alt text-base-content/60">{{ hint }}</span>
    </div>

    <!-- Error message -->
    <Transition name="error-message" appear>
      <div v-if="hasError" class="label">
        <span
          :id="errorId"
          :class="['label-text-alt flex items-center gap-2', errorTextColor]"
          role="alert"
        >
          <span class="text-sm">{{ errorIcon }}</span>
          <span>{{ error }}</span>
        </span>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, useSlots } from "vue";
import { useI18n } from "vue-i18n";
export type InputType =
  | "text"
  | "email"
  | "password"
  | "number"
  | "tel"
  | "url"
  | "search"
  | "checkbox"
  | "radio";

interface Props {
  type?: InputType;
  name?: string;
  value?: string | number | boolean;
  checked?: boolean;
  label?: string;
  placeholder?: string;
  hint?: string;
  error?: string;
  errorSeverity?: "error" | "warning" | "info";
  disabled?: boolean;
  required?: boolean;
  minlength?: number;
  maxlength?: number;
  pattern?: string;
  size?: "sm" | "md" | "lg";
}

const props = withDefaults(defineProps<Props>(), {
  type: "text",
  size: "md",
  errorSeverity: "error",
});

const emit = defineEmits<{
  input: [value: string | number | boolean];
  blur: [event: FocusEvent];
  focus: [event: FocusEvent];
}>();

const { t } = useI18n();

const slots = useSlots();
const inputRef = ref();
const inputId = computed(
  () => `input-${Math.random().toString(36).substr(2, 9)}`
);
const errorId = computed(() => `${inputId.value}-error`);

const hasError = computed(() => Boolean(props.error));
const errorSeverity = computed(() => props.errorSeverity);

const errorIcon = computed(() => {
  switch (errorSeverity.value) {
    case "warning":
      return "⚠️";
    case "info":
      return "ℹ️";
    default:
      return "❌";
  }
});

const errorTextColor = computed(() => {
  switch (errorSeverity.value) {
    case "warning":
      return "text-warning";
    case "info":
      return "text-info";
    default:
      return "text-error";
  }
});
const inputType = computed(() => {
  if (props.type === "checkbox" || props.type === "radio") return props.type;
  return props.type;
});

const showCharacterCounter = computed(() => {
  return (
    props.maxlength &&
    props.maxlength > 0 &&
    props.type !== "checkbox" &&
    props.type !== "radio"
  );
});

const characterCount = computed(() => {
  if (typeof props.value === "string") {
    return props.value.length;
  }
  return 0;
});

const characterCounterClasses = computed(() => {
  const count = characterCount.value;
  const max = props.maxlength || 0;
  const percentage = (count / max) * 100;

  if (percentage >= 100) {
    return "text-error font-medium";
  } else if (percentage >= 90) {
    return "text-warning font-medium";
  } else {
    return "text-base-content/60";
  }
});

const wrapperClasses = computed(() => ({
  "w-full": true,
}));

const labelClasses = computed(() => ({
  "text-error": hasError.value,
}));

const inputClasses = computed(() => {
  const baseClasses = [
    "input",
    "input-bordered",
    `input-${props.size === "sm" ? "sm" : props.size === "lg" ? "lg" : ""}`,
    {
      "input-error": hasError.value,
      "input-disabled": props.disabled,
      "pl-10": Boolean(slots.prefix),
      "pr-10": Boolean(slots.suffix),
    },
  ];

  if (props.type === "checkbox") {
    return [
      "checkbox",
      "checkbox-primary",
      { "checkbox-error": hasError.value },
    ];
  }

  if (props.type === "radio") {
    return ["radio", "radio-primary", { "radio-error": hasError.value }];
  }

  return baseClasses.filter(Boolean);
});

const handleInput = (event: Event) => {
  const target = event.target as HTMLInputElement;
  let value: string | number | boolean = target.value;

  if (props.type === "checkbox") {
    value = target.checked;
  } else if (props.type === "number") {
    value = target.valueAsNumber || 0;
  }

  emit("input", value);
};

const handleBlur = (event: FocusEvent) => {
  emit("blur", event);
};

const handleFocus = (event: FocusEvent) => {
  emit("focus", event);
};

// Expose methods for parent components
defineExpose({
  focus: () => inputRef.value?.focus(),
  blur: () => inputRef.value?.blur(),
});
</script>

<style scoped>
/* Custom focus styles for better accessibility */
.input:focus-visible,
.checkbox:focus-visible,
.radio:focus-visible {
  @apply outline-none ring-2 ring-primary ring-offset-2;
}

/* Error state styling */
.input-error {
  @apply border-error focus:border-error focus:ring-error;
}

.checkbox-error {
  @apply border-error;
}

.radio-error {
  @apply border-error;
}

/* Error message transition animations */
.error-message-enter-active,
.error-message-leave-active {
  transition: all 0.3s ease;
}

.error-message-enter-from {
  opacity: 0;
  transform: translateY(-5px);
}

.error-message-leave-to {
  opacity: 0;
  transform: translateY(-5px);
}
</style>
