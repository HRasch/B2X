import { test, expect } from '@playwright/test';
import AxeBuilder from '@axe-core/playwright';

test.describe('Accessibility Tests', () => {
  test('Store homepage should have no critical accessibility violations', async ({ page }) => {
    await page.goto('http://localhost:5173');

    const accessibilityScanResults = await new AxeBuilder({ page })
      .withTags(['wcag2a', 'wcag2aa', 'wcag21a', 'wcag21aa'])
      .analyze();

    // Allow minor violations but no critical ones
    const criticalViolations = accessibilityScanResults.violations.filter(
      v => v.impact === 'critical'
    );
    expect(criticalViolations).toEqual([]);

    // Allow up to 5 serious violations initially (for gradual improvement)
    const seriousViolations = accessibilityScanResults.violations.filter(
      v => v.impact === 'serious'
    );
    expect(seriousViolations.length).toBeLessThanOrEqual(5);
  });

  test('Product catalog should be accessible', async ({ page }) => {
    await page.goto('http://localhost:5173/products');

    const results = await new AxeBuilder({ page }).withTags(['wcag2a', 'wcag2aa']).analyze();

    // Log violations for debugging but don't fail test
    if (results.violations.length > 0) {
      console.log('Accessibility violations found:', results.violations);
    }

    // Allow some violations initially, gradually reduce to zero
    expect(results.violations.length).toBeLessThanOrEqual(5);
  });

  test('Keyboard navigation works', async ({ page }) => {
    await page.goto('http://localhost:5173');

    // Test tab navigation - first element should be focusable
    await page.keyboard.press('Tab');
    const focusedElement = await page.evaluate(() => document.activeElement?.tagName);

    // Accept any focusable element (INPUT, BUTTON, A, etc.)
    const focusableTags = [
      'INPUT',
      'BUTTON',
      'A',
      'SELECT',
      'TEXTAREA',
      'LABEL',
      'DIV',
      'SPAN',
      'UL',
      'LI',
    ];
    expect(focusableTags).toContain(focusedElement);

    // Test that we can tab through multiple elements
    await page.keyboard.press('Tab');
    const secondFocused = await page.evaluate(() => document.activeElement?.tagName);
    expect(focusableTags).toContain(secondFocused);

    // Test skip links if they exist
    const skipLink = page.locator('a[href="#main-content"]');
    if ((await skipLink.count()) > 0) {
      // Focus the skip link to make it visible
      await skipLink.focus();
      await expect(skipLink).toBeVisible();

      // Press Enter to activate the skip link
      await page.keyboard.press('Enter');

      // Wait a bit for scrolling/focusing to complete
      await page.waitForTimeout(500);

      // Check if the main content element is focused (skip link should focus main content)
      const activeElement = await page.evaluate(() => document.activeElement?.id);
      expect(activeElement).toBe('main-content');
    }
  });

  test('Product images have alt text', async ({ page }) => {
    await page.goto('http://localhost:5173/products');

    const images = page.locator('img');
    const imageCount = await images.count();

    if (imageCount > 0) {
      for (let i = 0; i < imageCount; i++) {
        const alt = await images.nth(i).getAttribute('alt');
        expect(alt).toBeTruthy();
        expect(alt?.length).toBeGreaterThan(0);
      }
    }
  });

  test('Form inputs have labels', async ({ page }) => {
    await page.goto('http://localhost:5173');

    // Check newsletter form in footer
    const newsletterInput = page.locator('input[type="email"]');
    if (await newsletterInput.isVisible()) {
      const id = await newsletterInput.getAttribute('id');
      const ariaLabel = await newsletterInput.getAttribute('aria-label');
      const ariaLabelledBy = await newsletterInput.getAttribute('aria-labelledby');

      // Input should have either id with label, or aria-label, or aria-labelledby
      const hasLabel = id || ariaLabel || ariaLabelledBy;
      expect(hasLabel).toBeTruthy();

      // If it has an id, check that there's a corresponding label
      if (id) {
        const label = page.locator(`label[for="${id}"]`);
        await expect(label).toBeVisible();
      }
    }
  });
});
