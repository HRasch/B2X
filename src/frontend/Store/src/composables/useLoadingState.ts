import { ref, computed, type Ref } from 'vue';

export interface LoadingState {
  loading: Ref<boolean>;
  error: Ref<string | null>;
  success: Ref<boolean>;
  progress: Ref<number>;
}

export interface LoadingStateActions {
  startLoading: () => void;
  stopLoading: () => void;
  setError: (error: string | null) => void;
  setSuccess: (success: boolean) => void;
  setProgress: (progress: number) => void;
  reset: () => void;
}

export function useLoadingState(initialLoading = false): LoadingState & LoadingStateActions {
  const loading = ref(initialLoading);
  const error = ref<string | null>(null);
  const success = ref(false);
  const progress = ref(0);

  const startLoading = () => {
    loading.value = true;
    error.value = null;
    success.value = false;
    progress.value = 0;
  };

  const stopLoading = () => {
    loading.value = false;
  };

  const setError = (err: string | null) => {
    error.value = err;
    loading.value = false;
    success.value = false;
  };

  const setSuccess = (val: boolean) => {
    success.value = val;
    loading.value = false;
    error.value = null;
  };

  const setProgress = (prog: number) => {
    progress.value = Math.max(0, Math.min(100, prog));
  };

  const reset = () => {
    loading.value = false;
    error.value = null;
    success.value = false;
    progress.value = 0;
  };

  return {
    loading: computed(() => loading.value),
    error: computed(() => error.value),
    success: computed(() => success.value),
    progress: computed(() => progress.value),
    startLoading,
    stopLoading,
    setError,
    setSuccess,
    setProgress,
    reset,
  };
}

// Specialized loading state for async operations
export function useAsyncLoadingState() {
  const loadingState = useLoadingState();

  const executeAsync = async <R>(
    operation: () => Promise<R>,
    options: {
      onSuccess?: (result: R) => void;
      onError?: (error: Error) => void;
      showProgress?: boolean;
    } = {}
  ): Promise<R | null> => {
    loadingState.startLoading();

    try {
      if (options.showProgress) {
        loadingState.setProgress(50);
      }

      const result = await operation();

      if (options.showProgress) {
        loadingState.setProgress(100);
      }

      loadingState.setSuccess(true);
      options.onSuccess?.(result);

      return result;
    } catch (err: unknown) {
      const error = err as { response?: { data?: { message?: string } }; message?: string };
      const errorMessage =
        error.response?.data?.message || error.message || 'An unexpected error occurred';

      loadingState.setError(errorMessage);
      options.onError?.(err as Error);

      return null;
    }
  };

  return {
    ...loadingState,
    executeAsync,
  };
}

// Loading state mixin for components (Vue 3 compatible)
// Note: Prefer using the useLoadingState composable instead of this mixin
interface LoadingStateMixinData {
  loading: boolean;
  error: string | null;
  success: boolean;
  progress: number;
}

export const loadingStateMixin = {
  data(): LoadingStateMixinData {
    return {
      loading: false,
      error: null,
      success: false,
      progress: 0,
    };
  },
  computed: {
    isLoading(this: LoadingStateMixinData): boolean {
      return this.loading;
    },
    hasError(this: LoadingStateMixinData): boolean {
      return this.error !== null;
    },
    isSuccess(this: LoadingStateMixinData): boolean {
      return this.success;
    },
  },
  methods: {
    startLoading(this: LoadingStateMixinData) {
      this.loading = true;
      this.error = null;
      this.success = false;
      this.progress = 0;
    },
    stopLoading(this: LoadingStateMixinData) {
      this.loading = false;
    },
    setError(this: LoadingStateMixinData, error: string | null) {
      this.error = error;
      this.loading = false;
      this.success = false;
    },
    setSuccess(this: LoadingStateMixinData, success: boolean) {
      this.success = success;
      this.loading = false;
      this.error = null;
    },
    setProgress(this: LoadingStateMixinData, progress: number) {
      this.progress = Math.max(0, Math.min(100, progress));
    },
    resetLoadingState(this: LoadingStateMixinData) {
      this.loading = false;
      this.error = null;
      this.success = false;
      this.progress = 0;
    },
  },
};
