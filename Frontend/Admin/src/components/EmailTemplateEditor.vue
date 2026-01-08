<template>
  <div class="email-template-editor">
    <div class="editor-header" :class="{ 'unsaved-changes': hasUnsavedChanges }">
      <h2>{{ isEditing ? $t('email.templates.edit') : $t('email.templates.create') }}</h2>
      <div class="actions">
        <button
          @click="toggleEmailPreviewMode"
          class="btn-preview-mode"
          :title="emailPreviewMode === 'dark' ? 'Preview in light mode' : 'Preview in dark mode'"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            width="20"
            height="20"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <rect x="2" y="3" width="20" height="14" rx="2" ry="2"></rect>
            <line x1="8" y1="21" x2="16" y2="21"></line>
            <line x1="12" y1="17" x2="12" y2="21"></line>
          </svg>
          <span class="preview-mode-label">{{ emailPreviewMode === 'dark' ? '🌙' : '☀️' }}</span>
        </button>
        <div class="editor-mode-toggle">
          <button
            @click="editorMode = 'visual'"
            :class="{ active: editorMode === 'visual' }"
            class="btn-mode"
          >
            {{ $t('email.templates.visualMode') }}
          </button>
          <button
            @click="editorMode = 'code'"
            :class="{ active: editorMode === 'code' }"
            class="btn-mode"
          >
            {{ $t('email.templates.codeMode') }}
          </button>
        </div>
        <button @click="preview" class="btn-secondary">{{ $t('email.templates.preview') }}</button>
        <button @click="testSend" class="btn-secondary">{{ $t('email.templates.test') }}</button>
        <button
          v-if="hasUnsavedChanges"
          @click="discardChanges"
          class="btn-secondary"
          :title="$t('email.templates.discardChanges')"
        >
          {{ $t('email.templates.discardChanges') }}
        </button>
        <button @click="save" :disabled="!isValid" class="btn-primary">
          {{ hasUnsavedChanges ? $t('email.templates.saveChanges') : $t('ui.save') }}
        </button>
      </div>
    </div>

    <form @submit.prevent="save" class="template-form">
      <div class="form-row">
        <div class="form-group">
          <label for="templateKey">{{ $t('email.templates.key') }} *</label>
          <input
            id="templateKey"
            v-model="form.templateKey"
            type="text"
            required
            :disabled="isEditing"
            :placeholder="$t('email.templates.key')"
          />
        </div>
        <div class="form-group">
          <label for="locale">{{ $t('email.templates.locale') }} *</label>
          <select id="locale" v-model="form.locale" required>
            <option value="en">{{ $t('email.templates.english') }}</option>
            <option value="de">{{ $t('email.templates.german') }}</option>
            <option value="fr">{{ $t('email.templates.french') }}</option>
            <option value="es">{{ $t('email.templates.spanish') }}</option>
            <option value="it">{{ $t('email.templates.italian') }}</option>
            <option value="pt">{{ $t('email.templates.portuguese') }}</option>
            <option value="nl">{{ $t('email.templates.dutch') }}</option>
            <option value="pl">{{ $t('email.templates.polish') }}</option>
          </select>
        </div>
      </div>

      <div class="form-group">
        <label for="name">{{ $t('email.templates.name') }} *</label>
        <input
          id="name"
          v-model="form.name"
          type="text"
          required
          :placeholder="$t('email.templates.name')"
        />
      </div>

      <div class="form-group">
        <label for="description">{{ $t('email.templates.description') }}</label>
        <textarea
          id="description"
          v-model="form.description"
          :placeholder="$t('email.templates.description')"
          rows="2"
        ></textarea>
      </div>

      <div class="form-group">
        <label for="subject">{{ $t('email.templates.subject') }} *</label>
        <input
          id="subject"
          v-model="form.subject"
          type="text"
          required
          :placeholder="$t('email.templates.subject')"
        />
      </div>

      <div class="form-group">
        <label for="layout">{{ $t('email.templates.layout') }}</label>
        <select id="layout" v-model="form.layoutId">
          <option value="">{{ $t('email.templates.noLayout') }}</option>
        </select>
      </div>

      <div class="form-group">
        <label for="htmlBody">{{ $t('email.templates.htmlBody') }} *</label>
        <div class="editor-container">
          <EmailBuilderWYSIWYG
            v-if="editorMode === 'visual'"
            v-model="visualContent"
            :height="'600px'"
            :preview-mode="emailPreviewMode"
          />
          <CodeEditor
            v-else
            id="htmlBody"
            v-model="codeContent"
            language="html"
            :height="'600px'"
            :theme="monacoTheme"
            :read-only="false"
          />
        </div>
        <div class="liquid-help">
          <div>{{ $t('email.templates.liquidVariables') }}</div>
          <div class="variables-list">
            <div v-for="variable in liquidVariables" :key="variable.name" class="variable-item">
              <span>{{ variable.name }}</span>
              <span>{{ variable.description }}</span>
              <span>{{ variable.example }}</span>
            </div>
          </div>
        </div>
      </div>

      <div class="form-group">
        <label for="plainTextBody">{{ $t('email.templates.plainTextBody') }}</label>
        <textarea
          id="plainTextBody"
          v-model="form.plainTextBody"
          :placeholder="$t('email.templates.plainTextBodyPlaceholder')"
          rows="10"
        ></textarea>
      </div>
    </form>

    <!-- Preview Modal -->
    <div v-if="showPreview" class="modal" @click="showPreview = false">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>{{ $t('email.templates.previewModalTitle') }}</h3>
          <button @click="showPreview = false" class="close-btn">
            {{ $t('email.templates.closeButton') }}
          </button>
        </div>
        <div class="preview-content">
          <div class="preview-subject">
            {{ $t('email.templates.subject') }}: {{ previewData.subject }}
          </div>
          <div class="preview-body" v-html="safePreviewBody"></div>
        </div>
      </div>
    </div>

    <!-- Test Send Modal -->
    <div v-if="showTestSend" class="modal" @click="showTestSend = false">
      <div class="modal-content" @click.stop>
        <form @submit.prevent="sendTestEmail">
          <div class="modal-header">
            <h3>{{ $t('email.templates.testSendModalTitle') }}</h3>
            <button @click="showTestSend = false" class="close-btn">
              {{ $t('email.templates.closeButton') }}
            </button>
          </div>
          <div class="modal-body">
            <div class="form-group">
              <label for="testEmail">{{ $t('email.templates.testEmailAddress') }} *</label>
              <input
                id="testEmail"
                v-model="testEmail"
                type="email"
                required
                :placeholder="$t('email.templates.testEmailPlaceholder')"
              />
            </div>
            <div class="form-group">
              <label for="sampleData">{{ $t('email.templates.sampleData') }}</label>
              <textarea
                id="sampleData"
                v-model="sampleDataJson"
                :placeholder="$t('email.templates.sampleDataPlaceholder')"
                rows="5"
              ></textarea>
            </div>
          </div>
          <div class="modal-footer">
            <button type="button" @click="showTestSend = false" class="btn-secondary">
              {{ $t('ui.cancel') }}
            </button>
            <button type="submit" class="btn-primary">{{ $t('email.templates.sendTest') }}</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from 'vue';
import { useEmailStore } from '@/stores/email';
import { useThemeStore } from '@/stores/theme';
import { useSafeHtml } from '@/utils/sanitize';
import CodeEditor from '@/components/common/CodeEditor.vue';
import EmailBuilderWYSIWYG from '@/components/email/EmailBuilderWYSIWYG.vue';
import type { EmailTemplate, EmailTemplateForm, EmailLayout, LiquidVariable } from '@/types/email';

interface Props {
  template?: EmailTemplate;
  isEditing?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  isEditing: false,
});

const emit = defineEmits<{
  saved: [template: EmailTemplate];
  cancelled: [];
}>();

const emailStore = useEmailStore();

// Use global theme from theme store
const themeStore = useThemeStore();
const globalTheme = computed(() => themeStore.effectiveTheme);

// Computed Monaco theme based on global theme
const monacoTheme = computed(() => (globalTheme.value === 'dark' ? 'vs-dark' : 'vs-light'));

// Email preview mode (independent from global theme)
const emailPreviewMode = ref<'light' | 'dark'>('light');
const toggleEmailPreviewMode = () => {
  emailPreviewMode.value = emailPreviewMode.value === 'light' ? 'dark' : 'light';
};

// Editor mode state
const editorMode = ref<'visual' | 'code'>('visual');

// Draft state for editor content (unsaved changes)
const draftHtmlBody = ref<string>('');

// Track if there are unsaved changes
const hasUnsavedChanges = computed(() => {
  return draftHtmlBody.value !== form.value.htmlBody;
});

// Form data
const form = ref<EmailTemplateForm>({
  templateKey: '',
  locale: 'en',
  name: '',
  description: '',
  subject: '',
  htmlBody: '',
  plainTextBody: '',
  layoutId: '',
});

// UI state
const showPreview = ref(false);
const showTestSend = ref(false);
const testEmail = ref('');
const sampleDataJson = ref('');

// Data
const layouts = ref<EmailLayout[]>([]);
const liquidVariables = ref<LiquidVariable[]>([
  {
    name: 'customer.name',
    description: $t('email.templates.customerName'),
    category: 'Customer',
    example: $t('email.templates.customerNameExample').replace('Example: ', ''),
    type: 'string',
  },
  {
    name: 'customer.email',
    description: $t('email.templates.customerEmail'),
    category: 'Customer',
    example: $t('email.templates.customerEmailExample'),
    type: 'string',
  },
  {
    name: 'order.id',
    description: 'Order ID',
    category: 'Order',
    example: 'ORD-12345',
    type: 'string',
  },
  {
    name: 'order.total',
    description: 'Order total',
    category: 'Order',
    example: '99.99',
    type: 'number',
  },
  {
    name: 'store.name',
    description: 'Store name',
    category: 'Store',
    example: 'My Store',
    type: 'string',
  },
  {
    name: 'store.url',
    description: 'Store URL',
    category: 'Store',
    example: 'https://store.example.com',
    type: 'string',
  },
]);

// Computed
const isValid = computed(() => {
  return (
    form.value.templateKey &&
    form.value.locale &&
    form.value.name &&
    form.value.subject &&
    form.value.htmlBody
  );
});

const previewData = computed(() => {
  return {
    subject: form.value.subject,
    htmlBody: form.value.htmlBody,
  };
});

const safePreviewBody = useSafeHtml(previewData.value.htmlBody);

// Content synchronization between GrapesJS and Monaco (using draft state)
const visualContent = computed({
  get: () => draftHtmlBody.value || form.value.htmlBody,
  set: (value: string) => {
    draftHtmlBody.value = value;
  },
});

const codeContent = computed({
  get: () => {
    const content = draftHtmlBody.value || form.value.htmlBody;
    // Extract body content from full HTML for Monaco editor
    if (!content) return '';
    try {
      const parser = new DOMParser();
      const doc = parser.parseFromString(content, 'text/html');
      const body = doc.querySelector('body');
      return body ? body.innerHTML : content;
    } catch (error) {
      console.warn('Failed to parse HTML for code editor:', error);
      return content;
    }
  },
  set: (value: string) => {
    // Convert Monaco content back to full HTML document
    const fullHtml = `<!DOCTYPE html>
<html>
<head>
  <meta name="color-scheme" content="light dark">
  <meta name="supported-color-schemes" content="light dark">
  <style>
    @media (prefers-color-scheme: dark) {
      body { background-color: #1f2937 !important; color: #f9fafb !important; }
      .container { background-color: #111827 !important; }
      a { color: #60a5fa !important; }
      h1, h2, h3, h4, h5, h6 { color: #f9fafb !important; }
    }
  </style>
</head>
<body>
  ${value}
</body>
</html>`;
    draftHtmlBody.value = fullHtml;
  },
});

// Methods
const loadLayouts = async () => {
  try {
    layouts.value = await emailStore.fetchLayouts();
  } catch (error) {
    console.error('Failed to load layouts:', error);
  }
};

const loadTemplate = () => {
  if (props.template) {
    form.value = {
      templateKey: props.template.templateKey,
      locale: props.template.locale,
      name: props.template.name,
      description: props.template.description || '',
      subject: props.template.subject,
      htmlBody: props.template.htmlBody,
      plainTextBody: props.template.plainTextBody || '',
      layoutId: props.template.layoutId || '',
    };
    // Initialize draft with current content
    draftHtmlBody.value = props.template.htmlBody;
  }
};

const save = async () => {
  if (!isValid.value) return;

  try {
    // Apply draft changes to form before saving
    if (hasUnsavedChanges.value) {
      form.value.htmlBody = draftHtmlBody.value;
    }

    const savedTemplate = await emailStore.saveTemplate(form.value, props.isEditing);
    emit('saved', savedTemplate);

    // Clear draft state after successful save
    draftHtmlBody.value = form.value.htmlBody;
  } catch (error) {
    console.error('Failed to save template:', error);
    // Handle error (show toast, etc.)
  }
};

const discardChanges = () => {
  // Reset draft to current form state
  draftHtmlBody.value = form.value.htmlBody;
};

const preview = () => {
  showPreview.value = true;
};

const testSend = () => {
  showTestSend.value = true;
};

const sendTestEmail = async () => {
  try {
    const sampleData = sampleDataJson.value ? JSON.parse(sampleDataJson.value) : {};
    await emailStore.sendTestEmail(
      form.value.templateKey,
      form.value.locale,
      testEmail.value,
      sampleData
    );
    showTestSend.value = false;
    testEmail.value = '';
    sampleDataJson.value = '';
    // Show success message
  } catch (error) {
    console.error('Failed to send test email:', error);
    // Handle error
  }
};

// Lifecycle
onMounted(() => {
  loadLayouts();
  if (props.isEditing) {
    loadTemplate();
  }

  // Warn user about unsaved changes when leaving page
  const handleBeforeUnload = (event: BeforeUnloadEvent) => {
    if (hasUnsavedChanges.value) {
      event.preventDefault();
      event.returnValue = '';
    }
  };

  window.addEventListener('beforeunload', handleBeforeUnload);

  // Cleanup on unmount
  onBeforeUnmount(() => {
    window.removeEventListener('beforeunload', handleBeforeUnload);
  });
});
</script>

<style scoped>
.email-template-editor {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
}

.editor-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid #e5e7eb;
}

.editor-header.unsaved-changes {
  border-bottom-color: #f59e0b;
}

.unsaved-indicator {
  position: absolute;
  top: -8px;
  right: -8px;
  width: 8px;
  height: 8px;
  background-color: #f59e0b;
  border-radius: 50%;
  border: 2px solid #ffffff;
}

.actions {
  display: flex;
  gap: 1rem;
  align-items: center;
}

.editor-mode-toggle {
  display: flex;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  overflow: hidden;
}

.btn-mode {
  padding: 0.5rem 1rem;
  background: #ffffff;
  border: none;
  color: #6b7280;
  cursor: pointer;
  transition: all 0.2s;
  font-size: 0.875rem;
  font-weight: 500;
}

.btn-mode:hover {
  background: #f9fafb;
}

.btn-mode.active {
  background: #3b82f6;
  color: #ffffff;
}

.btn-mode:not(:last-child) {
  border-right: 1px solid #d1d5db;
}

.template-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-group label {
  font-weight: 600;
  color: #374151;
}

.form-group input,
.form-group select,
.form-group textarea {
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  font-size: 0.875rem;
}

.form-group input:focus,
.form-group select:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.code-editor {
  font-family: 'Monaco', 'Menlo', 'Ubuntu Mono', monospace;
  font-size: 0.875rem;
  line-height: 1.5;
}

.liquid-help {
  margin-top: 1rem;
  padding: 1rem;
  background: #f9fafb;
  border-radius: 0.375rem;
}

.liquid-help h4 {
  margin: 0 0 1rem 0;
  color: #374151;
}

.variables-list {
  display: grid;
  gap: 0.75rem;
}

.variable-item {
  padding: 0.75rem;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 0.25rem;
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.variable-item code {
  font-weight: 600;
  color: #3b82f6;
}

.btn-primary {
  background: #3b82f6;
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 0.375rem;
  font-weight: 600;
  cursor: pointer;
}

.btn-primary:hover:not(:disabled) {
  background: #2563eb;
}

.btn-primary:disabled {
  background: #9ca3af;
  cursor: not-allowed;
}

.btn-secondary {
  background: white;
  color: #374151;
  border: 1px solid #d1d5db;
  padding: 0.75rem 1.5rem;
  border-radius: 0.375rem;
  font-weight: 600;
  cursor: pointer;
}

.btn-secondary:hover {
  background: #f9fafb;
}

.btn-preview-mode {
  background: transparent;
  border: 1px solid #d1d5db;
  padding: 0.5rem 0.75rem;
  border-radius: 0.375rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  transition: all 0.2s;
  margin-right: 0.5rem;
  font-size: 0.875rem;
  font-weight: 500;
}

.btn-preview-mode:hover {
  background: #f3f4f6;
  border-color: #9ca3af;
}

.btn-preview-mode svg {
  width: 16px;
  height: 16px;
}

.preview-mode-label {
  font-size: 16px;
  line-height: 1;
}

.modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 0.5rem;
  max-width: 800px;
  width: 90%;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-header {
  padding: 1.5rem;
  border-bottom: 1px solid #e5e7eb;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #6b7280;
}

.modal-body {
  padding: 1.5rem;
}

.modal-footer {
  padding: 1.5rem;
  border-top: 1px solid #e5e7eb;
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
}

.preview-content {
  padding: 1.5rem;
}

.preview-subject {
  margin-bottom: 1rem;
  padding: 1rem;
  background: #f9fafb;
  border-radius: 0.25rem;
}

.preview-body {
  border: 1px solid #e5e7eb;
  border-radius: 0.25rem;
  padding: 1rem;
  min-height: 400px;
}

/* Dark mode handled by global html.dark styles in main.css */
html.dark .btn-preview-mode {
  border-color: #4b5563;
}

html.dark .btn-preview-mode:hover {
  background-color: #374151;
  border-color: #6b7280;
}
</style>
