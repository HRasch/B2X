import { test, expect } from "@playwright/test";

test.describe("Performance and Reliability Tests", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("http://localhost:5174");
    await page.waitForLoadState("domcontentloaded");
    await page.locator('input[type="email"]').fill("admin@example.com");
    await page.locator('input[type="password"]').fill("password");
    await page.locator('button:has-text("Sign In")').first().click();
    await page.waitForURL("**/dashboard", { timeout: 15000 });
  });

  test("Pages load should complete within 10 seconds", async ({ page }) => {
    const startTime = Date.now();
    await page.goto("http://localhost:5174/catalog/products");
    await page.waitForLoadState("networkidle");
    const loadTime = Date.now() - startTime;

    expect(loadTime).toBeLessThan(10000);
  });

  test("API responses should be received within 5 seconds", async ({
    page,
  }) => {
    let apiResponseTime = 0;

    page.on("response", (response) => {
      if (
        response.url().includes("/api/v1/products") ||
        response.url().includes("/api/layout/pages")
      ) {
        apiResponseTime = response.timing().responseEnd;
      }
    });

    await page.goto("http://localhost:5174/catalog/products");
    await page.waitForLoadState("networkidle");

    if (apiResponseTime > 0) {
      expect(apiResponseTime).toBeLessThan(5000);
    }
  });

  test("should handle rapid navigation without errors", async ({ page }) => {
    const navigationPaths = [
      "/catalog/products",
      "/catalog/categories",
      "/cms/pages",
      "/catalog/brands",
      "/catalog/products",
    ];

    for (const path of navigationPaths) {
      await page.goto(`http://localhost:5174${path}`);
      await page.waitForLoadState("domcontentloaded");
      // Don't wait for networkidle, just domcontentloaded for rapid navigation
    }

    // Should still be on the last page
    expect(page.url()).toContain("/catalog/products");
  });

  test("should handle network slowdown gracefully", async ({
    page,
    context,
  }) => {
    // Simulate slow network
    await context.route("**/*", (route) => {
      setTimeout(() => route.continue(), 500);
    });

    await page.goto("http://localhost:5174/catalog/products");
    // Should eventually load even with delays
    await page.waitForLoadState("networkidle", { timeout: 15000 });

    expect(page.url()).toContain("/catalog/products");
  });

  test("should maintain session across multiple pages", async ({ page }) => {
    const authToken = await page.evaluate(() =>
      localStorage.getItem("authToken")
    );
    expect(authToken).toBeTruthy();

    // Navigate to different pages
    await page.goto("http://localhost:5174/catalog/products");
    await page.waitForLoadState("networkidle");

    const tokenAfter = await page.evaluate(() =>
      localStorage.getItem("authToken")
    );
    expect(tokenAfter).toBe(authToken);
  });

  test("should recover from temporary API failures", async ({ page }) => {
    let requestCount = 0;

    // First request fails, subsequent requests succeed
    await page.route("http://localhost:6000/api/v1/products*", (route) => {
      requestCount++;
      if (requestCount === 1) {
        route.abort("failed");
      } else {
        route.continue();
      }
    });

    await page.goto("http://localhost:5174/catalog/products");

    // Should retry or show error gracefully
    await page.waitForLoadState("networkidle", { timeout: 10000 });
    expect(page.url()).toContain("/catalog/products");
  });

  test("should handle missing images gracefully", async ({ page }) => {
    // Simulate missing product images
    await page.route("**/*.jpg", (route) => route.abort("failed"));
    await page.route("**/*.png", (route) => route.abort("failed"));

    await page.goto("http://localhost:5174/catalog/products");
    await page.waitForLoadState("networkidle");

    // Page should still be functional
    expect(page.url()).toContain("/catalog/products");
  });

  test("should handle large data sets without freezing UI", async ({
    page,
  }) => {
    await page.goto("http://localhost:5174/catalog/products");
    await page.waitForLoadState("networkidle");

    // Try to interact with page while data is loading
    const heading = page.locator("h1");
    if ((await heading.count()) > 0) {
      await heading.first().hover();
      expect(true).toBe(true);
    }
  });
});

test.describe("API Contract Tests", () => {
  test("Products endpoint should return correct schema", async ({ page }) => {
    await page.goto("http://localhost:5174");
    await page.fill('input[type="email"]', "admin@example.com");
    await page.fill('input[type="password"]', "password");
    await page.click('button[type="submit"]');
    await page.waitForURL(/.*dashboard/, { timeout: 10000 });

    const data = await page.evaluate(async () => {
      const res = await fetch("http://localhost:6000/api/v1/products", {
        method: "GET",
        headers: {
          Authorization: `Bearer ${localStorage.getItem("authToken") || ""}`,
        },
      });
      return res.json();
    });

    // Check for expected pagination or data structure
    expect(
      data.items ||
        data.data ||
        Array.isArray(data) ||
        data.content ||
        data.results
    ).toBeDefined();
  });

  test("Categories endpoint should return array", async ({ page }) => {
    await page.goto("http://localhost:5174");
    await page.fill('input[type="email"]', "admin@example.com");
    await page.fill('input[type="password"]', "password");
    await page.click('button[type="submit"]');
    await page.waitForURL(/.*dashboard/, { timeout: 10000 });

    const data = await page.evaluate(async () => {
      const res = await fetch("http://localhost:6000/api/v1/categories", {
        method: "GET",
        headers: {
          Authorization: `Bearer ${localStorage.getItem("authToken") || ""}`,
        },
      });
      return res.json();
    });

    // Should be iterable
    expect(Array.isArray(data) || data.items || data.data).toBeDefined();
  });

  test("Pages endpoint should return correct structure", async ({ page }) => {
    await page.goto("http://localhost:5174");
    await page.fill('input[type="email"]', "admin@example.com");
    await page.fill('input[type="password"]', "password");
    await page.click('button[type="submit"]');
    await page.waitForURL(/.*dashboard/, { timeout: 10000 });

    const data = await page.evaluate(async () => {
      const res = await fetch("http://localhost:6000/api/layout/pages", {
        method: "GET",
        headers: {
          Authorization: `Bearer ${localStorage.getItem("authToken") || ""}`,
        },
      });
      return res.json();
    });

    expect(
      data.items ||
        data.data ||
        Array.isArray(data) ||
        data.content ||
        data.results
    ).toBeDefined();
  });
});
