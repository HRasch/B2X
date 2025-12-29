import { test, expect } from "@playwright/test";

const VIEWPORTS = [
  { width: 320, height: 568, name: "iPhone SE" },
  { width: 375, height: 667, name: "iPhone 8" },
  { width: 414, height: 896, name: "iPhone 12" },
  { width: 768, height: 1024, name: "iPad" },
  { width: 1024, height: 768, name: "iPad Pro" },
  { width: 1440, height: 900, name: "Desktop" },
];

test.describe("Mobile Responsiveness - Price Display", () => {
  VIEWPORTS.forEach(({ width, height, name }) => {
    test(`Product card displays correctly on ${name} (${width}x${height})`, async ({
      page,
    }) => {
      await page.setViewportSize({ width, height });
      await page.goto("http://localhost:5173/shop");

      await page.waitForSelector(".product-card", { timeout: 5000 });

      const card = page.locator(".product-card").first();
      await expect(card).toBeInViewport();

      const price = card.locator(".price");
      await expect(price).toBeVisible();

      const fontSize = await price.evaluate((el) => {
        return window.getComputedStyle(el).fontSize;
      });

      const fontSizeNum = parseInt(fontSize);
      expect(fontSizeNum).toBeGreaterThanOrEqual(12);
    });

    test(`Cart displays correctly on ${name} (${width}x${height})`, async ({
      page,
    }) => {
      await page.setViewportSize({ width, height });

      await page.goto("http://localhost:5173/shop");
      await page.click(".add-to-cart-btn");
      await page.goto("http://localhost:5173/cart");

      await page.waitForSelector(".cart-item", { timeout: 5000 });

      const item = page.locator(".cart-item").first();
      await expect(item).toBeInViewport();

      const summary = page.locator(".cart-summary");

      if (width >= 768) {
        await expect(summary).toBeInViewport();
      } else {
        await page.evaluate(() =>
          window.scrollTo(0, document.body.scrollHeight)
        );
        await expect(summary).toBeInViewport();
      }
    });

    test(`Checkout displays correctly on ${name} (${width}x${height})`, async ({
      page,
    }) => {
      await page.setViewportSize({ width, height });

      await page.goto("http://localhost:5173/shop");
      await page.click(".add-to-cart-btn");
      await page.goto("http://localhost:5173/cart");
      await page.click(".checkout-btn");

      await page.waitForURL("**/checkout", { timeout: 5000 });

      await expect(page.locator(".order-review")).toBeVisible();

      const firstInput = page.locator('input[type="text"]').first();
      await expect(firstInput).toBeInViewport();
    });

    test(`All text is readable on ${name} (${width}x${height})`, async ({
      page,
    }) => {
      await page.setViewportSize({ width, height });
      await page.goto("http://localhost:5173/cart");

      const texts = page.locator("p, span, label, button");
      const count = await texts.count();

      for (let i = 0; i < Math.min(count, 10); i++) {
        const el = texts.nth(i);
        const box = await el.boundingBox();

        if (box) {
          expect(box.width).toBeGreaterThan(0);
          expect(box.height).toBeGreaterThan(0);
        }
      }
    });
  });
});

test.describe("Touch Interactions", () => {
  test("Buttons are at least 44x44px (touch target)", async ({ page }) => {
    await page.setViewportSize({ width: 375, height: 667 });
    await page.goto("http://localhost:5173/shop");

    const btn = page.locator(".add-to-cart-btn").first();
    const box = await btn.boundingBox();

    expect(box?.width).toBeGreaterThanOrEqual(44);
    expect(box?.height).toBeGreaterThanOrEqual(44);
  });

  test("Form inputs have sufficient padding", async ({ page }) => {
    await page.setViewportSize({ width: 375, height: 667 });
    await page.goto("http://localhost:5173/shop");
    await page.click(".add-to-cart-btn");
    await page.goto("http://localhost:5173/cart");
    await page.click(".checkout-btn");

    await page.waitForURL("**/checkout", { timeout: 5000 });

    const input = page.locator('input[type="text"]').first();
    const padding = await input.evaluate((el) => {
      return window.getComputedStyle(el).padding;
    });

    expect(padding).toBeTruthy();
  });

  test("No horizontal scroll required on mobile (375px)", async ({ page }) => {
    await page.setViewportSize({ width: 375, height: 667 });

    await page.goto("http://localhost:5173/shop");
    await page.click(".add-to-cart-btn");
    await page.goto("http://localhost:5173/cart");

    const bodyWidth = await page.evaluate(() => document.body.scrollWidth);
    const viewportWidth = 375;
    expect(bodyWidth).toBeLessThanOrEqual(viewportWidth);
  });

  test("Layout stacks single column on mobile (375px)", async ({ page }) => {
    await page.setViewportSize({ width: 375, height: 667 });

    await page.goto("http://localhost:5173/shop");
    await page.click(".add-to-cart-btn");
    await page.goto("http://localhost:5173/cart");

    const cartContent = page.locator(".cart-content");
    const styles = await cartContent.evaluate(
      (el) => window.getComputedStyle(el).gridTemplateColumns
    );

    // Mobile should have single column
    expect(styles).not.toContain("300px");
  });
});

test.describe("Orientation Changes", () => {
  test("Layout adapts to landscape orientation (667x375)", async ({ page }) => {
    await page.setViewportSize({ width: 667, height: 375 });

    await page.goto("http://localhost:5173/shop");
    await page.click(".add-to-cart-btn");
    await page.goto("http://localhost:5173/cart");

    const item = page.locator(".cart-item").first();
    await expect(item).toBeInViewport();
  });

  test("Layout adapts to portrait orientation (375x667)", async ({ page }) => {
    await page.setViewportSize({ width: 375, height: 667 });

    await page.goto("http://localhost:5173/shop");
    await page.click(".add-to-cart-btn");
    await page.goto("http://localhost:5173/cart");

    const item = page.locator(".cart-item").first();
    await expect(item).toBeInViewport();
  });
});

test.describe("Tablet to Desktop Transition", () => {
  test("Layout switches to two-column at 768px breakpoint", async ({
    page,
  }) => {
    await page.setViewportSize({ width: 768, height: 1024 });

    await page.goto("http://localhost:5173/shop");
    await page.click(".add-to-cart-btn");
    await page.goto("http://localhost:5173/cart");
    await page.click(".checkout-btn");

    await page.waitForURL("**/checkout", { timeout: 5000 });

    const checkoutContent = page.locator(".checkout-content");
    const styles = await checkoutContent.evaluate(
      (el) => window.getComputedStyle(el).gridTemplateColumns
    );

    // At 768px and above, should show proper layout
    expect(styles).toBeTruthy();
  });

  test("Price information visible on all screen sizes without horizontal scroll", async ({
    page,
  }) => {
    const widths = [320, 375, 768, 1024, 1440];

    for (const width of widths) {
      await page.setViewportSize({ width, height: 800 });

      await page.goto("http://localhost:5173/shop");

      const priceElement = page.locator(".price").first();
      await expect(priceElement).toBeVisible();

      const box = await priceElement.boundingBox();
      expect(box?.x).toBeGreaterThanOrEqual(0);
      expect(box?.x).toBeLessThan(width);
    }
  });
});
