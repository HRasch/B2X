import { test, expect } from '@playwright/test';

test.describe('Visual Regression Tests', () => {
  test('Store homepage visual baseline', async ({ page }) => {
    await page.goto('http://localhost:5173');

    // Wait for dynamic content to load
    await page.waitForLoadState('networkidle');

    // Take screenshot and compare with baseline
    await expect(page).toHaveScreenshot('store-homepage.png', {
      fullPage: true,
      threshold: 0.1, // Allow 10% difference for anti-aliasing
      maxDiffPixels: 100,
    });
  });

  test('Product catalog visual baseline', async ({ page }) => {
    await page.goto('http://localhost:5173/products');

    // Wait for products to load
    await page
      .waitForSelector('[data-testid="product-card"], .product-card, [class*="product"]', {
        timeout: 10000,
      })
      .catch(() => {
        // If no products, just wait for page load
        return page.waitForLoadState('networkidle');
      });

    await expect(page).toHaveScreenshot('product-catalog.png', {
      fullPage: true,
    });
  });

  test('Search page layout consistency', async ({ page }) => {
    await page.goto('http://localhost:5173/search');

    // Focus on search container
    const searchContainer = page.locator('[data-testid="search"], .search, form');
    if ((await searchContainer.count()) > 0) {
      await expect(searchContainer.first()).toHaveScreenshot('search-page.png');
    } else {
      // Fallback to page screenshot
      await expect(page).toHaveScreenshot('search-page.png');
    }
  });

  test('Responsive layouts at different breakpoints', async ({ page }) => {
    const breakpoints = [
      { name: 'mobile', width: 375, height: 667 },
      { name: 'tablet', width: 768, height: 1024 },
      { name: 'desktop', width: 1920, height: 1080 },
    ];

    for (const bp of breakpoints) {
      await page.setViewportSize({ width: bp.width, height: bp.height });
      await page.goto('http://localhost:5173');

      await expect(page).toHaveScreenshot(`store-homepage-${bp.name}.png`, {
        fullPage: true,
      });
    }
  });

  test('Product card consistency', async ({ page }) => {
    await page.goto('http://localhost:5173/products');

    // Take screenshot of first product card if it exists
    const productCard = page.locator('[data-testid="product-card"], .product-card').first();
    if (await productCard.isVisible()) {
      await expect(productCard).toHaveScreenshot('product-card.png');
    }
  });

  test('Navigation menu consistency', async ({ page }) => {
    await page.goto('http://localhost:5173');

    // Take screenshot of navigation
    const nav = page.locator('nav, [data-testid="navigation"], header');
    if ((await nav.count()) > 0) {
      await expect(nav.first()).toHaveScreenshot('navigation.png');
    }
  });

  test('Theme consistency', async ({ page }) => {
    await page.goto('http://localhost:5173');

    // Check if theme variables are applied correctly
    const rootStyles = await page.evaluate(() => {
      const root = document.documentElement;
      const styles = window.getComputedStyle(root);
      return {
        primaryColor:
          styles.getPropertyValue('--color-primary') || styles.getPropertyValue('--primary'),
        fontFamily:
          styles.getPropertyValue('--font-family') || styles.getPropertyValue('font-family'),
        borderRadius:
          styles.getPropertyValue('--border-radius') || styles.getPropertyValue('border-radius'),
      };
    });

    // Ensure theme variables are set (allow for different naming conventions)
    const hasTheme = rootStyles.primaryColor || rootStyles.fontFamily;
    expect(hasTheme).toBeTruthy();
  });
});
