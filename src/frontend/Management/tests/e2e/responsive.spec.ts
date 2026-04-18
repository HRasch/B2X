import { test, expect } from '@playwright/test';

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

      test('Management dashboard layout adapts correctly', async ({ page }) => {
        await page.goto('http://localhost:5175');

        // Check if mobile menu exists on small screens
        if (viewport.width < 768) {
          const mobileMenu = page.locator(
            '[data-testid="mobile-menu"], .mobile-menu, [aria-label*="menu"]'
          );
          const menuExists = (await mobileMenu.count()) > 0;
          if (menuExists) {
            await expect(mobileMenu).toBeVisible();
          }
        } else {
          // Desktop should show sidebar navigation
          const sidebar = page.locator('[data-testid="sidebar"], .sidebar, aside');
          const sidebarExists = (await sidebar.count()) > 0;
          if (sidebarExists) {
            await expect(sidebar.first()).toBeVisible();
          }
        }

        // Content should not overflow horizontally
        const body = page.locator('body');
        const scrollWidth = await body.evaluate(el => el.scrollWidth);
        const clientWidth = await body.evaluate(el => el.clientWidth);
        expect(scrollWidth).toBeLessThanOrEqual(clientWidth + 10); // Allow small tolerance
      });

      test('Data tables are responsive', async ({ page }) => {
        await page.goto('http://localhost:5175/tenants');

        const tables = page.locator('table');
        if ((await tables.count()) > 0) {
          const firstTable = tables.first();
          await expect(firstTable).toBeVisible();

          // On mobile, tables should either be scrollable or converted to cards
          if (viewport.width < 768) {
            // Check if table is in a scrollable container
            const tableContainer = firstTable.locator('..').locator('..');
            const overflow = await tableContainer.evaluate(
              el => window.getComputedStyle(el).overflowX
            );

            // Either scrollable or converted to responsive format
            const isResponsive = overflow === 'auto' || overflow === 'scroll';
            if (!isResponsive) {
              // Check if table is replaced with cards on mobile
              const cards = page.locator('[data-testid="tenant-card"], .tenant-card');
              const hasCards = (await cards.count()) > 0;
              expect(hasCards || isResponsive).toBeTruthy();
            }
          }
        }
      });

      test('Admin forms are usable on small screens', async ({ page }) => {
        await page.goto('http://localhost:5175/tenants/new');

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

      test('Action buttons are accessible on mobile', async ({ page }) => {
        await page.goto('http://localhost:5175/tenants');

        // Check action buttons (edit, delete, etc.)
        const actionButtons = page.locator('button, [role="button"]');
        const buttonCount = await actionButtons.count();

        if (buttonCount > 0) {
          // On mobile, buttons should be properly sized
          if (viewport.width < 768) {
            for (let i = 0; i < Math.min(buttonCount, 5); i++) {
              // Check first 5 buttons
              const button = actionButtons.nth(i);
              const box = await button.boundingBox();
              if (box) {
                expect(box.width).toBeGreaterThanOrEqual(44);
                expect(box.height).toBeGreaterThanOrEqual(44);
              }
            }
          }
        }
      });

      test('Navigation remains accessible', async ({ page }) => {
        await page.goto('http://localhost:5175');

        // Navigation should be accessible regardless of screen size
        const nav = page.locator('nav, [data-testid="navigation"], header');
        await expect(nav.first()).toBeVisible();

        // Check for navigation links
        const navLinks = nav.first().locator('a, button');
        const linkCount = await navLinks.count();
        expect(linkCount).toBeGreaterThan(0);
      });
    });
  }

  test('Orientation changes work correctly', async ({ page, browserName }) => {
    // Skip on webkit due to orientation issues
    test.skip(browserName === 'webkit');

    await page.setViewportSize({ width: 375, height: 667 }); // Mobile portrait
    await page.goto('http://localhost:5175');

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
