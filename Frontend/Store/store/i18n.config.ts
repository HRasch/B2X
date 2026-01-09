import { defineI18nConfig } from 'nuxt-i18n';
import en from './src/locales/en.json';
import de from './src/locales/de.json';
import fr from './src/locales/fr.json';

// Minimal i18n config used during incremental upgrades. Full translations
// are kept in JSON under ./locales. This file is intentionally small and
// uses `// @ts-nocheck` to avoid complex inferred-type errors from
// @nuxtjs/i18n runtime types while we finish the migration.
export default defineI18nConfig(() => ({
  legacy: false,
  locales: [
    { code: 'en', iso: 'en-US', name: 'English', flag: '🇬🇧' },
    { code: 'de', iso: 'de-DE', name: 'Deutsch', flag: '🇩🇪' },
    { code: 'fr', iso: 'fr-FR', name: 'Français', flag: '🇫🇷' },
  ],
  defaultLocale: 'en',
  vueI18n: {
    legacy: false,
    locale: 'en',
    messages: {
      en,
      de,
      fr,
    },
  },
}));
