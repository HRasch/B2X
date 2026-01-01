import { test, expect } from "@playwright/test";
import {
  loginAsUser,
  logoutUser,
  checkRouteAccess,
  testRouteAccessForRoles,
  TEST_USERS,
} from "./helpers";

const APP_URL = "http://localhost:5174";

test.describe("Browser Navigation - Back Button Support", () => {
  test.describe("Public Pages Navigation", () => {
    test.beforeEach(async ({ page }) => {
      // Navigate to login page first (no auth required for navigation testing)
      await page.goto(`${APP_URL}/login`);
      await page.waitForLoadState("networkidle");
    });

    test("should handle browser back button on login page", async ({
      page,
    }) => {
      // Navigate to unauthorized page
      await page.goto(`${APP_URL}/unauthorized`);
      await page.waitForLoadState("networkidle");

      // Use browser back button
      await page.goBack();
      await expect(page).toHaveURL(/.*\/login/);
    });

    test("should handle forward/back navigation between public pages", async ({
      page,
    }) => {
      // Go to unauthorized
      await page.goto(`${APP_URL}/unauthorized`);
      await page.waitForLoadState("networkidle");

      // Go back to login
      await page.goBack();
      await expect(page).toHaveURL(/.*\/login/);

      // Go forward to unauthorized
      await page.goForward();
      await expect(page).toHaveURL(/.*\/unauthorized/);
    });

    test("should redirect to login when accessing protected routes", async ({
      page,
    }) => {
      // Try to access dashboard without auth
      await page.goto(`${APP_URL}/dashboard`);
      await page.waitForLoadState("networkidle");

      // Should redirect to login
      await expect(page).toHaveURL(/.*\/login/);
    });

    test("should maintain URL after page refresh on public pages", async ({
      page,
    }) => {
      // Navigate to unauthorized page
      await page.goto(`${APP_URL}/unauthorized`);
      await page.waitForLoadState("networkidle");

      // Refresh the page
      await page.reload();
      await page.waitForLoadState("networkidle");

      // Should still be on unauthorized page
      await expect(page).toHaveURL(/.*\/unauthorized/);
    });
  });

  test.describe("Authenticated Navigation", () => {
    test.describe("Admin User Navigation", () => {
      test.beforeEach(async ({ page }) => {
        // Login as admin for authenticated tests
        await loginAsUser(page, "admin");
      });

      test.afterEach(async ({ page }) => {
        await logoutUser(page);
      });

      test("should navigate between main sections and handle back button", async ({
        page,
      }) => {
        // Start on dashboard
        await expect(page.locator('[data-test="main-layout"]')).toBeVisible();
        await expect(page).toHaveURL(/.*\/dashboard/);

        // Navigate to CMS (Benutzer in German)
        await page.click("text=CMS");
        await expect(page).toHaveURL(/.*\/cms\/pages/);
        await expect(page.locator('[data-test="main-layout"]')).toBeVisible();

        // Navigate to Shop
        await page.click("text=Shop");
        await expect(page).toHaveURL(/.*\/shop\/products/);
        await expect(page.locator('[data-test="main-layout"]')).toBeVisible();

        // Navigate to Users (Benutzer)
        await page.click("text=Benutzer");
        await expect(page).toHaveURL(/.*\/users/);
        await expect(page.locator('[data-test="main-layout"]')).toBeVisible();

        // Navigate to Jobs
        await page.click("text=Jobs");
        await expect(page).toHaveURL(/.*\/jobs\/queue/);
        await expect(page.locator('[data-test="main-layout"]')).toBeVisible();

        // Use browser back button - should go back to Users
        await page.goBack();
        await expect(page).toHaveURL(/.*\/users/);
        await expect(page.locator('[data-test="main-layout"]')).toBeVisible();

        // Back again to Shop
        await page.goBack();
        await expect(page).toHaveURL(/.*\/shop\/products/);
        await expect(page.locator('[data-test="main-layout"]')).toBeVisible();

        // Back to CMS
        await page.goBack();
        await expect(page).toHaveURL(/.*\/cms\/pages/);
        await expect(page.locator('[data-test="main-layout"]')).toBeVisible();

        // Back to Dashboard
        await page.goBack();
        await expect(page).toHaveURL(/.*\/dashboard/);
        await expect(page.locator('[data-test="main-layout"]')).toBeVisible();
      });

      test("should handle forward navigation after back button", async ({
        page,
      }) => {
        // Navigate: Dashboard -> CMS -> Shop
        await page.click("text=CMS");
        await page.click("text=Shop");
        await expect(page).toHaveURL(/.*\/shop\/products/);

        // Go back to CMS
        await page.goBack();
        await expect(page).toHaveURL(/.*\/cms\/pages/);

        // Go back to Dashboard
        await page.goBack();
        await expect(page).toHaveURL(/.*\/dashboard/);

        // Go forward to CMS
        await page.goForward();
        await expect(page).toHaveURL(/.*\/cms\/pages/);

        // Go forward to Shop
        await page.goForward();
        await expect(page).toHaveURL(/.*\/shop\/products/);
      });

      test("should handle direct URL navigation to authenticated pages", async ({
        page,
      }) => {
        // Navigate directly to CMS via URL
        await page.goto(`${APP_URL}/cms/pages`);
        await expect(page).toHaveURL(/.*\/cms\/pages/);
        await expect(page.locator('[data-test="main-layout"]')).toBeVisible();

        // Navigate directly to Shop via URL
        await page.goto(`${APP_URL}/shop/products`);
        await expect(page).toHaveURL(/.*\/shop\/products/);
        await expect(page.locator('[data-test="main-layout"]')).toBeVisible();

        // Navigate directly to Users via URL
        await page.goto(`${APP_URL}/users`);
        await expect(page).toHaveURL(/.*\/users/);
        await expect(page.locator('[data-test="main-layout"]')).toBeVisible();

        // Navigate directly to Jobs via URL
        await page.goto(`${APP_URL}/jobs/queue`);
        await expect(page).toHaveURL(/.*\/jobs\/queue/);
        await expect(page.locator('[data-test="main-layout"]')).toBeVisible();
      });

      test("should maintain navigation state after page refresh", async ({
        page,
      }) => {
        // Navigate to CMS
        await page.click("text=CMS");
        await expect(page).toHaveURL(/.*\/cms\/pages/);

        // Refresh the page
        await page.reload();
        await page.waitForSelector('[data-test="main-layout"]', {
          timeout: 10000,
        });

        // Should still be on CMS page
        await expect(page).toHaveURL(/.*\/cms\/pages/);
        await expect(page.locator('[data-test="main-layout"]')).toBeVisible();

        // Navigate to Shop and refresh again
        await page.click("text=Shop");
        await expect(page).toHaveURL(/.*\/shop\/products/);

        await page.reload();
        await page.waitForSelector('[data-test="main-layout"]', {
          timeout: 10000,
        });

        await expect(page).toHaveURL(/.*\/shop\/products/);
        await expect(page.locator('[data-test="main-layout"]')).toBeVisible();
      });

      test("should handle sidebar navigation menu clicks", async ({ page }) => {
        // Test sidebar navigation links
        const navLinks = [
          { text: "Dashboard", url: /.*\/dashboard/ },
          { text: "CMS", url: /.*\/cms\/pages/ },
          { text: "Shop", url: /.*\/shop\/products/ },
          { text: "Benutzer", url: /.*\/users/ },
          { text: "Jobs", url: /.*\/jobs\/queue/ },
        ];

        for (const link of navLinks) {
          await page.click(`text=${link.text}`);
          await expect(page).toHaveURL(link.url);
          await expect(page.locator('[data-test="main-layout"]')).toBeVisible();
        }
      });

      test("should handle logout and redirect to login", async ({ page }) => {
        // Navigate to a different page first
        await page.click("text=CMS");
        await expect(page).toHaveURL(/.*\/cms\/pages/);

        // Click logout (assuming there's a logout button in user menu)
        await page.click('[data-test="user-menu"]'); // Assuming user menu trigger
        await page.click("text=Logout");

        // Should redirect to login
        await expect(page).toHaveURL(/.*\/login/);

        // Try to access dashboard - should stay on login
        await page.goto(`${APP_URL}/dashboard`);
        await expect(page).toHaveURL(/.*\/login/);
      });
    });

    test.describe("TenantAdmin User Navigation", () => {
      test.beforeEach(async ({ page }) => {
        // Login as tenant admin
        await loginAsUser(page, "tenantAdmin");
      });

      test.afterEach(async ({ page }) => {
        await logoutUser(page);
      });

      test("should have access to admin routes as tenant admin", async ({
        page,
      }) => {
        // Tenant admin should have access to all routes (admin bypass)
        await expect(page.locator('[data-test="main-layout"]')).toBeVisible();
        await expect(page).toHaveURL(/.*\/dashboard/);

        // Navigate to CMS
        await page.click("text=CMS");
        await expect(page).toHaveURL(/.*\/cms\/pages/);
        await expect(page.locator('[data-test="main-layout"]')).toBeVisible();
      });
    });

    test.describe("Regular User Navigation", () => {
      test.beforeEach(async ({ page }) => {
        // Login as regular user
        await loginAsUser(page, "regularUser");
      });

      test.afterEach(async ({ page }) => {
        await logoutUser(page);
      });

      test("should have limited access as regular user", async ({ page }) => {
        // Regular user should be redirected to unauthorized for admin routes
        await expect(page).toHaveURL(/.*\/unauthorized/);
      });
    });
  });

  test.describe("Role-based Access Control", () => {
    test("should control access to admin routes based on user role", async ({
      page,
    }) => {
      // Test dashboard access for different roles
      await testRouteAccessForRoles(page, "/dashboard", {
        admin: true,
        tenantAdmin: true,
        regularUser: false,
      });

      // Test CMS access for different roles
      await testRouteAccessForRoles(page, "/cms/pages", {
        admin: true,
        tenantAdmin: true,
        regularUser: false,
      });

      // Test users management access for different roles
      await testRouteAccessForRoles(page, "/users", {
        admin: true,
        tenantAdmin: true,
        regularUser: false,
      });
    });

    test("should redirect to unauthorized for insufficient permissions", async ({
      page,
    }) => {
      // Login as regular user
      await loginAsUser(page, "regularUser");

      // Try to access admin routes - should be redirected to unauthorized
      const hasDashboardAccess = await checkRouteAccess(page, "/dashboard");
      expect(hasDashboardAccess).toBe(false);

      const hasCmsAccess = await checkRouteAccess(page, "/cms/pages");
      expect(hasCmsAccess).toBe(false);

      const hasUsersAccess = await checkRouteAccess(page, "/users");
      expect(hasUsersAccess).toBe(false);

      await logoutUser(page);
    });

    test("should allow tenant admin full access within tenant scope", async ({
      page,
    }) => {
      // Login as tenant admin
      await loginAsUser(page, "tenantAdmin");

      // Tenant admin should have access to all routes (admin bypass)
      const hasDashboardAccess = await checkRouteAccess(page, "/dashboard");
      expect(hasDashboardAccess).toBe(true);

      const hasCmsAccess = await checkRouteAccess(page, "/cms/pages");
      expect(hasCmsAccess).toBe(true);

      const hasUsersAccess = await checkRouteAccess(page, "/users");
      expect(hasUsersAccess).toBe(true);

      await logoutUser(page);
    });
  });
});
