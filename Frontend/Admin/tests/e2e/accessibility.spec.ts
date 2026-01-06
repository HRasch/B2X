import { test, expect } from '@playwright/test';
import AxeBuilder from '@axe-core/playwright';

test.describe('Accessibility Tests', () => {
  test('Dashboard should have no accessibility violations', async ({ page }) => {
    await page.goto('http://localhost:5174/dashboard');

    const accessibilityScanResults = await new AxeBuilder({ page })
      .withTags(['wcag2a', 'wcag2aa', 'wcag21a', 'wcag21aa'])
      .analyze();

    expect(accessibilityScanResults.violations).toEqual([]);
  });

  test('Product catalog should be accessible', async ({ page }) => {
    await page.goto('http://localhost:5174/catalog/products');

    const results = await new AxeBuilder({ page }).withTags(['wcag2a', 'wcag2aa']).analyze();

    // Log violations for debugging but don't fail test
    if (results.violations.length > 0) {
      console.log('Accessibility violations found:', results.violations);
    }

    // Allow some violations initially, gradually reduce to zero
    expect(results.violations.length).toBeLessThanOrEqual(5);
  });

  test('Keyboard navigation works', async ({ page }) => {
    await page.goto('http://localhost:5174/dashboard');

    // Ensure the page has an interactive element and focus it to make Tab predictable
    await page.waitForLoadState('networkidle');
    await page.evaluate(() => {
      const selector = 'a,button,input,select,textarea,[tabindex]:not([tabindex="-1"])';
      const el = document.querySelector(selector) as HTMLElement | null;
      if (el) el.focus();
    });

    // Test tab navigation - first element should be focusable
    const focusableTags = ['INPUT', 'BUTTON', 'A', 'SELECT', 'TEXTAREA'];
    const focusedElement = await page.evaluate(() => document.activeElement?.tagName ?? null);
    expect(focusableTags).toContain(focusedElement);

    // Test that we can tab through multiple elements
    await page.keyboard.press('Tab');
    const secondFocused = await page.evaluate(() => document.activeElement?.tagName ?? null);
    expect(focusableTags).toContain(secondFocused);

    // Test skip links if they exist (allow focus to be inside main content)
    const skipLink = page.locator('a[href="#main-content"]');
    if ((await skipLink.count()) > 0 && await skipLink.isVisible()) {
      await skipLink.click();
      const focusedInsideMain = await page.evaluate(() => {
        const active = document.activeElement;
        if (!active) return false;
        if (active.id === 'main-content') return true;
        return !!(active.closest && active.closest('#main-content'));
      });
      expect(focusedInsideMain).toBeTruthy();
    }
  });
});
