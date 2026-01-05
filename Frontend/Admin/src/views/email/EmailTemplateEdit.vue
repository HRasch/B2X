<template>
  <div class="email-template-edit-page">
    <div class="page-header">
      <div class="header-content">
        <h1>{{ $t('email.templates.edit') }}</h1>
        <p>{{ $t('email.templates.subtitle') }}</p>
      </div>
      <div class="header-actions">
        <button @click="cancel" class="btn-secondary">{{ $t('ui.cancel') }}</button>
      </div>
    </div>

    <div v-if="template" class="editor-container">
      <EmailTemplateEditor
        :template="template"
        :is-editing="true"
        @saved="onTemplateSaved"
        @cancelled="onCancelled"
      />
    </div>

    <div v-else class="loading-state">
      <div class="spinner"></div>
      <p>{{ $t('ui.loading') }}</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { useEmailStore } from '@/stores/email';
import EmailTemplateEditor from '@/components/EmailTemplateEditor.vue';
import type { EmailTemplate } from '@/types/email';

const router = useRouter();
const route = useRoute();
const { t } = useI18n();
const emailStore = useEmailStore();

const template = ref<EmailTemplate | null>(null);
const templateId = route.params.id as string;

const loadTemplate = async () => {
  try {
    template.value = await emailStore.fetchTemplate(templateId);
  } catch (error) {
    console.error('Failed to load template:', error);
    // Handle error - maybe redirect to list
    router.push('/email/templates');
  }
};

const onTemplateSaved = (updatedTemplate: EmailTemplate) => {
  // Navigate back to the template list
  router.push('/email/templates');
};

const onCancelled = () => {
  router.push('/email/templates');
};

onMounted(() => {
  loadTemplate();
});
</script>

<style scoped>
.email-template-edit-page {
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

.editor-container {
  background: white;
  margin: 2rem;
  border-radius: 0.75rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem;
  color: #6b7280;
}

.spinner {
  width: 2rem;
  height: 2rem;
  border: 3px solid #e5e7eb;
  border-top: 3px solid #3b82f6;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 1rem;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}
</style>
