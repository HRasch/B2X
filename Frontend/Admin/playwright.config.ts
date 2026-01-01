import { defineConfig, devices } from "@playwright/test";

// Dynamic port configuration from environment (Aspire assigns ports dynamically)
const ADMIN_PORT =
  process.env.PLAYWRIGHT_ADMIN_PORT || process.env.ADMIN_PORT || "5174";
const ADMIN_BASE_URL =
  process.env.PLAYWRIGHT_BASE_URL || `http://localhost:${ADMIN_PORT}`;

export default defineConfig({
  testDir: "./tests/e2e",
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : 4,
  reporter: "html",

  timeout: 1000,
  expect: { timeout: 1000 },

  use: {
    baseURL: ADMIN_BASE_URL,
    trace: "on-first-retry",
    screenshot: "only-on-failure",
    video: "retain-on-failure",
    actionTimeout: 1000,
    navigationTimeout: 1000,
  },

  projects: [
    {
      name: "chromium",
      use: { ...devices["Desktop Chrome"] },
    },
  ],

  webServer: {
    command: `npm run dev -- --port ${ADMIN_PORT}`,
    url: ADMIN_BASE_URL,
    reuseExistingServer: true,
    timeout: 120000,
  },
});
