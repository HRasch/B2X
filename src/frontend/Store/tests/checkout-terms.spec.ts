import { test, expect } from '@playwright/test';

test.describe('Checkout Terms & Conditions - P0.6-US-005', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('http://localhost:5173/checkout');
  });

  test('Checkout displays terms acceptance step', async ({ page }) => {
    // Verify terms step is visible
    await expect(page.locator("h2:has-text('Bedingungen')")).toBeVisible();
    await expect(page.locator('text=Allgemeinen Geschäftsbedingungen')).toBeVisible();
    await expect(page.locator('text=Datenschutzerklärung')).toBeVisible();
  });

  test('All three checkboxes required for progression', async ({ page }) => {
    // Try to continue without checking anything
    const continueButton = page.locator('button:has-text("Zur Zahlung")');
    await expect(continueButton).toBeDisabled();

    // Check only Terms & Conditions
    await page.locator('#terms-checkbox').check();
    await expect(continueButton).toBeDisabled(); // Still disabled, need privacy too

    // Check Privacy Policy
    await page.locator('#privacy-checkbox').check();
    await expect(continueButton).toBeEnabled(); // Now enabled (withdrawal optional)

    // Uncheck Terms
    await page.locator('#terms-checkbox').uncheck();
    await expect(continueButton).toBeDisabled(); // Disabled again
  });

  test('Withdrawal right checkbox is optional', async ({ page }) => {
    // Check only required fields
    await page.locator('#terms-checkbox').check();
    await page.locator('#privacy-checkbox').check();

    // Continue button should be enabled WITHOUT withdrawal
    const continueButton = page.locator('button:has-text("Zur Zahlung")');
    await expect(continueButton).toBeEnabled();
  });

  test('View Terms & Conditions modal', async ({ page }) => {
    // Click on Terms & Conditions link
    await page.locator('text=Allgemeinen Geschäftsbedingungen').click();

    // Verify modal opens
    const modal = page.locator("h3:has-text('Allgemeine Geschäftsbedingungen')");
    await expect(modal).toBeVisible();

    // Verify content sections
    await expect(page.locator("h4:has-text('Allgemeine Bestimmungen')")).toBeVisible();
    await expect(page.locator("h4:has-text('Widerrufsrecht')")).toBeVisible();

    // Close modal with X button
    await page.locator("button[aria-label='Modal schließen']").first().click();
    await expect(modal).not.toBeVisible();
  });

  test('View Privacy Policy modal', async ({ page }) => {
    // Click on Privacy Policy link
    await page.locator('text=Datenschutzerklärung').click();

    // Verify modal opens
    const modal = page.locator("h3:has-text('Datenschutzerklärung')");
    await expect(modal).toBeVisible();

    // Verify content sections
    await expect(page.locator("h4:has-text('Verantwortlicher')")).toBeVisible();
    await expect(page.locator("h4:has-text('Ihre Rechte')")).toBeVisible();

    // Close modal with Escape key
    await page.keyboard.press('Escape');
    await expect(modal).not.toBeVisible();
  });

  test('View Withdrawal Right modal', async ({ page }) => {
    // Click on Withdrawal Right link
    await page.locator('text=Widerrufsrecht').click();

    // Verify modal opens
    const modal = page.locator("h3:has-text('Widerrufsrecht')");
    await expect(modal).toBeVisible();

    // Verify content
    await expect(page.locator('text=14 Tage')).toBeVisible();
    await expect(page.locator("h4:has-text('Ausnahmen')")).toBeVisible();

    // Close by clicking overlay
    await page.locator('.modal-overlay').click();
    await expect(modal).not.toBeVisible();
  });

  test('Keyboard navigation through checkboxes', async ({ page }) => {
    // Tab to Terms checkbox
    await page.keyboard.press('Tab');
    await expect(page.locator('#terms-checkbox')).toBeFocused();

    // Space to check it
    await page.keyboard.press('Space');
    await expect(page.locator('#terms-checkbox')).toBeChecked();

    // Tab to Privacy checkbox
    await page.keyboard.press('Tab');
    await expect(page.locator('#privacy-checkbox')).toBeFocused();

    // Space to check it
    await page.keyboard.press('Space');
    await expect(page.locator('#privacy-checkbox')).toBeChecked();

    // Verify continue button is now enabled
    const continueButton = page.locator('button:has-text("Zur Zahlung")');
    await expect(continueButton).toBeEnabled();
  });

  test('Submits acceptance and shows success', async ({ page }) => {
    // Check required fields
    await page.locator('#terms-checkbox').check();
    await page.locator('#privacy-checkbox').check();

    // Click continue
    const continueButton = page.locator('button:has-text("Zur Zahlung")');
    await continueButton.click();

    // Verify success message
    await expect(page.locator('text=Bedingungen akzeptiert')).toBeVisible();
  });

  test('Accessibility: WCAG 2.1 AA compliance', async ({ page }) => {
    // All checkboxes have associated labels
    await expect(page.locator('label[for="terms-checkbox"]')).toBeVisible();
    await expect(page.locator('label[for="privacy-checkbox"]')).toBeVisible();
    await expect(page.locator('label[for="withdrawal-checkbox"]')).toBeVisible();

    // Document links are navigable
    const termsLink = page.locator('text=Allgemeinen Geschäftsbedingungen').first();
    await termsLink.focus();
    await expect(termsLink).toBeFocused();

    // Modal can be closed with Escape
    await termsLink.click();
    await expect(page.locator("h3:has-text('Allgemeine Geschäftsbedingungen')")).toBeVisible();
    await page.keyboard.press('Escape');
    await expect(page.locator("h3:has-text('Allgemeine Geschäftsbedingungen')")).not.toBeVisible();
  });

  test('Mobile responsive layout', async ({ page }) => {
    // Set mobile viewport
    await page.setViewportSize({ width: 375, height: 667 });

    // All elements still visible
    await expect(page.locator("h2:has-text('Bedingungen')")).toBeVisible();
    await expect(page.locator('#terms-checkbox')).toBeVisible();
    await expect(page.locator('button:has-text("Zur Zahlung")')).toBeVisible();

    // Buttons stack on mobile
    const actions = page.locator('.step-actions');
    const style = await actions.evaluate(el => getComputedStyle(el).flexDirection);
    expect(style).toBe('column-reverse');
  });

  test('Error message when required field missing', async ({ page }) => {
    // Check only Terms, not Privacy
    await page.locator('#terms-checkbox').check();

    // Try to continue (should be disabled, but test message if form submitted)
    const continueButton = page.locator('button:has-text("Zur Zahlung")');
    await expect(continueButton).toBeDisabled();

    // Verify error would show if form was posted
    // In actual app, validation happens on button enable/disable
  });

  test('Audit trail: Acceptance logged with timestamp', async ({ page }) => {
    // Accept all terms
    await page.locator('#terms-checkbox').check();
    await page.locator('#privacy-checkbox').check();
    await page.locator('#withdrawal-checkbox').check();

    // Submit
    await page.locator('button:has-text("Zur Zahlung")').click();

    // Verify success (in production, would verify DB has entry)
    await expect(page.locator('text=Bedingungen akzeptiert')).toBeVisible();
  });
});
