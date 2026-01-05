import { createApp } from 'vue';
import { createPinia } from 'pinia';
import router from './router';
import { setupAuthMiddleware } from './middleware/auth';
import { errorLoggingPlugin } from './services/errorLogging';
import { i18n } from './locales';
import App from './App.vue';
import './main.css';

// Default tenant GUID for admin authentication
const DEFAULT_TENANT_ID =
  import.meta.env.VITE_DEFAULT_TENANT_ID || '00000000-0000-0000-0000-000000000001';

// Initialize default tenant if not set
if (!localStorage.getItem('tenantId')) {
  localStorage.setItem('tenantId', DEFAULT_TENANT_ID);
}

const app = createApp(App);

app.use(createPinia());
app.use(router);
app.use(i18n);

// Initialize error logging (captures Vue errors, unhandled rejections, network errors)
app.use(errorLoggingPlugin, { router });

// Setup auth middleware
setupAuthMiddleware(router);

app.mount('#app');
