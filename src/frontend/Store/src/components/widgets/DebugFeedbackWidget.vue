<template>
  <div v-if="isVisible" class="debug-feedback-widget">
    <div class="debug-widget-overlay" @click="close"></div>
    <div class="debug-widget-panel">
      <div class="debug-widget-header">
        <h3 class="debug-widget-title">
          <i class="fas fa-bug"></i>
          {{ $t('debug.feedback.title') }}
        </h3>
        <button class="debug-widget-close" @click="close">
          <i class="fas fa-times"></i>
        </button>
      </div>

      <div class="debug-widget-body">
        <form @submit.prevent="submitFeedback" class="debug-feedback-form">
          <!-- Feedback Type -->
          <div class="form-group">
            <label class="form-label">{{ $t('debug.feedback.type') }}</label>
            <select v-model="feedback.type" class="form-select" required>
              <option value="bug">{{ $t('debug.feedback.types.bug') }}</option>
              <option value="feature">{{ $t('debug.feedback.types.feature') }}</option>
              <option value="improvement">{{ $t('debug.feedback.types.improvement') }}</option>
              <option value="other">{{ $t('debug.feedback.types.other') }}</option>
            </select>
          </div>

          <!-- Title -->
          <div class="form-group">
            <label class="form-label">{{ $t('debug.feedback.titleField') }}</label>
            <input
              v-model="feedback.title"
              type="text"
              class="form-input"
              :placeholder="$t('debug.feedback.titlePlaceholder')"
              required
              maxlength="100"
            />
          </div>

          <!-- Description -->
          <div class="form-group">
            <label class="form-label">{{ $t('debug.feedback.description') }}</label>
            <textarea
              v-model="feedback.description"
              class="form-textarea"
              :placeholder="$t('debug.feedback.descriptionPlaceholder')"
              rows="4"
              required
              maxlength="1000"
            ></textarea>
          </div>

          <!-- Rating (for bug reports) -->
          <div v-if="feedback.type === 'bug-report'" class="form-group">
            <label class="form-label">{{ $t('debug.feedback.severity') }}</label>
            <div class="rating-stars">
              <button
                v-for="star in 5"
                :key="star"
                type="button"
                class="rating-star"
                :class="{ active: feedback.rating && feedback.rating >= star }"
                @click="feedback.rating = star"
              >
                <i class="fas fa-star"></i>
              </button>
            </div>
          </div>

          <!-- Screenshot Option -->
          <div class="form-group">
            <label class="checkbox-label">
              <input v-model="feedback.includeScreenshot" type="checkbox" class="checkbox-input" />
              <span class="checkbox-text">{{ $t('debug.feedback.includeScreenshot') }}</span>
            </label>
          </div>

          <!-- Debug Info -->
          <div v-if="isRecording" class="debug-info">
            <div class="debug-info-header">
              <i class="fas fa-info-circle"></i>
              {{ $t('debug.feedback.debugInfo') }}
            </div>
            <div class="debug-info-content">
              <div class="debug-stat">
                <span class="stat-label">{{ $t('debug.feedback.actionsRecorded') }}:</span>
                <span class="stat-value">{{ actions.length }}</span>
              </div>
              <div class="debug-stat">
                <span class="stat-label">{{ $t('debug.feedback.errorsCaptured') }}:</span>
                <span class="stat-value">{{ errors.length }}</span>
              </div>
              <div class="debug-stat">
                <span class="stat-label">{{ $t('debug.feedback.sessionId') }}:</span>
                <span class="stat-value">{{ session?.id?.substring(0, 8) }}...</span>
              </div>
            </div>
          </div>

          <!-- Submit Button -->
          <div class="form-actions">
            <button type="button" class="btn-secondary" @click="close" :disabled="isSubmitting">
              {{ $t('common.cancel') }}
            </button>
            <button type="submit" class="btn-primary" :disabled="isSubmitting || !isFormValid">
              <i v-if="isSubmitting" class="fas fa-spinner fa-spin"></i>
              {{ isSubmitting ? $t('common.submitting') : $t('debug.feedback.submit') }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useDebugContext } from '@/composables/useDebugContext';

interface Props {
  modelValue: boolean;
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void;
}

const props = defineProps<Props>();
const emit = defineEmits<Emits>();

interface FeedbackForm {
  type: 'bug-report' | 'feature-request' | 'general-feedback';
  title: string;
  description: string;
  rating?: number;
  includeScreenshot: boolean;
  url: string;
  userAgent: string;
}

const {
  session,
  isRecording,
  actions,
  errors,
  submitFeedback: submitDebugFeedback,
} = useDebugContext();

const isVisible = computed({
  get: () => props.modelValue,
  set: value => emit('update:modelValue', value),
});

const isSubmitting = ref(false);

const feedback = ref<FeedbackForm>({
  type: 'bug-report',
  title: '',
  description: '',
  rating: undefined,
  includeScreenshot: true,
  url: typeof window !== 'undefined' ? window.location.href : '',
  userAgent: typeof navigator !== 'undefined' ? navigator.userAgent : '',
});

const isFormValid = computed(() => {
  return (
    feedback.value.title.trim().length > 0 &&
    feedback.value.description.trim().length > 0 &&
    feedback.value.type
  );
});

function close() {
  isVisible.value = false;
  resetForm();
}

function resetForm() {
  feedback.value = {
    type: 'bug-report',
    title: '',
    description: '',
    rating: undefined,
    includeScreenshot: true,
    url: typeof window !== 'undefined' ? window.location.href : '',
    userAgent: typeof navigator !== 'undefined' ? navigator.userAgent : '',
  };
}

async function submitFeedback() {
  if (!isFormValid.value) return;

  isSubmitting.value = true;

  try {
    const success = await submitDebugFeedback(feedback.value);

    if (success) {
      // Show success message
      console.log('Feedback submitted successfully');

      // Close widget
      close();
    } else {
      console.error('Failed to submit feedback');
    }
  } catch (error) {
    console.error('Error submitting feedback:', error);
  } finally {
    isSubmitting.value = false;
  }
}

// Watch for visibility changes to reset form
watch(
  () => props.modelValue,
  newValue => {
    if (newValue) {
      // Update URL when opening
      feedback.value.url = typeof window !== 'undefined' ? window.location.href : '';
    }
  }
);
</script>

<style scoped>
.debug-feedback-widget {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: 9999;
  display: flex;
  align-items: center;
  justify-content: center;
}

.debug-widget-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  backdrop-filter: blur(2px);
}

.debug-widget-panel {
  position: relative;
  background: white;
  border-radius: 12px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
  max-width: 500px;
  width: 90vw;
  max-height: 90vh;
  overflow: hidden;
  animation: slideIn 0.3s ease-out;
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: scale(0.9) translateY(-20px);
  }
  to {
    opacity: 1;
    transform: scale(1) translateY(0);
  }
}

.debug-widget-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 20px 24px;
  border-bottom: 1px solid #e5e7eb;
  background: #f9fafb;
}

.debug-widget-title {
  margin: 0;
  font-size: 18px;
  font-weight: 600;
  color: #111827;
  display: flex;
  align-items: center;
  gap: 8px;
}

.debug-widget-title i {
  color: #ef4444;
}

.debug-widget-close {
  background: none;
  border: none;
  font-size: 18px;
  color: #6b7280;
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  transition: all 0.2s;
}

.debug-widget-close:hover {
  background: #e5e7eb;
  color: #374151;
}

.debug-widget-body {
  padding: 24px;
  overflow-y: auto;
  max-height: calc(90vh - 80px);
}

.form-group {
  margin-bottom: 20px;
}

.form-label {
  display: block;
  font-size: 14px;
  font-weight: 500;
  color: #374151;
  margin-bottom: 6px;
}

.form-select,
.form-input {
  width: 100%;
  padding: 10px 12px;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-size: 14px;
  transition: border-color 0.2s;
}

.form-select:focus,
.form-input:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.form-textarea {
  width: 100%;
  padding: 10px 12px;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-size: 14px;
  font-family: inherit;
  resize: vertical;
  transition: border-color 0.2s;
}

.form-textarea:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.rating-stars {
  display: flex;
  gap: 4px;
}

.rating-star {
  background: none;
  border: none;
  font-size: 18px;
  color: #d1d5db;
  cursor: pointer;
  transition: color 0.2s;
}

.rating-star.active,
.rating-star:hover {
  color: #fbbf24;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  font-size: 14px;
}

.checkbox-input {
  width: 16px;
  height: 16px;
  accent-color: #3b82f6;
}

.debug-info {
  background: #f3f4f6;
  border-radius: 8px;
  padding: 16px;
  margin-bottom: 20px;
}

.debug-info-header {
  font-size: 14px;
  font-weight: 500;
  color: #374151;
  margin-bottom: 12px;
  display: flex;
  align-items: center;
  gap: 6px;
}

.debug-info-header i {
  color: #3b82f6;
}

.debug-stat {
  display: flex;
  justify-content: space-between;
  font-size: 13px;
  margin-bottom: 4px;
}

.stat-label {
  color: #6b7280;
}

.stat-value {
  font-weight: 500;
  color: #111827;
}

.form-actions {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
  padding-top: 20px;
  border-top: 1px solid #e5e7eb;
}

.btn-primary,
.btn-secondary {
  padding: 10px 20px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  border: none;
  display: flex;
  align-items: center;
  gap: 6px;
}

.btn-primary {
  background: #3b82f6;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background: #2563eb;
}

.btn-primary:disabled {
  background: #9ca3af;
  cursor: not-allowed;
}

.btn-secondary {
  background: #f3f4f6;
  color: #374151;
  border: 1px solid #d1d5db;
}

.btn-secondary:hover:not(:disabled) {
  background: #e5e7eb;
}

.btn-secondary:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Responsive */
@media (max-width: 640px) {
  .debug-widget-panel {
    width: 95vw;
    margin: 10px;
  }

  .debug-widget-header,
  .debug-widget-body {
    padding: 16px;
  }

  .form-actions {
    flex-direction: column;
  }

  .btn-primary,
  .btn-secondary {
    width: 100%;
    justify-content: center;
  }
}
</style>
