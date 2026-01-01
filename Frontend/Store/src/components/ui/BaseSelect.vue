<template>
  <div class="form-control" :class="wrapperClasses">
    <!-- Label -->
    <label v-if="label" :for="selectId" class="label">
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

    <!-- Select wrapper -->
    <div class="relative">
      <!-- Single Select Mode -->
      <div v-if="!isMultiSelect" class="relative">
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
        <div
          class="absolute right-3 top-1/2 -translate-y-1/2 pointer-events-none"
        >
          <svg
            class="w-4 h-4 text-base-content/60"
            fill="currentColor"
            viewBox="0 0 20 20"
          >
            <path
              fill-rule="evenodd"
              d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 010-1.414L5.293 7.293a1 1 0 01-1.414-1.414z"
              clip-rule="evenodd"
            />
          </svg>
        </div>
      </div>

      <!-- Multi Select Mode -->
      <div v-else class="relative">
        <!-- Trigger Button -->
        <button
          :id="selectId"
          type="button"
          :disabled="disabled"
          :aria-expanded="isDropdownOpen"
          :aria-haspopup="true"
          :aria-describedby="errorId"
          :aria-invalid="hasError"
          :class="multiSelectTriggerClasses"
          @click="toggleDropdown"
          @blur="handleBlur"
          @focus="handleFocus"
        >
          <!-- Prefix -->
          <div
            v-if="$slots.prefix"
            class="flex items-center mr-2 text-base-content/60"
          >
            <slot name="prefix" />
          </div>

          <!-- Selected Tags -->
          <div class="flex flex-wrap gap-1 flex-1 min-w-0">
            <span
              v-for="option in selectedOptions"
              :key="option.value"
              class="inline-flex items-center gap-1 px-2 py-1 text-xs bg-primary/10 text-primary rounded-md"
            >
              {{ option.label }}
              <button
                type="button"
                class="hover:bg-primary/20 rounded-full p-0.5"
                @click.stop="removeTag(option.value)"
                :aria-label="t('ui.removeSelection', { item: option.label })"
              >
                <svg class="w-3 h-3" fill="currentColor" viewBox="0 0 20 20">
                  <path
                    fill-rule="evenodd"
                    d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 011.414-1.414z"
                    clip-rule="evenodd"
                  />
                </svg>
              </button>
            </span>

            <!-- Placeholder/Display Text -->
            <span
              v-if="selectedValues.length === 0"
              class="text-base-content/60"
            >
              {{ placeholder || t("ui.selectOptions") }}
            </span>
          </div>

          <!-- Dropdown Arrow -->
          <svg
            class="w-4 h-4 text-base-content/60 ml-2 flex-shrink-0"
            :class="{ 'rotate-180': isDropdownOpen }"
            fill="currentColor"
            viewBox="0 0 20 20"
          >
            <path
              fill-rule="evenodd"
              d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 010-1.414L5.293 7.293a1 1 0 01-1.414-1.414z"
              clip-rule="evenodd"
            />
          </svg>
        </button>

        <!-- Dropdown Menu -->
        <Transition name="dropdown-menu" appear>
          <ul
            v-if="isDropdownOpen"
            class="absolute top-full left-0 right-0 mt-1 bg-base-100 border border-base-300 rounded-box shadow-lg z-50 max-h-60 overflow-y-auto"
            role="listbox"
            :aria-multiselectable="isMultiSelect"
          >
            <!-- Clear All Option (for multi-select) -->
            <li
              v-if="isMultiSelect && selectedValues.length > 0"
              class="px-3 py-2 hover:bg-base-200 cursor-pointer border-b border-base-300"
              @click="clearSelection"
            >
              <span class="text-sm text-base-content/60">{{
                t("ui.clearAll")
              }}</span>
            </li>

            <!-- Options -->
            <li
              v-for="option in options"
              :key="option.value"
              class="px-3 py-2 hover:bg-base-200 cursor-pointer flex items-center gap-2"
              :class="{
                'bg-primary/10': selectedValues.includes(option.value),
              }"
              @click="selectOption(option)"
              role="option"
              :aria-selected="selectedValues.includes(option.value)"
            >
              <!-- Checkbox for multi-select -->
              <input
                v-if="isMultiSelect"
                type="checkbox"
                :checked="selectedValues.includes(option.value)"
                class="checkbox checkbox-primary checkbox-sm"
                @click.stop
                readonly
              />

              <!-- Radio for single-select -->
              <input
                v-else
                type="radio"
                :checked="props.value === option.value"
                class="radio radio-primary radio-sm"
                @click.stop
                readonly
              />

              <span
                :class="{
                  'font-medium': selectedValues.includes(option.value),
                }"
              >
                {{ option.label }}
              </span>
            </li>
          </ul>
        </Transition>
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
import { computed, ref, useSlots } from "vue";
import { useI18n } from "vue-i18n";
export interface SelectOption {
  value: string | number;
  label: string;
  disabled?: boolean;
}

interface Props {
  name?: string;
  value?: string | number | (string | number)[];
  options: SelectOption[];
  label?: string;
  placeholder?: string;
  hint?: string;
  error?: string;
  errorSeverity?: "error" | "warning" | "info";
  disabled?: boolean;
  required?: boolean;
  multiple?: boolean;
  maxSelections?: number;
  size?: "sm" | "md" | "lg";
}

const props = withDefaults(defineProps<Props>(), {
  size: "md",
  options: () => [],
  multiple: false,
  maxSelections: undefined,
  errorSeverity: "error",
});

const emit = defineEmits<{
  change: [value: string | number | (string | number)[]];
  blur: [event: FocusEvent];
  focus: [event: FocusEvent];
}>();

const { t } = useI18n();

const slots = useSlots();
const selectRef = ref();
const selectId = computed(
  () => `select-${Math.random().toString(36).substr(2, 9)}`
);
const errorId = computed(() => `${selectId.value}-error`);

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

const isMultiSelect = computed(() => props.multiple);
const selectedValues = computed(() => {
  if (isMultiSelect.value) {
    return Array.isArray(props.value) ? props.value : [];
  }
  return props.value ? [props.value] : [];
});

const selectedOptions = computed(() => {
  return props.options.filter((option) =>
    selectedValues.value.includes(option.value)
  );
});

const displayText = computed(() => {
  if (!isMultiSelect.value) {
    const selectedOption = props.options.find(
      (option) => option.value === props.value
    );
    return selectedOption ? selectedOption.label : props.placeholder || "";
  }

  if (selectedValues.value.length === 0) {
    return props.placeholder || "";
  }

  if (selectedValues.value.length === 1) {
    const selectedOption = props.options.find(
      (option) => option.value === selectedValues.value[0]
    );
    return selectedOption ? selectedOption.label : "";
  }

  return `${selectedValues.value.length} ${t("ui.selected")}`;
});

const isDropdownOpen = ref(false);

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
    "pl-10": Boolean(slots.prefix),
  },
]);

const multiSelectTriggerClasses = computed(() => [
  "w-full min-h-12 px-3 py-2 text-left bg-base-100 border border-base-300 rounded-box flex items-center cursor-pointer",
  `text-${props.size === "sm" ? "sm" : props.size === "lg" ? "base" : "base"}`,
  {
    "border-error": hasError.value,
    "bg-base-200 cursor-not-allowed": props.disabled,
    "ring-2 ring-primary ring-offset-2": isDropdownOpen.value,
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

const toggleDropdown = () => {
  if (!props.disabled) {
    isDropdownOpen.value = !isDropdownOpen.value;
  }
};

const selectOption = (option: SelectOption) => {
  if (option.disabled) return;

  if (isMultiSelect.value) {
    const currentValues = Array.isArray(props.value) ? [...props.value] : [];
    const isSelected = currentValues.includes(option.value);

    let newValues: (string | number)[];
    if (isSelected) {
      // Remove the option
      newValues = currentValues.filter((v) => v !== option.value);
    } else {
      // Add the option (check max selections)
      if (props.maxSelections && currentValues.length >= props.maxSelections) {
        return; // Don't add if max reached
      }
      newValues = [...currentValues, option.value];
    }

    emit("change", newValues);
  } else {
    // Single select
    emit("change", option.value);
    isDropdownOpen.value = false;
  }
};

const removeTag = (value: string | number) => {
  if (isMultiSelect.value) {
    const currentValues = Array.isArray(props.value) ? [...props.value] : [];
    const newValues = currentValues.filter((v) => v !== value);
    emit("change", newValues);
  }
};

const clearSelection = () => {
  if (isMultiSelect.value) {
    emit("change", []);
  } else {
    emit("change", "");
  }
};
</script>

<style scoped>
/* Custom focus styles for better accessibility */
.select:focus-visible {
  @apply outline-none ring-2 ring-primary ring-offset-2;
}

/* Multi-select trigger focus styles */
button:focus-visible {
  @apply outline-none ring-2 ring-primary ring-offset-2;
}

/* Dropdown menu transition animations */
.dropdown-menu-enter-active,
.dropdown-menu-leave-active {
  transition: all 0.2s ease;
}

.dropdown-menu-enter-from {
  opacity: 0;
  transform: translateY(-10px) scale(0.95);
}

.dropdown-menu-leave-to {
  opacity: 0;
  transform: translateY(-10px) scale(0.95);
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
