<template>
  <div class="card-container" :class="{ 'card-container--elevated': elevated }">
    <header v-if="title || $slots.header" class="card-container__header">
      <slot name="header">
        <h3 v-if="title" class="card-container__title">{{ title }}</h3>
        <p v-if="subtitle" class="card-container__subtitle">{{ subtitle }}</p>
      </slot>
    </header>
    <div class="card-container__body" :class="{ 'card-container__body--no-padding': noPadding }">
      <slot />
    </div>
    <footer v-if="$slots.footer" class="card-container__footer">
      <slot name="footer" />
    </footer>
  </div>
</template>

<script setup lang="ts">
/**
 * CardContainer Component
 * Standard card wrapper for content sections
 *
 * @example
 * <CardContainer title="User Settings">
 *   <FormSection>...</FormSection>
 *   <template #footer>
 *     <button class="btn-primary">Save</button>
 *   </template>
 * </CardContainer>
 */
defineProps<{
  /** Card title */
  title?: string;
  /** Card subtitle */
  subtitle?: string;
  /** Whether to show elevated shadow */
  elevated?: boolean;
  /** Remove body padding */
  noPadding?: boolean;
}>();
</script>

<style scoped>
.card-container {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 0.75rem;
  overflow: hidden;
}

.dark .card-container {
  background: #1e293b;
  border-color: #334155;
}

.card-container--elevated {
  border: none;
  box-shadow:
    0 1px 3px rgba(0, 0, 0, 0.1),
    0 1px 2px rgba(0, 0, 0, 0.06);
}

.card-container__header {
  padding: 1.25rem 1.5rem;
  border-bottom: 1px solid #e5e7eb;
}

.dark .card-container__header {
  border-bottom-color: #374151;
}

.card-container__title {
  margin: 0;
  font-size: 1.125rem;
  font-weight: 600;
  color: #111827;
}

.dark .card-container__title {
  color: #f9fafb;
}

.card-container__subtitle {
  margin: 0.25rem 0 0 0;
  font-size: 0.875rem;
  color: #6b7280;
}

.dark .card-container__subtitle {
  color: #9ca3af;
}

.card-container__body {
  padding: 1.5rem;
}

.card-container__body--no-padding {
  padding: 0;
}

.card-container__footer {
  padding: 1rem 1.5rem;
  border-top: 1px solid #e5e7eb;
  background: #f9fafb;
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
}

.dark .card-container__footer {
  border-top-color: #374151;
  background: #0f172a;
}
</style>
