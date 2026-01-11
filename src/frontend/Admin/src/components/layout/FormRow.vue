<template>
  <div class="form-row" :class="[`form-row--cols-${cols}`]">
    <slot />
  </div>
</template>

<script setup lang="ts">
/**
 * FormRow Component
 * Responsive multi-column form field layout
 *
 * @example
 * <FormRow :cols="2">
 *   <FormGroup label="First Name" required>
 *     <input v-model="firstName" type="text" />
 *   </FormGroup>
 *   <FormGroup label="Last Name" required>
 *     <input v-model="lastName" type="text" />
 *   </FormGroup>
 * </FormRow>
 */
withDefaults(
  defineProps<{
    /** Number of columns (1-4), defaults to 2 */
    cols?: 1 | 2 | 3 | 4;
  }>(),
  {
    cols: 2,
  }
);
</script>

<style scoped>
.form-row {
  display: grid;
  gap: var(--form-gap, 1.5rem);
  width: 100%;
}

.form-row--cols-1 {
  grid-template-columns: 1fr;
}

.form-row--cols-2 {
  grid-template-columns: repeat(2, 1fr);
}

.form-row--cols-3 {
  grid-template-columns: repeat(3, 1fr);
}

.form-row--cols-4 {
  grid-template-columns: repeat(4, 1fr);
}

/* Responsive: Collapse to single column */
@media (max-width: 1024px) {
  .form-row--cols-3,
  .form-row--cols-4 {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 640px) {
  .form-row--cols-2,
  .form-row--cols-3,
  .form-row--cols-4 {
    grid-template-columns: 1fr;
  }
}
</style>
