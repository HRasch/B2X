import { test, expect, type Page, type ConsoleMessage } from '@playwright/test';

/**
 * Build Health Tests
 *
 * These tests verify that the Vite build and CSS compilation works correctly.
 * They catch:
 * - Tailwind CSS compilation errors (e.g., unknown utility classes)
 * - DaisyUI component loading issues
 * - JavaScript/TypeScript compilation errors
 * - Module resolution errors
 *
 * Run with: npm run e2e -- --grep "Build Health"
 */

test.describe('Build Health - Vite Compilation', () => {
  test('should load app without console errors', async ({ page }: { page: Page }) => {
    const consoleErrors: string[] = [];
    const consoleWarnings: string[] = [];

    // Collect console messages
    page.on('console', (msg: ConsoleMessage) => {
      if (msg.type() === 'error') {
        consoleErrors.push(msg.text());
      }
      if (msg.type() === 'warning' && msg.text().includes('CSS')) {
        consoleWarnings.push(msg.text());
      }
    });

    // Listen for page errors (uncaught exceptions)
    const pageErrors: Error[] = [];
    page.on('pageerror', (error: Error) => {
      pageErrors.push(error);
    });

    await page.goto('/');
    await page.waitForLoadState('networkidle');

    // Check for CSS-related errors (Tailwind/DaisyUI)
    const cssErrors = consoleErrors.filter(
      err =>
        err.includes('CSS') ||
        err.includes('utility class') ||
        err.includes('@apply') ||
        err.includes('tailwind') ||
        err.includes('daisyui')
    );

    expect(cssErrors, 'Should have no CSS compilation errors').toHaveLength(0);
    expect(pageErrors, 'Should have no uncaught JavaScript errors').toHaveLength(0);
  });

  test('should not have Vite HMR errors', async ({ page }: { page: Page }) => {
    const viteErrors: string[] = [];

    page.on('console', (msg: ConsoleMessage) => {
      const text = msg.text();
      if (text.includes('[vite]') && msg.type() === 'error') {
        viteErrors.push(text);
      }
    });

    await page.goto('/');
    await page.waitForLoadState('networkidle');

    // Wait a bit for HMR to stabilize
    await page.waitForTimeout(1000);

    expect(viteErrors, 'Should have no Vite HMR errors').toHaveLength(0);
  });

  test('should load all critical CSS', async ({ page }: { page: Page }) => {
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    // Check that stylesheets are loaded
    const stylesheets = await page.evaluate(() => {
      return Array.from(document.styleSheets).map(sheet => ({
        href: sheet.href,
        rulesCount: sheet.cssRules?.length || 0,
      }));
    });

    // Should have at least one stylesheet with rules
    const hasStyles = stylesheets.some(s => s.rulesCount > 0);
    expect(hasStyles, 'Should have loaded CSS with rules').toBe(true);
  });
});

test.describe('Build Health - Tailwind CSS Classes', () => {
  test('should render Tailwind utility classes correctly', async ({ page }: { page: Page }) => {
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    // Test that common Tailwind classes are applied correctly
    const testCases = [
      { class: 'flex', property: 'display', expected: 'flex' },
      { class: 'hidden', property: 'display', expected: 'none' },
      { class: 'text-center', property: 'text-align', expected: 'center' },
    ];

    for (const testCase of testCases) {
      // Create a test element with the class
      const result = await page.evaluate(tc => {
        const el = document.createElement('div');
        el.className = tc.class;
        el.id = 'tailwind-test-element';
        document.body.appendChild(el);
        const style = window.getComputedStyle(el);
        const value = style.getPropertyValue(tc.property);
        document.body.removeChild(el);
        return value;
      }, testCase);

      expect(result, `Tailwind class "${testCase.class}" should work`).toBe(testCase.expected);
    }
  });

  test('should render DaisyUI component classes correctly', async ({ page }: { page: Page }) => {
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    // Test DaisyUI semantic classes
    const daisyClasses = ['btn', 'card', 'badge', 'alert'];

    for (const className of daisyClasses) {
      const hasStyles = await page.evaluate(cls => {
        const el = document.createElement('div');
        el.className = cls;
        document.body.appendChild(el);
        const style = window.getComputedStyle(el);
        // DaisyUI classes should add some styling
        const hasBackground = style.backgroundColor !== 'rgba(0, 0, 0, 0)';
        const hasPadding = style.padding !== '0px';
        const hasBorderRadius = style.borderRadius !== '0px';
        document.body.removeChild(el);
        return hasBackground || hasPadding || hasBorderRadius;
      }, className);

      expect(hasStyles, `DaisyUI class "${className}" should apply styles`).toBe(true);
    }
  });

  test('should render DaisyUI color semantic classes', async ({ page }: { page: Page }) => {
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    // Test DaisyUI semantic color classes
    const colorClasses = ['bg-base-100', 'bg-base-200', 'bg-primary', 'text-base-content'];

    for (const className of colorClasses) {
      const hasColor = await page.evaluate(cls => {
        const el = document.createElement('div');
        el.className = cls;
        el.textContent = 'test';
        document.body.appendChild(el);
        const style = window.getComputedStyle(el);
        const bg = style.backgroundColor;
        const color = style.color;
        document.body.removeChild(el);
        // Should have some color applied (not transparent/default)
        return bg !== 'rgba(0, 0, 0, 0)' || color !== 'rgb(0, 0, 0)';
      }, className);

      expect(hasColor, `DaisyUI class "${className}" should apply color`).toBe(true);
    }
  });
});

test.describe('Build Health - Page Load Verification', () => {
  const criticalPages = [
    { path: '/', name: 'Home' },
    { path: '/login', name: 'Login' },
    { path: '/products', name: 'Products' },
  ];

  for (const pageInfo of criticalPages) {
    test(`should load ${pageInfo.name} page without errors`, async ({ page }: { page: Page }) => {
      const errors: string[] = [];

      page.on('console', (msg: ConsoleMessage) => {
        if (msg.type() === 'error') {
          errors.push(msg.text());
        }
      });

      page.on('pageerror', (error: Error) => {
        errors.push(error.message);
      });

      const response = await page.goto(pageInfo.path);

      // Check HTTP response
      expect(response?.status(), `${pageInfo.name} should return 200 OK`).toBeLessThan(400);

      await page.waitForLoadState('networkidle');

      // Filter critical errors
      const criticalErrors = errors.filter(
        err =>
          !err.includes('favicon') &&
          !err.includes('404') &&
          !err.includes('net::ERR') &&
          !err.includes('API') // Ignore API errors in this test
      );

      expect(criticalErrors, `${pageInfo.name} should have no critical errors`).toHaveLength(0);
    });
  }
});

test.describe('Build Health - Asset Loading', () => {
  test('should load JavaScript bundles', async ({ page }: { page: Page }) => {
    const failedRequests: string[] = [];

    page.on('requestfailed', request => {
      if (request.url().endsWith('.js') || request.url().endsWith('.ts')) {
        failedRequests.push(request.url());
      }
    });

    await page.goto('/');
    await page.waitForLoadState('networkidle');

    expect(failedRequests, 'All JS bundles should load').toHaveLength(0);
  });

  test('should load CSS files', async ({ page }: { page: Page }) => {
    const failedRequests: string[] = [];

    page.on('requestfailed', request => {
      if (request.url().endsWith('.css')) {
        failedRequests.push(request.url());
      }
    });

    await page.goto('/');
    await page.waitForLoadState('networkidle');

    expect(failedRequests, 'All CSS files should load').toHaveLength(0);
  });

  test('should have working Vue app mount', async ({ page }: { page: Page }) => {
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    // Check Vue app is mounted
    const appMounted = await page.evaluate(() => {
      const app = document.getElementById('app');
      return app !== null && app.children.length > 0;
    });

    expect(appMounted, 'Vue app should be mounted').toBe(true);
  });
});
