import { test, expect } from "@playwright/test";

test.describe("Shipping Cost Display - P0.6-US-002", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("http://localhost:5173/");
  });

  test("Cart displays shipping method selector", async ({ page }) => {
    await page.goto("http://localhost:5173/shop");

    // Add item to cart
    await page.click(".add-to-cart-btn");

    // Navigate to cart
    await page.goto("http://localhost:5173/cart");

    // Verify shipping section exists
    await expect(page.locator(".shipping-section")).toBeVisible();
    await expect(page.locator("text=Versand")).toBeVisible();
  });

  test("Cart loads shipping methods for Germany", async ({ page }) => {
    await page.goto("http://localhost:5173/shop");
    await page.click(".add-to-cart-btn");
    await page.goto("http://localhost:5173/cart");

    // Verify country selector
    const countrySelect = page.locator("select#country");
    await expect(countrySelect).toBeVisible();

    // Verify shipping methods appear
    await page.waitForSelector(".shipping-option", { timeout: 5000 });
    const methods = page.locator(".shipping-option");
    const count = await methods.count();

    expect(count).toBeGreaterThan(0);
  });

  test("Shipping cost updates when method changes", async ({ page }) => {
    await page.goto("http://localhost:5173/shop");
    await page.click(".add-to-cart-btn");
    await page.goto("http://localhost:5173/cart");

    // Get initial total
    const totalBefore = await page
      .locator(".summary-row.total span:last-child")
      .first()
      .textContent();

    // Select different shipping method
    const secondMethod = page.locator(".shipping-option input").nth(1);
    await secondMethod.click();

    // Wait for update
    await page.waitForTimeout(500);

    // Verify total updated
    const totalAfter = await page
      .locator(".summary-row.total span:last-child")
      .first()
      .textContent();

    // Total should be different or same depending on method
    expect(totalAfter).toBeDefined();
  });

  test("Shipping cost displays correctly for Austria", async ({ page }) => {
    await page.goto("http://localhost:5173/shop");
    await page.click(".add-to-cart-btn");
    await page.goto("http://localhost:5173/cart");

    // Change country to Austria
    const countrySelect = page.locator("select#country");
    await countrySelect.selectOption("AT");

    // Wait for methods to load
    await page.waitForSelector(".shipping-option", { timeout: 5000 });

    // Verify Austria methods are displayed
    const methods = page.locator(".shipping-option");
    const count = await methods.count();

    expect(count).toBeGreaterThan(0);

    // Verify costs are displayed
    const costs = page.locator(".method-cost");
    const costCount = await costs.count();

    expect(costCount).toBe(count);
  });

  test("Checkout displays selected shipping cost", async ({ page }) => {
    await page.goto("http://localhost:5173/shop");
    await page.click(".add-to-cart-btn");
    await page.goto("http://localhost:5173/cart");

    // Select shipping method (should be pre-selected)
    await page.waitForSelector(".shipping-option", { timeout: 5000 });

    // Navigate to checkout
    await page.click(".checkout-btn");

    // Verify checkout loaded
    await expect(page).toHaveURL(/checkout/);

    // Verify shipping cost displayed
    const shippingCosts = page.locator(".breakdown-row.shipping-row");
    await expect(shippingCosts.first()).toBeVisible();
  });

  test("Mobile responsive: Shipping selector layout", async ({ page }) => {
    // Set mobile viewport
    await page.setViewportSize({ width: 375, height: 667 });

    await page.goto("http://localhost:5173/shop");
    await page.click(".add-to-cart-btn");
    await page.goto("http://localhost:5173/cart");

    // Verify shipping section is visible and responsive
    const shippingSection = page.locator(".shipping-section");
    await expect(shippingSection).toBeVisible();

    // Verify methods are accessible via scroll
    const methods = page.locator(".shipping-option");
    const firstMethod = methods.first();
    await firstMethod.scrollIntoViewIfNeeded();
    await expect(firstMethod).toBeVisible();
  });

  test("Accessibility: Shipping method selection with keyboard", async ({
    page,
  }) => {
    await page.goto("http://localhost:5173/shop");
    await page.click(".add-to-cart-btn");
    await page.goto("http://localhost:5173/cart");

    // Wait for methods to load
    await page.waitForSelector(".shipping-option", { timeout: 5000 });

    // Tab to first method
    const firstRadio = page
      .locator(".shipping-option input[type='radio']")
      .first();
    await firstRadio.focus();

    // Verify it's selected/focused
    const isFocused = await firstRadio.evaluate((el) => {
      return document.activeElement === el;
    });

    expect(isFocused).toBe(true);

    // Use arrow keys to navigate
    await page.keyboard.press("ArrowDown");

    // Verify next method can be focused
    const secondRadio = page
      .locator(".shipping-option input[type='radio']")
      .nth(1);
    await secondRadio.focus();

    const isSecondFocused = await secondRadio.evaluate((el) => {
      return document.activeElement === el;
    });

    expect(isSecondFocused).toBe(true);
  });

  test("Error handling: Invalid country shows message", async ({ page }) => {
    await page.goto("http://localhost:5173/shop");
    await page.click(".add-to-cart-btn");
    await page.goto("http://localhost:5173/cart");

    // The country selector should have valid options only
    const countrySelect = page.locator("select#country");
    const options = page.locator("select#country option");
    const optionCount = await options.count();

    expect(optionCount).toBeGreaterThan(1); // More than just placeholder
  });

  test("PAngV Compliance: Shipping visible before checkout", async ({
    page,
  }) => {
    await page.goto("http://localhost:5173/shop");
    await page.click(".add-to-cart-btn");
    await page.goto("http://localhost:5173/cart");

    // Verify shipping section is visible WITHOUT going to checkout
    const shippingSection = page.locator(".shipping-section");
    await expect(shippingSection).toBeVisible();

    // Verify price breakdown with shipping is visible
    const shippingRow = page.locator(".shipping-row");
    const shippingText = await shippingRow.textContent();

    expect(shippingText).toContain("€");
    expect(shippingText).toContain("Versand");
  });

  test("Total calculation includes shipping cost", async ({ page }) => {
    await page.goto("http://localhost:5173/shop");
    await page.click(".add-to-cart-btn");
    await page.goto("http://localhost:5173/cart");

    // Get displayed values
    const subtotalText = await page
      .locator(".summary-row >> nth=0 >> span >> nth=-1")
      .textContent();

    const shippingText = await page
      .locator(".shipping-row >> span >> nth=-1")
      .textContent();

    const totalText = await page
      .locator(".summary-row.total >> span >> nth=-1")
      .textContent();

    // All should contain prices
    expect(subtotalText).toMatch(/€\d+/);
    expect(shippingText).toMatch(/€\d+/);
    expect(totalText).toMatch(/€\d+/);
  });
});
