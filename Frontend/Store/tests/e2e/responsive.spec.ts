import { test, expect, devices } from '@playwright/test';

test.describe('Responsive Design Tests', () => {
  // Test different viewport sizes
  const viewports = [
    { name: 'Mobile', width: 375, height: 667 },
    { name: 'Tablet', width: 768, height: 1024 },
    { name: 'Desktop', width: 1920, height: 1080 },
    { name: 'Large Desktop', width: 2560, height: 1440 },
  ];

  for (const viewport of viewports) {
    test.describe(`${viewport.name} (${viewport.width}x${viewport.height})`, () => {
      test.use({ viewport: { width: viewport.width, height: viewport.height } });

      test('Store homepage layout adapts correctly', async ({ page }) => {
        await page.goto('http://localhost:5173');

        // Check if mobile menu exists on small screens
        if (viewport.width < 768) {
          const mobileMenu = page.locator(
            '[data-testid="mobile-menu"], .mobile-menu, [aria-label*="menu"]'
          );
          // Mobile menu might not be immediately visible, check if it exists
          const menuExists = (await mobileMenu.count()) > 0;
          if (menuExists) {
            await expect(mobileMenu).toBeVisible();
          }
        } else {
          // Desktop should show navigation
          const nav = page.locator('nav, [data-testid="navigation"], header');
          await expect(nav.first()).toBeVisible();
        }

        // Content should not overflow horizontally
        const body = page.locator('body');
        const scrollWidth = await body.evaluate(el => el.scrollWidth);
        const clientWidth = await body.evaluate(el => el.clientWidth);
        expect(scrollWidth).toBeLessThanOrEqual(clientWidth + 10); // Allow small tolerance
      });

      test('Product grid is responsive', async ({ page }) => {
        await page.goto('http://localhost:5173/products');

        const productCards = page.locator(
          '[data-testid="product-card"], .product-card, [class*="product"]'
        );

        // Should show products if they exist
        const cardCount = await productCards.count();
        if (cardCount > 0) {
          await expect(productCards.first()).toBeVisible();

          // Check grid layout adapts
          if (viewport.width >= 1024) {
            // Large screens: multiple columns
            const container = page.locator(
              '[data-testid="products-grid"], .products-grid, [class*="grid"]'
            );
            if ((await container.count()) > 0) {
              const gridStyle = await container.evaluate(el => window.getComputedStyle(el).display);
              expect(['grid', 'flex']).toContain(gridStyle);
            }
          }
        }
      });

      test('Search and forms are usable on small screens', async ({ page }) => {
        await page.goto('http://localhost:5173/search');

        // Search input should be accessible
        const searchInput = page.locator('input[type="search"], input[placeholder*="search" i]');
        if ((await searchInput.count()) > 0) {
          await expect(searchInput.first()).toBeVisible();

          // Check if inputs are properly sized for touch on mobile
          if (viewport.width < 768) {
            const height = await searchInput
              .first()
              .evaluate(el => parseInt(window.getComputedStyle(el).height));
            expect(height).toBeGreaterThanOrEqual(44); // iOS touch target size
          }
        }
      });

      test('Shopping cart is accessible on mobile', async ({ page }) => {
        await page.goto('http://localhost:5173');

        // Look for cart icon/button
        const cartButton = page.locator(
          '[data-testid="cart"], .cart, [aria-label*="cart" i], [title*="cart" i]'
        );
        if ((await cartButton.count()) > 0) {
          await expect(cartButton.first()).toBeVisible();

          // On mobile, cart should be easily accessible
          if (viewport.width < 768) {
            const cartRect = await cartButton.first().boundingBox();
            if (cartRect) {
              // Cart should not be too small
              expect(cartRect.width).toBeGreaterThanOrEqual(44);
              expect(cartRect.height).toBeGreaterThanOrEqual(44);
            }
          }
        }
      });
    });
  }

  test('Orientation changes work correctly', async ({ page, browserName }) => {
    // Skip on webkit due to orientation issues
    test.skip(browserName === 'webkit');

    await page.setViewportSize({ width: 375, height: 667 }); // Mobile portrait
    await page.goto('http://localhost:5173');

    // Simulate orientation change
    await page.setViewportSize({ width: 667, height: 375 }); // Mobile landscape

    // Layout should adapt
    const content = page.locator('[data-testid="main-content"], main, #app');
    await expect(content.first()).toBeVisible();

    // Check for horizontal scrolling issues
    const scrollWidth = await page.evaluate(() => document.body.scrollWidth);
    const clientWidth = await page.evaluate(() => document.body.clientWidth);
    expect(scrollWidth).toBeLessThanOrEqual(clientWidth);
  });
});
