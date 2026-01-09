import { ref, computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';

export interface ThemeConfig {
  tenantId: string;
  themeId?: string;
  baseUrl?: string;
}

export interface ThemeState {
  isLoading: boolean;
  isLoaded: boolean;
  error: string | null;
  lastUpdated: Date | null;
}

const themeState = ref<ThemeState>({
  isLoading: false,
  isLoaded: false,
  error: null,
  lastUpdated: null,
});

const currentThemeConfig = ref<ThemeConfig | null>(null);

export const useTheme = () => {
  const { t } = useI18n();

  const isLoading = computed(() => themeState.value.isLoading);
  const isLoaded = computed(() => themeState.value.isLoaded);
  const error = computed(() => themeState.value.error);
  const lastUpdated = computed(() => themeState.value.lastUpdated);

  const loadTheme = async (config: ThemeConfig) => {
    if (!config.tenantId) {
      themeState.value.error = t('theme.errors.invalidConfig');
      return;
    }

    themeState.value.isLoading = true;
    themeState.value.error = null;

    try {
      const baseUrl =
        config.baseUrl || import.meta.env.VITE_API_GATEWAY_URL || 'http://localhost:8000';
      const themeUrl = `${baseUrl}/api/themes/${config.tenantId}/theme.css`;

      // Check if theme CSS is already loaded
      const existingLink = document.querySelector(
        `link[data-theme="${config.tenantId}"]`
      ) as HTMLLinkElement;

      if (existingLink) {
        // Update existing link
        existingLink.href = themeUrl;
      } else {
        // Create new link element
        const link = document.createElement('link');
        link.rel = 'stylesheet';
        link.href = themeUrl;
        link.setAttribute('data-theme', config.tenantId);
        link.setAttribute('data-theme-id', config.themeId || 'default');

        // Add to head
        document.head.appendChild(link);
      }

      themeState.value.isLoaded = true;
      themeState.value.lastUpdated = new Date();
    } catch (err) {
      console.error('Failed to load theme:', err);
      themeState.value.error = t('theme.errors.loadFailed');
    } finally {
      themeState.value.isLoading = false;
    }
  };

  const unloadTheme = (tenantId: string) => {
    const link = document.querySelector(`link[data-theme="${tenantId}"]`) as HTMLLinkElement;
    if (link) {
      link.remove();
    }
    themeState.value.isLoaded = false;
    themeState.value.lastUpdated = null;
  };

  const refreshTheme = async () => {
    if (currentThemeConfig.value) {
      await loadTheme(currentThemeConfig.value);
    }
  };

  const setThemeConfig = (config: ThemeConfig) => {
    currentThemeConfig.value = config;
  };

  // Auto-load theme on mount if config is available
  onMounted(() => {
    if (currentThemeConfig.value) {
      loadTheme(currentThemeConfig.value);
    }
  });

  return {
    // State
    isLoading,
    isLoaded,
    error,
    lastUpdated,

    // Methods
    loadTheme,
    unloadTheme,
    refreshTheme,
    setThemeConfig,
  };
};
