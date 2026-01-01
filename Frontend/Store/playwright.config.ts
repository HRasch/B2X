import { defineConfig, devices } from "@playwright/test";

// Dynamic port configuration from environment (Aspire assigns ports dynamically)
const STORE_PORT =
  process.env.PLAYWRIGHT_STORE_PORT || process.env.STORE_PORT || "5173";
const STORE_BASE_URL =
  process.env.PLAYWRIGHT_BASE_URL || `http://localhost:${STORE_PORT}`;

export default defineConfig({
  testDir: "./tests/e2e",
  // Timeout 1s for backend integration
  timeout: 1000,
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: "html",

  use: {
    // 1s timeouts
    actionTimeout: 1000,
    navigationTimeout: 1000,
    expect: { timeout: 1000 },
    baseURL: STORE_BASE_URL,
    trace: "on-first-retry",
    screenshot: "only-on-failure",
    video: "retain-on-failure",
  },

  projects: [
    {
      name: "chromium",
      use: { ...devices["Desktop Chrome"] },
    },
  ],

  webServer: {
    command: `npm run dev -- --port ${STORE_PORT}`,
    url: STORE_BASE_URL,
    reuseExistingServer: true,
    timeout: 120000,
  },
});
