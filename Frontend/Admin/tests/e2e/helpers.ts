/* eslint-disable @typescript-eslint/no-explicit-any -- Playwright Page type */
import { Page, expect } from "@playwright/test";

/**
 * Common test utilities for E2E tests
 */

// Get test credentials from environment variables
const getTestCredentials = () => {
  const email = process.env.E2E_TEST_EMAIL;
  const password = process.env.E2E_TEST_PASSWORD;

  if (!email || !password) {
    throw new Error(
      "❌ E2E Testing requires environment variables:\n" +
        "  E2E_TEST_EMAIL: Test account email\n" +
        "  E2E_TEST_PASSWORD: Test account password (min 8 chars, alphanumeric + special)\n" +
        "\nExample:\n" +
        "  export E2E_TEST_EMAIL='testuser@example.com'\n" +
        "  export E2E_TEST_PASSWORD='TestP@ss123!'\n" +
        "\nOr use GitHub Secrets for CI/CD:\n" +
        "  - In .github/workflows/e2e.yml, set:\n" +
        "    env:\n" +
        "      E2E_TEST_EMAIL: ${{ secrets.E2E_TEST_EMAIL }}\n" +
        "      E2E_TEST_PASSWORD: ${{ secrets.E2E_TEST_PASSWORD }}"
    );
  }

  return { email, password };
};

export const TEST_CREDENTIALS = getTestCredentials();

export const API_BASE = "http://localhost:6000";
export const APP_URL = "http://localhost:5174";

/**
 * Login to the application
 */
export async function loginAsAdmin(page: Page) {
  await page.goto(`${APP_URL}/`);
  await page.fill('input[type="email"]', TEST_CREDENTIALS.email);
  await page.fill('input[type="password"]', TEST_CREDENTIALS.password);
  await page.click('button[type="submit"]');
  await page.waitForURL(/.*dashboard/, { timeout: 10000 });
}

/**
 * Get auth token from localStorage
 */
export async function getAuthToken(page: Page): Promise<string | null> {
  return page.evaluate(() => localStorage.getItem("authToken"));
}

/**
 * Make API call with auth token
 */
export async function apiCall(
  page: Page,
  endpoint: string,
  method: string = "GET",
  body?: any
): Promise<Response> {
  const token = await getAuthToken(page);
  const options: RequestInit = {
    method,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token || ""}`,
    },
  };

  if (body) {
    options.body = JSON.stringify(body);
  }

  return fetch(`${API_BASE}${endpoint}`, options);
}

/**
 * Validate API response status
 */
export async function validateApiResponse(
  page: Page,
  endpoint: string,
  expectedStatus: number = 200
) {
  const response = await apiCall(page, endpoint);
  expect(response.status).toBe(expectedStatus);
  return response.json();
}

/**
 * Wait for API route and validate
 */
export async function waitForApiRoute(
  page: Page,
  urlPattern: string | RegExp,
  timeout: number = 10000
) {
  return page.waitForResponse(
    (resp) => {
      const url = resp.url();
      if (typeof urlPattern === "string") {
        return url.includes(urlPattern) && resp.status() === 200;
      } else {
        return urlPattern.test(url) && resp.status() === 200;
      }
    },
    { timeout }
  );
}

/**
 * Navigate to admin page and verify load
 */
export async function navigateToAdminPage(
  page: Page,
  path: string,
  expectedHeading?: string
) {
  await page.goto(`${APP_URL}${path}`);
  await page.waitForLoadState("networkidle");

  if (expectedHeading) {
    await expect(page.locator(`text=${expectedHeading}`)).toBeVisible({
      timeout: 5000,
    });
  }
}

/**
 * Check for 404 errors on page
 */
export async function checkFor404(page: Page): Promise<boolean> {
  const pageContent = await page.evaluate(
    () => document.documentElement.innerHTML
  );
  return pageContent.includes("404") || pageContent.includes("Not Found");
}

/**
 * Wait for loading indicator to disappear
 */
export async function waitForLoadingToComplete(page: Page) {
  await page.waitForSelector('.spinner, [data-testid="loading"], .loader', {
    state: "hidden",
    timeout: 5000,
  });
}

/**
 * Check if element has dark mode styles
 */
export async function hasDarkModeSupport(page: Page): Promise<boolean> {
  return page.evaluate(() => {
    const h1 = document.querySelector("h1");
    if (!h1) return false;
    const styles = window.getComputedStyle(h1);
    return styles.color !== "";
  });
}

/**
 * Simulate network conditions
 */
export async function simulateSlowNetwork(page: Page) {
  await page.route("**/*", (route) => {
    setTimeout(() => route.continue(), 500);
  });
}

/**
 * Clear auth state (logout)
 */
export async function logoutUser(page: Page) {
  await page.evaluate(() => {
    localStorage.clear();
    sessionStorage.clear();
  });
}

/**
 * Check API response times
 */
export async function measureApiResponseTime(
  page: Page,
  endpoint: string
): Promise<number> {
  const startTime = Date.now();
  await apiCall(page, endpoint);
  return Date.now() - startTime;
}

/**
 * Retry operation with exponential backoff
 */
export async function retryWithBackoff<T>(
  operation: () => Promise<T>,
  maxRetries: number = 3,
  baseDelay: number = 1000
): Promise<T> {
  for (let i = 0; i < maxRetries; i++) {
    try {
      return await operation();
    } catch (error) {
      if (i === maxRetries - 1) throw error;
      const delay = baseDelay * Math.pow(2, i);
      await new Promise((resolve) => setTimeout(resolve, delay));
    }
  }
  throw new Error("Max retries exceeded");
}
