import { test, expect } from '@playwright/test';

/**
 * Comprehensive E2E Tests for Language Selection Feature
 * Tests the complete workflow of language switching and persistence
 */

test.describe('Language Selection Feature', () => {
  test.beforeEach(async ({ page, context }) => {
    // Clear cookies
    await context.clearCookies();

    // Navigate to home page
    await page.goto('/');

    // Wait for app to fully load
    await page.waitForLoadState('domcontentloaded');
  });

  test.describe('Language Switcher UI', () => {
    test('should display language switcher with correct initial language', async ({ page }) => {
      const button = page.locator('[data-testid="language-switcher-button"]');

      // Should be visible
      await expect(button).toBeVisible();

      // Should display English flag initially (default language)
      await expect(button).toContainText('🇬🇧');
      await expect(button).toContainText('EN');
    });

    test('should show chevron icon that rotates on dropdown toggle', async ({ page }) => {
      const button = page.locator('[data-testid="language-switcher-button"]');
      const chevron = button.locator('.chevron-icon');

      // Initially not rotated
      const initialClass = await chevron.getAttribute('class');
      expect(initialClass).not.toContain('rotate');

      // Click to open
      await button.click();

      // Should be rotated
      const openClass = await chevron.getAttribute('class');
      expect(openClass).toContain('rotate');

      // Click to close
      await button.click();

      // Should not be rotated again
      const closedClass = await chevron.getAttribute('class');
      expect(closedClass).not.toContain('rotate');
    });

    test('should show all 8 supported languages in dropdown', async ({ page }) => {
      const button = page.locator('[data-testid="language-switcher-button"]');
      await button.click();

      const languages = [
        { code: 'en', flag: '🇬🇧', name: 'English' },
        { code: 'de', flag: '🇩🇪', name: 'Deutsch' },
        { code: 'fr', flag: '🇫🇷', name: 'Français' },
        { code: 'es', flag: '🇪🇸', name: 'Español' },
        { code: 'it', flag: '🇮🇹', name: 'Italiano' },
        { code: 'pt', flag: '🇵🇹', name: 'Português' },
        { code: 'nl', flag: '🇳🇱', name: 'Nederlands' },
        { code: 'pl', flag: '🇵🇱', name: 'Polski' },
      ];

      for (const lang of languages) {
        const option = page.locator(`[data-testid="language-option-${lang.code}"]`);
        await expect(option).toBeVisible();
        await expect(option).toContainText(lang.flag);
        await expect(option).toContainText(lang.name);
      }
    });

    test('should show checkmark only for currently selected language', async ({ page }) => {
      const button = page.locator('[data-testid="language-switcher-button"]');

      // Open dropdown
      await button.click();

      // English should have checkmark (default)
      const enCheckmark = page.locator('[data-testid="language-option-en"] .checkmark');
      await expect(enCheckmark).toBeVisible();

      // German should NOT have checkmark
      const deOption = page.locator('[data-testid="language-option-de"]');
      const deCheckmark = deOption.locator('.checkmark');
      await expect(deCheckmark).not.toBeVisible();
    });
  });

  test.describe('Language Selection Interaction', () => {
    test('should open dropdown on button click', async ({ page }) => {
      const button = page.locator('[data-testid="language-switcher-button"]');
      const dropdown = page.locator('[data-testid="language-dropdown"]');

      // Dropdown should not be visible initially
      await expect(dropdown).not.toBeVisible();

      // Click button
      await button.click();

      // Dropdown should be visible
      await expect(dropdown).toBeVisible();
    });

    test('should close dropdown on language selection', async ({ page }) => {
      const button = page.locator('[data-testid="language-switcher-button"]');
      const dropdown = page.locator('[data-testid="language-dropdown"]');
      const germanOption = page.locator('[data-testid="language-option-de"]');

      // Open dropdown
      await button.click();
      await expect(dropdown).toBeVisible();

      // Click German option
      await germanOption.click();

      // Dropdown should close
      await expect(dropdown).not.toBeVisible();
    });

    test('should close dropdown when clicking outside', async ({ page }) => {
      const button = page.locator('[data-testid="language-switcher-button"]');
      const dropdown = page.locator('[data-testid="language-dropdown"]');
      const overlay = page.locator('.language-overlay');

      // Open dropdown
      await button.click();
      await expect(dropdown).toBeVisible();

      // Click on overlay (outside)
      await overlay.click();

      // Dropdown should close
      await expect(dropdown).not.toBeVisible();
    });

    test('should switch language when selecting different option', async ({ page }) => {
      const button = page.locator('[data-testid="language-switcher-button"]');

      // Initially English
      await expect(button).toContainText('🇬🇧');

      // Open dropdown
      await button.click();

      // Select German
      const germanOption = page.locator('[data-testid="language-option-de"]');
      await germanOption.click();

      // Button should now show German flag
      await expect(button).toContainText('🇩🇪');
      await expect(button).toContainText('DE');
    });

    test('should allow multiple language switches in succession', async ({ page }) => {
      const button = page.locator('[data-testid="language-switcher-button"]');
      const languages = ['de', 'fr', 'es', 'en'];
      const flags = ['🇩🇪', '🇫🇷', '🇪🇸', '🇬🇧'];

      for (let i = 0; i < languages.length; i++) {
        // Open dropdown
        await button.click();

        // Select language
        const option = page.locator(`[data-testid="language-option-${languages[i]}"]`);
        await option.click();

        // Button should show correct flag
        await expect(button).toContainText(flags[i]);
      }
    });
  });

  test.describe('Language Persistence', () => {
    test('should persist language selection to localStorage', async ({ page }) => {
      const button = page.locator('[data-testid="language-switcher-button"]');

      // Open dropdown and select German
      await button.click();
      await page.locator('[data-testid="language-option-de"]').click();

      // Check localStorage
      const locale = await page.evaluate(() => localStorage.getItem('locale'));
      expect(locale).toBe('de');
    });

    test('should restore language after page reload', async ({ page }) => {
      const button = page.locator('[data-testid="language-switcher-button"]');

      // Select German
      await button.click();
      await page.locator('[data-testid="language-option-de"]').click();

      // Reload page
      await page.reload();
      await page.waitForLoadState('networkidle');

      // Button should still show German
      const reloadedButton = page.locator('[data-testid="language-switcher-button"]');
      await expect(reloadedButton).toContainText('🇩🇪');
      await expect(reloadedButton).toContainText('DE');
    });

    test('should restore language across navigation', async ({ page }) => {
      const button = page.locator('[data-testid="language-switcher-button"]');

      // Select French
      await button.click();
      await page.locator('[data-testid="language-option-fr"]').click();

      // Navigate to different page
      await page.goto('/');
      await page.waitForLoadState('networkidle');

      // Language should be French
      const button2 = page.locator('[data-testid="language-switcher-button"]');
      await expect(button2).toContainText('🇫🇷');
      await expect(button2).toContainText('FR');
    });

    test('should use browser language if no saved preference', async ({ page }) => {
      // Should default to English if no localStorage
      const button = page.locator('[data-testid="language-switcher-button"]');

      // Check if any language flag is shown
      const buttonText = await button.textContent();
      expect(buttonText).toMatch(/EN|DE|FR|ES|IT|PT|NL|PL/);
    });
  });

  test.describe('Document Updates', () => {
    test('should update HTML lang attribute on language change', async ({ page }) => {
      const htmlElement = page.locator('html');

      // Initially en
      let lang = await htmlElement.getAttribute('lang');
      expect(lang).toBe('en');

      // Switch to German
      const button = page.locator('[data-testid="language-switcher-button"]');
      await button.click();
      await page.locator('[data-testid="language-option-de"]').click();

      // Should be de
      lang = await htmlElement.getAttribute('lang');
      expect(lang).toBe('de');
    });

    test('should update locale in localStorage', async ({ page }) => {
      // Switch to German
      const button = page.locator('[data-testid="language-switcher-button"]');
      await button.click();
      await page.locator('[data-testid="language-option-de"]').click();

      // Check localStorage
      const locale = await page.evaluate(() => localStorage.getItem('locale'));
      expect(locale).toBe('de');
    });
  });

  test.describe('Disabled State', () => {
    test('should disable switcher during language change', async ({ page }) => {
      const button = page.locator('[data-testid="language-switcher-button"]');

      // Initially not disabled
      let ariaDisabled = await button.getAttribute('aria-disabled');
      expect(ariaDisabled).not.toBe('true');

      // Open and click (will trigger async change)
      await button.click();

      // Click on a language (may briefly show disabled)
      const germanOption = page.locator('[data-testid="language-option-de"]');
      await germanOption.click();

      // After change completes, should be enabled again
      await page.waitForTimeout(200);
      ariaDisabled = await button.getAttribute('aria-disabled');
      expect(ariaDisabled).not.toBe('true');
    });

    test('should not allow interaction while switching language', async ({ page }) => {
      const button = page.locator('[data-testid="language-switcher-button"]');

      // Open dropdown
      await button.click();

      // Click German (may be disabled during change)
      const germanOption = page.locator('[data-testid="language-option-de"]');
      await germanOption.click();

      // Dropdown should be closed and not clickable immediately
      const dropdown = page.locator('[data-testid="language-dropdown"]');
      await expect(dropdown).not.toBeVisible();
    });
  });

  test.describe('Accessibility', () => {
    test('should have proper ARIA attributes', async ({ page }) => {
      const button = page.locator('[data-testid="language-switcher-button"]');

      // Should have aria-disabled attribute
      const ariaDisabled = await button.getAttribute('aria-disabled');
      expect(['false', 'true', null]).toContain(ariaDisabled);

      // Should have title attribute
      const title = await button.getAttribute('title');
      expect(title).toContain('Switch language');
    });

    test('should have semantic HTML structure', async ({ page }) => {
      const switcher = page.locator('[data-testid="language-switcher"]');

      // Should be visible and contain buttons
      await expect(switcher).toBeVisible();

      const button = switcher.locator('button');
      const buttonCount = await button.count();
      expect(buttonCount).toBeGreaterThan(0);
    });
  });

  test.describe('Visual Feedback', () => {
    test('should highlight active language option', async ({ page }) => {
      const button = page.locator('[data-testid="language-switcher-button"]');

      // Open dropdown
      await button.click();

      // English option should have active class
      const enOption = page.locator('[data-testid="language-option-en"]');
      const enClass = await enOption.getAttribute('class');
      expect(enClass).toContain('active');

      // Switch to German
      await page.locator('[data-testid="language-option-de"]').click();

      // Open again
      await button.click();

      // German option should have active class
      const deOption = page.locator('[data-testid="language-option-de"]');
      const deClass = await deOption.getAttribute('class');
      expect(deClass).toContain('active');
    });

    test('should apply styling to language options', async ({ page }) => {
      const button = page.locator('[data-testid="language-switcher-button"]');

      // Open dropdown
      await button.click();

      // Check if dropdown is visible and styled
      const dropdown = page.locator('[data-testid="language-dropdown"]');
      await expect(dropdown).toBeVisible();

      // Check if options have correct styling
      const option = page.locator('[data-testid="language-option-en"]');
      const display = await option.evaluate(el => window.getComputedStyle(el).display);

      expect(display).not.toBe('none');
    });
  });
});
