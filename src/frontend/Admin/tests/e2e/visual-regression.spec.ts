import { test, expect } from '@playwright/test';

test.describe('Visual Regression Tests', () => {
  test('Dashboard visual baseline', async ({ page }) => {
    await page.goto('http://localhost:5174/dashboard');

    // Wait for dynamic content to load
    await page.waitForLoadState('networkidle');

    // Take screenshot and compare with baseline
    await expect(page).toHaveScreenshot('dashboard.png', {
      fullPage: true,
      threshold: 0.1, // Allow 10% difference for anti-aliasing
      maxDiffPixels: 100,
    });
  });

  test('Product catalog visual baseline', async ({ page }) => {
    await page.goto('http://localhost:5174/catalog/products');

    // Wait for products to load
    await page.waitForSelector('[data-testid="product-card"]');

    await expect(page).toHaveScreenshot('product-catalog.png', {
      fullPage: true,
    });
  });

  test('Form layouts remain consistent', async ({ page }) => {
    await page.goto('http://localhost:5174/catalog/products/new');

    // Focus on form container to avoid layout shifts
    const form = page.locator('form');
    await expect(form).toHaveScreenshot('product-form.png');
  });

  test('Responsive layouts at different breakpoints', async ({ page }) => {
    const breakpoints = [
      { name: 'mobile', width: 375, height: 667 },
      { name: 'tablet', width: 768, height: 1024 },
      { name: 'desktop', width: 1920, height: 1080 },
    ];

    for (const bp of breakpoints) {
      await page.setViewportSize({ width: bp.width, height: bp.height });
      await page.goto('http://localhost:5174/dashboard');

      await expect(page).toHaveScreenshot(`dashboard-${bp.name}.png`, {
        fullPage: true,
      });
    }
  });

  test('Theme consistency', async ({ page }) => {
    await page.goto('http://localhost:5174/dashboard');

    // Check if theme variables are applied correctly
    const rootStyles = await page.evaluate(() => {
      const root = document.documentElement;
      const styles = window.getComputedStyle(root);
      return {
        primaryColor: styles.getPropertyValue('--color-primary'),
        fontFamily: styles.getPropertyValue('--font-family'),
        borderRadius: styles.getPropertyValue('--border-radius'),
      };
    });

    // Ensure theme variables are set
    expect(rootStyles.primaryColor).toBeTruthy();
    expect(rootStyles.fontFamily).toBeTruthy();
  });
});
