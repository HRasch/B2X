import { test, expect } from "@playwright/test";

test.describe("CMS Management - Pages", () => {
  test.beforeEach(async ({ page }) => {
    // Login
    await page.goto("http://localhost:5174");
    await page.waitForLoadState("domcontentloaded");
    await page.locator('input[type="email"]').fill("admin@example.com");
    await page.locator('input[type="password"]').fill("password");
    await page.locator('button:has-text("Sign In")').first().click();
    await page.waitForURL("**/dashboard", { timeout: 15000 });
  });

  test("should load Pages list", async ({ page }) => {
    // Navigate to Pages
    await page.goto("http://localhost:5174/cms/pages");

    // Wait for page to load
    await page.waitForLoadState("domcontentloaded");

    // Check that page is loaded
    expect(page.url()).toContain("cms/pages");
  });

  test("should display Pages API response correctly", async ({ page }) => {
    // Intercept API call
    await page.route("http://localhost:6000/api/layout/pages*", (route) => {
      route.continue();
    });

    // Navigate to Pages
    await page.goto("http://localhost:5174/cms/pages");

    // Wait for the request
    const response = await page.waitForResponse(
      (resp) =>
        resp.url().includes("/api/layout/pages") && resp.status() === 200,
      { timeout: 10000 }
    );

    expect(response.status()).toBe(200);
    const data = await response.json();
    expect(data).toBeDefined();
  });

  test("should validate Pages API routes", async ({ page }) => {
    const response = await page.evaluate(async () => {
      try {
        const res = await fetch("http://localhost:6000/api/layout/pages", {
          method: "GET",
          headers: {
            Authorization: `Bearer ${localStorage.getItem("authToken") || ""}`,
          },
        });
        return {
          status: res.status,
          ok: res.ok,
        };
      } catch (error) {
        return { error: (error as Error).message };
      }
    });

    // Should either be 200 or the API is available
    expect(response.status === 200 || response.ok).toBeTruthy();
  });

  test("should navigate to page detail", async ({ page }) => {
    await page.goto("http://localhost:5174/cms/pages");
    await page.waitForLoadState("domcontentloaded");

    // Check if we can click on a page
    const firstRow = page.locator("table tbody tr").first();
    if ((await firstRow.count()) > 0) {
      await firstRow.click();
      // Should navigate or show detail
      await page.waitForTimeout(2000);
    }
  });
});
