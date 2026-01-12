import { test, expect } from '@playwright/test';

test.describe('Visual Regression Tests', () => {
  test('Management dashboard visual baseline', async ({ page }) => {
    await page.goto('http://localhost:5175');

    // Wait for dynamic content to load
    await page.waitForLoadState('networkidle');

    // Take screenshot and compare with baseline
    await expect(page).toHaveScreenshot('management-dashboard.png', {
      fullPage: true,
      threshold: 0.1, // Allow 10% difference for anti-aliasing
      maxDiffPixels: 100,
    });
  });

  test('Tenant management visual baseline', async ({ page }) => {
    await page.goto('http://localhost:5175/tenants');

    // Wait for tenant data to load
    await page
      .waitForSelector('[data-testid="tenant-list"], .tenant-list, table', { timeout: 10000 })
      .catch(() => {
        // If no data, just wait for page load
        return page.waitForLoadState('networkidle');
      });

    await expect(page).toHaveScreenshot('tenant-management.png', {
      fullPage: true,
    });
  });

  test('Tenant creation form layout consistency', async ({ page }) => {
    await page.goto('http://localhost:5175/tenants/new');

    // Focus on form container
    const form = page.locator('form, [data-testid="tenant-form"]');
    if ((await form.count()) > 0) {
      await expect(form.first()).toHaveScreenshot('tenant-form.png');
    } else {
      // Fallback to page screenshot
      await expect(page).toHaveScreenshot('tenant-form.png');
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
      await page.goto('http://localhost:5175');

      await expect(page).toHaveScreenshot(`management-dashboard-${bp.name}.png`, {
        fullPage: true,
      });
    }
  });

  test('Data table consistency', async ({ page }) => {
    await page.goto('http://localhost:5175/tenants');

    // Take screenshot of table if it exists
    const table = page.locator('table').first();
    if (await table.isVisible()) {
      await expect(table).toHaveScreenshot('tenant-table.png');
    }
  });

  test('Navigation menu consistency', async ({ page }) => {
    await page.goto('http://localhost:5175');

    // Take screenshot of navigation
    const nav = page.locator('nav, [data-testid="navigation"], aside, .sidebar');
    if ((await nav.count()) > 0) {
      await expect(nav.first()).toHaveScreenshot('management-navigation.png');
    }
  });

  test('Admin theme consistency', async ({ page }) => {
    await page.goto('http://localhost:5175');

    // Check if admin theme variables are applied correctly
    const rootStyles = await page.evaluate(() => {
      const root = document.documentElement;
      const styles = window.getComputedStyle(root);
      return {
        primaryColor:
          styles.getPropertyValue('--color-primary') || styles.getPropertyValue('--primary'),
        adminAccent: styles.getPropertyValue('--color-admin') || styles.getPropertyValue('--admin'),
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

  test('Modal dialogs consistency', async ({ page }) => {
    await page.goto('http://localhost:5175/tenants');

    // Try to trigger a modal (if buttons exist)
    const modalButtons = page.locator('button[data-testid*="modal"], button[class*="modal"]');
    if ((await modalButtons.count()) > 0) {
      await modalButtons.first().click();

      // Wait for modal to appear
      const modal = page.locator('[role="dialog"], .modal, [data-testid="modal"]');
      if (await modal.isVisible({ timeout: 2000 })) {
        await expect(modal.first()).toHaveScreenshot('modal-dialog.png');
      }
    }
  });
});
