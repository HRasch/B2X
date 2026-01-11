<template>
  <div class="page-header">
    <div class="page-header__content">
      <h1 class="page-header__title">{{ title }}</h1>
      <p v-if="subtitle" class="page-header__subtitle">{{ subtitle }}</p>
    </div>
    <div v-if="$slots.actions" class="page-header__actions">
      <slot name="actions" />
    </div>
  </div>
</template>

<script setup lang="ts">
/**
 * PageHeader Component
 * Unified header for all Admin pages
 *
 * @example
 * <PageHeader
 *   :title="$t('email.templates.create')"
 *   :subtitle="$t('email.templates.subtitle')"
 * >
 *   <template #actions>
 *     <button class="btn-secondary">Cancel</button>
 *     <button class="btn-primary">Save</button>
 *   </template>
 * </PageHeader>
 */
defineProps<{
  /** Page title (required) */
  title: string;
  /** Optional subtitle/description */
  subtitle?: string;
}>();
</script>

<style scoped>
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 1.5rem;
  padding: 1.5rem 2rem;
  background: white;
  border-bottom: 1px solid #e5e7eb;
}

.dark .page-header {
  background: #1e293b;
  border-bottom-color: #334155;
}

.page-header__content {
  flex: 1;
  min-width: 0;
}

.page-header__title {
  margin: 0;
  font-size: 1.875rem;
  font-weight: 700;
  color: #111827;
  line-height: 1.2;
}

.dark .page-header__title {
  color: #f9fafb;
}

.page-header__subtitle {
  margin: 0.5rem 0 0 0;
  font-size: 0.9375rem;
  color: #6b7280;
}

.dark .page-header__subtitle {
  color: #9ca3af;
}

.page-header__actions {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  flex-shrink: 0;
}

/* Responsive: Stack on mobile */
@media (max-width: 640px) {
  .page-header {
    flex-direction: column;
    align-items: stretch;
    padding: 1rem 1.5rem;
  }

  .page-header__actions {
    justify-content: flex-end;
  }
}
</style>
