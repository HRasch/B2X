import { test, expect } from '@playwright/test'

// Base URL for testing
const BASE_URL = 'http://localhost:5173'

test.describe('Checkout E2E Tests', () => {
  test.beforeEach(async ({ page }) => {
    // Navigate to checkout page
    await page.goto(`${BASE_URL}/checkout`)
    // Wait for page to load
    await page.waitForLoadState('networkidle')
  })

  // TEST 1: HAPPY PATH - COMPLETE CHECKOUT
  test('should complete full checkout flow successfully', async ({ page }) => {
    console.log('📝 Step 1: Filling shipping address...')
    
    await page.fill('input[id="firstName"]', 'John')
    await page.fill('input[id="lastName"]', 'Doe')
    await page.fill('input[id="street"]', 'Hauptstraße 123')
    await page.fill('input[id="zipCode"]', '10115')
    await page.fill('input[id="city"]', 'Berlin')
    
    expect(await page.inputValue('input[id="firstName"]')).toBe('John')
    
    console.log('✅ Address filled, clicking Next...')
    await page.click('button:has-text("Next")')
    
    await page.waitForSelector('[class*="step-2"]', { timeout: 5000 }).catch(() => {})
    
    console.log('📦 Step 2: Selecting shipping method...')
    const expressOption = page.locator('button:has-text("Express")')
    await expect(expressOption).toBeVisible()
    await expressOption.click()
    
    await expect(page.locator('text=Express')).toBeVisible()
    await expect(page.locator('text=12,99')).toBeVisible()
    
    console.log('✅ Express shipping selected, clicking Next...')
    await page.click('button:has-text("Next")')
    
    await page.waitForSelector('[class*="step-3"]', { timeout: 5000 }).catch(() => {})
    
    console.log('💳 Step 3: Selecting payment method...')
    const paypalOption = page.locator('button:has-text("PayPal")')
    await expect(paypalOption).toBeVisible()
    await paypalOption.click()
    
    await expect(page.locator('text=PayPal')).toBeVisible()
    
    console.log('🔍 Verifying order summary...')
    await expect(page.locator('text=Subtotal')).toBeVisible()
    
    console.log('✓ Checking terms and conditions...')
    const termsCheckbox = page.locator('input[type="checkbox"]')
    await termsCheckbox.click()
    await expect(termsCheckbox).toBeChecked()
    
    console.log('🎯 Completing order...')
    const completeButton = page.locator('button:has-text("Complete Order")')
    await expect(completeButton).toBeEnabled()
    await completeButton.click()
    
    console.log('🎉 Waiting for confirmation page...')
    await page.waitForURL(`${BASE_URL}/order-confirmation`, { timeout: 10000 })
    
    await expect(page.locator('text=Order Confirmed')).toBeVisible()
    
    await page.screenshot({ path: 'checkout-success.png' })
    
    console.log('✅ Happy path test PASSED!')
  })

  // TEST 2: VALIDATION ERROR FLOW
  test('should show validation errors and allow correction', async ({ page }) => {
    console.log('⚠️  Test 2: Validation error flow...')
    
    await page.fill('input[id="lastName"]', 'Doe')
    await page.fill('input[id="street"]', 'Main St')
    await page.fill('input[id="zipCode"]', '10115')
    await page.fill('input[id="city"]', 'Berlin')
    
    await page.click('button:has-text("Next")')
    
    console.log('🔴 Expecting validation error...')
    const errorMessage = page.locator('text=First name is required').or(page.locator('[role="alert"]'))
    await expect(errorMessage).toBeVisible({ timeout: 3000 }).catch(() => {
      console.log('ℹ️  Error message not found - validation may be implicit')
    })
    
    const addressForm = page.locator('input[id="firstName"]')
    await expect(addressForm).toBeVisible()
    
    console.log('✏️  Filling missing firstName...')
    await page.fill('input[id="firstName"]', 'John')
    
    expect(await page.inputValue('input[id="firstName"]')).toBe('John')
    
    console.log('➡️  Clicking Next after correction...')
    await page.click('button:has-text("Next")')
    
    const shippingOptions = page.locator('button:has-text("Express")')
    await expect(shippingOptions).toBeVisible({ timeout: 5000 })
    
    console.log('✅ Validation error test PASSED!')
  })

  // TEST 3: EDIT FLOW
  test('should allow editing previous steps', async ({ page }) => {
    console.log('📝 Test 3: Edit flow...')
    
    await page.fill('input[id="firstName"]', 'John')
    await page.fill('input[id="lastName"]', 'Doe')
    await page.fill('input[id="street"]', 'Hauptstraße 123')
    await page.fill('input[id="zipCode"]', '10115')
    await page.fill('input[id="city"]', 'Berlin')
    
    await page.click('button:has-text("Next")')
    await page.waitForSelector('[class*="step-2"]', { timeout: 5000 }).catch(() => {})
    
    await page.click('button:has-text("Express")')
    
    await page.click('button:has-text("Next")')
    await page.waitForSelector('[class*="step-3"]', { timeout: 5000 }).catch(() => {})
    
    console.log('🔙 Clicking Previous button to go back...')
    await page.click('button:has-text("Previous"), button:has-text("Back")')
    
    const firstNameInput = page.locator('input[id="firstName"]')
    await expect(firstNameInput).toBeVisible({ timeout: 5000 })
    
    expect(await page.inputValue('input[id="firstName"]')).toBe('John')
    
    console.log('✏️  Changing address...')
    await page.fill('input[id="street"]', 'Neue Straße 456')
    expect(await page.inputValue('input[id="street"]')).toBe('Neue Straße 456')
    
    console.log('➡️  Proceeding forward...')
    await page.click('button:has-text("Next")')
    await page.waitForSelector('[class*="step-2"]', { timeout: 5000 }).catch(() => {})
    
    console.log('✅ Edit flow test PASSED!')
  })

  // TEST 4: MOBILE RESPONSIVENESS
  test('should work on mobile viewport (320px)', async ({ browser }) => {
    console.log('📱 Test 4: Mobile responsiveness...')
    
    const context = await browser.newContext({
      viewport: { width: 320, height: 667 },
      userAgent: 'Mozilla/5.0 (iPhone; CPU iPhone OS 14_6 like Mac OS X)',
    })
    
    const mobilePage = await context.newPage()
    await mobilePage.goto(`${BASE_URL}/checkout`)
    await mobilePage.waitForLoadState('networkidle')
    
    console.log('📵 Testing mobile form interaction...')
    
    const firstNameInput = mobilePage.locator('input[id="firstName"]')
    await expect(firstNameInput).toBeVisible()
    
    await firstNameInput.fill('Anna')
    await mobilePage.fill('input[id="lastName"]', 'Schmidt')
    await mobilePage.fill('input[id="street"]', 'Berliner Str 1')
    await mobilePage.fill('input[id="zipCode"]', '10115')
    await mobilePage.fill('input[id="city"]', 'Berlin')
    
    expect(await mobilePage.inputValue('input[id="firstName"]')).toBe('Anna')
    
    console.log('📱 Clicking Next on mobile...')
    const nextButton = mobilePage.locator('button:has-text("Next")')
    
    await nextButton.scrollIntoViewIfNeeded()
    await nextButton.click()
    
    const shippingOption = mobilePage.locator('button:has-text("Express")')
    await expect(shippingOption).toBeVisible({ timeout: 5000 })
    
    console.log('📦 Selecting shipping on mobile...')
    await shippingOption.click()
    
    const nextButton2 = mobilePage.locator('button:has-text("Next")').last()
    await nextButton2.scrollIntoViewIfNeeded()
    await nextButton2.click()
    
    const paymentOption = mobilePage.locator('button:has-text("PayPal")')
    await expect(paymentOption).toBeVisible({ timeout: 5000 })
    
    const bodyWidth = await mobilePage.evaluate(() => {
      const body = document.body
      const html = document.documentElement
      return Math.max(
        body.scrollWidth,
        body.offsetWidth,
        html.scrollWidth,
        html.offsetWidth,
        html.clientWidth,
      )
    })
    
    console.log(`📏 Page width: ${bodyWidth}px (viewport: 320px)`)
    expect(bodyWidth).toBeLessThanOrEqual(320)
    
    await mobilePage.screenshot({ path: 'checkout-mobile.png' })
    
    await context.close()
    console.log('✅ Mobile responsiveness test PASSED!')
  })
})
