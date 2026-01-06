<template>
  <div class="form-group" :class="{ 'form-group--error': error }">
    <label v-if="label" :for="inputId" class="form-group__label">
      {{ label }}
      <span v-if="required" class="form-group__required">*</span>
    </label>
    <div class="form-group__input">
      <slot />
    </div>
    <p v-if="error" class="form-group__error">{{ error }}</p>
    <p v-else-if="hint" class="form-group__hint">{{ hint }}</p>
  </div>
</template>

<script setup lang="ts">
/**
 * FormGroup Component
 * Wraps form inputs with label, error, and hint text
 *
 * @example
 * <FormGroup
 *   :label="$t('user.form.email')"
 *   :error="errors.email"
 *   required
 * >
 *   <input v-model="email" type="email" id="email" />
 * </FormGroup>
 */
defineProps<{
  /** Field label */
  label?: string;
  /** Input element ID for label association */
  inputId?: string;
  /** Whether field is required */
  required?: boolean;
  /** Error message to display */
  error?: string;
  /** Hint text displayed below input */
  hint?: string;
}>();
</script>

<style scoped>
.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.375rem;
}

.form-group__label {
  font-size: 0.875rem;
  font-weight: 500;
  color: #374151;
}

.dark .form-group__label {
  color: #d1d5db;
}

.form-group__required {
  color: #dc2626;
  margin-left: 0.125rem;
}

.form-group__input :deep(input),
.form-group__input :deep(select),
.form-group__input :deep(textarea) {
  width: 100%;
  padding: 0.625rem 0.875rem;
  font-size: 0.9375rem;
  color: #111827;
  background: white;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  transition:
    border-color 0.15s,
    box-shadow 0.15s;
}

.dark .form-group__input :deep(input),
.dark .form-group__input :deep(select),
.dark .form-group__input :deep(textarea) {
  color: #f9fafb;
  background: #1e293b;
  border-color: #475569;
}

.form-group__input :deep(input):focus,
.form-group__input :deep(select):focus,
.form-group__input :deep(textarea):focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.form-group__input :deep(input):disabled,
.form-group__input :deep(select):disabled,
.form-group__input :deep(textarea):disabled {
  background: #f3f4f6;
  cursor: not-allowed;
}

.dark .form-group__input :deep(input):disabled,
.dark .form-group__input :deep(select):disabled,
.dark .form-group__input :deep(textarea):disabled {
  background: #334155;
}

/* Error state */
.form-group--error .form-group__input :deep(input),
.form-group--error .form-group__input :deep(select),
.form-group--error .form-group__input :deep(textarea) {
  border-color: #dc2626;
}

.form-group--error .form-group__input :deep(input):focus,
.form-group--error .form-group__input :deep(select):focus,
.form-group--error .form-group__input :deep(textarea):focus {
  box-shadow: 0 0 0 3px rgba(220, 38, 38, 0.1);
}

.form-group__error {
  margin: 0;
  font-size: 0.8125rem;
  color: #dc2626;
}

.form-group__hint {
  margin: 0;
  font-size: 0.8125rem;
  color: #6b7280;
}

.dark .form-group__hint {
  color: #9ca3af;
}
</style>
