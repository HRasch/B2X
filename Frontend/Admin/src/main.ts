import { createApp } from "vue";
import { createPinia } from "pinia";
import router from "./router";
import { setupAuthMiddleware } from "./middleware/auth";
import App from "./App.vue";
import "./main.css";
import { TestModePlugin, createTestMode, getTestMode } from "./utils/testMode";

// Default tenant GUID for admin authentication
const DEFAULT_TENANT_ID =
  import.meta.env.VITE_DEFAULT_TENANT_ID ||
  "00000000-0000-0000-0000-000000000001";

// Initialize default tenant if not set
if (!localStorage.getItem("tenantId")) {
  localStorage.setItem("tenantId", DEFAULT_TENANT_ID);
}

const app = createApp(App);

app.use(createPinia());
app.use(router);

// Setup auth middleware
setupAuthMiddleware(router);

// Initialize TestMode if enabled via URL parameter or localStorage
const urlParams = new URLSearchParams(window.location.search);
const testModeEnabled =
  urlParams.has("testmode") ||
  localStorage.getItem("testModeEnabled") === "true" ||
  import.meta.env.DEV; // Enable in development by default

if (testModeEnabled) {
  app.use(TestModePlugin, {
    enabled: true,
    autoFix: true,
    logLevel: "info",
    visualIndicators: true,
    performanceMonitoring: true,
  });

  // Store test mode preference
  localStorage.setItem("testModeEnabled", "true");

  console.log(
    "🧪 TestMode aktiviert - Browser-Aktionen werden überwacht und Fehler automatisch behoben"
  );
} else {
  // Create disabled instance for programmatic access
  createTestMode({ enabled: false });
}

// Make TestMode globally available for debugging
declare global {
  interface Window {
    getTestMode?: () => TestModeManager | null;
    enableTestMode?: () => void;
    disableTestMode?: () => void;
  }
}

if (testModeEnabled) {
  window.getTestMode = getTestMode;
  window.enableTestMode = () => getTestMode()?.enable();
  window.disableTestMode = () => getTestMode()?.disable();
}

app.mount("#app");
