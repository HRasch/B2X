<template>
  <div class="cli-tools-container">
    <!-- Header -->
    <div class="cli-header">
      <div class="header-content">
        <h1>{{ $t('tools.cli.title') }}</h1>
        <p class="subtitle">{{ $t('tools.cli.subtitle') }}</p>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="loading-state">
      <div class="spinner"></div>
      <p>{{ $t('common.loading') }}</p>
    </div>

    <!-- Error State -->
    <div v-else-if="errorMessage" class="alert alert-danger">
      <i class="icon-alert-circle"></i>
      <div>
        <strong>{{ $t('common.error') }}</strong>
        <p>{{ errorMessage }}</p>
      </div>
      <button @click="errorMessage = ''" class="close-btn">×</button>
    </div>

    <!-- CLI Tools Grid -->
    <div v-else class="cli-tools-grid">
      <!-- Administration CLI Card -->
      <div class="tool-card">
        <div class="tool-header">
          <i class="icon-terminal"></i>
          <h2>Administration CLI</h2>
        </div>

        <div class="tool-content">
          <p class="tool-description">
            {{ $t('tools.cli.administration.description') }}
          </p>

          <div class="tool-info">
            <div class="info-item">
              <span class="label">{{ $t('tools.version') }}:</span>
              <span class="value">{{ administrationCliLatestVersion }}</span>
            </div>
            <div class="info-item">
              <span class="label">{{ $t('tools.supportedOS') }}:</span>
              <span class="value">
                <span class="badge">Windows</span>
                <span class="badge">Linux</span>
                <span class="badge">macOS</span>
              </span>
            </div>
            <div class="info-item">
              <span class="label">{{ $t('tools.updated') }}:</span>
              <span class="value">{{ formatDate(administrationCliUpdated) }}</span>
            </div>
          </div>
        </div>

        <div class="tool-actions">
          <!-- OS Selection -->
          <div class="os-selector">
            <label>{{ $t('tools.selectOS') }}:</label>
            <div class="os-buttons">
              <button
                v-for="os in ['win', 'linux', 'osx']"
                :key="os"
                @click="selectedAdminOs = os"
                :class="['os-btn', { active: selectedAdminOs === os }]"
              >
                {{ getOsLabel(os) }}
              </button>
            </div>
          </div>

          <!-- Version Selection -->
          <div class="version-selector">
            <label>{{ $t('tools.selectVersion') }}:</label>
            <select v-model="selectedAdminVersion" class="version-select">
              <option value="latest">Latest</option>
              <option v-for="v in administrationCliVersions" :key="v.version" :value="v.version">
                {{ v.version }} ({{ formatDate(v.releaseDate) }})
              </option>
            </select>
          </div>

          <!-- Action Buttons -->
          <div class="action-buttons">
            <button @click="downloadAdminCli" :disabled="isDownloading" class="btn btn-primary">
              <i class="icon-download"></i>
              {{ isDownloading ? $t('common.downloading') : $t('tools.download') }}
            </button>
            <button @click="showAdminInstructions" class="btn btn-secondary">
              <i class="icon-book"></i>
              {{ $t('tools.instructions') }}
            </button>
          </div>
        </div>
      </div>

      <!-- ERP Connector Card -->
      <div class="tool-card">
        <div class="tool-header">
          <i class="icon-link"></i>
          <h2>ERP Connector</h2>
        </div>

        <div class="tool-content">
          <p class="tool-description">
            {{ $t('tools.cli.erp.description') }}
          </p>

          <div class="tool-info">
            <div class="info-item">
              <span class="label">{{ $t('tools.version') }}:</span>
              <span class="value">{{ erpConnectorLatestVersion }}</span>
            </div>
            <div class="info-item">
              <span class="label">{{ $t('tools.supportedOS') }}:</span>
              <span class="value">
                <span class="badge">Windows</span>
                <span class="badge">Linux</span>
              </span>
            </div>
            <div class="info-item">
              <span class="label">{{ $t('tools.updated') }}:</span>
              <span class="value">{{ formatDate(erpConnectorUpdated) }}</span>
            </div>
          </div>
        </div>

        <div class="tool-actions">
          <!-- ERP Type Selection -->
          <div class="erp-selector">
            <label>{{ $t('tools.erp.selectType') }}:</label>
            <select v-model="selectedErpType" class="erp-select">
              <option value="">{{ $t('tools.erp.selectPlaceholder') }}</option>
              <option value="enventa">enventa Trade ERP</option>
              <option value="craft">Craft Software</option>
            </select>
          </div>

          <!-- OS Selection -->
          <div class="os-selector">
            <label>{{ $t('tools.selectOS') }}:</label>
            <div class="os-buttons">
              <button
                v-for="os in ['win', 'linux']"
                :key="os"
                @click="selectedErpOs = os"
                :class="['os-btn', { active: selectedErpOs === os }]"
              >
                {{ getOsLabel(os) }}
              </button>
            </div>
          </div>

          <!-- Version Selection -->
          <div class="version-selector">
            <label>{{ $t('tools.selectVersion') }}:</label>
            <select v-model="selectedErpVersion" class="version-select">
              <option value="latest">Latest</option>
              <option v-for="v in erpConnectorVersions" :key="v.version" :value="v.version">
                {{ v.version }} ({{ formatDate(v.releaseDate) }})
              </option>
            </select>
          </div>

          <!-- Action Buttons -->
          <div class="action-buttons">
            <button
              @click="downloadErpConnector"
              :disabled="!selectedErpType || isDownloading"
              class="btn btn-primary"
            >
              <i class="icon-download"></i>
              {{ isDownloading ? $t('common.downloading') : $t('tools.download') }}
            </button>
            <button
              @click="showErpInstructions"
              :disabled="!selectedErpType"
              class="btn btn-secondary"
            >
              <i class="icon-book"></i>
              {{ $t('tools.instructions') }}
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Instructions Modal -->
    <div v-if="showInstructionsModal" class="modal-overlay" @click="closeInstructions">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h2>{{ instructionsData?.toolType }} - {{ instructionsData?.operatingSystem }}</h2>
          <button @click="closeInstructions" class="close-btn">×</button>
        </div>

        <div class="modal-body">
          <!-- Prerequisites -->
          <div v-if="instructionsData?.prerequisites.length" class="section">
            <h3>{{ $t('tools.instructions.prerequisites') }}</h3>
            <ul>
              <li v-for="(prereq, idx) in instructionsData.prerequisites" :key="idx">
                {{ prereq }}
              </li>
            </ul>
          </div>

          <!-- Steps -->
          <div v-if="instructionsData?.steps.length" class="section">
            <h3>{{ $t('tools.instructions.steps') }}</h3>
            <ol>
              <li v-for="(step, idx) in instructionsData.steps" :key="idx">
                {{ step }}
              </li>
            </ol>
          </div>

          <!-- Configuration Template -->
          <div v-if="instructionsData?.configurationTemplate" class="section">
            <h3>{{ $t('tools.instructions.configuration') }}</h3>
            <div class="code-block">
              <pre>{{ instructionsData.configurationTemplate }}</pre>
              <button
                @click="copyToClipboard(instructionsData.configurationTemplate)"
                class="copy-btn"
              >
                <i class="icon-copy"></i>
              </button>
            </div>
          </div>

          <!-- Troubleshooting -->
          <div v-if="instructionsData?.troubleshootingTips.length" class="section">
            <h3>{{ $t('tools.instructions.troubleshooting') }}</h3>
            <ul>
              <li v-for="(tip, idx) in instructionsData.troubleshootingTips" :key="idx">
                {{ tip }}
              </li>
            </ul>
          </div>
        </div>

        <div class="modal-footer">
          <button @click="closeInstructions" class="btn btn-secondary">
            {{ $t('common.close') }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import {
  cliToolsApi,
  type CliToolInfo,
  type CliVersionInfo,
  type CliInstallationInstructions,
} from '@/services/api/cliTools';

const { t } = useI18n();

// State
const isLoading = ref(true);
const isDownloading = ref(false);
const errorMessage = ref('');
const showInstructionsModal = ref(false);

const availableTools = ref<CliToolInfo[]>([]);
const administrationCliVersions = ref<CliVersionInfo[]>([]);
const erpConnectorVersions = ref<CliVersionInfo[]>([]);
const instructionsData = ref<CliInstallationInstructions | null>(null);

// UI State
const selectedAdminOs = ref('win');
const selectedAdminVersion = ref('latest');
const selectedErpType = ref('');
const selectedErpOs = ref('win');
const selectedErpVersion = ref('latest');

// Computed properties
const administrationCliLatestVersion = () => {
  const tool = availableTools.value.find(t => t.toolType === 'administration-cli');
  return tool?.latestVersion || '1.2.0';
};

const administrationCliUpdated = () => {
  const tool = availableTools.value.find(t => t.toolType === 'administration-cli');
  return tool?.lastUpdated || new Date().toISOString();
};

const erpConnectorLatestVersion = () => {
  const tool = availableTools.value.find(t => t.toolType === 'erp-connector');
  return tool?.latestVersion || '1.1.0';
};

const erpConnectorUpdated = () => {
  const tool = availableTools.value.find(t => t.toolType === 'erp-connector');
  return tool?.lastUpdated || new Date().toISOString();
};

// Methods
const loadAvailableTools = async () => {
  try {
    isLoading.value = true;
    errorMessage.value = '';
    availableTools.value = await cliToolsApi.getAvailableTools();

    // Load versions for both tools
    administrationCliVersions.value = await cliToolsApi.getAvailableVersions('administration-cli');
    erpConnectorVersions.value = await cliToolsApi.getAvailableVersions('erp-connector');
  } catch (error) {
    console.error('Error loading CLI tools:', error);
    errorMessage.value = 'Failed to load available CLI tools. Please try again.';
  } finally {
    isLoading.value = false;
  }
};

const downloadAdminCli = async () => {
  try {
    isDownloading.value = true;
    const blob = await cliToolsApi.downloadAdministrationCli(
      selectedAdminVersion.value,
      selectedAdminOs.value
    );
    downloadBlob(blob, `b2connect-cli-${selectedAdminVersion.value}-${selectedAdminOs.value}`);
  } catch (error) {
    console.error('Error downloading Administration-CLI:', error);
    errorMessage.value = 'Failed to download Administration-CLI. Please try again.';
  } finally {
    isDownloading.value = false;
  }
};

const downloadErpConnector = async () => {
  try {
    isDownloading.value = true;
    const blob = await cliToolsApi.downloadErpConnector(
      selectedErpType.value,
      selectedErpVersion.value
    );
    downloadBlob(blob, `erp-connector-${selectedErpType.value}-${selectedErpVersion.value}`);
  } catch (error) {
    console.error('Error downloading ERP-Connector:', error);
    errorMessage.value = 'Failed to download ERP-Connector. Please try again.';
  } finally {
    isDownloading.value = false;
  }
};

const showAdminInstructions = async () => {
  try {
    instructionsData.value = await cliToolsApi.getInstallationInstructions(
      'administration-cli',
      selectedAdminOs.value
    );
    showInstructionsModal.value = true;
  } catch (error) {
    console.error('Error loading instructions:', error);
    errorMessage.value = 'Failed to load installation instructions.';
  }
};

const showErpInstructions = async () => {
  try {
    instructionsData.value = await cliToolsApi.getInstallationInstructions(
      'erp-connector',
      selectedErpOs.value
    );
    showInstructionsModal.value = true;
  } catch (error) {
    console.error('Error loading instructions:', error);
    errorMessage.value = 'Failed to load installation instructions.';
  }
};

const closeInstructions = () => {
  showInstructionsModal.value = false;
  instructionsData.value = null;
};

const formatDate = (dateString: string): string => {
  const date = new Date(dateString);
  return date.toLocaleDateString(undefined, { year: 'numeric', month: 'short', day: 'numeric' });
};

const getOsLabel = (os: string): string => {
  return { win: 'Windows', linux: 'Linux', osx: 'macOS' }[os] || os;
};

const downloadBlob = (blob: Blob, filename: string) => {
  const url = window.URL.createObjectURL(blob);
  const link = document.createElement('a');
  link.href = url;
  link.download = filename;
  link.click();
  window.URL.revokeObjectURL(url);
};

const copyToClipboard = async (text: string) => {
  try {
    await navigator.clipboard.writeText(text);
    // Show toast notification
    console.log('Copied to clipboard');
  } catch (error) {
    console.error('Failed to copy to clipboard:', error);
  }
};

// Lifecycle
onMounted(() => {
  loadAvailableTools();
});
</script>

<style scoped>
.cli-tools-container {
  padding: 2rem;
}

.cli-header {
  margin-bottom: 2rem;
}

.cli-header h1 {
  font-size: 2rem;
  font-weight: 600;
  margin-bottom: 0.5rem;
  color: var(--color-text-primary);
}

.subtitle {
  font-size: 1rem;
  color: var(--color-text-secondary);
  margin: 0;
}

.cli-tools-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
  gap: 2rem;
}

.tool-card {
  background: white;
  border: 1px solid var(--color-border);
  border-radius: 8px;
  padding: 2rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  transition: box-shadow 0.2s ease;
}

.tool-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.tool-header {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.tool-header i {
  font-size: 1.5rem;
  color: var(--color-primary);
}

.tool-header h2 {
  font-size: 1.25rem;
  font-weight: 600;
  margin: 0;
  color: var(--color-text-primary);
}

.tool-description {
  color: var(--color-text-secondary);
  margin: 0 0 1.5rem 0;
  line-height: 1.5;
}

.tool-info {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-bottom: 1.5rem;
  padding: 1rem;
  background: var(--color-background-secondary);
  border-radius: 6px;
}

.info-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.info-item .label {
  font-weight: 600;
  color: var(--color-text-secondary);
}

.info-item .value {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.badge {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  background: var(--color-primary);
  color: white;
  border-radius: 12px;
  font-size: 0.875rem;
}

.tool-actions {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.os-selector,
.version-selector,
.erp-selector {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.os-selector label,
.version-selector label,
.erp-selector label {
  font-weight: 600;
  font-size: 0.875rem;
  color: var(--color-text-secondary);
}

.os-buttons {
  display: flex;
  gap: 0.5rem;
}

.os-btn {
  flex: 1;
  padding: 0.5rem 1rem;
  border: 2px solid var(--color-border);
  background: white;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
  transition: all 0.2s ease;
}

.os-btn:hover {
  border-color: var(--color-primary);
  background: var(--color-primary-light);
}

.os-btn.active {
  background: var(--color-primary);
  color: white;
  border-color: var(--color-primary);
}

.version-select,
.erp-select {
  padding: 0.5rem;
  border: 1px solid var(--color-border);
  border-radius: 6px;
  font-size: 0.875rem;
}

.action-buttons {
  display: flex;
  gap: 0.5rem;
}

.btn {
  flex: 1;
  padding: 0.75rem 1rem;
  border: none;
  border-radius: 6px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  font-size: 0.875rem;
}

.btn-primary {
  background: var(--color-primary);
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background: var(--color-primary-dark);
}

.btn-secondary {
  background: var(--color-background-secondary);
  color: var(--color-text-primary);
  border: 1px solid var(--color-border);
}

.btn-secondary:hover:not(:disabled) {
  background: var(--color-border);
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 2rem;
}

.modal-content {
  background: white;
  border-radius: 8px;
  width: 100%;
  max-width: 600px;
  max-height: 80vh;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 1px solid var(--color-border);
}

.modal-header h2 {
  font-size: 1.25rem;
  margin: 0;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: var(--color-text-secondary);
}

.close-btn:hover {
  color: var(--color-text-primary);
}

.modal-body {
  flex: 1;
  padding: 1.5rem;
}

.section {
  margin-bottom: 2rem;
}

.section h3 {
  font-size: 1rem;
  font-weight: 600;
  margin-bottom: 1rem;
  color: var(--color-text-primary);
}

.section ul,
.section ol {
  margin: 0;
  padding-left: 1.5rem;
  color: var(--color-text-secondary);
}

.section li {
  margin-bottom: 0.5rem;
  line-height: 1.5;
}

.code-block {
  position: relative;
  background: var(--color-background-secondary);
  border: 1px solid var(--color-border);
  border-radius: 6px;
  padding: 1rem;
  overflow-x: auto;
}

.code-block pre {
  margin: 0;
  font-family: 'Courier New', monospace;
  font-size: 0.875rem;
  white-space: pre-wrap;
  word-break: break-word;
}

.copy-btn {
  position: absolute;
  top: 0.5rem;
  right: 0.5rem;
  background: var(--color-primary);
  color: white;
  border: none;
  border-radius: 4px;
  padding: 0.5rem;
  cursor: pointer;
  display: flex;
  align-items: center;
}

.copy-btn:hover {
  background: var(--color-primary-dark);
}

.modal-footer {
  padding: 1.5rem;
  border-top: 1px solid var(--color-border);
  display: flex;
  justify-content: flex-end;
}

/* Loading State */
.loading-state {
  text-align: center;
  padding: 4rem 2rem;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid var(--color-border);
  border-top: 4px solid var(--color-primary);
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 1rem;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

/* Alert */
.alert {
  display: flex;
  gap: 1rem;
  padding: 1.5rem;
  border-radius: 8px;
  margin-bottom: 2rem;
}

.alert-danger {
  background: #fee;
  color: #c33;
  border: 1px solid #fcc;
}

.alert i {
  flex-shrink: 0;
}

@media (max-width: 768px) {
  .cli-tools-grid {
    grid-template-columns: 1fr;
  }

  .os-buttons {
    flex-wrap: wrap;
  }

  .action-buttons {
    flex-direction: column;
  }

  .modal-overlay {
    padding: 1rem;
  }
}
</style>
