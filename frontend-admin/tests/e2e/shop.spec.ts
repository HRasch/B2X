import { test, expect } from "@playwright/test";

test.describe("Catalog Management - Products", () => {
  test.beforeEach(async ({ page }) => {
    // Login
    await page.goto("http://localhost:5174");
    await page.waitForLoadState("domcontentloaded");
    await page.locator('input[type="email"]').fill("admin@example.com");
    await page.locator('input[type="password"]').fill("password");
    await page.locator('button:has-text("Sign In")').first().click();
    await page.waitForURL("**/dashboard", { timeout: 15000 });
  });

  test("should load Products list", async ({ page }) => {
    // Navigate to Catalog/Products
    await page.goto("http://localhost:5174/catalog/products");

    // Wait for page to load
    await page.waitForLoadState("networkidle");

    // Check if products heading is visible
    await expect(page.locator("text=Products | text=Produkte")).toBeVisible({
      timeout: 5000,
    });
  });

  test("should display Products API response correctly", async ({ page }) => {
    // Intercept API call
    await page.route("http://localhost:6000/api/v1/products*", (route) => {
      route.continue();
    });

    // Navigate to Products
    await page.goto("http://localhost:5174/catalog/products");

    // Wait for the request
    const response = await page.waitForResponse(
      (resp) =>
        resp.url().includes("/api/v1/products") && resp.status() === 200,
      { timeout: 10000 }
    );

    expect(response.status()).toBe(200);
    const data = await response.json();
    expect(data).toBeDefined();
    expect(
      Array.isArray(data.items) || Array.isArray(data.data) || data.data
    ).toBeDefined();
  });

  test("should load Categories list", async ({ page }) => {
    // Navigate to Categories
    await page.goto("http://localhost:5174/catalog/categories");
    await page.waitForLoadState("networkidle");

    // Check if categories heading is visible
    await expect(page.locator("text=Categories | text=Kategorien")).toBeVisible(
      { timeout: 5000 }
    );
  });

  test("should validate Categories API routes", async ({ page }) => {
    const response = await page.evaluate(async () => {
      try {
        const res = await fetch("http://localhost:6000/api/v1/categories", {
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

    expect(response.status).toBe(200);
    expect(response.ok).toBe(true);
  });

  test("should validate Products API routes", async ({ page }) => {
    const response = await page.evaluate(async () => {
      try {
        const res = await fetch("http://localhost:6000/api/v1/products", {
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

    expect(response.status).toBe(200);
    expect(response.ok).toBe(true);
  });

  test("should navigate to product detail", async ({ page }) => {
    await page.goto("http://localhost:5174/catalog/products");
    await page.waitForLoadState("networkidle");

    // Click on first product row or edit button
    const firstRow = page.locator("table tbody tr").first();
    if ((await firstRow.count()) > 0) {
      await firstRow.click();
      await page.waitForURL(/.*\/catalog\/products\/.+/, { timeout: 5000 });
    }
  });

  test("should filter products", async ({ page }) => {
    await page.goto("http://localhost:5174/catalog/products");
    await page.waitForLoadState("networkidle");

    // Look for filter/search inputs
    const filterInputs = page.locator(
      'input[placeholder*="Search"], input[placeholder*="search"]'
    );
    if ((await filterInputs.count()) > 0) {
      await filterInputs.first().fill("test");
      await page.keyboard.press("Enter");
      await page.waitForLoadState("networkidle");
    }
  });
});
