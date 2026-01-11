import { createApp } from 'vue';
import { createPinia } from 'pinia';
import router from './router';
import { setupAuthMiddleware } from './middleware/auth';
import { errorLoggingPlugin } from './services/errorLogging';
import { errorLogger } from './services/errorLogger';
import i18n from './locales';
import App from './App.vue';
import './main.css';

// Monaco Editor
import { install as VueMonacoEditorPlugin } from '@guolao/vue-monaco-editor';
// Global layout components
import { PageHeader, CardContainer, FormSection, FormRow, FormGroup } from '@/components/layout';

// Configure Monaco locale based on current i18n locale
const configureMonacoLocale = async () => {
  const currentLocale = localStorage.getItem('locale') || navigator.language.split('-')[0] || 'en';

  // Monaco supported locales mapping
  const monacoLocales: Record<string, string> = {
    de: 'de',
    fr: 'fr',
    es: 'es',
    it: 'it',
    pt: 'pt-br', // Portuguese Brazil
    pl: 'pl',
    // nl not supported by Monaco, will use English
  };

  const monacoLocale = monacoLocales[currentLocale];
  if (monacoLocale) {
    try {
      // Dynamically import the locale file
      await import(/* @vite-ignore */ `monaco-editor/esm/vs/nls.${monacoLocale}.js`);
      console.log(`Monaco locale loaded: ${monacoLocale}`);
    } catch (error) {
      console.warn(`Failed to load Monaco locale: ${monacoLocale}`, error);
    }
  }
};

// Default tenant GUID for admin authentication
const DEFAULT_TENANT_ID =
  import.meta.env.VITE_DEFAULT_TENANT_ID || '00000000-0000-0000-0000-000000000001';

// Initialize default tenant if not set
if (!localStorage.getItem('tenantId')) {
  localStorage.setItem('tenantId', DEFAULT_TENANT_ID);
}

// Initialize the app asynchronously to configure Monaco locale
const initApp = async () => {
  const app = createApp(App);

  app.use(createPinia());
  app.use(router);
  app.use(i18n);

  // Configure and install Monaco Editor with locale support
  await configureMonacoLocale();
  app.use(VueMonacoEditorPlugin);

  // Initialize error logging (captures Vue errors, unhandled rejections, network errors)
  app.use(errorLoggingPlugin, { router });

  // Install simple client-side logger that forwards to backend error endpoint
  errorLogger.installGlobalHandlers(() => ({
    tenantId: localStorage.getItem('tenantId') || undefined,
    userId: localStorage.getItem('userId') || undefined,
    route: router.currentRoute.value?.path,
  }));
  errorLogger.start();

  // Setup auth middleware
  setupAuthMiddleware(router);

  // Register shared layout components globally to simplify migration
  app.component('PageHeader', PageHeader);
  app.component('CardContainer', CardContainer);
  app.component('FormSection', FormSection);
  app.component('FormRow', FormRow);
  app.component('FormGroup', FormGroup);

  app.mount('#app');
};

initApp();
