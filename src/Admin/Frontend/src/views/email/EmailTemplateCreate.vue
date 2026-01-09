<template>
  <div class="email-template-create-page">
    <PageHeader :title="$t('email.templates.create')" :subtitle="$t('email.templates.subtitle')">
      <template #actions>
        <button @click="cancel" class="btn-secondary">{{ $t('ui.cancel') }}</button>
      </template>
    </PageHeader>

    <EmailTemplateEditor @saved="onTemplateSaved" @cancelled="onCancelled" />
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router';
import EmailTemplateEditor from '@/components/EmailTemplateEditor.vue';
import { PageHeader } from '@/components/layout';
import type { EmailTemplate } from '@/types/email';

const router = useRouter();

const cancel = () => {
  router.push('/email/templates');
};

const onTemplateSaved = (_template: EmailTemplate) => {
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

.dark .email-template-create-page {
  background: #0f172a;
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

.dark .btn-secondary {
  background: #374151;
  color: #e5e7eb;
  border-color: #4b5563;
}

.dark .btn-secondary:hover {
  background: #4b5563;
}
</style>
