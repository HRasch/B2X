<template>
  <div class="form-control" :class="wrapperClasses">
    <!-- Label -->
    <label v-if="label" :for="textareaId" class="label">
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
import { computed, ref, watch, nextTick, onMounted } from "vue";
import { useI18n } from "vue-i18n";
interface Props {
  name?: string;
  value?: string;
  label?: string;
  placeholder?: string;
  hint?: string;
  error?: string;
  errorSeverity?: "error" | "warning" | "info";
  disabled?: boolean;
  required?: boolean;
  rows?: number;
  maxlength?: number;
  showCounter?: boolean;
  autoResize?: boolean;
  size?: "sm" | "md" | "lg";
}

const props = withDefaults(defineProps<Props>(), {
  rows: 3,
  size: "md",
  showCounter: false,
  autoResize: false,
  errorSeverity: "error",
});

const emit = defineEmits<{
  input: [value: string];
  blur: [event: FocusEvent];
  focus: [event: FocusEvent];
}>();

const { t } = useI18n();

const textareaRef = ref();
const textareaId = computed(
  () => `textarea-${Math.random().toString(36).substr(2, 9)}`
);
const errorId = computed(() => `${textareaId.value}-error`);

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
    "resize-none": props.autoResize,
  },
]);

const autoResizeTextarea = () => {
  if (!props.autoResize || !textareaRef.value) return;

  const textarea = textareaRef.value;
  // Reset height to auto to get the correct scrollHeight
  textarea.style.height = "auto";
  // Set height to scrollHeight to fit content
  textarea.style.height = `${textarea.scrollHeight}px`;
};

const handleInput = (event: Event) => {
  const target = event.target as HTMLTextAreaElement;
  const value = target.value;
  emit("input", value);

  // Auto-resize if enabled
  if (props.autoResize) {
    autoResizeTextarea();
  }
};

const handleBlur = (event: FocusEvent) => {
  emit("blur", event);
};

const handleFocus = (event: FocusEvent) => {
  emit("focus", event);
};

// Auto-resize on mount and value changes
onMounted(() => {
  if (props.autoResize) {
    nextTick(() => autoResizeTextarea());
  }
});

watch(
  () => props.value,
  () => {
    if (props.autoResize) {
      nextTick(() => autoResizeTextarea());
    }
  }
);
</script>

<style scoped>
/* Custom focus styles for better accessibility */
.textarea:focus-visible {
  @apply outline-none ring-2 ring-primary ring-offset-2;
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
