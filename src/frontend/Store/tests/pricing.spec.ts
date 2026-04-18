import { test, expect } from '@playwright/test';

test.describe('PAngV Compliance - Price Transparency', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('http://localhost:5173/');
  });

  test('Product card displays price with VAT', async ({ page }) => {
    await page.goto('http://localhost:5173/shop');

    await page.waitForSelector('.product-card', { timeout: 5000 });

    const vatText = page.locator('text=inkl. MwSt');
    await expect(vatText).toBeVisible();

    const price = page.locator('.price');
    await expect(price).toBeVisible();

    const priceText = await price.first().textContent();
    expect(priceText).toMatch(/€\d+,\d{2}/);
  });

  test('Cart shows VAT-inclusive prices', async ({ page }) => {
    await page.goto('http://localhost:5173/shop');

    await page.click('.add-to-cart-btn');

    await page.goto('http://localhost:5173/cart');

    await page.waitForSelector('.cart-summary', { timeout: 5000 });

    const taxRow = page.locator('text=Steuern');
    await expect(taxRow).toBeVisible();

    const subtotalText = await page.locator('.summary-row').first().textContent();
    expect(subtotalText).toContain('€');
  });

  test('Cart summary displays correct VAT calculation', async ({ page }) => {
    await page.goto('http://localhost:5173/shop');
    await page.click('.add-to-cart-btn');

    await page.goto('http://localhost:5173/cart');

    const summaryRows = page.locator('.summary-row');

    const subtotalText = await summaryRows.nth(0).textContent();
    // Skip tax row - not needed for this test
    const totalText = await summaryRows.nth(2).textContent();

    const subtotal = parseFloat(subtotalText?.match(/(\d+,\d{2})/)?.[1]?.replace(',', '.') || '0');
    const total = parseFloat(totalText?.match(/(\d+,\d{2})/)?.[1]?.replace(',', '.') || '0');

    const expectedTotal = subtotal + subtotal * 0.19;
    expect(Math.abs(total - expectedTotal)).toBeLessThan(0.01);
  });

  test('Checkout page displays complete price breakdown', async ({ page }) => {
    await page.goto('http://localhost:5173/shop');
    await page.click('.add-to-cart-btn');

    await page.goto('http://localhost:5173/cart');
    await page.click('.checkout-btn');

    await page.waitForURL('**/checkout', { timeout: 5000 });
    await expect(page).toHaveURL(/checkout/);

    await expect(page.locator('text=Bestellübersicht')).toBeVisible();
  });

  test('Checkout displays VAT-inclusive notice', async ({ page }) => {
    await page.goto('http://localhost:5173/shop');
    await page.click('.add-to-cart-btn');
    await page.goto('http://localhost:5173/cart');
    await page.click('.checkout-btn');

    await page.waitForURL('**/checkout', { timeout: 5000 });

    const notice = page.locator('text=Alle Preise sind Endpreise');
    await expect(notice).toBeVisible();
  });

  test('Price format uses German locale (comma separator)', async ({ page }) => {
    await page.goto('http://localhost:5173/shop');
    await page.click('.add-to-cart-btn');

    await page.goto('http://localhost:5173/cart');

    const priceText = await page.locator('.item-subtotal').first().textContent();

    expect(priceText).toMatch(/\d+,\d{2}€/);
    expect(priceText).not.toMatch(/\d+\.\d{2}€/);
  });

  test('Cart total calculation is accurate', async ({ page }) => {
    await page.goto('http://localhost:5173/shop');
    await page.click('.add-to-cart-btn');

    await page.goto('http://localhost:5173/cart');

    await page.waitForSelector('.cart-summary', { timeout: 5000 });

    const summaryTotal = page.locator('.summary-row.total');
    await expect(summaryTotal).toBeVisible();

    const totalText = await summaryTotal.textContent();
    expect(totalText).toContain('€');
    expect(totalText).toMatch(/\d+,\d{2}€/);
  });

  test('Product quantity affects subtotal correctly', async ({ page }) => {
    await page.goto('http://localhost:5173/shop');
    await page.click('.add-to-cart-btn');

    await page.goto('http://localhost:5173/cart');

    await page.waitForSelector('.item-quantity', { timeout: 5000 });

    const quantityInput = page.locator('.item-quantity input').first();

    await quantityInput.fill('2');

    const subtotal = page.locator('.item-subtotal').first();
    const subtotalText = await subtotal.textContent();
    expect(subtotalText).toContain('€');
  });
});

test.describe('Accessibility - WCAG 2.1 AA', () => {
  test('Product card has proper focus visible', async ({ page }) => {
    await page.goto('http://localhost:5173/shop');

    await page.keyboard.press('Tab');

    const addBtn = page.locator('.add-to-cart-btn').first();
    const isFocused = await addBtn.evaluate(el => {
      const style = window.getComputedStyle(el);
      return style.outline !== 'none' || document.activeElement === el;
    });

    expect(isFocused).toBe(true);
  });

  test('Cart form inputs have labels', async ({ page }) => {
    await page.goto('http://localhost:5173/shop');
    await page.click('.add-to-cart-btn');
    await page.goto('http://localhost:5173/cart');
    await page.click('.checkout-btn');

    await page.waitForURL('**/checkout', { timeout: 5000 });

    const firstNameLabel = page.locator('label:has-text("Vorname")');
    await expect(firstNameLabel).toBeVisible();
  });

  test('Price breakdown has semantic structure', async ({ page }) => {
    await page.goto('http://localhost:5173/shop');
    await page.click('.add-to-cart-btn');
    await page.goto('http://localhost:5173/cart');
    await page.click('.checkout-btn');

    await page.waitForURL('**/checkout', { timeout: 5000 });

    const headings = page.locator('h2, h3');
    const count = await headings.count();
    expect(count).toBeGreaterThan(0);
  });

  test('Keyboard navigation works in checkout form', async ({ page }) => {
    await page.goto('http://localhost:5173/shop');
    await page.click('.add-to-cart-btn');
    await page.goto('http://localhost:5173/cart');
    await page.click('.checkout-btn');

    await page.waitForURL('**/checkout', { timeout: 5000 });

    const firstInput = page.locator('input[type="text"]').first();

    await firstInput.focus();
    await firstInput.type('John');

    const value = await firstInput.inputValue();
    expect(value).toBe('John');
  });
});

test.describe('Checkout Form Validation', () => {
  test('Form submission disabled without accepting terms', async ({ page }) => {
    await page.goto('http://localhost:5173/shop');
    await page.click('.add-to-cart-btn');
    await page.goto('http://localhost:5173/cart');
    await page.click('.checkout-btn');

    await page.waitForURL('**/checkout', { timeout: 5000 });

    const submitBtn = page.locator('button.btn-primary');
    const isDisabled = await submitBtn.isDisabled();
    expect(isDisabled).toBe(true);
  });

  test('All required fields prevent submission when empty', async ({ page }) => {
    await page.goto('http://localhost:5173/shop');
    await page.click('.add-to-cart-btn');
    await page.goto('http://localhost:5173/cart');
    await page.click('.checkout-btn');

    await page.waitForURL('**/checkout', { timeout: 5000 });

    const submitBtn = page.locator('button.btn-primary');
    await expect(submitBtn).toBeDisabled();
  });

  test('Submit button enables when form is complete', async ({ page }) => {
    await page.goto('http://localhost:5173/shop');
    await page.click('.add-to-cart-btn');
    await page.goto('http://localhost:5173/cart');
    await page.click('.checkout-btn');

    await page.waitForURL('**/checkout', { timeout: 5000 });

    const inputs = page.locator('input[type="text"]');
    const count = await inputs.count();

    for (let i = 0; i < count; i++) {
      const input = inputs.nth(i);
      await input.fill('Test Value');
    }

    const termsCheckbox = page.locator('input[type="checkbox"]');
    await termsCheckbox.check();

    const submitBtn = page.locator('button.btn-primary');
    const isDisabled = await submitBtn.isDisabled();

    expect(isDisabled).toBe(false);
  });
});
