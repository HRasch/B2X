import { defineI18nConfig } from 'nuxt-i18n';
import en from './src/locales/en.json';
import de from './src/locales/de.json';
import fr from './src/locales/fr.json';
import es from './src/locales/es.json';
import it from './src/locales/it.json';
import pt from './src/locales/pt.json';
import nl from './src/locales/nl.json';
import pl from './src/locales/pl.json';

// Nuxt i18n expects vue-i18n options here; keep messages at the top level so
// all locales are registered on both server and client.
export default defineI18nConfig(() => ({
  legacy: false,
  locale: 'en',
  fallbackLocale: 'en',
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
}));
