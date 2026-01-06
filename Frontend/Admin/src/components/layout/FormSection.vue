<template>
  <section class="form-section">
    <header v-if="title || description" class="form-section__header">
      <h2 v-if="title" class="form-section__title">{{ title }}</h2>
      <p v-if="description" class="form-section__description">{{ description }}</p>
    </header>
    <div class="form-section__content">
      <slot />
    </div>
  </section>
</template>

<script setup lang="ts">
/**
 * FormSection Component
 * Groups related form fields with optional heading
 *
 * @example
 * <FormSection
 *   :title="$t('user.form.basicInfo')"
 *   :description="$t('user.form.basicInfoDesc')"
 * >
 *   <FormRow>
 *     <!-- form fields -->
 *   </FormRow>
 * </FormSection>
 */
defineProps<{
  /** Section title */
  title?: string;
  /** Section description */
  description?: string;
}>();
</script>

<style scoped>
.form-section {
  margin-bottom: var(--section-gap, 2rem);
}

.form-section:last-child {
  margin-bottom: 0;
}

.form-section__header {
  margin-bottom: 1.25rem;
  padding-bottom: 0.75rem;
  border-bottom: 1px solid #e5e7eb;
}

.dark .form-section__header {
  border-bottom-color: #374151;
}

.form-section__title {
  margin: 0;
  font-size: 1.125rem;
  font-weight: 600;
  color: #111827;
}

.dark .form-section__title {
  color: #f9fafb;
}

.form-section__description {
  margin: 0.375rem 0 0 0;
  font-size: 0.875rem;
  color: #6b7280;
}

.dark .form-section__description {
  color: #9ca3af;
}

.form-section__content {
  display: flex;
  flex-direction: column;
  gap: var(--form-gap, 1.5rem);
}
</style>
