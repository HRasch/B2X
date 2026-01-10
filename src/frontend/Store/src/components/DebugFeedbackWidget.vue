<template>
  <div class="debug-feedback-overlay" @click.self="handleClose">
    <div class="feedback-widget">
      <div class="widget-header">
        <h3>{{ $t('debug.feedback.title') }}</h3>
        <button @click="handleClose" class="close-btn">×</button>
      </div>

      <div class="widget-content">
        <form @submit.prevent="submitFeedback" class="feedback-form">
          <!-- Feedback Type -->
          <div class="form-group">
            <label for="feedback-type">{{ $t('debug.feedback.type') }}</label>
            <select id="feedback-type" v-model="feedback.type" required class="form-select">
              <option value="bug-report">{{ $t('debug.feedback.types.bug') }}</option>
              <option value="feature-request">{{ $t('debug.feedback.types.feature') }}</option>
              <option value="general-feedback">{{ $t('debug.feedback.types.other') }}</option>
            </select>
          </div>

          <!-- Title -->
          <div class="form-group">
            <label for="feedback-title">{{ $t('debug.feedback.titleField') }}</label>
            <input
              id="feedback-title"
              type="text"
              v-model="feedback.title"
              required
              :placeholder="$t('debug.feedback.titlePlaceholder')"
              class="form-input"
              maxlength="100"
            />
          </div>

          <!-- Description -->
          <div class="form-group">
            <label for="feedback-description">{{ $t('debug.feedback.description') }}</label>
            <textarea
              id="feedback-description"
              v-model="feedback.description"
              required
              :placeholder="$t('debug.feedback.descriptionPlaceholder')"
              class="form-textarea"
              rows="4"
              maxlength="1000"
            ></textarea>
          </div>

          <!-- Rating -->
          <div class="form-group">
            <label>{{ $t('debug.feedback.rating') }}</label>
            <div class="rating-stars">
              <button
                type="button"
                v-for="star in 5"
                :key="star"
                @click="feedback.rating = star"
                class="star-btn"
                :class="{ active: star <= feedback.rating }"
              >
                ★
              </button>
            </div>
          </div>

          <!-- Screenshot -->
          <div class="form-group">
            <label>{{ $t('debug.feedback.screenshot') }}</label>
            <div class="screenshot-section">
              <button
                type="button"
                @click="captureScreenshot"
                class="btn secondary small"
                :disabled="isCapturingScreenshot"
              >
                {{
                  isCapturingScreenshot
                    ? $t('debug.feedback.capturing')
                    : $t('debug.feedback.captureScreenshot')
                }}
              </button>
              <div v-if="feedback.screenshot" class="screenshot-preview">
                <img :src="feedback.screenshot" alt="Screenshot" class="screenshot-img" />
                <button type="button" @click="removeScreenshot" class="remove-screenshot">×</button>
              </div>
            </div>
          </div>

          <!-- Debug Info -->
          <div class="form-group">
            <label>{{ $t('debug.feedback.includeDebugInfo') }}</label>
            <div class="debug-info-toggle">
              <label class="toggle">
                <input type="checkbox" v-model="includeDebugInfo" @change="updateDebugInfo" />
                <span class="toggle-slider"></span>
                {{ $t('debug.feedback.attachSessionData') }}
              </label>
            </div>
            <div v-if="includeDebugInfo && debugInfo" class="debug-info-preview">
              <details>
                <summary>{{ $t('debug.feedback.debugDataSummary') }}</summary>
                <pre class="debug-json">{{ JSON.stringify(debugInfo, null, 2) }}</pre>
              </details>
            </div>
          </div>

          <!-- Submit Button -->
          <div class="form-actions">
            <button type="button" @click="handleClose" class="btn secondary">
              {{ $t('common.cancel') }}
            </button>
            <button type="submit" class="btn primary" :disabled="isSubmitting || !isFormValid">
              {{ isSubmitting ? $t('debug.feedback.submitting') : $t('debug.feedback.submit') }}
            </button>
          </div>
        </form>

        <!-- Success Message -->
        <div v-if="submitSuccess" class="success-message">
          <div class="success-icon">✅</div>
          <h4>{{ $t('debug.feedback.successTitle') }}</h4>
          <p>{{ $t('debug.feedback.successMessage') }}</p>
          <button @click="handleClose" class="btn primary small">
            {{ $t('common.close') }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useDebugContext } from '~/composables/useDebugContext';
import type { FeedbackInput } from '~/stores/debug';
import html2canvas from 'html2canvas';

interface Props {
  onClose?: () => void;
}

const props = defineProps<Props>();

const emit = defineEmits<{
  close: [];
}>();

const {
  session,
  actions,
  errors,
  feedbacks,
  submitFeedback: submitDebugFeedback,
} = useDebugContext();

const feedback = ref({
  type: 'bug-report' as 'bug-report' | 'feature-request' | 'general-feedback',
  title: '',
  description: '',
  rating: 3,
  screenshot: null as string | null,
});

const includeDebugInfo = ref(true);
const isSubmitting = ref(false);
const isCapturingScreenshot = ref(false);
const submitSuccess = ref(false);
interface DebugInfo {
  sessionId: string;
  startedAt: Date;
  actionsCount: number;
  errorsCount: number;
  feedbacksCount: number;
  userAgent: string;
  url: string;
  timestamp: string;
}

const debugInfo = ref<DebugInfo | null>(null);

const isFormValid = computed(() => {
  return (
    feedback.value.title.trim().length > 0 &&
    feedback.value.description.trim().length > 0 &&
    feedback.value.rating >= 1 &&
    feedback.value.rating <= 5
  );
});

const updateDebugInfo = () => {
  if (includeDebugInfo.value && session) {
    debugInfo.value = {
      sessionId: session.id,
      startedAt: session.startTime,
      actionsCount: actions.length,
      errorsCount: errors.length,
      feedbacksCount: feedbacks.length,
      userAgent: navigator.userAgent,
      url: window.location.href,
      timestamp: new Date().toISOString(),
    };
  } else {
    debugInfo.value = null;
  }
};

const captureScreenshot = async () => {
  try {
    isCapturingScreenshot.value = true;
    const canvas = await html2canvas(document.body, {
      useCORS: true,
      allowTaint: false,
      scale: 1,
      width: window.innerWidth,
      height: window.innerHeight,
    });
    const screenshot = canvas.toDataURL('image/png');
    feedback.value.screenshot = screenshot;
  } catch (error) {
    console.error('Failed to capture screenshot:', error);
  } finally {
    isCapturingScreenshot.value = false;
  }
};

const removeScreenshot = () => {
  feedback.value.screenshot = null;
};

const submitFeedback = async () => {
  if (!isFormValid.value) return;

  try {
    isSubmitting.value = true;

    const feedbackData: FeedbackInput = {
      type: feedback.value.type,
      title: feedback.value.title,
      description: feedback.value.description,
      rating: feedback.value.rating,
      includeScreenshot: !!feedback.value.screenshot,
      url: typeof window !== 'undefined' ? window.location.href : '',
      userAgent: typeof navigator !== 'undefined' ? navigator.userAgent : '',
    };

    await submitDebugFeedback(feedbackData);
    submitSuccess.value = true;

    // Reset form after successful submission
    setTimeout(() => {
      feedback.value = {
        type: 'bug-report',
        title: '',
        description: '',
        rating: 3,
        screenshot: null,
      };
      includeDebugInfo.value = true;
      debugInfo.value = null;
    }, 2000);
  } catch (error) {
    console.error('Failed to submit feedback:', error);
    // Could show error message here
  } finally {
    isSubmitting.value = false;
  }
};

const handleClose = () => {
  if (!isSubmitting.value) {
    emit('close');
    props.onClose?.();
  }
};

// Initialize debug info when component mounts
watch([includeDebugInfo, session], updateDebugInfo, { immediate: true });
</script>

<style scoped>
.debug-feedback-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 10000;
  padding: 20px;
}

.feedback-widget {
  background: white;
  border-radius: 12px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
  max-width: 500px;
  width: 100%;
  max-height: 90vh;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

.widget-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 24px;
  background: #f9fafb;
  border-bottom: 1px solid #e5e7eb;
}

.widget-header h3 {
  margin: 0;
  font-size: 18px;
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

.widget-content {
  padding: 24px;
  overflow-y: auto;
  flex: 1;
}

.feedback-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-size: 14px;
  font-weight: 600;
  color: #374151;
}

.form-select,
.form-input,
.form-textarea {
  padding: 10px 12px;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-size: 14px;
  transition: border-color 0.2s ease;
}

.form-select:focus,
.form-input:focus,
.form-textarea:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.form-textarea {
  resize: vertical;
  min-height: 80px;
}

.rating-stars {
  display: flex;
  gap: 4px;
}

.star-btn {
  background: none;
  border: none;
  font-size: 24px;
  cursor: pointer;
  color: #d1d5db;
  transition: color 0.2s ease;
  padding: 0;
}

.star-btn.active {
  color: #fbbf24;
}

.star-btn:hover {
  color: #f59e0b;
}

.screenshot-section {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.screenshot-preview {
  position: relative;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  overflow: hidden;
  max-width: 200px;
}

.screenshot-img {
  width: 100%;
  height: auto;
  display: block;
}

.remove-screenshot {
  position: absolute;
  top: 8px;
  right: 8px;
  background: rgba(239, 68, 68, 0.9);
  color: white;
  border: none;
  border-radius: 50%;
  width: 24px;
  height: 24px;
  cursor: pointer;
  font-size: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.debug-info-toggle {
  margin-top: 8px;
}

.toggle {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  font-size: 13px;
  color: #6b7280;
}

.toggle input[type='checkbox'] {
  display: none;
}

.toggle-slider {
  position: relative;
  width: 36px;
  height: 20px;
  background: #d1d5db;
  border-radius: 10px;
  transition: background-color 0.2s ease;
}

.toggle-slider::before {
  content: '';
  position: absolute;
  top: 2px;
  left: 2px;
  width: 16px;
  height: 16px;
  background: white;
  border-radius: 50%;
  transition: transform 0.2s ease;
}

.toggle input[type='checkbox']:checked + .toggle-slider {
  background: #3b82f6;
}

.toggle input[type='checkbox']:checked + .toggle-slider::before {
  transform: translateX(16px);
}

.debug-info-preview {
  margin-top: 12px;
  padding: 12px;
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
}

.debug-info-preview summary {
  cursor: pointer;
  font-weight: 600;
  color: #374151;
  margin-bottom: 8px;
}

.debug-json {
  font-size: 12px;
  color: #6b7280;
  white-space: pre-wrap;
  word-break: break-all;
  max-height: 200px;
  overflow-y: auto;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  margin-top: 8px;
}

.btn {
  padding: 10px 20px;
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

.btn.secondary {
  background: #6b7280;
  color: white;
}

.btn.secondary:hover:not(:disabled) {
  background: #4b5563;
}

.btn.small {
  padding: 8px 16px;
  font-size: 13px;
}

.success-message {
  text-align: center;
  padding: 40px 20px;
}

.success-icon {
  font-size: 48px;
  margin-bottom: 16px;
}

.success-message h4 {
  margin: 0 0 12px 0;
  font-size: 18px;
  color: #111827;
}

.success-message p {
  margin: 0 0 24px 0;
  color: #6b7280;
}

/* Responsive adjustments */
@media (max-width: 640px) {
  .debug-feedback-overlay {
    padding: 10px;
  }

  .feedback-widget {
    max-height: 95vh;
  }

  .widget-header,
  .widget-content {
    padding: 16px 20px;
  }

  .form-actions {
    flex-direction: column;
  }

  .btn {
    width: 100%;
  }
}
</style>
