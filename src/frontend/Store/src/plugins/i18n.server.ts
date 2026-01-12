// plugins/i18n.server.ts
import en from '~/locales/en.json';
import de from '~/locales/de.json';
import fr from '~/locales/fr.json';
import es from '~/locales/es.json';
import it from '~/locales/it.json';
import pt from '~/locales/pt.json';
import nl from '~/locales/nl.json';
import pl from '~/locales/pl.json';

export default defineNuxtPlugin(async nuxtApp => {
  // Only run on server-side
  if (!process.server) return;

  const i18n = nuxtApp.$i18n as any;

  // Load static translations synchronously for SSR
  i18n.setLocaleMessage('en', en);
  i18n.setLocaleMessage('de', de);
  i18n.setLocaleMessage('fr', fr);
  i18n.setLocaleMessage('es', es);
  i18n.setLocaleMessage('it', it);
  i18n.setLocaleMessage('pt', pt);
  i18n.setLocaleMessage('nl', nl);
  i18n.setLocaleMessage('pl', pl);

  console.log('SSR: Loaded static translations for all locales');
});
