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

      test('Dashboard layout adapts correctly', async ({ page }) => {
        await page.goto('http://localhost:5174/dashboard');

        // Check if mobile menu exists on small screens
        if (viewport.width < 768) {
          const mobileMenu = page.locator('[data-testid="mobile-menu"]');
          await expect(mobileMenu).toBeVisible();
        } else {
          // Desktop should show sidebar
          const sidebar = page.locator('[data-testid="sidebar"]');
          await expect(sidebar).toBeVisible();
        }

        // Content should not overflow horizontally
        const body = page.locator('body');
        const scrollWidth = await body.evaluate(el => el.scrollWidth);
        const clientWidth = await body.evaluate(el => el.clientWidth);
        expect(scrollWidth).toBeLessThanOrEqual(clientWidth + 10); // Allow small tolerance
      });

      test('Product grid is responsive', async ({ page }) => {
        await page.goto('http://localhost:5174/catalog/products');

        const productCards = page.locator('[data-testid="product-card"]');

        // Should show products
        await expect(productCards.first()).toBeVisible();

        // Check grid layout adapts
        if (viewport.width >= 1024) {
          // Large screens: multiple columns
          const container = page.locator('[data-testid="products-grid"]');
          const gridStyle = await container.evaluate(el => window.getComputedStyle(el).display);
          expect(['grid', 'flex']).toContain(gridStyle);
        }
      });

      test('Forms are usable on small screens', async ({ page }) => {
        await page.goto('http://localhost:5174/catalog/products/new');

        // Form inputs should be accessible
        const inputs = page.locator('input, textarea, select');
        await expect(inputs.first()).toBeVisible();

        // Check if inputs are properly sized for touch on mobile
        if (viewport.width < 768) {
          for (const input of await inputs.all()) {
            const height = await input.evaluate(el => parseInt(window.getComputedStyle(el).height));
            expect(height).toBeGreaterThanOrEqual(44); // iOS touch target size
          }
        }
      });
    });
  }

  test('Orientation changes work correctly', async ({ page, browserName }) => {
    // Skip on webkit due to orientation issues
    test.skip(browserName === 'webkit');

    await page.setViewportSize({ width: 375, height: 667 }); // Mobile portrait
    await page.goto('http://localhost:5174/dashboard');

    // Simulate orientation change
    await page.setViewportSize({ width: 667, height: 375 }); // Mobile landscape

    // Layout should adapt
    const content = page.locator('[data-testid="main-content"]');
    await expect(content).toBeVisible();

    // Check for horizontal scrolling issues
    const scrollWidth = await page.evaluate(() => document.body.scrollWidth);
    const clientWidth = await page.evaluate(() => document.body.clientWidth);
    expect(scrollWidth).toBeLessThanOrEqual(clientWidth);
  });
});
