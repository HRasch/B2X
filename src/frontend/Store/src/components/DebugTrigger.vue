<template>
  <div class="debug-trigger">
    <!-- Debug Mode Indicator -->
    <div v-if="isDebugMode" class="debug-indicator" @click="toggleDebugPanel">
      <div class="debug-icon">🐛</div>
      <div class="debug-stats">
        <span class="stat">{{ actions?.length || 0 }}</span>
        <span class="stat error">{{ errors?.length || 0 }}</span>
      </div>
    </div>

    <!-- Debug Panel -->
    <div v-if="showPanel" class="debug-panel">
      <div class="panel-header">
        <h3>{{ $t('debug.title') }}</h3>
        <button @click="toggleDebugPanel" class="close-btn">×</button>
      </div>

      <div class="panel-content">
        <!-- Session Controls -->
        <div class="section">
          <h4>{{ $t('debug.session.title') }}</h4>
          <div class="controls">
            <button v-if="!session" @click="startSession" class="btn primary" :disabled="isLoading">
              {{ $t('debug.session.start') }}
            </button>
            <button v-else @click="stopSession" class="btn danger" :disabled="isLoading">
              {{ $t('debug.session.stop') }}
            </button>
            <button @click="openFeedback" class="btn secondary" :disabled="!session">
              {{ $t('debug.feedback.open') }}
            </button>
          </div>
        </div>

        <!-- Session Info -->
        <div v-if="session" class="section">
          <h4>{{ $t('debug.session.info') }}</h4>
          <div class="session-details">
            <p>
              <strong>{{ $t('debug.session.id') }}:</strong> {{ session.id }}
            </p>
            <p>
              <strong>{{ $t('debug.session.started') }}:</strong>
              {{ formatDate(session.startTime) }}
            </p>
            <p>
              <strong>{{ $t('debug.session.duration') }}:</strong> {{ sessionDuration }}
            </p>
          </div>
        </div>

        <!-- Statistics -->
        <div v-if="session" class="section">
          <h4>{{ $t('debug.stats.title') }}</h4>
          <div class="stats-grid">
            <div class="stat-item">
              <span class="label">{{ $t('debug.stats.actions') }}</span>
              <span class="value">{{ actions?.length || 0 }}</span>
            </div>
            <div class="stat-item">
              <span class="label">{{ $t('debug.stats.errors') }}</span>
              <span class="value error">{{ errors?.length || 0 }}</span>
            </div>
            <div class="stat-item">
              <span class="label">{{ $t('debug.stats.feedbacks') }}</span>
              <span class="value">{{ 0 }}</span>
            </div>
          </div>
        </div>

        <!-- Quick Actions -->
        <div class="section">
          <h4>{{ $t('debug.actions.title') }}</h4>
          <div class="quick-actions">
            <button @click="recordManualAction" class="btn small">
              {{ $t('debug.actions.record') }}
            </button>
            <button @click="captureManualError" class="btn small danger">
              {{ $t('debug.actions.error') }}
            </button>
          </div>
        </div>

        <!-- Settings -->
        <div class="section">
          <h4>{{ $t('debug.settings.title') }}</h4>
          <div class="settings">
            <label class="setting">
              <input type="checkbox" v-model="autoRecord" @change="updateSettings" />
              {{ $t('debug.settings.autoRecord') }}
            </label>
            <label class="setting">
              <input type="checkbox" v-model="captureErrors" @change="updateSettings" />
              {{ $t('debug.settings.captureErrors') }}
            </label>
            <label class="setting">
              <input type="checkbox" v-model="enableSignalR" @change="updateSettings" />
              {{ $t('debug.settings.signalr') }}
            </label>
          </div>
        </div>
      </div>
    </div>

    <!-- Feedback Widget Overlay -->
    <DebugFeedbackWidget v-if="showFeedback" @close="closeFeedback" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { useDebugContext } from '~/composables/useDebugContext';
import DebugFeedbackWidget from './DebugFeedbackWidget.vue';

const {
  session,
  isEnabled: isDebugMode,
  isRecording: isLoading,
  actions,
  errors,
  startSession,
  stopSession,
  recordAction,
  captureError,
} = useDebugContext();

const showPanel = ref(false);
const showFeedback = ref(false);
const autoRecord = ref(true);
const captureErrors = ref(true);
const enableSignalR = ref(true);

const sessionDuration = computed(() => {
  if (!session?.startTime) return '0:00';

  const now = new Date();
  const start = new Date(session.startTime);
  const diff = Math.floor((now.getTime() - start.getTime()) / 1000);

  const minutes = Math.floor(diff / 60);
  const seconds = diff % 60;

  return `${minutes}:${seconds.toString().padStart(2, '0')}`;
});

const toggleDebugPanel = () => {
  showPanel.value = !showPanel.value;
};

const openFeedback = () => {
  showFeedback.value = true;
};

const closeFeedback = () => {
  showFeedback.value = false;
};

const recordManualAction = () => {
  recordAction({
    type: 'custom',
    target: 'manual-action',
    timestamp: Date.now(),
    data: { manual: true },
  });
};

const captureManualError = () => {
  captureError(new Error('Manual error capture'), { manual: true });
};

const formatDate = (date: Date) => {
  return new Intl.DateTimeFormat('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
  }).format(date);
};

const updateSettings = () => {
  // Update debug settings based on current values
  // This could save to localStorage or send to server
  localStorage.setItem('debug-auto-record', autoRecord.value.toString());
  localStorage.setItem('debug-capture-errors', captureErrors.value.toString());
  localStorage.setItem('debug-enable-signalr', enableSignalR.value.toString());
};

onMounted(() => {
  // Initialize settings from localStorage
  autoRecord.value = localStorage.getItem('debug-auto-record') !== 'false';
  captureErrors.value = localStorage.getItem('debug-capture-errors') !== 'false';
  enableSignalR.value = localStorage.getItem('debug-enable-signalr') !== 'false';

  // Keyboard shortcut to toggle debug panel
  const handleKeydown = (event: KeyboardEvent) => {
    if (event.ctrlKey && event.shiftKey && event.key === 'D') {
      event.preventDefault();
      if (isDebugMode) {
        toggleDebugPanel();
      }
    }
  };

  window.addEventListener('keydown', handleKeydown);

  onUnmounted(() => {
    window.removeEventListener('keydown', handleKeydown);
  });
});
</script>

<style scoped>
.debug-trigger {
  position: fixed;
  bottom: 20px;
  right: 20px;
  z-index: 9999;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
}

.debug-indicator {
  width: 60px;
  height: 60px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 50%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  transition: all 0.3s ease;
  color: white;
}

.debug-indicator:hover {
  transform: scale(1.05);
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.25);
}

.debug-icon {
  font-size: 20px;
  margin-bottom: 2px;
}

.debug-stats {
  display: flex;
  gap: 4px;
  font-size: 10px;
  font-weight: bold;
}

.stat {
  background: rgba(255, 255, 255, 0.2);
  padding: 1px 4px;
  border-radius: 8px;
  min-width: 12px;
  text-align: center;
}

.stat.error {
  background: rgba(239, 68, 68, 0.8);
}

.debug-panel {
  position: absolute;
  bottom: 80px;
  right: 0;
  width: 350px;
  max-height: 600px;
  background: white;
  border-radius: 12px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
  border: 1px solid #e5e7eb;
  overflow: hidden;
}

.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 20px;
  background: #f9fafb;
  border-bottom: 1px solid #e5e7eb;
}

.panel-header h3 {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
  color: #111827;
}

.close-btn {
  background: none;
  border: none;
  font-size: 24px;
  cursor: pointer;
  color: #6b7280;
  padding: 0;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.close-btn:hover {
  color: #374151;
}

.panel-content {
  padding: 16px 20px;
  max-height: 500px;
  overflow-y: auto;
}

.section {
  margin-bottom: 20px;
}

.section h4 {
  margin: 0 0 12px 0;
  font-size: 14px;
  font-weight: 600;
  color: #374151;
}

.controls {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}

.btn {
  padding: 8px 16px;
  border: none;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn.primary {
  background: #3b82f6;
  color: white;
}

.btn.primary:hover:not(:disabled) {
  background: #2563eb;
}

.btn.danger {
  background: #ef4444;
  color: white;
}

.btn.danger:hover:not(:disabled) {
  background: #dc2626;
}

.btn.secondary {
  background: #6b7280;
  color: white;
}

.btn.secondary:hover:not(:disabled) {
  background: #4b5563;
}

.btn.small {
  padding: 6px 12px;
  font-size: 12px;
}

.btn.warning {
  background: #f59e0b;
  color: white;
}

.btn.warning:hover:not(:disabled) {
  background: #d97706;
}

.session-details p {
  margin: 4px 0;
  font-size: 13px;
  color: #6b7280;
}

.stats-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
}

.stat-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 12px;
  background: #f9fafb;
  border-radius: 6px;
  border: 1px solid #e5e7eb;
}

.stat-item .label {
  font-size: 12px;
  color: #6b7280;
}

.stat-item .value {
  font-size: 14px;
  font-weight: 600;
  color: #111827;
}

.stat-item .value.error {
  color: #ef4444;
}

.stat-item .value.connected {
  color: #10b981;
}

.stat-item .value.disconnected {
  color: #ef4444;
}

.quick-actions {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}

.settings {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.setting {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
  color: #374151;
  cursor: pointer;
}

.setting input[type='checkbox'] {
  width: 16px;
  height: 16px;
  accent-color: #3b82f6;
}

/* Responsive adjustments */
@media (max-width: 480px) {
  .debug-panel {
    width: calc(100vw - 40px);
    max-width: 350px;
    right: -10px;
  }

  .debug-indicator {
    width: 50px;
    height: 50px;
  }

  .debug-icon {
    font-size: 16px;
  }

  .debug-stats {
    font-size: 9px;
  }
}
</style>
