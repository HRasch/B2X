<template>
  <div class="email-template-editor">
    <div class="editor-header">
      <h2>{{ isEditing ? $t('email.templates.edit') : $t('email.templates.create') }}</h2>
      <div class="actions">
        <button @click="preview" class="btn-secondary">{{ $t('email.templates.preview') }}</button>
        <button @click="testSend" class="btn-secondary">{{ $t('email.templates.test') }}</button>
        <button @click="save" :disabled="!isValid" class="btn-primary">{{ $t('ui.save') }}</button>
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
          <textarea
            id="htmlBody"
            v-model="form.htmlBody"
            required
            :placeholder="$t('email.templates.htmlBodyPlaceholder')"
            rows="20"
            class="code-editor"
          ></textarea>
        </div>
        <div class="liquid-help">
          <div>{{ $t('email.templates.liquidVariables') }}</div>
          <div class="variables-list">
            <div class="variable-item">
              <span>customer.name</span>
              <span>{{ $t('email.templates.customerName') }}</span>
              <span>{{ $t('email.templates.customerNameExample') }}</span>
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
          <button @click="showPreview = false" class="close-btn">&times;</button>
        </div>
        <div class="preview-content">
          <div class="preview-subject">
            {{ $t('email.templates.subject') }}: {{ previewData.subject }}
          </div>
          <div class="preview-body" v-html="previewData.htmlBody"></div>
        </div>
      </div>
    </div>

    <!-- Test Send Modal -->
    <div v-if="showTestSend" class="modal" @click="showTestSend = false">
      <div class="modal-content" @click.stop>
        <form @submit.prevent="sendTestEmail">
          <div class="modal-header">
            <h3>{{ $t('email.templates.testSendModalTitle') }}</h3>
            <button @click="showTestSend = false" class="close-btn">&times;</button>
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
import { ref, computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { useEmailStore } from '@/stores/email';
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
const { t } = useI18n();

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
    description: 'Customer full name',
    category: 'Customer',
    example: 'John Doe',
    type: 'string',
  },
  {
    name: 'customer.email',
    description: 'Customer email',
    category: 'Customer',
    example: 'john@example.com',
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
  }
};

const save = async () => {
  if (!isValid.value) return;

  try {
    const savedTemplate = await emailStore.saveTemplate(form.value, props.isEditing);
    emit('saved', savedTemplate);
  } catch (error) {
    console.error('Failed to save template:', error);
    // Handle error (show toast, etc.)
  }
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

.actions {
  display: flex;
  gap: 1rem;
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
</style>
