<template>
  <div class="seeding-container">
    <!-- Header -->
    <div class="seeding-header">
      <div class="header-content">
        <h1>{{ $t('tools.seeding.title') }}</h1>
        <p class="subtitle">{{ $t('tools.seeding.subtitle') }}</p>
      </div>
    </div>

    <!-- Status Card -->
    <div class="status-card">
      <div class="status-header">
        <h3>{{ $t('tools.seeding.currentStatus') }}</h3>
        <button @click="refreshStatus" class="refresh-btn" :disabled="isRefreshing">
          <i class="icon-refresh" :class="{ spinning: isRefreshing }"></i>
          {{ $t('common.refresh') }}
        </button>
      </div>

      <div v-if="statusLoading" class="loading-state">
        <div class="spinner"></div>
        <p>{{ $t('common.loading') }}</p>
      </div>

      <div v-else-if="statusError" class="alert alert-danger">
        <i class="icon-alert-circle"></i>
        <div>
          <strong>{{ $t('common.error') }}</strong>
          <p>{{ statusError }}</p>
        </div>
      </div>

      <div v-else class="status-content">
        <div class="status-item">
          <span class="label">{{ $t('tools.seeding.isSeeded') }}:</span>
          <span
            class="value"
            :class="{ seeded: seedingStatus.isSeeded, 'not-seeded': !seedingStatus.isSeeded }"
          >
            {{ seedingStatus.isSeeded ? $t('common.yes') : $t('common.no') }}
          </span>
        </div>

        <div v-if="seedingStatus.lastSeededAt" class="status-item">
          <span class="label">{{ $t('tools.seeding.lastSeededAt') }}:</span>
          <span class="value">{{ formatDate(seedingStatus.lastSeededAt) }}</span>
        </div>

        <div v-if="seedingStatus.seededWith" class="status-item">
          <span class="label">{{ $t('tools.seeding.seededWith') }}:</span>
          <span class="value">{{ seedingStatus.seededWith }}</span>
        </div>

        <div class="status-item">
          <span class="label">{{ $t('tools.seeding.tenantCount') }}:</span>
          <span class="value">{{ seedingStatus.tenantCount }}</span>
        </div>

        <div class="status-item">
          <span class="label">{{ $t('tools.seeding.userCount') }}:</span>
          <span class="value">{{ seedingStatus.userCount }}</span>
        </div>

        <div class="status-item">
          <span class="label">{{ $t('tools.seeding.productCount') }}:</span>
          <span class="value">{{ seedingStatus.productCount }}</span>
        </div>
      </div>
    </div>

    <!-- Seeding Actions -->
    <div class="seeding-actions">
      <h3>{{ $t('tools.seeding.actions') }}</h3>

      <div class="action-grid">
        <!-- Seed All -->
        <div class="action-card">
          <div class="action-header">
            <i class="icon-database"></i>
            <h4>{{ $t('tools.seeding.seedAll.title') }}</h4>
          </div>
          <p class="action-description">{{ $t('tools.seeding.seedAll.description') }}</p>
          <button @click="seedAll" class="action-btn primary" :disabled="isSeeding">
            <i class="icon-play" v-if="!isSeeding"></i>
            <i class="icon-spinner spinning" v-else></i>
            {{ isSeeding ? $t('common.processing') : $t('tools.seeding.seedAll.button') }}
          </button>
        </div>

        <!-- Seed Core -->
        <div class="action-card">
          <div class="action-header">
            <i class="icon-server"></i>
            <h4>{{ $t('tools.seeding.seedCore.title') }}</h4>
          </div>
          <p class="action-description">{{ $t('tools.seeding.seedCore.description') }}</p>
          <button @click="seedCore" class="action-btn secondary" :disabled="isSeeding">
            <i class="icon-play" v-if="!isSeeding"></i>
            <i class="icon-spinner spinning" v-else></i>
            {{ isSeeding ? $t('common.processing') : $t('tools.seeding.seedCore.button') }}
          </button>
        </div>

        <!-- Seed Catalog -->
        <div class="action-card">
          <div class="action-header">
            <i class="icon-shopping-cart"></i>
            <h4>{{ $t('tools.seeding.seedCatalog.title') }}</h4>
          </div>
          <p class="action-description">{{ $t('tools.seeding.seedCatalog.description') }}</p>
          <button @click="seedCatalog" class="action-btn secondary" :disabled="isSeeding">
            <i class="icon-play" v-if="!isSeeding"></i>
            <i class="icon-spinner spinning" v-else></i>
            {{ isSeeding ? $t('common.processing') : $t('tools.seeding.seedCatalog.button') }}
          </button>
        </div>

        <!-- Seed CMS -->
        <div class="action-card">
          <div class="action-header">
            <i class="icon-file-text"></i>
            <h4>{{ $t('tools.seeding.seedCms.title') }}</h4>
          </div>
          <p class="action-description">{{ $t('tools.seeding.seedCms.description') }}</p>
          <button @click="seedCms" class="action-btn secondary" :disabled="isSeeding">
            <i class="icon-play" v-if="!isSeeding"></i>
            <i class="icon-spinner spinning" v-else></i>
            {{ isSeeding ? $t('common.processing') : $t('tools.seeding.seedCms.button') }}
          </button>
        </div>

        <!-- Reset All -->
        <div class="action-card danger">
          <div class="action-header">
            <i class="icon-alert-triangle"></i>
            <h4>{{ $t('tools.seeding.resetAll.title') }}</h4>
          </div>
          <p class="action-description">{{ $t('tools.seeding.resetAll.description') }}</p>
          <button @click="confirmReset" class="action-btn danger" :disabled="isSeeding">
            <i class="icon-trash"></i>
            {{ $t('tools.seeding.resetAll.button') }}
          </button>
        </div>
      </div>
    </div>

    <!-- Operation Results -->
    <div v-if="lastOperation" class="operation-results">
      <h3>{{ $t('tools.seeding.lastOperation') }}</h3>
      <div
        class="result-card"
        :class="{ success: lastOperation.success, error: !lastOperation.success }"
      >
        <div class="result-header">
          <i :class="lastOperation.success ? 'icon-check-circle' : 'icon-x-circle'"></i>
          <span>{{ lastOperation.operation }}</span>
          <span class="timestamp">{{ formatDate(lastOperation.timestamp) }}</span>
        </div>

        <div v-if="lastOperation.response" class="result-content">
          <pre>{{ JSON.stringify(lastOperation.response, null, 2) }}</pre>
        </div>

        <div v-if="lastOperation.error" class="result-error">
          <strong>{{ $t('common.error') }}:</strong> {{ lastOperation.error }}
        </div>
      </div>
    </div>

    <!-- Confirm Reset Modal -->
    <div v-if="showResetConfirm" class="modal-overlay" @click="showResetConfirm = false">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>{{ $t('tools.seeding.resetAll.confirmTitle') }}</h3>
          <button @click="showResetConfirm = false" class="close-btn">×</button>
        </div>
        <div class="modal-body">
          <p>{{ $t('tools.seeding.resetAll.confirmMessage') }}</p>
          <div class="warning-box">
            <i class="icon-alert-triangle"></i>
            <strong>{{ $t('common.warning') }}:</strong>
            {{ $t('tools.seeding.resetAll.warningMessage') }}
          </div>
        </div>
        <div class="modal-footer">
          <button @click="showResetConfirm = false" class="btn secondary">
            {{ $t('common.cancel') }}
          </button>
          <button @click="resetAll" class="btn danger" :disabled="isSeeding">
            <i class="icon-trash" v-if="!isSeeding"></i>
            <i class="icon-spinner spinning" v-else></i>
            {{ isSeeding ? $t('common.processing') : $t('tools.seeding.resetAll.confirmButton') }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';

// Composables
const { t } = useI18n();

// Reactive state
const seedingStatus = ref({
  isSeeded: false,
  lastSeededAt: null as string | null,
  seededWith: null as string | null,
  tenantCount: 0,
  userCount: 0,
  productCount: 0,
});

const isRefreshing = ref(false);
const statusLoading = ref(false);
const statusError = ref('');
const isSeeding = ref(false);
const lastOperation = ref(null as any);
const showResetConfirm = ref(false);

// Methods
const refreshStatus = async () => {
  isRefreshing.value = true;
  statusLoading.value = true;
  statusError.value = '';

  try {
    const response = await fetch('http://localhost:8096/api/seeding/status');
    if (!response.ok) {
      throw new Error(`HTTP ${response.status}: ${response.statusText}`);
    }
    const data = await response.json();
    seedingStatus.value = data.status;
  } catch (error: any) {
    statusError.value = error.message;
  } finally {
    statusLoading.value = false;
    isRefreshing.value = false;
  }
};

const seedAll = async () => {
  await performSeedingOperation('seed-all', 'http://localhost:8096/api/seeding/seed-all');
};

const seedCore = async () => {
  await performSeedingOperation('seed-core', 'http://localhost:8096/api/seeding/seed-core');
};

const seedCatalog = async () => {
  await performSeedingOperation('seed-catalog', 'http://localhost:8096/api/seeding/seed-catalog');
};

const seedCms = async () => {
  await performSeedingOperation('seed-cms', 'http://localhost:8096/api/seeding/seed-cms');
};

const confirmReset = () => {
  showResetConfirm.value = true;
};

const resetAll = async () => {
  showResetConfirm.value = false;
  await performSeedingOperation('reset', 'http://localhost:8096/api/seeding/reset');
};

const performSeedingOperation = async (operation: string, url: string) => {
  isSeeding.value = true;
  const startTime = new Date();

  try {
    const response = await fetch(url, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    const data = await response.json();

    if (!response.ok) {
      throw new Error(data.error || `HTTP ${response.status}: ${response.statusText}`);
    }

    lastOperation.value = {
      operation,
      success: true,
      response: data,
      timestamp: startTime.toISOString(),
      error: null,
    };

    // Refresh status after successful operation
    await refreshStatus();
  } catch (error: any) {
    lastOperation.value = {
      operation,
      success: false,
      response: null,
      timestamp: startTime.toISOString(),
      error: error.message,
    };
  } finally {
    isSeeding.value = false;
  }
};

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleString();
};

// Lifecycle
onMounted(() => {
  refreshStatus();
});
</script>

<style scoped>
.seeding-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

.seeding-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 40px 0;
  margin-bottom: 30px;
  border-radius: 0.5rem;
}

.header-content {
  text-align: center;
}

.header-content h1 {
  margin: 0 0 10px 0;
  font-size: 2.5rem;
  font-weight: 300;
}

.subtitle {
  margin: 0;
  font-size: 1.1rem;
  opacity: 0.9;
}

.status-card,
.seeding-actions,
.operation-results {
  background: white;
  border-radius: 0.5rem;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  margin-bottom: 30px;
  padding: 25px;
}

.status-header,
.action-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.status-header h3,
.seeding-actions h3,
.operation-results h3 {
  margin: 0;
  color: #333;
}

.refresh-btn {
  background: #007bff;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 0.25rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 8px;
}

.refresh-btn:hover:not(:disabled) {
  background: #0056b3;
}

.refresh-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.status-content {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 15px;
}

.status-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 0;
  border-bottom: 1px solid #eee;
}

.status-item:last-child {
  border-bottom: none;
}

.label {
  font-weight: 500;
  color: #666;
}

.value {
  font-weight: 600;
}

.value.seeded {
  color: #28a745;
}

.value.not-seeded {
  color: #dc3545;
}

.action-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 20px;
}

.action-card {
  background: white;
  border: 1px solid #e0e0e0;
  border-radius: 0.5rem;
  padding: 20px;
  transition:
    box-shadow 0.3s,
    transform 0.3s;
}

.action-card:hover {
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
  transform: translateY(-2px);
}

.action-card.danger {
  border-color: #dc3545;
}

.action-card.danger .action-header h4 {
  color: #dc3545;
}

.action-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 15px;
}

.action-header i {
  font-size: 1.5rem;
  color: #007bff;
}

.action-card.danger .action-header i {
  color: #dc3545;
}

.action-header h4 {
  margin: 0;
  color: #333;
}

.action-description {
  color: #666;
  margin-bottom: 20px;
  line-height: 1.5;
}

.action-btn {
  width: 100%;
  padding: 12px;
  border: none;
  border-radius: 0.25rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  font-weight: 500;
  transition: background-color 0.3s;
}

.action-btn.primary {
  background: #007bff;
  color: white;
}

.action-btn.primary:hover:not(:disabled) {
  background: #0056b3;
}

.action-btn.secondary {
  background: #6c757d;
  color: white;
}

.action-btn.secondary:hover:not(:disabled) {
  background: #545b62;
}

.action-btn.danger {
  background: #dc3545;
  color: white;
}

.action-btn.danger:hover:not(:disabled) {
  background: #c82333;
}

.action-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.result-card {
  border-radius: 0.5rem;
  padding: 20px;
  margin-top: 15px;
}

.result-card.success {
  background: #d4edda;
  border: 1px solid #c3e6cb;
}

.result-card.error {
  background: #f8d7da;
  border: 1px solid #f5c6cb;
}

.result-header {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 15px;
}

.result-header i {
  font-size: 1.2rem;
}

.result-card.success .result-header i {
  color: #155724;
}

.result-card.error .result-header i {
  color: #721c24;
}

.result-header span:first-of-type {
  font-weight: 600;
}

.timestamp {
  margin-left: auto;
  color: #666;
  font-size: 0.9rem;
}

.result-content pre {
  background: rgba(255, 255, 255, 0.8);
  padding: 15px;
  border-radius: 0.25rem;
  overflow-x: auto;
  font-size: 0.9rem;
}

.result-error {
  background: rgba(255, 255, 255, 0.8);
  padding: 15px;
  border-radius: 0.25rem;
  color: #721c24;
}

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
}

.modal-content {
  background: white;
  border-radius: 0.5rem;
  max-width: 500px;
  width: 90%;
  max-height: 80vh;
  overflow-y: auto;
}

.modal-header {
  padding: 20px;
  border-bottom: 1px solid #e0e0e0;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal-header h3 {
  margin: 0;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #666;
}

.modal-body {
  padding: 20px;
}

.warning-box {
  background: #fff3cd;
  border: 1px solid #ffeaa7;
  border-radius: 0.25rem;
  padding: 15px;
  margin-top: 15px;
  display: flex;
  align-items: flex-start;
  gap: 10px;
}

.warning-box i {
  color: #856404;
  font-size: 1.2rem;
  margin-top: 2px;
}

.modal-footer {
  padding: 20px;
  border-top: 1px solid #e0e0e0;
  display: flex;
  justify-content: flex-end;
  gap: 10px;
}

.btn {
  padding: 10px 20px;
  border: none;
  border-radius: 0.25rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 500;
}

.btn.secondary {
  background: #6c757d;
  color: white;
}

.btn.secondary:hover {
  background: #545b62;
}

.btn.danger {
  background: #dc3545;
  color: white;
}

.btn.danger:hover:not(:disabled) {
  background: #c82333;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.loading-state {
  text-align: center;
  padding: 40px;
}

.spinner {
  border: 4px solid #f3f3f3;
  border-top: 4px solid #007bff;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  animation: spin 1s linear infinite;
  margin: 0 auto 20px;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}

.spinning {
  animation: spin 1s linear infinite;
}

.alert {
  padding: 15px;
  border-radius: 0.25rem;
  display: flex;
  align-items: flex-start;
  gap: 10px;
  margin-bottom: 20px;
}

.alert-danger {
  background: #f8d7da;
  border: 1px solid #f5c6cb;
  color: #721c24;
}

.alert i {
  font-size: 1.2rem;
  margin-top: 2px;
}

.badge {
  background: #007bff;
  color: white;
  padding: 2px 8px;
  border-radius: 0.25rem;
  font-size: 0.8rem;
  margin-right: 5px;
}

/* Responsive */
@media (max-width: 768px) {
  .seeding-container {
    padding: 10px;
  }

  .header-content h1 {
    font-size: 2rem;
  }

  .action-grid {
    grid-template-columns: 1fr;
  }

  .status-content {
    grid-template-columns: 1fr;
  }

  .status-header,
  .action-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 10px;
  }
}
</style>
