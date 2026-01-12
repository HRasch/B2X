<template>
  <div v-if="isDebugEnabled" class="debug-trigger">
    <!-- Debug Panel Toggle -->
    <button
      v-if="!isPanelOpen"
      class="debug-trigger-button"
      @click="togglePanel"
      :title="$t('debug.trigger.openPanel')"
    >
      <i class="fas fa-bug"></i>
      <span class="debug-indicator" :class="{ active: isRecording }"></span>
    </button>

    <!-- Debug Panel -->
    <div v-else class="debug-panel">
      <div class="debug-panel-header">
        <h4 class="debug-panel-title">
          <i class="fas fa-bug"></i>
          {{ $t('debug.panel.title') }}
        </h4>
        <button class="debug-panel-close" @click="togglePanel">
          <i class="fas fa-times"></i>
        </button>
      </div>

      <div class="debug-panel-body">
        <!-- Session Status -->
        <div class="debug-section">
          <div class="debug-section-header">
            <i
              class="fas fa-circle"
              :class="{ 'text-green-500': isRecording, 'text-gray-400': !isRecording }"
            ></i>
            {{ isRecording ? $t('debug.panel.sessionActive') : $t('debug.panel.sessionInactive') }}
          </div>

          <div v-if="session" class="debug-session-info">
            <div class="debug-stat">
              <span class="stat-label">{{ $t('debug.panel.sessionId') }}:</span>
              <span class="stat-value">{{ session.id.substring(0, 8) }}...</span>
            </div>
            <div class="debug-stat">
              <span class="stat-label">{{ $t('debug.panel.started') }}:</span>
              <span class="stat-value">{{ formatTime(session.startTime) }}</span>
            </div>
            <div class="debug-stat">
              <span class="stat-label">{{ $t('debug.panel.environment') }}:</span>
              <span class="stat-value">{{ session.environment }}</span>
            </div>
          </div>
        </div>

        <!-- Quick Actions -->
        <div class="debug-section">
          <div class="debug-section-header">
            <i class="fas fa-bolt"></i>
            {{ $t('debug.panel.quickActions') }}
          </div>

          <div class="debug-actions">
            <button v-if="!isRecording" class="debug-action-btn primary" @click="startSession">
              <i class="fas fa-play"></i>
              {{ $t('debug.panel.startRecording') }}
            </button>

            <button v-else class="debug-action-btn secondary" @click="stopSession">
              <i class="fas fa-stop"></i>
              {{ $t('debug.panel.stopRecording') }}
            </button>

            <button class="debug-action-btn info" @click="openFeedback">
              <i class="fas fa-comment"></i>
              {{ $t('debug.panel.reportIssue') }}
            </button>

            <button class="debug-action-btn warning" @click="captureScreenshot">
              <i class="fas fa-camera"></i>
              {{ $t('debug.panel.takeScreenshot') }}
            </button>
          </div>
        </div>

        <!-- Statistics -->
        <div class="debug-section">
          <div class="debug-section-header">
            <i class="fas fa-chart-bar"></i>
            {{ $t('debug.panel.statistics') }}
          </div>

          <div class="debug-stats">
            <div class="debug-stat">
              <span class="stat-label">{{ $t('debug.panel.actions') }}:</span>
              <span class="stat-value">{{ actions.length }}</span>
            </div>
            <div class="debug-stat">
              <span class="stat-label">{{ $t('debug.panel.errors') }}:</span>
              <span class="stat-value">{{ errors.length }}</span>
            </div>
            <div class="debug-stat">
              <span class="stat-label">{{ $t('debug.panel.environment') }}:</span>
              <span class="stat-value">{{ environment }}</span>
            </div>
          </div>
        </div>

        <!-- Settings -->
        <div class="debug-section">
          <div class="debug-section-header">
            <i class="fas fa-cog"></i>
            {{ $t('debug.panel.settings') }}
          </div>

          <div class="debug-settings">
            <label class="debug-setting">
              <input type="checkbox" v-model="autoRecordClicks" @change="updateAutoRecording" />
              <span class="setting-label">{{ $t('debug.panel.autoRecordClicks') }}</span>
            </label>

            <label class="debug-setting">
              <input type="checkbox" v-model="autoRecordErrors" @change="updateAutoRecording" />
              <span class="setting-label">{{ $t('debug.panel.autoRecordErrors') }}</span>
            </label>
          </div>
        </div>
      </div>

      <div class="debug-panel-footer">
        <button class="debug-footer-btn" @click="disableDebug">
          <i class="fas fa-power-off"></i>
          {{ $t('debug.panel.disableDebug') }}
        </button>
      </div>
    </div>

    <!-- Feedback Widget -->
    <DebugFeedbackWidget v-model="showFeedback" @feedback-submitted="onFeedbackSubmitted" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { useDebugContext } from '@/composables/useDebugContext';
import DebugFeedbackWidget from '@/components/widgets/DebugFeedbackWidget.vue';

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const { t } = useI18n();
const {
  session,
  isRecording,
  actions,
  errors,
  environment,
  startSession: startDebugSession,
  stopSession: stopDebugSession,
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  recordAction,
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  captureError,
  setDebugEnabled,
} = useDebugContext();

const isPanelOpen = ref(false);
const showFeedback = ref(false);
const autoRecordClicks = ref(true);
const autoRecordErrors = ref(true);

const isDebugEnabled = computed(() => {
  if (typeof localStorage === 'undefined') return false;
  return localStorage.getItem('debug-enabled') === 'true';
});

function togglePanel() {
  isPanelOpen.value = !isPanelOpen.value;
}

async function startSession() {
  await startDebugSession();
}

function stopSession() {
  stopDebugSession();
}

function openFeedback() {
  showFeedback.value = true;
  isPanelOpen.value = false;
}

function captureScreenshot() {
  // Trigger screenshot capture
  console.log('Screenshot captured');
  // In real implementation, this would integrate with html2canvas
}

function disableDebug() {
  setDebugEnabled(false);
  localStorage.removeItem('debug-enabled');
  isPanelOpen.value = false;
  location.reload(); // Reload to disable debug mode
}

function updateAutoRecording() {
  // Update auto-recording settings
  console.log('Auto recording settings updated:', {
    clicks: autoRecordClicks.value,
    errors: autoRecordErrors.value,
  });
}

function onFeedbackSubmitted() {
  console.log('Feedback submitted successfully');
}

function formatTime(date: Date): string {
  return new Intl.DateTimeFormat('en-US', {
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
  }).format(date);
}

// Initialize settings from localStorage
onMounted(() => {
  const savedClicks = localStorage.getItem('debug-auto-clicks');
  const savedErrors = localStorage.getItem('debug-auto-errors');

  if (savedClicks !== null) {
    autoRecordClicks.value = savedClicks === 'true';
  }
  if (savedErrors !== null) {
    autoRecordErrors.value = savedErrors === 'true';
  }
});

// Save settings to localStorage when changed
watch(autoRecordClicks, value => {
  localStorage.setItem('debug-auto-clicks', value.toString());
});

watch(autoRecordErrors, value => {
  localStorage.setItem('debug-auto-errors', value.toString());
});
</script>

<style scoped>
.debug-trigger {
  position: fixed;
  bottom: 20px;
  right: 20px;
  z-index: 1000;
}

.debug-trigger-button {
  width: 56px;
  height: 56px;
  border-radius: 50%;
  background: #ef4444;
  border: none;
  color: white;
  font-size: 20px;
  cursor: pointer;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
}

.debug-trigger-button:hover {
  background: #dc2626;
  transform: scale(1.05);
}

.debug-indicator {
  position: absolute;
  top: 8px;
  right: 8px;
  width: 12px;
  height: 12px;
  border-radius: 50%;
  background: #6b7280;
  transition: background-color 0.3s;
}

.debug-indicator.active {
  background: #10b981;
  box-shadow: 0 0 6px rgba(16, 185, 129, 0.6);
}

.debug-panel {
  position: absolute;
  bottom: 70px;
  right: 0;
  width: 320px;
  max-height: 70vh;
  background: white;
  border-radius: 12px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
  overflow: hidden;
  animation: slideUp 0.3s ease-out;
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.debug-panel-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px 20px;
  border-bottom: 1px solid #e5e7eb;
  background: #f9fafb;
}

.debug-panel-title {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
  color: #111827;
  display: flex;
  align-items: center;
  gap: 8px;
}

.debug-panel-title i {
  color: #ef4444;
}

.debug-panel-close {
  background: none;
  border: none;
  font-size: 16px;
  color: #6b7280;
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  transition: all 0.2s;
}

.debug-panel-close:hover {
  background: #e5e7eb;
  color: #374151;
}

.debug-panel-body {
  padding: 16px 20px;
  overflow-y: auto;
  max-height: calc(70vh - 120px);
}

.debug-section {
  margin-bottom: 20px;
}

.debug-section-header {
  font-size: 14px;
  font-weight: 600;
  color: #374151;
  margin-bottom: 12px;
  display: flex;
  align-items: center;
  gap: 6px;
}

.debug-session-info,
.debug-stats {
  background: #f9fafb;
  border-radius: 6px;
  padding: 12px;
}

.debug-stat {
  display: flex;
  justify-content: space-between;
  font-size: 13px;
  margin-bottom: 6px;
}

.debug-stat:last-child {
  margin-bottom: 0;
}

.stat-label {
  color: #6b7280;
}

.stat-value {
  font-weight: 500;
  color: #111827;
  font-family: monospace;
  font-size: 12px;
}

.debug-actions {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.debug-action-btn {
  padding: 8px 12px;
  border: none;
  border-radius: 6px;
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  gap: 6px;
  text-align: left;
}

.debug-action-btn.primary {
  background: #10b981;
  color: white;
}

.debug-action-btn.primary:hover {
  background: #059669;
}

.debug-action-btn.secondary {
  background: #ef4444;
  color: white;
}

.debug-action-btn.secondary:hover {
  background: #dc2626;
}

.debug-action-btn.info {
  background: #3b82f6;
  color: white;
}

.debug-action-btn.info:hover {
  background: #2563eb;
}

.debug-action-btn.warning {
  background: #f59e0b;
  color: white;
}

.debug-action-btn.warning:hover {
  background: #d97706;
}

.debug-settings {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.debug-setting {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  font-size: 13px;
}

.debug-setting input[type='checkbox'] {
  width: 14px;
  height: 14px;
  accent-color: #3b82f6;
}

.setting-label {
  color: #374151;
}

.debug-panel-footer {
  padding: 12px 20px;
  border-top: 1px solid #e5e7eb;
  background: #f9fafb;
  text-align: center;
}

.debug-footer-btn {
  background: #6b7280;
  color: white;
  border: none;
  padding: 6px 12px;
  border-radius: 4px;
  font-size: 12px;
  cursor: pointer;
  transition: background-color 0.2s;
}

.debug-footer-btn:hover {
  background: #4b5563;
}

/* Responsive */
@media (max-width: 640px) {
  .debug-trigger {
    bottom: 10px;
    right: 10px;
  }

  .debug-panel {
    width: calc(100vw - 20px);
    max-width: none;
    right: -10px;
  }
}
</style>
