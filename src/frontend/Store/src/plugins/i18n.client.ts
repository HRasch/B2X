// plugins/i18n.client.ts
import type { NuxtI18nRuntime } from '~/types';
import en from '~/locales/en.json';
import de from '~/locales/de.json';
import fr from '~/locales/fr.json';
import es from '~/locales/es.json';
import it from '~/locales/it.json';
import pt from '~/locales/pt.json';
import nl from '~/locales/nl.json';
import pl from '~/locales/pl.json';

interface TenantTranslationResponse {
  translations: Record<string, Record<string, unknown>>;
}

const STATIC_MESSAGES = {
  en,
  de,
  fr,
  es,
  it,
  pt,
  nl,
  pl,
};

export default defineNuxtPlugin(async nuxtApp => {
  // Only run on client-side
  if (process.server) return;

  const runtimeConfig = useRuntimeConfig();

  // Get tenant from runtime config
  const tenantId = runtimeConfig.public.tenantId;

  const loadTenantTranslations = async (locale: string) => {
    try {
      // Load tenant-specific translations from backend API
      const response = await $fetch<TenantTranslationResponse>(
        `/api/v1/localization/tenant/${tenantId}/${locale}`,
        {
          baseURL: (runtimeConfig.public.apiBase as string) || 'http://localhost:8080',
        }
      );

      // Set the locale messages directly since we're not using static files
      const i18n = (nuxtApp as unknown as { $i18n: NuxtI18nRuntime }).$i18n;

      if (response?.translations && typeof response.translations === 'object') {
        i18n.setLocaleMessage(locale, response.translations);
      } else {
        throw new Error('Invalid API response format');
      }
    } catch (error) {
      console.warn(
        `Failed to load translations from API for tenant ${tenantId}, locale ${locale}:`,
        error
      );

      // Fallback: Load from static locale files
      const i18n = (nuxtApp as unknown as { $i18n: NuxtI18nRuntime }).$i18n;
      const staticTranslations = STATIC_MESSAGES[locale as keyof typeof STATIC_MESSAGES];
      if (staticTranslations) {
        i18n.setLocaleMessage(locale, staticTranslations);
        console.log('Loaded translations from static files as fallback');
      } else {
        console.error('No static translations available for locale:', locale);
        i18n.setLocaleMessage(locale, {});
      }
    }
  };

  // Load tenant translations for current locale on client-side hydration
  const i18n = (nuxtApp as unknown as { $i18n: NuxtI18nRuntime }).$i18n;

  // Load tenant translations (static translations are already loaded server-side)
  const currentLocale = i18n.locale.value;
  loadTenantTranslations(currentLocale);
});
