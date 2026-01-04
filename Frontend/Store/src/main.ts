import { createApp } from 'vue';
import { createPinia } from 'pinia';
import type { Locale } from 'vue-i18n';
import router from './router';
import i18n from './locales';
import App from './App.vue';
import './main.css';

const app = createApp(App);

app.use(createPinia());
app.use(i18n);
app.use(router);

/**
 * Initialize locale from localStorage or browser language preference.
 * Falls back to 'en' if neither is available.
 */
const getInitialLocale = (): string => {
  const storedLocale = localStorage.getItem('locale');
  if (storedLocale) return storedLocale;

  const browserLocale = navigator.language.split('-')[0];
  return browserLocale || 'en';
};

const locale = getInitialLocale();

// Set locale in i18n
if (typeof i18n.global.locale === 'object' && 'value' in i18n.global.locale) {
  i18n.global.locale.value = locale as Locale;
} else {
  (i18n.global as { locale: string }).locale = locale;
}

// Set document language attribute
document.documentElement.lang = locale;

app.mount('#app');
