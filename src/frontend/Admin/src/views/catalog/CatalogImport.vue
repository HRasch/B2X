&lt;template&gt;
  &lt;div class="catalog-import"&gt;
    &lt;PageHeader :title="$t('catalog.import.title')" :subtitle="$t('catalog.import.subtitle')"&gt;
      &lt;template #actions&gt;
        &lt;router-link to="/catalog" class="btn btn-secondary"&gt;
          &lt;span class="icon"&gt;←&lt;/span&gt; {{ $t('common.back') }}
        &lt;/router-link&gt;
      &lt;/template&gt;
    &lt;/PageHeader&gt;

    &lt;CardContainer&gt;
      &lt;!-- Import Form --&gt;
      &lt;div class="import-form"&gt;
        &lt;h3&gt;{{ $t('catalog.import.upload.title') }}&lt;/h3&gt;
        &lt;p&gt;{{ $t('catalog.import.upload.description') }}&lt;/p&gt;

        &lt;form @submit.prevent="handleImport" enctype="multipart/form-data"&gt;
          &lt;div class="form-group"&gt;
            &lt;label for="file"&gt;{{ $t('catalog.import.file.label') }} *&lt;/label&gt;
            &lt;input
              id="file"
              type="file"
              @change="handleFileChange"
              accept=".xml,.csv,.xlsx,.xls"
              required
              class="file-input"
            /&gt;
            &lt;small class="help-text"&gt;{{ $t('catalog.import.file.help') }}&lt;/small&gt;
          &lt;/div&gt;

          &lt;div class="form-group"&gt;
            &lt;label for="format"&gt;{{ $t('catalog.import.format.label') }}&lt;/label&gt;
            &lt;select id="format" v-model="importData.format" class="select"&gt;
              &lt;option value=""&gt;{{ $t('catalog.import.format.auto') }}&lt;/option&gt;
              &lt;option value="bmecat"&gt;BMEcat XML&lt;/option&gt;
              &lt;option value="icecat"&gt;icecat XML&lt;/option&gt;
              &lt;option value="csv"&gt;CSV&lt;/option&gt;
            &lt;/select&gt;
            &lt;small class="help-text"&gt;{{ $t('catalog.import.format.help') }}&lt;/small&gt;
          &lt;/div&gt;

          &lt;div class="form-group"&gt;
            &lt;label for="schema"&gt;{{ $t('catalog.import.schema.label') }}&lt;/label&gt;
            &lt;input
              id="schema"
              type="text"
              v-model="importData.customSchemaPath"
              placeholder="Optional custom XSD schema path"
              class="text-input"
            /&gt;
            &lt;small class="help-text"&gt;{{ $t('catalog.import.schema.help') }}&lt;/small&gt;
          &lt;/div&gt;

          &lt;div class="form-actions"&gt;
            &lt;button type="submit" :disabled="loading || !selectedFile" class="btn btn-primary"&gt;
              &lt;span v-if="loading" class="spinner"&gt;&lt;/span&gt;
              {{ loading ? $t('catalog.import.uploading') : $t('catalog.import.upload') }}
            &lt;/button&gt;
          &lt;/div&gt;
        &lt;/form&gt;
      &lt;/div&gt;

      &lt;!-- Alert Messages --&gt;
      &lt;div v-if="error" class="alert alert-error"&gt;
        {{ error }}
        &lt;button @click="clearError" class="close"&gt;×&lt;/button&gt;
      &lt;/div&gt;
      &lt;div v-if="successMessage" class="alert alert-success"&gt;
        {{ successMessage }}
        &lt;button @click="clearSuccess" class="close"&gt;×&lt;/button&gt;
      &lt;/div&gt;

      &lt;!-- Import History --&gt;
      &lt;div v-if="imports.length &gt; 0" class="import-history"&gt;
        &lt;h3&gt;{{ $t('catalog.import.history.title') }}&lt;/h3&gt;
        &lt;div class="history-list"&gt;
          &lt;div v-for="importItem in imports" :key="importItem.importId" class="history-item"&gt;
            &lt;div class="history-header"&gt;
              &lt;strong&gt;{{ importItem.supplierId }} - {{ importItem.catalogId }}&lt;/strong&gt;
              &lt;span :class="'status status-' + importItem.status.toLowerCase()"&gt;
                {{ importItem.status }}
              &lt;/span&gt;
            &lt;/div&gt;
            &lt;div class="history-details"&gt;
              &lt;p&gt;{{ importItem.productCount }} {{ $t('catalog.import.products') }}&lt;/p&gt;
              &lt;p&gt;{{ formatDate(importItem.importTimestamp) }}&lt;/p&gt;
              &lt;p v-if="importItem.message"&gt;{{ importItem.message }}&lt;/p&gt;
            &lt;/div&gt;
          &lt;/div&gt;
        &lt;/div&gt;
      &lt;/div&gt;
    &lt;/CardContainer&gt;
  &lt;/div&gt;
&lt;/template&gt;

&lt;script setup lang="ts"&gt;
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import PageHeader from '@/components/common/PageHeader.vue'
import CardContainer from '@/components/common/CardContainer.vue'
import { catalogApi } from '@/services/catalogApi'

interface ImportData {
  file: File | null
  format: string
  customSchemaPath: string
}

interface ImportResponse {
  importId: string
  status: string
  supplierId: string
  catalogId: string
  productCount: number
  importTimestamp: string
  message: string
}

interface ImportHistory {
  importId: string
  status: string
  supplierId: string
  catalogId: string
  productCount: number
  importTimestamp: string
  message: string
}

const router = useRouter()
const selectedFile = ref&lt;File | null&gt;(null)
const importData = ref&lt;ImportData&gt;({
  file: null,
  format: '',
  customSchemaPath: ''
})
const loading = ref(false)
const error = ref('')
const successMessage = ref('')
const imports = ref&lt;ImportHistory[]&gt;([])

const handleFileChange = (event: Event) =&gt; {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  if (file) {
    selectedFile.value = file
    importData.value.file = file
  }
}

const handleImport = async () =&gt; {
  if (!selectedFile.value) return

  loading.value = true
  error.value = ''
  successMessage.value = ''

  try {
    const formData = new FormData()
    formData.append('file', selectedFile.value)
    if (importData.value.format) {
      formData.append('format', importData.value.format)
    }
    if (importData.value.customSchemaPath) {
      formData.append('customSchemaPath', importData.value.customSchemaPath)
    }

    const response = await catalogApi.importCatalog(formData)
    const result: ImportResponse = response.data

    successMessage.value = result.message
    loadImportHistory() // Refresh history

    // Reset form
    selectedFile.value = null
    importData.value = {
      file: null,
      format: '',
      customSchemaPath: ''
    }
    const fileInput = document.getElementById('file') as HTMLInputElement
    if (fileInput) fileInput.value = ''

  } catch (err: any) {
    error.value = err.response?.data?.message || err.message || 'Import failed'
  } finally {
    loading.value = false
  }
}

const loadImportHistory = async () =&gt; {
  try {
    const response = await catalogApi.getImportHistory()
    imports.value = response.data
  } catch (err) {
    console.error('Failed to load import history:', err)
  }
}

const formatDate = (dateString: string) =&gt; {
  return new Date(dateString).toLocaleString()
}

const clearError = () =&gt; {
  error.value = ''
}

const clearSuccess = () =&gt; {
  successMessage.value = ''
}

onMounted(() =&gt; {
  loadImportHistory()
})
&lt;/script&gt;

&lt;style scoped&gt;
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
}

.file-input:focus,
.text-input:focus,
.select:focus {
  outline: none;
  border-color: var(--primary-color);
  box-shadow: 0 0 0 2px rgba(var(--primary-rgb), 0.2);
}

.help-text {
  display: block;
  margin-top: 0.25rem;
  font-size: 0.875rem;
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
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-primary {
  background: var(--primary-color);
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background: var(--primary-hover);
}

.btn-secondary {
  background: var(--bg-secondary);
  color: var(--text-primary);
  border: 1px solid var(--border-color);
}

.btn-secondary:hover {
  background: var(--bg-hover);
}

.spinner {
  width: 1rem;
  height: 1rem;
  border: 2px solid transparent;
  border-top: 2px solid currentColor;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.alert {
  padding: 1rem;
  border-radius: 0.375rem;
  margin-bottom: 1rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
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
  font-size: 1.5rem;
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
  margin-bottom: 0.5rem;
}

.status {
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.875rem;
  font-weight: 500;
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
&lt;/style&gt;</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\src\Admin\Frontend\src\views\catalog\CatalogImport.vue