import { createI18n } from 'vue-i18n';
import type { I18n } from 'vue-i18n';
import type { WritableComputedRef } from 'vue';
import type { TenantStore } from '~/types';
import en from './en.json';
import de from './de.json';
import fr from './fr.json';
import es from './es.json';
import it from './it.json';
import pt from './pt.json';
import nl from './nl.json';
import pl from './pl.json';

// All available locale messages
const ALL_MESSAGES = {
  en,
  de,
  fr,
  es,
  it,
  pt,
  nl,
  pl,
};

// All supported locales (static definition for build-time inclusion)
export const ALL_SUPPORTED_LOCALES = [
  { code: 'en', name: 'English', flag: '🇬🇧' },
  { code: 'de', name: 'Deutsch', flag: '🇩🇪' },
  { code: 'fr', name: 'Français', flag: '🇫🇷' },
  { code: 'es', name: 'Español', flag: '🇪🇸' },
  { code: 'it', name: 'Italiano', flag: '🇮🇹' },
  { code: 'pt', name: 'Português', flag: '🇵🇹' },
  { code: 'nl', name: 'Nederlands', flag: '🇳🇱' },
  { code: 'pl', name: 'Polski', flag: '🇵🇱' },
];

// Default supported locales (always available)
export const DEFAULT_SUPPORTED_LOCALES = [
  { code: 'en', name: 'English', flag: '🇬🇧' },
  { code: 'de', name: 'Deutsch', flag: '🇩🇪' },
];

/**
 * Get supported locales based on tenant configuration
 * Falls back to defaults if tenant config is not available
 */
export const getSupportedLocales = () => {
  // Check if we're on client side and tenant store is available
  if (typeof window !== 'undefined' && window.$tenantStore) {
    const tenantStore = window.$tenantStore as TenantStore;
    const tenantLanguages = tenantStore.supportedLanguages || ['en', 'de'];

    return ALL_SUPPORTED_LOCALES.filter(locale => tenantLanguages.includes(locale.code));
  }

  // Fallback to defaults during SSR or before tenant store is loaded
  return DEFAULT_SUPPORTED_LOCALES;
};

/**
 * Create and configure vue-i18n instance
 */
const i18n: I18n = createI18n({
  legacy: false,
  locale: 'en', // Will be updated dynamically
  fallbackLocale: 'en',
  globalInjection: true,
  messages: ALL_MESSAGES, // Include all messages for build-time
  missingWarn: false,
  missingFallbackWarn: false,
});

/**
 * Initialize i18n with tenant-aware locale detection
 */
export const initializeI18n = async () => {
  if (typeof window === 'undefined') return; // Skip on server

  try {
    // Get stored locale or detect from browser
    let locale = localStorage.getItem('locale') || navigator.language.split('-')[0] || 'en';

    // Wait for tenant store to be available and loaded
    if (window.$tenantStore) {
      const tenantStore = window.$tenantStore as TenantStore;
      await tenantStore.loadTenantConfig();

      // Check if current locale is supported by tenant
      const supportedLanguages = tenantStore.supportedLanguages || ['en', 'de'];
      if (!supportedLanguages.includes(locale)) {
        locale = tenantStore.defaultLanguage || 'en';
      }
    }

    // Set the locale
    (i18n.global.locale as WritableComputedRef<string>).value = locale;
    localStorage.setItem('locale', locale);
  } catch (error) {
    console.warn('Failed to initialize i18n with tenant config, using defaults:', error);
    // Fallback to basic initialization
    const locale = localStorage.getItem('locale') || navigator.language.split('-')[0] || 'en';
    (i18n.global.locale as WritableComputedRef<string>).value = locale;
  }
};

// Export for dynamic access
export { ALL_MESSAGES };
export default i18n;
