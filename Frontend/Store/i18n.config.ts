import commonEn from './locales/default/en/common.json';
import { defineI18nConfig } from 'nuxt-i18n';

// Minimal i18n config used during incremental upgrades. Full translations
// are kept in JSON under ./locales. This file is intentionally small and
// uses `// @ts-nocheck` to avoid complex inferred-type errors from
// @nuxtjs/i18n runtime types while we finish the migration.
export default defineI18nConfig(() => ({
  legacy: false,
  locales: [
    { code: 'en', iso: 'en-US', name: 'English' },
    { code: 'de', iso: 'de-DE', name: 'Deutsch' },
  ],
  defaultLocale: 'en',
  vueI18n: {
    legacy: false,
    locale: 'en',
    messages: {
      en: {
        common: commonEn as Record<string, any>, // eslint-disable-line @typescript-eslint/no-explicit-any
      },
    } as Record<string, any>, // eslint-disable-line @typescript-eslint/no-explicit-any
  },
}));
