<template>
  <div class="catalog-import">
    <PageHeader :title="$t('catalog.import.title')" :subtitle="$t('catalog.import.subtitle')">
      <template #actions>
        <router-link to="/catalog" class="btn btn-secondary">
          <span class="icon">←</span> {{ $t('common.back') }}
        </router-link>
      </template>
    </PageHeader>

    <CardContainer>
      <!-- Import Form -->
      <div class="import-form">
        <h3>{{ $t('catalog.import.upload.title') }}</h3>
        <p>{{ $t('catalog.import.upload.description') }}</p>

        <form @submit.prevent="handleImport" enctype="multipart/form-data">
          <div class="form-group">
            <label for="file">{{ $t('catalog.import.file.label') }} *</label>
            <input
              id="file"
              type="file"
              @change="handleFileChange"
              accept=".xml,.csv,.xlsx,.xls"
              required
              class="file-input"
            />
            <small class="help-text">{{ $t('catalog.import.file.help') }}</small>
          </div>

          <div class="form-group">
            <label for="format">{{ $t('catalog.import.format.label') }}</label>
            <select id="format" v-model="importData.format" class="select">
              <option value="">{{ $t('catalog.import.format.auto') }}</option>
              <option value="bmecat">BMEcat XML</option>
              <option value="icecat">icecat XML</option>
              <option value="csv">CSV</option>
            </select>
            <small class="help-text">{{ $t('catalog.import.format.help') }}</small>
          </div>

          <div class="form-group">
            <label for="schema">{{ $t('catalog.import.schema.label') }}</label>
            <input
              id="schema"
              type="text"
              v-model="importData.customSchemaPath"
              placeholder="Optional custom XSD schema path"
              class="text-input"
            />
            <small class="help-text">{{ $t('catalog.import.schema.help') }}</small>
          </div>

          <div class="form-actions">
            <button type="submit" :disabled="loading || !selectedFile" class="btn btn-primary">
              <span v-if="loading" class="spinner"></span>
              {{ loading ? $t('catalog.import.uploading') : $t('catalog.import.upload') }}
            </button>
          </div>
        </form>
      </div>

      <!-- Alert Messages -->
      <div v-if="error" class="alert alert-error">
        {{ error }}
        <button @click="clearError" class="close">×</button>
      </div>
      <div v-if="successMessage" class="alert alert-success">
        {{ successMessage }}
        <button @click="clearSuccess" class="close">×</button>
      </div>

      <!-- Import History -->
      <div v-if="imports.length > 0" class="import-history">
        <h3>{{ $t('catalog.import.history.title') }}</h3>
        <div class="history-list">
          <div v-for="importItem in imports" :key="importItem.importId" class="history-item">
            <div class="history-header">
              <strong>{{ importItem.supplierId }} - {{ importItem.catalogId }}</strong>
              <span :class="'status status-' + importItem.status.toLowerCase()">
                {{ importItem.status }}
              </span>
            </div>
            <div class="history-details">
              <p>{{ importItem.productCount }} {{ $t('catalog.import.products') }}</p>
              <p>{{ formatDate(importItem.importTimestamp) }}</p>
              <p v-if="importItem.message">{{ importItem.message }}</p>
            </div>
          </div>
        </div>
      </div>
    </CardContainer>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import PageHeader from '@/components/common/PageHeader.vue';
import CardContainer from '@/components/common/CardContainer.vue';
import { catalogApi } from '@/services/catalogApi';

interface ImportData {
  file: File | null;
  format: string;
  customSchemaPath: string;
}

interface ImportResponse {
  importId: string;
  status: string;
  supplierId: string;
  catalogId: string;
  productCount: number;
  importTimestamp: string;
  message: string;
}

interface ImportHistory {
  importId: string;
  status: string;
  supplierId: string;
  catalogId: string;
  productCount: number;
  importTimestamp: string;
  message: string;
}

const router = useRouter();
const selectedFile = ref<File | null>(null);
const importData = ref<ImportData>({
  file: null,
  format: '',
  customSchemaPath: '',
});
const loading = ref(false);
const error = ref('');
const successMessage = ref('');
const imports = ref<ImportHistory[]>([]);

const handleFileChange = (event: Event) => {
  const target = event.target as HTMLInputElement;
  const file = target.files?.[0];
  if (file) {
    selectedFile.value = file;
    importData.value.file = file;
  }
};

const handleImport = async () => {
  if (!selectedFile.value) return;

  loading.value = true;
  error.value = '';
  successMessage.value = '';

  try {
    const formData = new FormData();
    formData.append('file', selectedFile.value);
    if (importData.value.format) {
      formData.append('format', importData.value.format);
    }
    if (importData.value.customSchemaPath) {
      formData.append('customSchemaPath', importData.value.customSchemaPath);
    }

    const response = await catalogApi.importCatalog(formData);
    const result: ImportResponse = response.data;

    successMessage.value = result.message;
    loadImportHistory(); // Refresh history

    // Reset form
    selectedFile.value = null;
    importData.value = {
      file: null,
      format: '',
      customSchemaPath: '',
    };
    const fileInput = document.getElementById('file') as HTMLInputElement;
    if (fileInput) fileInput.value = '';
  } catch (err: any) {
    error.value = err.response?.data?.message || err.message || 'Import failed';
  } finally {
    loading.value = false;
  }
};

const loadImportHistory = async () => {
  try {
    const response = await catalogApi.getImportHistory();
    imports.value = response.data.items;
  } catch (err) {
    console.error('Failed to load import history:', err);
  }
};

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleString();
};

const clearError = () => {
  error.value = '';
};

const clearSuccess = () => {
  successMessage.value = '';
};

onMounted(() => {
  loadImportHistory();
});
</script>

<style scoped>
.catalog-import {
  padding: 1rem;
}

.import-form {
  margin-bottom: 2rem;
}

.import-form h3 {
  margin-bottom: 0.5rem;
  color: var(--text-primary);
}

.import-form p {
  margin-bottom: 1.5rem;
  color: var(--text-secondary);
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: var(--text-primary);
}

.file-input,
.text-input,
.select {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid var(--border-color);
  border-radius: 0.375rem;
  background: var(--bg-primary);
  color: var(--text-primary);
  font-size: 0.875rem;
}

.file-input:focus,
.text-input:focus,
.select:focus {
  outline: none;
  border-color: var(--primary-color);
  box-shadow: 0 0 0 2px var(--primary-color-light);
}

.help-text {
  display: block;
  margin-top: 0.25rem;
  font-size: 0.75rem;
  color: var(--text-secondary);
}

.form-actions {
  margin-top: 2rem;
}

.btn {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-primary {
  background: var(--primary-color);
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background: var(--primary-color-dark);
}

.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-secondary {
  background: var(--bg-secondary);
  color: var(--text-primary);
  border: 1px solid var(--border-color);
}

.btn-secondary:hover {
  background: var(--bg-hover);
}

.alert {
  padding: 1rem;
  border-radius: 0.375rem;
  margin-bottom: 1rem;
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
}

.alert-error {
  background: var(--error-bg);
  color: var(--error-color);
  border: 1px solid var(--error-border);
}

.alert-success {
  background: var(--success-bg);
  color: var(--success-color);
  border: 1px solid var(--success-border);
}

.close {
  background: none;
  border: none;
  font-size: 1.25rem;
  cursor: pointer;
  color: inherit;
  opacity: 0.7;
}

.close:hover {
  opacity: 1;
}

.import-history {
  margin-top: 2rem;
}

.import-history h3 {
  margin-bottom: 1rem;
  color: var(--text-primary);
}

.history-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.history-item {
  padding: 1rem;
  border: 1px solid var(--border-color);
  border-radius: 0.375rem;
  background: var(--bg-primary);
}

.history-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.75rem;
}

.history-header strong {
  color: var(--text-primary);
}

.status {
  padding: 0.25rem 0.75rem;
  border-radius: 9999px;
  font-size: 0.75rem;
  font-weight: 500;
  text-transform: uppercase;
}

.status-completed {
  background: var(--success-bg);
  color: var(--success-color);
}

.status-failed {
  background: var(--error-bg);
  color: var(--error-color);
}

.status-processing {
  background: var(--warning-bg);
  color: var(--warning-color);
}

.status-pending {
  background: var(--info-bg);
  color: var(--info-color);
}

.history-details p {
  margin: 0.25rem 0;
  font-size: 0.875rem;
  color: var(--text-secondary);
}
</style>
