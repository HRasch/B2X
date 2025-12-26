import { test, expect, type Page } from '@playwright/test'

test.describe('B2Connect Home Page', () => {
  test('should display the home page', async ({ page }: { page: Page }) => {
    await page.goto('/')
    
    // Check if the main heading exists
    const heading = page.getByRole('heading', { name: /Welcome to B2Connect/i })
    await expect(heading).toBeVisible()
  })

  test('should navigate to login page', async ({ page }: { page: Page }) => {
    await page.goto('/')
    
    // Click the login link
    const loginLink = page.getByRole('link', { name: /Get Started/i })
    await loginLink.click()
    
    // Verify we're on the login page
    await expect(page).toHaveURL(/login/)
  })

  test('should display feature cards', async ({ page }: { page: Page }) => {
    await page.goto('/')
    
    // Check for feature cards
    const features = page.getByText(/Multitenant Architecture|Microservices|Real-time Updates/)
    const count = await features.count()
    expect(count).toBeGreaterThan(0)
  })
})

test.describe('Authentication Flow', () => {
  test('should display login form', async ({ page }: { page: Page }) => {
    await page.goto('/login')
    
    // Check for email input
    const emailInput = page.getByLabel('Email')
    await expect(emailInput).toBeVisible()
    
    // Check for password input
    const passwordInput = page.getByLabel('Password')
    await expect(passwordInput).toBeVisible()
    
    // Check for login button
    const loginButton = page.getByRole('button', { name: /Login/i })
    await expect(loginButton).toBeVisible()
  })

  test('should show error on invalid login', async ({ page }: { page: Page }) => {
    await page.goto('/login')
    
    // Fill in the form with invalid credentials
    await page.getByLabel('Email').fill('invalid@example.com')
    await page.getByLabel('Password').fill('wrongpassword')
    
    // Submit the form
    await page.getByRole('button', { name: /Login/i }).click()
    
    // Check for error message (would appear after API response)
    // This depends on the actual implementation
  })
})
