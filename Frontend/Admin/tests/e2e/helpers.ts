import { Page, expect } from "@playwright/test";

/**
 * Common test utilities for E2E tests
 */

export interface TestUser {
  email: string;
  password: string;
  role: string;
  tenantId: string;
  description: string;
}

// Test accounts seeded in the backend
export const TEST_USERS: Record<string, TestUser> = {
  admin: {
    email: "e2e-admin@test.com",
    password: "test123!",
    role: "Admin",
    tenantId: "default",
    description: "Full system administrator with all permissions",
  },
  tenantAdmin: {
    email: "e2e-tenant-admin@test.com",
    password: "test123!",
    role: "TenantAdmin",
    tenantId: "test-tenant",
    description: "Tenant-scoped administrator",
  },
  regularUser: {
    email: "e2e-user@test.com",
    password: "test123!",
    role: "User",
    tenantId: "test-tenant",
    description: "Regular user with limited permissions",
  },
};

// Legacy support for environment variables (fallback)
const getTestCredentials = () => {
  const email = process.env.E2E_TEST_EMAIL || TEST_USERS.admin.email;
  const password = process.env.E2E_TEST_PASSWORD || TEST_USERS.admin.password;

  return { email, password };
};

export const TEST_CREDENTIALS = getTestCredentials();

export const API_BASE = "http://localhost:6000";
export const APP_URL = "http://localhost:5174";

/**
 * Login to the application as admin (legacy function)
 */
export async function loginAsAdmin(page: Page) {
  await loginAsUser(page, "admin");
}

/**
 * Login to the application with a specific user role
 */
export async function loginAsUser(
  page: Page,
  userRole: keyof typeof TEST_USERS
) {
  const user = TEST_USERS[userRole];

  // Navigate to login page
  await page.goto(`${APP_URL}/login`);
  await page.waitForLoadState("networkidle");

  // Fill in login form
  await page.fill('input[type="email"]', user.email);
  await page.fill('input[type="password"]', user.password);

  // Click login button
  await page.click('button[type="submit"]');

  // Wait for navigation to dashboard or main layout
  await page.waitForSelector('[data-test="main-layout"]', { timeout: 10000 });

  // Verify we're logged in with the correct user
  await expect(page.locator('[data-test="user-info"]')).toContainText(
    user.email
  );
}

/**
 * Get the current user from the test users
 */
export function getTestUser(userRole: keyof typeof TEST_USERS): TestUser {
  return TEST_USERS[userRole];
}

/**
 * Check if a user has access to a specific route
 * @param page - Playwright page object
 * @param route - The route to check (e.g., '/dashboard', '/cms/pages')
 * @returns true if access is granted, false if redirected to unauthorized
 */
export async function checkRouteAccess(
  page: Page,
  route: string
): Promise<boolean> {
  await page.goto(`${APP_URL}${route}`);
  await page.waitForLoadState("networkidle");

  // Check if we're on the unauthorized page
  const currentUrl = page.url();
  return (
    !currentUrl.includes("/unauthorized") && !currentUrl.includes("/login")
  );
}

/**
 * Test route access for different user roles
 * @param page - Playwright page object
 * @param route - The route to test
 * @param expectedAccess - Object mapping user roles to expected access (true/false)
 */
export async function testRouteAccessForRoles(
  page: Page,
  route: string,
  expectedAccess: Record<string, boolean>
) {
  for (const [role, shouldHaveAccess] of Object.entries(expectedAccess)) {
    await loginAsUser(page, role as keyof typeof TEST_USERS);

    const hasAccess = await checkRouteAccess(page, route);

    if (shouldHaveAccess) {
      expect(
        hasAccess,
        `User with role '${role}' should have access to ${route}`
      ).toBe(true);
    } else {
      expect(
        hasAccess,
        `User with role '${role}' should NOT have access to ${route}`
      ).toBe(false);
    }

    await logoutUser(page);
  }
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
 * Get table data as array of objects
 */
export async function getTableData(
  page: Page,
  tableSelector: string = "table"
) {
  return page.evaluate((selector) => {
    const table = document.querySelector(selector);
    if (!table) return [];

    const headers = Array.from(table.querySelectorAll("thead th")).map((h) =>
      h.textContent?.trim()
    );

    const rows = Array.from(table.querySelectorAll("tbody tr")).map((row) => {
      const cells = Array.from(row.querySelectorAll("td")).map((c) =>
        c.textContent?.trim()
      );
      const obj: any = {};
      headers.forEach((header, index) => {
        if (header) obj[header] = cells[index];
      });
      return obj;
    });

    return rows;
  }, tableSelector);
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

/**
 * Validate paginated API response
 */
export async function validatePaginatedResponse(data: any): Promise<boolean> {
  // Check for common pagination structures
  return (
    (data.items && Array.isArray(data.items)) ||
    (data.data && Array.isArray(data.data)) ||
    (data.content && Array.isArray(data.content)) ||
    (data.results && Array.isArray(data.results)) ||
    Array.isArray(data)
  );
}
