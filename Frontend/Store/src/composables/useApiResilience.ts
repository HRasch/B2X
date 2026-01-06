import { ref, computed, type Ref } from 'vue';
import { resilientApiClient } from '../services/resilientApiClient';
import { toast } from 'vue3-toastify';
import { useI18n } from 'vue-i18n';
import type { AxiosRequestConfig, AxiosResponse, AxiosError } from 'axios';

interface ApiState<T> {
  data: Ref<T | null>;
  loading: Ref<boolean>;
  error: Ref<string | null>;
  retryCount: Ref<number>;
}

interface UseApiResilienceOptions {
  immediate?: boolean;
  retryOnError?: boolean;
  showToastOnError?: boolean;
  customErrorHandler?: (error: AxiosError) => void;
}

export function useApiResilience<T>(
  apiCall: () => Promise<AxiosResponse<T>>,
  options: UseApiResilienceOptions = {}
) {
  const { t } = useI18n();

  const state: ApiState<T> = {
    data: ref(null),
    loading: ref(false),
    error: ref(null),
    retryCount: ref(0),
  };

  const isSuccess = computed(
    () => !state.loading.value && !state.error.value && state.data.value !== null
  );
  const isError = computed(() => !state.loading.value && state.error.value !== null);
  const isLoading = computed(() => state.loading.value);

  const execute = async (): Promise<T | null> => {
    state.loading.value = true;
    state.error.value = null;

    try {
      const response = await apiCall();
      state.data.value = response.data;
      state.retryCount.value = 0;
      return response.data;
    } catch (error: unknown) {
      const axiosError = error as AxiosError;
      state.error.value =
        (axiosError.response?.data as { message?: string })?.message ||
        axiosError.message ||
        t('api.unknownError');

      if (options.showToastOnError !== false) {
        toast.error(state.error.value);
      }

      if (options.customErrorHandler) {
        options.customErrorHandler(axiosError);
      }

      // Auto-retry logic
      if (options.retryOnError && state.retryCount.value < 3) {
        state.retryCount.value++;
        toast.info(t('api.retrying', { attempt: state.retryCount.value }));

        // Wait before retrying (exponential backoff)
        await new Promise(resolve =>
          setTimeout(resolve, 1000 * Math.pow(2, state.retryCount.value - 1))
        );

        return execute();
      }

      throw error;
    } finally {
      state.loading.value = false;
    }
  };

  const retry = () => {
    state.retryCount.value = 0;
    return execute();
  };

  const reset = () => {
    state.data.value = null;
    state.error.value = null;
    state.retryCount.value = 0;
  };

  // Execute immediately if requested
  if (options.immediate) {
    execute();
  }

  return {
    ...state,
    isSuccess,
    isError,
    isLoading,
    execute,
    retry,
    reset,
  };
}

// Specialized composables for common HTTP methods
export function useApiGet<T>(
  url: string,
  config?: AxiosRequestConfig,
  options?: UseApiResilienceOptions
) {
  return useApiResilience<T>(() => resilientApiClient.get<T>(url, config), options);
}

export function useApiPost<T>(
  url: string,
  data?: unknown,
  config?: AxiosRequestConfig,
  options?: UseApiResilienceOptions
) {
  return useApiResilience<T>(() => resilientApiClient.post<T>(url, data, config), options);
}

export function useApiPut<T>(
  url: string,
  data?: unknown,
  config?: AxiosRequestConfig,
  options?: UseApiResilienceOptions
) {
  return useApiResilience<T>(() => resilientApiClient.put<T>(url, data, config), options);
}

export function useApiDelete<T>(
  url: string,
  config?: AxiosRequestConfig,
  options?: UseApiResilienceOptions
) {
  return useApiResilience<T>(() => resilientApiClient.delete<T>(url, config), options);
}

// Health check composable
export function useApiHealth() {
  const isHealthy = ref(true);
  const checking = ref(false);

  const checkHealth = async () => {
    checking.value = true;
    try {
      isHealthy.value = await resilientApiClient.healthCheck();
    } catch {
      isHealthy.value = false;
    } finally {
      checking.value = false;
    }
  };

  return {
    isHealthy: computed(() => isHealthy.value),
    checking: computed(() => checking.value),
    checkHealth,
  };
}
