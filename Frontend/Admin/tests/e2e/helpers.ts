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
export async function navigateToAdminPage(\n  page: Page,\n  path: string,\n  expectedHeading?: string\n) {\n  await page.goto(`${APP_URL}${path}`);\n  await page.waitForLoadState(\"networkidle\");\n\n  if (expectedHeading) {\n    await expect(page.locator(`text=${expectedHeading}`)).toBeVisible({\n      timeout: 5000,\n    });\n  }\n}\n\n/**\n * Check for 404 errors on page\n */\nexport async function checkFor404(page: Page): Promise<boolean> {\n  const pageContent = await page.evaluate(() =>\n    document.documentElement.innerHTML\n  );\n  return (\n    pageContent.includes(\"404\") || pageContent.includes(\"Not Found\")\n  );\n}\n\n/**\n * Get table data as array of objects\n */\nexport async function getTableData(\n  page: Page,\n  tableSelector: string = \"table\"\n) {\n  return page.evaluate((selector) => {\n    const table = document.querySelector(selector);\n    if (!table) return [];\n\n    const headers = Array.from(\n      table.querySelectorAll(\"thead th\")\n    ).map((h) => h.textContent?.trim());\n\n    const rows = Array.from(table.querySelectorAll(\"tbody tr\")).map((row) => {\n      const cells = Array.from(row.querySelectorAll(\"td\")).map(\n        (c) => c.textContent?.trim()\n      );\n      const obj: any = {};\n      headers.forEach((header, index) => {\n        if (header) obj[header] = cells[index];\n      });\n      return obj;\n    });\n\n    return rows;\n  }, tableSelector);\n}\n\n/**\n * Wait for loading indicator to disappear\n */\nexport async function waitForLoadingToComplete(page: Page) {\n  await page.waitForSelector(\n    '.spinner, [data-testid=\"loading\"], .loader',\n    {\n      state: \"hidden\",\n      timeout: 5000,\n    }\n  );\n}\n\n/**\n * Check if element has dark mode styles\n */\nexport async function hasDarkModeSupport(page: Page): Promise<boolean> {\n  return page.evaluate(() => {\n    const h1 = document.querySelector(\"h1\");\n    if (!h1) return false;\n    const styles = window.getComputedStyle(h1);\n    return styles.color !== \"\";\n  });\n}\n\n/**\n * Simulate network conditions\n */\nexport async function simulateSlowNetwork(page: Page) {\n  await page.route(\"**/*\", (route) => {\n    setTimeout(() => route.continue(), 500);\n  });\n}\n\n/**\n * Clear auth state (logout)\n */\nexport async function logoutUser(page: Page) {\n  await page.evaluate(() => {\n    localStorage.clear();\n    sessionStorage.clear();\n  });\n}\n\n/**\n * Check API response times\n */\nexport async function measureApiResponseTime(\n  page: Page,\n  endpoint: string\n): Promise<number> {\n  const startTime = Date.now();\n  await apiCall(page, endpoint);\n  return Date.now() - startTime;\n}\n\n/**\n * Retry operation with exponential backoff\n */\nexport async function retryWithBackoff<T>(\n  operation: () => Promise<T>,\n  maxRetries: number = 3,\n  baseDelay: number = 1000\n): Promise<T> {\n  for (let i = 0; i < maxRetries; i++) {\n    try {\n      return await operation();\n    } catch (error) {\n      if (i === maxRetries - 1) throw error;\n      const delay = baseDelay * Math.pow(2, i);\n      await new Promise((resolve) => setTimeout(resolve, delay));\n    }\n  }\n  throw new Error(\"Max retries exceeded\");\n}\n\n/**\n * Validate paginated API response\n */\nexport async function validatePaginatedResponse(data: any): Promise<boolean> {\n  // Check for common pagination structures\n  return (\n    (data.items && Array.isArray(data.items)) ||\n    (data.data && Array.isArray(data.data)) ||\n    (data.content && Array.isArray(data.content)) ||\n    (data.results && Array.isArray(data.results)) ||\n    Array.isArray(data)\n  );\n}\n