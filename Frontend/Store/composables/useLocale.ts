import { useI18n } from 'vue-i18n';
import { ref, computed, type ComputedRef, type Ref } from 'vue';
import type { LocaleCode } from '~/types';

/**
 * Supported locales with metadata
 */
const SUPPORTED_LOCALES = [
  { code: 'en', name: 'English', flag: '🇬🇧' },
  { code: 'de', name: 'Deutsch', flag: '🇩🇪' },
  { code: 'fr', name: 'Français', flag: '🇫🇷' },
  { code: 'es', name: 'Español', flag: '🇪🇸' },
  { code: 'it', name: 'Italiano', flag: '🇮🇹' },
  { code: 'pt', name: 'Português', flag: '🇵🇹' },
  { code: 'nl', name: 'Nederlands', flag: '🇳🇱' },
  { code: 'pl', name: 'Polski', flag: '🇵🇱' },
] as const;

/**
 * Type guard to check if a string is a valid locale code
 */
function isValidLocaleCode(code: string): code is (typeof SUPPORTED_LOCALES)[number]['code'] {
  return SUPPORTED_LOCALES.some(locale => locale.code === code);
}

/**
 * Locale object structure
 */
interface Locale {
  code: string;
  name: string;
  flag: string;
}

/**
 * Composable for managing application localization
 * Provides locale switching, preferences, and locale utilities
 */
interface UseLocaleReturn {
  locale: ComputedRef<string>;
  currentLocale: ComputedRef<Locale | undefined>;
  locales: readonly Locale[];
  isLoading: Ref<boolean>;
  t: (key: string, ...args: unknown[]) => string;
  setLocale: (code: string) => Promise<void>;
  initializeLocale: () => void;
  getLocaleName: (code: string) => string;
  getLocaleFlag: (code: string) => string;
  getSupportedLocaleCodes: () => string[];
}

export function useLocale(): UseLocaleReturn {
  const i18n = useI18n();
  const isLoading = ref(false);

  /**
   * Supported locales with metadata
   */
  const locales = SUPPORTED_LOCALES;

  /**
   * Current selected locale object
   */
  const currentLocale = computed(() => {
    return locales.find(l => l.code === i18n.locale.value) || locales[0];
  });

  /**
   * Get locale name in English
   */
  const getLocaleName = (code: string): string => {
    const loc = locales.find(l => l.code === code);
    return loc?.name || code;
  };

  /**
   * Get locale flag emoji
   */
  const getLocaleFlag = (code: string): string => {
    const loc = locales.find(l => l.code === code);
    return loc?.flag || '🌐';
  };

  /**
   * Set locale and persist to localStorage
   */
  const setLocale = async (code: string): Promise<void> => {
    // Validate locale code
    if (!isValidLocaleCode(code)) {
      console.warn(`Unsupported locale: ${code}`);
      return;
    }

    isLoading.value = true;
    try {
      // Update i18n locale (this is the key - must use i18n.locale.value)
      i18n.locale.value = code as any;
      localStorage.setItem('locale', code);
      document.documentElement.lang = code;

      // Emit event for other parts of app (e.g., API)
      window.dispatchEvent(new CustomEvent('locale-changed', { detail: { locale: code } }));
    } finally {
      isLoading.value = false;
    }
  };

  const initializeLocale = (): void => {
    const savedLocale = localStorage.getItem('locale');
    if (savedLocale && isValidLocaleCode(savedLocale)) {
      i18n.locale.value = savedLocale as any;
    } else {
      const browserLang = navigator.language.split('-')[0];
      const matchedLocale = locales.find(l => l.code === browserLang);
      if (matchedLocale && isValidLocaleCode(matchedLocale.code)) {
        i18n.locale.value = matchedLocale.code as any;
        localStorage.setItem('locale', matchedLocale.code);
      }
    }
    document.documentElement.lang = i18n.locale.value;
  };

  /**
   * Get all supported locale codes
   */
  const getSupportedLocaleCodes = (): string[] => {
    return locales.map(l => l.code);
  };

  return {
    locale: computed(() => i18n.locale.value),
    currentLocale,
    locales,
    isLoading,
    t: i18n.t,
    setLocale,
    initializeLocale,
    getLocaleName,
    getLocaleFlag,
    getSupportedLocaleCodes,
  };
}
