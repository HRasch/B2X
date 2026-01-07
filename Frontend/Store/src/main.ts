import { createApp } from 'vue';
import { createPinia } from 'pinia';
import type { Locale } from 'vue-i18n';
import router from './router';
import i18n, { initializeI18n } from './locales';
import App from './App.vue';
import './main.css';

// Import stores
import { useTenantStore } from './stores/tenant';

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

const pinia = createPinia();
app.use(pinia);
app.use(i18n);
app.use(router);
app.use(UserFeedbackPlugin);

// Register global components
app.component('ErrorBoundary', ErrorBoundary);

/**
 * Initialize application with tenant-aware i18n
 */
const initializeApp = async () => {
  try {
    // Initialize tenant store first
    const tenantStore = useTenantStore();
    window.$tenantStore = tenantStore;

    // Initialize i18n with tenant configuration
    await initializeI18n();

    // Get current locale for document language
    const currentLocale = i18n.global.locale.value;
    document.documentElement.lang = currentLocale;

    // Mount the app
    app.mount('#app');
  } catch (error) {
    console.error('Failed to initialize app:', error);

    // Fallback initialization
    const locale = localStorage.getItem('locale') || navigator.language.split('-')[0] || 'en';

    if (typeof i18n.global.locale === 'object' && 'value' in i18n.global.locale) {
      i18n.global.locale.value = locale as Locale;
    } else {
      (i18n.global as { locale: string }).locale = locale;
    }

    document.documentElement.lang = locale;
    app.mount('#app');
  }
};

// Initialize the application
initializeApp();
