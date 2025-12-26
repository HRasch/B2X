import { test, expect } from "@playwright/test";

test.describe("Admin Frontend - Authentication", () => {
  test.beforeEach(async ({ page }) => {
    // Navigate to the login page
    await page.goto("http://localhost:5174");
  });

  test("should display login form with email and password fields", async ({
    page,
  }) => {
    // Check for login form elements
    await expect(page.locator('input[type="email"]')).toBeVisible();
    await expect(page.locator('input[type="password"]')).toBeVisible();
    await expect(page.locator('button[type="submit"]')).toBeVisible();
  });

  test("should show error message with invalid credentials", async ({
    page,
  }) => {
    // Fill in invalid credentials
    await page.fill('input[type="email"]', "invalid@example.com");
    await page.fill('input[type="password"]', "wrongpassword");

    // Submit form
    await page.click('button[type="submit"]');

    // Wait for error message
    await expect(page.locator("text=Invalid credentials")).toBeVisible({
      timeout: 5000,
    });
  });

  test("should successfully login with valid credentials", async ({ page }) => {
    // Fill in valid credentials
    await page.fill('input[type="email"]', "admin@example.com");
    await page.fill('input[type="password"]', "password");

    // Submit form
    await page.click('button[type="submit"]');

    // Wait for navigation to dashboard
    await expect(page).toHaveURL(/.*dashboard/, { timeout: 10000 });

    // Check that dashboard is loaded
    await expect(page.locator("text=Dashboard")).toBeVisible({ timeout: 5000 });
  });

  test("should store auth token in localStorage after successful login", async ({
    page,
  }) => {
    // Fill in valid credentials
    await page.fill('input[type="email"]', "admin@example.com");
    await page.fill('input[type="password"]', "password");

    // Submit form
    await page.click('button[type="submit"]');

    // Wait for navigation
    await expect(page).toHaveURL(/.*dashboard/, { timeout: 10000 });

    // Check localStorage
    const token = await page.evaluate(() => localStorage.getItem("authToken"));
    expect(token).toBeTruthy();
    expect(token).not.toBeNull();
  });

  test("should display remember me checkbox", async ({ page }) => {
    const rememberMeCheckbox = page.locator('input[type="checkbox"]');
    await expect(rememberMeCheckbox).toBeVisible();
  });

  test("should disable submit button while loading", async ({ page }) => {
    await page.fill('input[type="email"]', "admin@example.com");
    await page.fill('input[type="password"]', "password");

    const submitButton = page.locator('button[type="submit"]');
    await submitButton.click();

    // Button should be disabled while loading
    await expect(submitButton).toBeDisabled({ timeout: 2000 });

    // Button should be enabled again after response
    await expect(submitButton).toBeEnabled({ timeout: 10000 });
  });

  test("should display demo credentials in footer", async ({ page }) => {
    await expect(page.locator("text=admin@example.com")).toBeVisible();
    await expect(page.locator("text=password")).toBeVisible();
  });
});

test.describe("Admin Frontend - Dashboard", () => {
  test.beforeEach(async ({ page }) => {
    // Login first
    await page.goto("http://localhost:5174");
    await page.fill('input[type="email"]', "admin@example.com");
    await page.fill('input[type="password"]', "password");
    await page.click('button[type="submit"]');

    // Wait for dashboard
    await expect(page).toHaveURL(/.*dashboard/, { timeout: 10000 });
  });

  test("should display dashboard with navigation", async ({ page }) => {
    // Check for main navigation elements
    await expect(page.locator("text=Dashboard")).toBeVisible();

    // Dashboard should be rendered
    await expect(page.locator('[class*="dashboard"]')).toBeVisible({
      timeout: 5000,
    });
  });

  test("should have functioning navigation sidebar", async ({ page }) => {
    // Check if sidebar is visible
    const sidebar = page.locator('[class*="sidebar"]');
    await expect(sidebar).toBeVisible({ timeout: 5000 });
  });

  test("should display user info in header", async ({ page }) => {
    // Check for user menu or user info
    const userSection = page.locator("text=Admin");
    await expect(userSection).toBeVisible({ timeout: 5000 });
  });

  test("should allow user to logout", async ({ page }) => {
    // Find and click logout button (depends on your UI)
    const logoutButton = page.locator(
      'button:has-text("Logout"), [class*="logout"]'
    );

    if ((await logoutButton.count()) > 0) {
      await logoutButton.click();

      // Should redirect to login
      await expect(page).toHaveURL(/.*login|auth/, { timeout: 5000 });
    }
  });

  test("should persist session across page refreshes", async ({ page }) => {
    // Get current URL
    const currentUrl = page.url();

    // Refresh page
    await page.reload();

    // Should still be on dashboard (session persisted)
    await expect(page).toHaveURL(/.*dashboard/, { timeout: 10000 });
  });

  test("should redirect to login if token is invalid", async ({ page }) => {
    // Clear auth token
    await page.evaluate(() => localStorage.removeItem("authToken"));

    // Refresh page
    await page.reload();

    // Should redirect to login
    await expect(page).toHaveURL(/.*login|auth/, { timeout: 5000 });
  });
});

test.describe("Admin Frontend - Soft UI Design", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("http://localhost:5174");
  });

  test("should apply Soft UI styling to form elements", async ({ page }) => {
    // Check if Soft UI classes are applied
    const emailInput = page.locator('input[type="email"]');
    const classList = await emailInput.evaluate((el) => el.className);

    // Check for soft UI related classes
    expect(classList).toMatch(/soft|input-soft/);
  });

  test("should display gradient background on login page", async ({ page }) => {
    const loginContainer = page.locator('[class*="gradient"]');
    const isVisible = await loginContainer.isVisible().catch(() => false);

    if (isVisible) {
      // Check if gradient classes are applied
      const containerClass = await loginContainer.evaluate(
        (el) => el.className
      );
      expect(containerClass).toMatch(/gradient|soft/);
    }
  });

  test("should have proper typography and spacing", async ({ page }) => {
    const heading = page.locator("h1");
    const isVisible = await heading.isVisible();

    expect(isVisible).toBeTruthy();
  });
});

test.describe("Admin Frontend - Accessibility", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("http://localhost:5174");
  });

  test("should have proper form labels", async ({ page }) => {
    const labels = page.locator("label");
    expect(await labels.count()).toBeGreaterThan(0);
  });

  test("should have proper button text", async ({ page }) => {
    const button = page.locator('button[type="submit"]');
    const text = await button.textContent();

    expect(text).toMatch(/sign in|login|submit/i);
  });

  test("should support keyboard navigation", async ({ page }) => {
    // Tab to email field
    await page.keyboard.press("Tab");
    const emailInput = page.locator('input[type="email"]');
    await expect(emailInput).toBeFocused();

    // Tab to password field
    await page.keyboard.press("Tab");
    const passwordInput = page.locator('input[type="password"]');
    await expect(passwordInput).toBeFocused();
  });
});
