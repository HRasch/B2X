import { createApp } from 'vue';
import { createPinia } from 'pinia';
import type { Locale } from 'vue-i18n';
import router from './router';
import i18n from './locales';
import App from './App.vue';
import './main.css';

// Import resilience features
import 'vue3-toastify/dist/index.css';
import { UserFeedbackPlugin } from './composables/useUserFeedback';
import ErrorBoundary from './components/common/ErrorBoundary.vue';

const app = createApp(App);

// Global error handler for unhandled promise rejections
window.addEventListener('unhandledrejection', event => {
  console.error('Unhandled promise rejection:', event.reason);
  // In a real app, send to error reporting service
  event.preventDefault(); // Prevent the default browser behavior
});

// Global error handler for JavaScript errors
window.addEventListener('error', event => {
  console.error('Global JavaScript error:', event.error);
  // In a real app, send to error reporting service
});

// Vue error handler
app.config.errorHandler = (err, instance, info) => {
  console.error('Vue error:', err, info);
  // In a real app, send to error reporting service
};

app.use(createPinia());
app.use(i18n);
app.use(router);
app.use(UserFeedbackPlugin);

// Register global components
app.component('ErrorBoundary', ErrorBoundary);

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
