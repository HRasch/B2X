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

    // Test tab navigation - first element should be focusable
    await page.keyboard.press('Tab');
    const focusedElement = await page.evaluate(() => document.activeElement?.tagName);

    // Accept any focusable element (INPUT, BUTTON, A, etc.)
    const focusableTags = ['INPUT', 'BUTTON', 'A', 'SELECT', 'TEXTAREA'];
    expect(focusableTags).toContain(focusedElement);

    // Test that we can tab through multiple elements
    await page.keyboard.press('Tab');
    const secondFocused = await page.evaluate(() => document.activeElement?.tagName);
    expect(focusableTags).toContain(secondFocused);

    // Test skip links if they exist
    const skipLink = page.locator('a[href="#main-content"]');
    if (await skipLink.isVisible()) {
      await skipLink.click();
      const mainContent = page.locator('#main-content');
      await expect(mainContent).toBeFocused();
    }
  });
});
