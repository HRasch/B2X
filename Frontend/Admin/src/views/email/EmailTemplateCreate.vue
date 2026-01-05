<template>
  <div class="email-template-create-page">
    <div class="page-header">
      <div class="header-content">
        <h1>{{ $t('email.templates.create') }}</h1>
        <p>{{ $t('email.templates.subtitle') }}</p>
      </div>
      <div class="header-actions">
        <button @click="cancel" class="btn-secondary">{{ $t('ui.cancel') }}</button>
      </div>
    </div>

    <EmailTemplateEditor @saved="onTemplateSaved" @cancelled="onCancelled" />
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import EmailTemplateEditor from '@/components/EmailTemplateEditor.vue';
import type { EmailTemplate } from '@/types/email';

const router = useRouter();
const { t } = useI18n();

const onTemplateSaved = (template: EmailTemplate) => {
  // Navigate to the template list or detail view
  router.push('/email/templates');
};

const onCancelled = () => {
  router.push('/email/templates');
};
</script>

<style scoped>
.email-template-create-page {
  min-height: 100vh;
  background: #f9fafb;
}

.page-header {
  background: white;
  padding: 2rem;
  border-bottom: 1px solid #e5e7eb;
  margin-bottom: 0;
}

.header-content h1 {
  margin: 0 0 0.5rem 0;
  font-size: 2rem;
  font-weight: 700;
  color: #111827;
}

.header-content p {
  margin: 0;
  color: #6b7280;
}

.header-actions {
  display: flex;
  gap: 1rem;
}

.btn-secondary {
  background: white;
  color: #374151;
  border: 1px solid #d1d5db;
  padding: 0.75rem 1.5rem;
  border-radius: 0.5rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-secondary:hover {
  background: #f9fafb;
  border-color: #9ca3af;
}
</style>
