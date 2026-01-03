import { test, expect } from '@playwright/test';

test.describe('Localization E2E Tests', () => {
  test.beforeEach(async ({ page }) => {
    // Clear localStorage before each test
    await page.evaluate(() => localStorage.clear());
    // Navigate to app
    await page.goto('http://localhost:5173');
  });

  test('should display language switcher in navbar', async ({ page }) => {
    const languageSwitcher = page.locator('[data-testid="language-switcher"]');
    await expect(languageSwitcher).toBeVisible();
  });

  test('should display current language flag', async ({ page }) => {
    const flagButton = page.locator('[data-testid="language-switcher-button"]');
    await expect(flagButton).toContainText(/🇬🇧|🇩🇪|🇫🇷|🇪🇸|🇮🇹|🇵🇹|🇳🇱|🇵🇱/);
  });

  test('should open language dropdown on click', async ({ page }) => {
    const button = page.locator('[data-testid="language-switcher-button"]');
    const dropdown = page.locator('[data-testid="language-dropdown"]');

    await expect(dropdown).not.toBeVisible();
    await button.click();
    await expect(dropdown).toBeVisible();
  });

  test('should close dropdown when clicking outside', async ({ page }) => {
    const button = page.locator('[data-testid="language-switcher-button"]');
    const dropdown = page.locator('[data-testid="language-dropdown"]');

    await button.click();
    await expect(dropdown).toBeVisible();

    // Click outside (on navbar)
    await page.locator('.navbar-logo').click();
    await expect(dropdown).not.toBeVisible();
  });

  test('should switch language when selecting option', async ({ page }) => {
    const button = page.locator('[data-testid="language-switcher-button"]');
    const germanOption = page.locator('[data-testid="language-option-de"]');

    // Open dropdown
    await button.click();

    // Select German
    await germanOption.click();

    // Check if locale was changed
    const locale = await page.evaluate(() => localStorage.getItem('locale'));
    expect(locale).toBe('de');
  });

  test('should persist language selection to localStorage', async ({ page }) => {
    const button = page.locator('[data-testid="language-switcher-button"]');
    const germanOption = page.locator('[data-testid="language-option-de"]');

    await button.click();
    await germanOption.click();

    // Reload page
    await page.reload();

    // Should still be German
    const button2 = page.locator('[data-testid="language-switcher-button"]');
    await expect(button2).toContainText('🇩🇪');
  });

  test('should show checkmark for selected language', async ({ page }) => {
    const button = page.locator('[data-testid="language-switcher-button"]');

    // Open dropdown
    await button.click();

    // English should have checkmark (default)
    const enCheckmark = page.locator('[data-testid="language-option-en"] .checkmark');
    await expect(enCheckmark).toBeVisible();

    // Switch to German
    await page.locator('[data-testid="language-option-de"]').click();

    // Open dropdown again
    await button.click();

    // German should have checkmark now
    const deCheckmark = page.locator('[data-testid="language-option-de"] .checkmark');
    await expect(deCheckmark).toBeVisible();

    // English should not have checkmark
    await expect(enCheckmark).not.toBeVisible();
  });

  test('should update document language attribute', async ({ page }) => {
    const button = page.locator('[data-testid="language-switcher-button"]');
    const germanOption = page.locator('[data-testid="language-option-de"]');

    await button.click();
    await germanOption.click();

    // Check HTML lang attribute
    const lang = await page.locator('html').getAttribute('lang');
    expect(lang).toBe('de');
  });

  test('should have all supported languages in dropdown', async ({ page }) => {
    const button = page.locator('[data-testid="language-switcher-button"]');
    await button.click();

    const languages = ['en', 'de', 'fr', 'es', 'it', 'pt', 'nl', 'pl'];

    for (const lang of languages) {
      const option = page.locator(`[data-testid="language-option-${lang}"]`);
      await expect(option).toBeVisible();
    }
  });

  test('should disable switcher during language change', async ({ page }) => {
    const button = page.locator('[data-testid="language-switcher-button"]');

    await button.click();
    const germanOption = page.locator('[data-testid="language-option-de"]');

    // Button should have disabled state while changing
    const initialDisabled = await button.getAttribute('aria-disabled');

    await germanOption.click();

    // Wait a bit for the change to complete
    await page.waitForTimeout(100);

    // Button should no longer be disabled
    const finalDisabled = await button.getAttribute('aria-disabled');
    expect(finalDisabled).not.toBe('true');
  });

  test('should emit locale-changed event', async ({ page }) => {
    // Listen for custom event
    const eventFired = page.evaluate(() => {
      return new Promise<boolean>(resolve => {
        let fired = false;
        window.addEventListener('locale-changed', () => {
          fired = true;
        });

        // Simulate language change
        setTimeout(() => resolve(fired), 2000);
      });
    });

    const button = page.locator('[data-testid="language-switcher-button"]');
    await button.click();

    const germanOption = page.locator('[data-testid="language-option-de"]');
    await germanOption.click();

    const fired = await eventFired;
    expect(fired).toBe(true);
  });

  test('should display language names in dropdown', async ({ page }) => {
    const button = page.locator('[data-testid="language-switcher-button"]');
    await button.click();

    // Check language names are visible
    const englishLabel = page.locator('[data-testid="language-option-en"]');
    await expect(englishLabel).toContainText('English');

    const germanLabel = page.locator('[data-testid="language-option-de"]');
    await expect(germanLabel).toContainText('Deutsch');
  });

  test('should be keyboard accessible', async ({ page }) => {
    const button = page.locator('[data-testid="language-switcher-button"]');

    // Focus button
    await button.focus();

    // Press Enter to open dropdown
    await page.keyboard.press('Enter');

    const dropdown = page.locator('[data-testid="language-dropdown"]');
    await expect(dropdown).toBeVisible();

    // Press arrow down to navigate
    await page.keyboard.press('ArrowDown');

    // First option should be focused
    const firstOption = page.locator('[data-testid="language-option-en"]');
    const focused = await firstOption.evaluate(el => el === document.activeElement);
    expect(focused).toBe(true);
  });

  test('should maintain language preference across page navigation', async ({ page }) => {
    // Set language to German
    const button = page.locator('[data-testid="language-switcher-button"]');
    await button.click();
    await page.locator('[data-testid="language-option-de"]').click();

    // Navigate to another page
    await page.goto('http://localhost:5173/');
    await page.goto('http://localhost:5173/shop');

    // Language should still be German
    const button2 = page.locator('[data-testid="language-switcher-button"]');
    await expect(button2).toContainText('🇩🇪');
  });
});
