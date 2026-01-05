import { createI18n } from 'vue-i18n';
import type { I18n } from 'vue-i18n';
import type { LocaleCode } from '~/types';
import en from './en.json';
import de from './de.json';
import fr from './fr.json';
import es from './es.json';
import it from './it.json';
import pt from './pt.json';
import nl from './nl.json';
import pl from './pl.json';

export const SUPPORTED_LOCALES = [
  { code: 'en', name: 'English', flag: '🇬🇧' },
  { code: 'de', name: 'Deutsch', flag: '🇩🇪' },
  { code: 'fr', name: 'Français', flag: '🇫🇷' },
  { code: 'es', name: 'Español', flag: '🇪🇸' },
  { code: 'it', name: 'Italiano', flag: '🇮🇹' },
  { code: 'pt', name: 'Português', flag: '🇵🇹' },
  { code: 'nl', name: 'Nederlands', flag: '🇳🇱' },
  { code: 'pl', name: 'Polski', flag: '🇵🇱' },
];

/**
 * Create and configure vue-i18n instance
 */
const i18n: I18n = createI18n({
  legacy: false,
  locale: (() => {
    const saved = localStorage.getItem('locale');
    if (saved && SUPPORTED_LOCALES.some(l => l.code === saved)) {
      return saved as LocaleCode;
    }
    const browser = navigator.language.split('-')[0];
    if (SUPPORTED_LOCALES.some(l => l.code === browser)) {
      return browser as LocaleCode;
    }
    return 'en';
  })(),
  fallbackLocale: 'en',
  globalInjection: true,
  messages: {
    en,
    de,
    fr,
    es,
    it,
    pt,
    nl,
    pl,
  },
  missingWarn: false,
  missingFallbackWarn: false,
});

export default i18n;
