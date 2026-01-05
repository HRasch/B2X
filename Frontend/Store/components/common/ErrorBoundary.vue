<template>
  <div>
    <slot v-if="!hasError" />
    <div v-else class="error-boundary">
      <div class="error-boundary__content">
        <h2 class="error-boundary__title">{{ $t('errorBoundary.title') }}</h2>
        <p class="error-boundary__message">{{ $t('errorBoundary.message') }}</p>
        <div class="error-boundary__actions">
          <button class="btn btn-primary" @click="handleRetry" :disabled="retrying">
            {{ retrying ? $t('errorBoundary.retrying') : $t('errorBoundary.retry') }}
          </button>
          <button class="btn btn-outline" @click="handleReport" :disabled="reporting">
            {{ reporting ? $t('errorBoundary.reporting') : $t('errorBoundary.report') }}
          </button>
        </div>
        <details v-if="showDetails" class="error-boundary__details">
          <summary>{{ $t('errorBoundary.details') }}</summary>
          <pre class="error-boundary__stack">{{ error?.stack }}</pre>
        </details>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onErrorCaptured, watch } from 'vue';
import { toast } from 'vue3-toastify';
import { useI18n } from 'vue-i18n';

interface Props {
  showDetails?: boolean;
  enableRetry?: boolean;
  maxRetries?: number;
  onError?: (error: Error) => void;
  onRetry?: () => void;
}

const props = withDefaults(defineProps<Props>(), {
  showDetails: false,
  enableRetry: true,
  maxRetries: 3,
});

const emit = defineEmits<{
  error: [error: Error];
  retry: [attempt: number];
  report: [error: Error];
}>();

const { t } = useI18n();

const hasError = ref(false);
const error = ref<Error | null>(null);
const retryCount = ref(0);
const retrying = ref(false);
const reporting = ref(false);

const handleError = (err: Error) => {
  hasError.value = true;
  error.value = err;

  // Report error to external service
  console.error('ErrorBoundary caught error:', err);

  // Emit error event
  emit('error', err);

  // Call custom error handler
  props.onError?.(err);

  // Show toast notification
  toast.error(t('errorBoundary.toastMessage'));
};

const handleRetry = async () => {
  if (!props.enableRetry || retryCount.value >= props.maxRetries) return;

  retrying.value = true;
  retryCount.value++;

  try {
    emit('retry', retryCount.value);
    props.onRetry?.();

    // Reset error state
    hasError.value = false;
    error.value = null;
  } catch (err) {
    console.error('Retry failed:', err);
  } finally {
    retrying.value = false;
  }
};

const handleReport = async () => {
  if (!error.value) return;

  reporting.value = true;

  try {
    // Report error to monitoring service
    const reportData = {
      message: error.value.message,
      stack: error.value.stack,
      timestamp: new Date().toISOString(),
      userAgent: navigator.userAgent,
      url: window.location.href,
    };

    // In a real app, send to error reporting service
    console.log('Reporting error:', reportData);

    emit('report', error.value);
    toast.success(t('errorBoundary.reportSuccess'));
  } catch (err) {
    console.error('Failed to report error:', err);
    toast.error(t('errorBoundary.reportFailed'));
  } finally {
    reporting.value = false;
  }
};

// Capture errors from child components
onErrorCaptured((err, instance, info) => {
  handleError(err as Error);
  return false; // Prevent error from propagating
});

// Watch for route changes to reset error state
watch(
  () => window.location.href,
  () => {
    if (hasError.value) {
      hasError.value = false;
      error.value = null;
      retryCount.value = 0;
    }
  }
);
</script>

<style scoped>
.error-boundary {
  @apply flex items-center justify-center min-h-screen p-6;
}

.error-boundary__content {
  @apply text-center max-w-md;
}

.error-boundary__title {
  @apply text-2xl font-bold text-error mb-4;
}

.error-boundary__message {
  @apply text-gray-600 mb-6;
}

.error-boundary__actions {
  @apply flex gap-3 justify-center mb-4;
}

.error-boundary__details {
  @apply text-left;
}

.error-boundary__stack {
  @apply text-xs bg-gray-100 p-3 rounded overflow-auto max-h-32;
}
</style>
