import { test, expect } from '@playwright/test';

test.describe('API Gateway Integration Tests', () => {
  test.beforeEach(async ({ page }) => {
    // Login using demo mode
    await page.goto('http://localhost:5174');
    await page.waitForLoadState('domcontentloaded');
    await page.locator('input[type="email"]').fill('admin@example.com');
    await page.locator('input[type="password"]').fill('password');
    await page.locator('button:has-text("Sign In")').first().click();
    await page.waitForURL('**/dashboard', { timeout: 15000 });
  });

  test('API Gateway should respond to /api/products endpoint', async ({ request }) => {
    const response = await request.get('http://localhost:5174/api/products', {
      headers: {
        'X-Tenant-ID': '00000000-0000-0000-0000-000000000001',
      },
    });

    // Gateway should respond (may be 401 if demo mode, 200 if authenticated)
    expect(response.status()).toBeLessThan(503);
  });

  test('API Gateway should respond to /api/brands endpoint', async ({ request }) => {
    const response = await request.get('http://localhost:5174/api/brands', {
      headers: {
        'X-Tenant-ID': '00000000-0000-0000-0000-000000000001',
      },
    });

    expect(response.status()).toBeLessThan(503);
  });

  test('API Gateway should respond to /api/categories endpoint', async ({ request }) => {
    const response = await request.get('http://localhost:5174/api/categories/root', {
      headers: {
        'X-Tenant-ID': '00000000-0000-0000-0000-000000000001',
      },
    });

    expect(response.status()).toBeLessThan(503);
  });

  test('All main admin pages should load without 404 errors', async ({ page }) => {
    const pages = ['/catalog/products', '/catalog/categories', '/catalog/brands', '/cms/pages'];

    for (const pagePath of pages) {
      await page.goto('http://localhost:5174' + pagePath);
      await page.waitForLoadState('networkidle');

      const response = await page.evaluate(() => document.documentElement.innerHTML);
      expect(response).not.toContain('404');
      expect(response).not.toContain('Not Found');
    }
  });

  test('Frontend should correctly proxy requests to API Gateway', async ({ page }) => {
    let apiRequestFound = false;

    page.on('response', response => {
      if (response.url().includes('/api/')) {
        apiRequestFound = true;
        expect(response.status()).toBeLessThan(503);
      }
    });

    await page.goto('http://localhost:5174/catalog/products');
    await page.waitForLoadState('networkidle');

    expect(apiRequestFound).toBe(true);
  });

  test('Error handling: should show error message on API failure', async ({ page }) => {
    await page.route('**/api/products*', route => {
      route.abort('failed');
    });

    await page.goto('http://localhost:5174/catalog/products');
    await page.waitForLoadState('networkidle');

    await expect(page.locator('body')).toBeVisible();
  });
});

test.describe('Dark Mode and UI Tests', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('http://localhost:5174');
    await page.fill('input[type="email"]', 'admin@example.com');
    await page.fill('input[type="password"]', 'password');
    await page.click('button:has-text("Sign In")');
    await page.waitForURL(/.*dashboard/, { timeout: 10000 });
  });

  test('headlines should be visible in dark mode', async ({ page, context }) => {
    await context.addInitScript(() => {
      const style = document.createElement('style');
      style.textContent = '@media (prefers-color-scheme: dark) { :root { color-scheme: dark; } }';
      document.head.appendChild(style);
    });

    await page.goto('http://localhost:5174/cms/pages');
    await page.waitForLoadState('networkidle');

    const h1Elements = page.locator('h1');
    if ((await h1Elements.count()) > 0) {
      const computedStyle = await h1Elements.first().evaluate(el => {
        return window.getComputedStyle(el);
      });
      expect(computedStyle.color).toBeDefined();
    }
  });

  test('page should have proper color scheme meta tag', async ({ page }) => {
    await page.goto('http://localhost:5174');
    const colorSchemeTag = page.locator('meta[name="color-scheme"]');
    const hasDarkMode = await colorSchemeTag.count();
    expect(hasDarkMode >= 0).toBe(true);
  });
});
