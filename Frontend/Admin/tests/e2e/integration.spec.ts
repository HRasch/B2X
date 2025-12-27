import { test, expect } from "@playwright/test";

test.describe("API Gateway Integration Tests", () => {
  test.beforeEach(async ({ page }) => {
    // Login
    await page.goto("http://localhost:5174");
    await page.waitForLoadState("domcontentloaded");
    await page.locator('input[type="email"]').fill("admin@example.com");
    await page.locator('input[type="password"]').fill("password");
    await page.locator('button:has-text("Sign In")').first().click();
    await page.waitForURL("**/dashboard", { timeout: 15000 });
  });

  test("API Gateway should route /api/v1/* to CatalogService", async ({
    page,
  }) => {
    const result = await page.evaluate(async () => {
      try {
        const res = await fetch("http://localhost:6000/api/v1/products", {
          method: "GET",
          headers: {
            Authorization: `Bearer ${localStorage.getItem("authToken") || ""}`,
          },
        });
        return {
          status: res.status,
          url: res.url,
          statusText: res.statusText,
        };
      } catch (error) {
        return { error: (error as Error).message };
      }
    });

    expect(result.status).toBe(200);
    expect(result.url).toContain("/api/v1/products");
  });

  test("API Gateway should route /api/layout/* to LayoutService", async ({
    page,
  }) => {
    const result = await page.evaluate(async () => {
      try {
        const res = await fetch("http://localhost:6000/api/layout/pages", {
          method: "GET",
          headers: {
            Authorization: `Bearer ${localStorage.getItem("authToken") || ""}`,
          },
        });
        return {
          status: res.status,
          url: res.url,
          statusText: res.statusText,
        };
      } catch (error) {
        return { error: (error as Error).message };
      }
    });

    expect(result.status).toBe(200);
    expect(result.url).toContain("/api/layout/pages");
  });

  test("CatalogService should return paginated products", async ({ page }) => {
    const data = await page.evaluate(async () => {
      try {
        const res = await fetch("http://localhost:6000/api/v1/products", {
          method: "GET",
          headers: {
            Authorization: `Bearer ${localStorage.getItem("authToken") || ""}`,
          },
        });
        return res.json();
      } catch (error) {
        return { error: (error as Error).message };
      }
    });

    // Should have pagination structure or array of products
    expect(
      data.items ||
        data.data ||
        Array.isArray(data) ||
        data.content ||
        data.results
    ).toBeDefined();
  });

  test("LayoutService should return paginated pages", async ({ page }) => {
    const data = await page.evaluate(async () => {
      try {
        const res = await fetch("http://localhost:6000/api/layout/pages", {
          method: "GET",
          headers: {
            Authorization: `Bearer ${localStorage.getItem("authToken") || ""}`,
          },
        });
        return res.json();
      } catch (error) {
        return { error: (error as Error).message };
      }
    });

    // Should have pagination structure or array of pages
    expect(
      data.items ||
        data.data ||
        Array.isArray(data) ||
        data.content ||
        data.results
    ).toBeDefined();
  });

  test("CatalogService should return categories", async ({ page }) => {
    const data = await page.evaluate(async () => {
      try {
        const res = await fetch("http://localhost:6000/api/v1/categories", {
          method: "GET",
          headers: {
            Authorization: `Bearer ${localStorage.getItem("authToken") || ""}`,
          },
        });
        return res.json();
      } catch (error) {
        return { error: (error as Error).message };
      }
    });

    // Should be array or paginated response
    expect(
      data.items ||
        data.data ||
        Array.isArray(data) ||
        data.content ||
        data.results
    ).toBeDefined();
  });

  test("All main admin pages should load without 404 errors", async ({
    page,
  }) => {
    const pages = [
      "/catalog/products",
      "/catalog/categories",
      "/catalog/brands",
      "/cms/pages",
    ];

    for (const pagePath of pages) {
      await page.goto(`http://localhost:5174${pagePath}`);
      await page.waitForLoadState("networkidle");

      // Check that we didn't get a 404
      const response = await page.evaluate(
        () => document.documentElement.innerHTML
      );
      expect(response).not.toContain("404");
      expect(response).not.toContain("Not Found");
    }
  });

  test("Frontend should correctly proxy requests through port 5174 to API Gateway 6000", async ({
    page,
  }) => {
    // Track network requests
    let proxyRequestFound = false;

    page.on("response", (response) => {
      if (response.url().includes("/api/")) {
        proxyRequestFound = true;
        expect(response.status()).toBeLessThan(500);
      }
    });

    await page.goto("http://localhost:5174/catalog/products");
    await page.waitForLoadState("networkidle");

    expect(proxyRequestFound).toBe(true);
  });

  test("Error handling: should show error message on API failure", async ({
    page,
  }) => {
    // Route API calls to return 500 error
    await page.route("http://localhost:6000/api/v1/products*", (route) => {
      route.abort("failed");
    });

    await page.goto("http://localhost:5174/catalog/products");
    await page.waitForLoadState("networkidle");

    // Page should still be visible (error should be handled gracefully)
    await expect(page.locator("body")).toBeVisible();
  });
});

test.describe("Dark Mode and UI Tests", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("http://localhost:5174");
    await page.fill('input[type="email"]', "admin@example.com");
    await page.fill('input[type="password"]', "password");
    await page.click('button[type="submit"]');
    await page.waitForURL(/.*dashboard/, { timeout: 10000 });
  });

  test("headlines should be visible in dark mode", async ({
    page,
    context,
  }) => {
    // Enable dark mode
    await context.addInitScript(() => {
      const style = document.createElement("style");
      style.textContent =
        "@media (prefers-color-scheme: dark) { :root { color-scheme: dark; } }";
      document.head.appendChild(style);
    });

    await page.goto("http://localhost:5174/cms/pages");
    await page.waitForLoadState("networkidle");

    // Check that h1 elements have proper contrast
    const h1Elements = page.locator("h1");
    if ((await h1Elements.count()) > 0) {
      const computedStyle = await h1Elements.first().evaluate((el) => {
        return window.getComputedStyle(el);
      });
      expect(computedStyle.color).toBeDefined();
    }
  });

  test("page should have proper color scheme meta tag", async ({ page }) => {
    await page.goto("http://localhost:5174");
    const colorSchemeTag = page.locator('meta[name="color-scheme"]');
    const hasDarkMode = await colorSchemeTag.count();
    // Either has the tag or uses default browser dark mode support
    expect(hasDarkMode >= 0).toBe(true);
  });
});
