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
        <span v-if="required" class="text-error ml-1" aria-label="required">*</span>
      </span>
    </label>

    <!-- Input wrapper for different types -->
    <div v-if="type === 'checkbox' || type === 'radio'" class="flex items-center gap-2">
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
      <div v-if="$slots.prefix" class="absolute left-3 top-1/2 -translate-y-1/2 text-base-content/60">
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
      <div v-if="$slots.suffix" class="absolute right-3 top-1/2 -translate-y-1/2 text-base-content/60">
        <slot name="suffix" />
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
export type InputType = 'text' | 'email' | 'password' | 'number' | 'tel' | 'url' | 'search' | 'checkbox' | 'radio'

interface Props {
  type?: InputType
  name?: string
  value?: string | number | boolean
  checked?: boolean
  label?: string
  placeholder?: string
  hint?: string
  error?: string
  disabled?: boolean
  required?: boolean
  minlength?: number
  maxlength?: number
  pattern?: string
  size?: 'sm' | 'md' | 'lg'
}

const props = withDefaults(defineProps<Props>(), {
  type: 'text',
  size: 'md'
})

const emit = defineEmits<{
  input: [value: string | number | boolean]
  blur: [event: FocusEvent]
  focus: [event: FocusEvent]
}>()

const inputRef = ref<HTMLInputElement>()
const inputId = computed(() => `input-${Math.random().toString(36).substr(2, 9)}`)
const errorId = computed(() => `${inputId.value}-error`)

const hasError = computed(() => Boolean(props.error))
const inputType = computed(() => {
  if (props.type === 'checkbox' || props.type === 'radio') return props.type
  return props.type
})

const wrapperClasses = computed(() => ({
  'w-full': true
}))

const labelClasses = computed(() => ({
  'text-error': hasError.value
}))

const inputClasses = computed(() => {
  const baseClasses = [
    'input',
    'input-bordered',
    `input-${props.size === 'sm' ? 'sm' : props.size === 'lg' ? 'lg' : ''}`,
    {
      'input-error': hasError.value,
      'input-disabled': props.disabled,
      'pl-10': Boolean($slots.prefix),
      'pr-10': Boolean($slots.suffix)
    }
  ]

  if (props.type === 'checkbox') {
    return ['checkbox', 'checkbox-primary', { 'checkbox-error': hasError.value }]
  }

  if (props.type === 'radio') {
    return ['radio', 'radio-primary', { 'radio-error': hasError.value }]
  }

  return baseClasses.filter(Boolean)
})

const handleInput = (event: Event) => {
  const target = event.target as HTMLInputElement
  let value: string | number | boolean = target.value

  if (props.type === 'checkbox') {
    value = target.checked
  } else if (props.type === 'number') {
    value = target.valueAsNumber || 0
  }

  emit('input', value)
}

const handleBlur = (event: FocusEvent) => {
  emit('blur', event)
}

const handleFocus = (event: FocusEvent) => {
  emit('focus', event)
}

// Expose methods for parent components
defineExpose({
  focus: () => inputRef.value?.focus(),
  blur: () => inputRef.value?.blur()
})
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
</style>