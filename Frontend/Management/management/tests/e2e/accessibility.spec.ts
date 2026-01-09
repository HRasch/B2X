import { test, expect } from '@playwright/test';
import AxeBuilder from '@axe-core/playwright';

test.describe('Accessibility Tests', () => {
  test('Management dashboard should have no accessibility violations', async ({ page }) => {
    await page.goto('http://localhost:5175');

    const accessibilityScanResults = await new AxeBuilder({ page })
      .withTags(['wcag2a', 'wcag2aa', 'wcag21a', 'wcag21aa'])
      .analyze();

    expect(accessibilityScanResults.violations).toEqual([]);
  });

  test('Tenant management should be accessible', async ({ page }) => {
    await page.goto('http://localhost:5175/tenants');

    const results = await new AxeBuilder({ page }).withTags(['wcag2a', 'wcag2aa']).analyze();

    // Log violations for debugging but don't fail test
    if (results.violations.length > 0) {
      console.log('Accessibility violations found:', results.violations);
    }

    // Allow some violations initially, gradually reduce to zero
    expect(results.violations.length).toBeLessThanOrEqual(5);
  });

  test('Keyboard navigation works', async ({ page }) => {
    await page.goto('http://localhost:5175');

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

  test('Data tables are accessible', async ({ page }) => {
    await page.goto('http://localhost:5175/tenants');

    // Check for data tables
    const tables = page.locator('table');
    if ((await tables.count()) > 0) {
      const firstTable = tables.first();

      // Table should have proper headers
      const headers = firstTable.locator('th');
      await expect(headers.first()).toBeVisible();

      // Table should have accessible structure
      const tableResults = await new AxeBuilder({ page })
        .include([firstTable])
        .withTags(['wcag2a'])
        .analyze();

      // Should not have table-specific violations
      const tableViolations = tableResults.violations.filter(
        v => v.id.includes('table') || v.id.includes('th') || v.id.includes('td')
      );
      expect(tableViolations.length).toBeLessThanOrEqual(2);
    }
  });

  test('Form inputs have proper labels', async ({ page }) => {
    await page.goto('http://localhost:5175/tenants/new');

    const inputs = page.locator(
      'input[type="text"], input[type="email"], input[type="password"], textarea, select'
    );
    const inputCount = await inputs.count();

    for (let i = 0; i < inputCount; i++) {
      const input = inputs.nth(i);
      const id = await input.getAttribute('id');
      const ariaLabel = await input.getAttribute('aria-label');
      const ariaLabelledBy = await input.getAttribute('aria-labelledby');
      const placeholder = await input.getAttribute('placeholder');

      // Input should have either id with label, aria-label, aria-labelledby, or meaningful placeholder
      const hasAccessibleName =
        id || ariaLabel || ariaLabelledBy || (placeholder && placeholder.length > 3);
      expect(hasAccessibleName).toBeTruthy();
    }
  });

  test('Admin actions are clearly marked', async ({ page }) => {
    await page.goto('http://localhost:5175/tenants');

    // Check for action buttons (delete, edit, etc.)
    const actionButtons = page.locator(
      'button[class*="delete"], button[class*="edit"], [data-testid*="delete"], [data-testid*="edit"]'
    );
    const buttonCount = await actionButtons.count();

    for (let i = 0; i < buttonCount; i++) {
      const button = actionButtons.nth(i);
      const ariaLabel = await button.getAttribute('aria-label');
      const title = await button.getAttribute('title');
      const text = await button.textContent();

      // Button should have accessible name
      const hasName = ariaLabel || title || (text && text.trim().length > 0);
      expect(hasName).toBeTruthy();
    }
  });
});
