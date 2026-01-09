import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { api } from '~/services/api';
import localizationApi from '~/services/localizationApi';
import { useAuthStore } from '~/stores/auth';

export interface TenantConfig {
  tenantId: string;
  isPublicStore: boolean;
  showPricesToAnonymous: boolean;
  allowGuestCheckout: boolean;
  supportedLanguages: string[];
  features: {
    multiLanguage: boolean;
  };
}

export const useTenantStore = defineStore('tenant', () => {
  const currentTenant = ref<TenantConfig | null>(null);
  const supportedLanguages = ref<string[]>(['en', 'de']); // Default fallback
  const isLoading = ref(false);
  const error = ref<string | null>(null);

  const isMultiLanguageEnabled = computed(
    () => currentTenant.value?.features.multiLanguage ?? false
  );

  const tenantLanguages = computed(() => {
    if (currentTenant.value?.supportedLanguages) {
      return currentTenant.value.supportedLanguages;
    }
    return supportedLanguages.value;
  });

  const defaultLanguage = computed(() => tenantLanguages.value[0] ?? 'en');

  const loadTenantConfig = async (tenantId?: string) => {
    if (!tenantId) {
      // Try to get from auth store or URL
      const authStore = useAuthStore();
      tenantId = authStore.tenantId || getTenantFromUrl() || undefined;
    }

    if (!tenantId) {
      console.warn('No tenant ID available, using defaults');
      return;
    }

    isLoading.value = true;
    error.value = null;

    try {
      const response = await api.get<TenantConfig>('/api/tenant/config');
      currentTenant.value = response.data;

      // Load supported languages from tenant config
      if (response.data.supportedLanguages) {
        supportedLanguages.value = response.data.supportedLanguages;
      }

      // Update i18n with tenant languages
      await updateI18nLanguages();
    } catch (err) {
      console.error('Failed to load tenant config:', err);
      error.value = 'Failed to load tenant configuration';

      // Fallback to API-based language detection
      try {
        const languages = await localizationApi.getSupportedLanguages();
        supportedLanguages.value = languages.length > 0 ? languages : ['en', 'de'];
        await updateI18nLanguages();
      } catch (langErr) {
        console.error('Failed to load supported languages:', langErr);
      }
    } finally {
      isLoading.value = false;
    }
  };

  const updateI18nLanguages = async () => {
    // This will be called to dynamically update the i18n instance
    // with the tenant's supported languages
    // i18n updates are handled by the locales/index.ts file
    // This function is kept for compatibility but does nothing
  };

  const getTenantFromUrl = (): string | null => {
    // Extract tenant from subdomain or path
    if (typeof window !== 'undefined') {
      const hostname = window.location.hostname;
      const subdomain = hostname.split('.')[0];

      // If subdomain is not 'www' or main domain, treat as tenant
      if (subdomain && subdomain !== 'www' && subdomain !== 'localhost') {
        return subdomain;
      }

      // Check for tenant in path
      const pathSegments = window.location.pathname.split('/');
      if (pathSegments[1] && pathSegments[1] !== '') {
        return pathSegments[1];
      }
    }

    return null;
  };

  const setTenantOverride = (tenantId: string) => {
    // Allow manual tenant override (for development/testing)
    sessionStorage.setItem('tenantOverride', tenantId);
    loadTenantConfig(tenantId);
  };

  const clearTenantOverride = () => {
    sessionStorage.removeItem('tenantOverride');
    const authStore = useAuthStore();
    loadTenantConfig(authStore.tenantId || undefined);
  };

  // Initialize on store creation
  if (process.client) {
    loadTenantConfig();
  }

  return {
    currentTenant,
    supportedLanguages: tenantLanguages,
    defaultLanguage,
    isLoading,
    error,
    isMultiLanguageEnabled,
    loadTenantConfig,
    updateI18nLanguages,
    setTenantOverride,
    clearTenantOverride,
  };
});
