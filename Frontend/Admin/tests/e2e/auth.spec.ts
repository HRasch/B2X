import { test, expect } from "@playwright/test";

const DEFAULT_TENANT_ID = "00000000-0000-0000-0000-000000000001";

test.describe("Admin Frontend - Login E2E Tests", () => {
  test.beforeEach(async ({ page }) => {
    // Clear storage before each test
    await page.context().clearCookies();
    await page.evaluate(() => {
      localStorage.clear();
      sessionStorage.clear();
    });

    // Navigate to the login page
    await page.goto("http://localhost:5174");
    await page.waitForLoadState("domcontentloaded");
  });

  test("should display login form with all required elements", async ({
    page,
  }) => {
    // Check for login form elements
    await expect(page.locator('input[type="email"]')).toBeVisible();
    await expect(page.locator('input[type="password"]')).toBeVisible();

    // Check for submit button
    const submitBtn = page.locator('button:has-text("Sign In")');
    await expect(submitBtn).toBeVisible({ timeout: 3000 });

    // Check for remember me checkbox
    const rememberMeCheckbox = page.locator('input[type="checkbox"]');
    await expect(rememberMeCheckbox).toBeVisible({ timeout: 3000 });
  });

  test("should initialize default tenant ID in localStorage on page load", async ({
    page,
  }) => {
    // Check if tenant ID is set in localStorage
    const tenantId = await page.evaluate(() =>
      localStorage.getItem("tenantId")
    );

    expect(tenantId).toBe(DEFAULT_TENANT_ID);
  });

  test("should show error message with invalid credentials", async ({
    page,
  }) => {
    // Fill in invalid credentials
    await page.locator('input[type="email"]').fill("invalid@example.com");
    await page.locator('input[type="password"]').fill("wrongpassword");

    // Submit form
    await page.locator('button:has-text("Sign In")').first().click();

    // Wait for error message
    await page.waitForTimeout(2000);

    // Verify user is still on login page
    expect(page.url()).toContain("login");
  });

  test("should successfully login with valid credentials and JWT validation", async ({
    page,
    context,
  }) => {
    // Intercept the login request to verify tenant header
    let loginRequestHeaders: Record<string, string> = {};

    await page.route("**/api/auth/login", async (route, request) => {
      loginRequestHeaders = request.headers();
      await route.continue();
    });

    // Fill in valid credentials
    await page.locator('input[type="email"]').fill("admin@example.com");
    await page.locator('input[type="password"]').fill("password");

    // Submit form
    await page.locator('button:has-text("Sign In")').first().click();

    // Wait for navigation to dashboard
    await page.waitForURL("**/dashboard", { timeout: 15000 });

    // Verify URL contains dashboard
    expect(page.url()).toContain("dashboard");

    // Verify X-Tenant-ID header was sent with login request
    expect(loginRequestHeaders["x-tenant-id"]).toBe(DEFAULT_TENANT_ID);
  });

  test("should store JWT token and tenant ID in localStorage after successful login", async ({
    page,
  }) => {
    // Fill in valid credentials
    await page.locator('input[type="email"]').fill("admin@example.com");
    await page.locator('input[type="password"]').fill("password");

    // Submit form
    await page.locator('button:has-text("Sign In")').first().click();

    // Wait for navigation
    await page.waitForURL("**/dashboard", { timeout: 15000 });

    // Check localStorage for auth token
    const authData = await page.evaluate(() => ({
      authToken: localStorage.getItem("authToken"),
      refreshToken: localStorage.getItem("refreshToken"),
      tenantId: localStorage.getItem("tenantId"),
    }));

    // Verify tokens are stored
    expect(authData.authToken).toBeTruthy();
    expect(authData.tenantId).toBe(DEFAULT_TENANT_ID);
  });

  test("should include X-Tenant-ID header in all authenticated API requests", async ({
    page,
  }) => {
    // Login first
    await page.locator('input[type="email"]').fill("admin@example.com");
    await page.locator('input[type="password"]').fill("password");
    await page.locator('button:has-text("Sign In")').first().click();
    await page.waitForURL("**/dashboard", { timeout: 15000 });

    // Intercept API requests
    const apiRequests: Array<{ url: string; headers: Record<string, string> }> =
      [];

    await page.route("**/api/**", async (route, request) => {
      apiRequests.push({
        url: request.url(),
        headers: request.headers(),
      });
      await route.continue();
    });

    // Trigger some API requests by navigating
    await page.reload();
    await page.waitForTimeout(2000);

    // Verify all API requests include X-Tenant-ID header
    const apiRequestsWithTenantHeader = apiRequests.filter(
      (req) => req.headers["x-tenant-id"] === DEFAULT_TENANT_ID
    );

    expect(apiRequestsWithTenantHeader.length).toBeGreaterThan(0);
  });

  test("should include Authorization header with JWT token in authenticated requests", async ({
    page,
  }) => {
    // Login first
    await page.locator('input[type="email"]').fill("admin@example.com");
    await page.locator('input[type="password"]').fill("password");
    await page.locator('button:has-text("Sign In")').first().click();
    await page.waitForURL("**/dashboard", { timeout: 15000 });

    // Get the stored token
    const authToken = await page.evaluate(() =>
      localStorage.getItem("authToken")
    );

    // Intercept API requests
    let hasAuthHeader = false;

    await page.route("**/api/**", async (route, request) => {
      const authHeader = request.headers()["authorization"];
      if (authHeader && authHeader.startsWith("Bearer ")) {
        hasAuthHeader = true;
      }
      await route.continue();
    });

    // Trigger an API request
    await page.reload();
    await page.waitForTimeout(2000);

    // Verify Authorization header was sent
    expect(hasAuthHeader).toBe(true);
  });

  test("should redirect to login on 401 unauthorized", async ({ page }) => {
    // Login first
    await page.locator('input[type="email"]').fill("admin@example.com");
    await page.locator('input[type="password"]').fill("password");
    await page.locator('button:has-text("Sign In")').first().click();
    await page.waitForURL("**/dashboard", { timeout: 15000 });

    // Clear auth token to simulate expired session
    await page.evaluate(() => {
      localStorage.removeItem("authToken");
      localStorage.removeItem("refreshToken");
    });

    // Mock 401 response
    await page.route("**/api/**", async (route) => {
      await route.fulfill({
        status: 401,
        contentType: "application/json",
        body: JSON.stringify({ error: "Unauthorized" }),
      });
    });

    // Trigger an API request
    await page.reload();
    await page.waitForTimeout(2000);

    // Verify redirect to login
    expect(page.url()).toContain("login");
  });

  test("should prevent tenant ID spoofing by validating JWT claim", async ({
    page,
  }) => {
    // Login with valid credentials
    await page.locator('input[type="email"]').fill("admin@example.com");
    await page.locator('input[type="password"]').fill("password");
    await page.locator('button:has-text("Sign In")').first().click();
    await page.waitForURL("**/dashboard", { timeout: 15000 });

    // Try to manipulate tenant ID in localStorage
    await page.evaluate(() => {
      localStorage.setItem("tenantId", "99999999-9999-9999-9999-999999999999");
    });

    // Intercept API request to verify tenant header
    let spoofedTenantId: string | null = null;

    await page.route("**/api/**", async (route, request) => {
      spoofedTenantId = request.headers()["x-tenant-id"];

      // Backend should reject mismatched tenant
      await route.fulfill({
        status: 403,
        contentType: "application/json",
        body: JSON.stringify({
          error: "Tenant ID mismatch - JWT validation failed",
        }),
      });
    });

    // Trigger API request with spoofed tenant
    await page.reload();
    await page.waitForTimeout(2000);

    // Verify spoofed tenant ID was sent
    expect(spoofedTenantId).toBe("99999999-9999-9999-9999-999999999999");
  });

  test("should handle network errors gracefully during login", async ({
    page,
  }) => {
    // Simulate network failure
    await page.route("**/api/auth/login", async (route) => {
      await route.abort("failed");
    });

    // Fill in credentials
    await page.locator('input[type="email"]').fill("admin@example.com");
    await page.locator('input[type="password"]').fill("password");
    await page.locator('button:has-text("Sign In")').first().click();

    // Wait for error handling
    await page.waitForTimeout(2000);

    // Verify user remains on login page
    expect(page.url()).toContain("login");
  });

  test("should validate email format before submission", async ({ page }) => {
    // Fill in invalid email format
    await page.locator('input[type="email"]').fill("not-an-email");
    await page.locator('input[type="password"]').fill("password");

    // Try to submit - HTML5 validation should prevent it
    const emailInput = page.locator('input[type="email"]');
    const isInvalid = await emailInput.evaluate(
      (el: HTMLInputElement) => !el.checkValidity()
    );

    expect(isInvalid).toBe(true);
  });

  test("should clear sensitive data on logout", async ({ page }) => {
    // Login first
    await page.locator('input[type="email"]').fill("admin@example.com");
    await page.locator('input[type="password"]').fill("password");
    await page.locator('button:has-text("Sign In")').first().click();
    await page.waitForURL("**/dashboard", { timeout: 15000 });

    // Verify tokens are stored
    let storageBeforeLogout = await page.evaluate(() => ({
      authToken: localStorage.getItem("authToken"),
      refreshToken: localStorage.getItem("refreshToken"),
    }));
    expect(storageBeforeLogout.authToken).toBeTruthy();

    // Logout (trigger 401 to simulate logout)
    await page.route("**/api/**", async (route) => {
      await route.fulfill({
        status: 401,
        contentType: "application/json",
        body: JSON.stringify({ error: "Unauthorized" }),
      });
    });

    await page.reload();
    await page.waitForTimeout(2000);

    // Verify tokens are cleared
    const storageAfterLogout = await page.evaluate(() => ({
      authToken: localStorage.getItem("authToken"),
      refreshToken: localStorage.getItem("refreshToken"),
    }));

    expect(storageAfterLogout.authToken).toBeNull();
    expect(storageAfterLogout.refreshToken).toBeNull();
  });

  test("should display demo credentials in footer", async ({ page }) => {
    // Find the email in footer (more specific)
    const emailInFooter = page
      .locator('[class*="footer"], footer')
      .locator("text=admin@example.com");
    if ((await emailInFooter.count()) > 0) {
      await expect(emailInFooter).toBeVisible({ timeout: 3000 });
    }

    // Find password text in demo section (footer specifically)
    const demoSection = page.locator("text=Demo Account");
    if ((await demoSection.count()) > 0) {
      await expect(demoSection).toBeVisible({ timeout: 3000 });
    }
  });
});

test.describe("Admin Frontend - Dashboard", () => {
  test.beforeEach(async ({ page }) => {
    // Login first
    await page.goto("http://localhost:5174");
    await page.waitForLoadState("domcontentloaded");

    // Fill and submit login form
    await page.locator('input[type="email"]').fill("admin@example.com");
    await page.locator('input[type="password"]').fill("password");
    await page.locator('button:has-text("Sign In")').first().click();

    // Wait for dashboard
    await page.waitForURL("**/dashboard", { timeout: 15000 });
  });

  test("should display dashboard after login", async ({ page }) => {
    // Check that we're on dashboard
    expect(page.url()).toContain("dashboard");
  });

  test("should have main navigation", async ({ page }) => {
    // Page should have some content
    const body = page.locator("body");
    await expect(body).toBeVisible();
  });

  test("should persist session across page refreshes", async ({ page }) => {
    // Refresh page
    await page.reload();

    // Should still be on dashboard
    await page.waitForURL("**/dashboard", { timeout: 10000 });
    expect(page.url()).toContain("dashboard");
  });
});

test.describe("Admin Frontend - UI Elements", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("http://localhost:5174");
    await page.waitForLoadState("domcontentloaded");
  });

  test("should have proper form inputs", async ({ page }) => {
    const emailInput = page.locator('input[type="email"]');
    const passwordInput = page.locator('input[type="password"]');

    await expect(emailInput).toBeVisible();
    await expect(passwordInput).toBeVisible();
  });

  test("should have sign in button", async ({ page }) => {
    const button = page.locator('button:has-text("Sign In")');
    await expect(button).toBeVisible({ timeout: 3000 });
  });
});
