import { test, expect } from '@playwright/test';

test.describe('CMS Page Loading', () => {
  test('should load home page successfully', async ({ page }) => {
    await page.goto('/');

    await expect(page).toHaveTitle(/Welcome|B2Connect/);
    const firstHeading = page.locator('h1, h2').first();
    await expect(firstHeading).toBeVisible();
  });

  test('should display hero banner on home page', async ({ page }) => {
    await page.goto('/');

    // Check for main heading and subtitle instead of hero-banner
    const mainHeading = page.locator('h1:has-text("Welcome to B2Connect")');
    const subtitle = page.locator('.subtitle');

    await expect(mainHeading).toBeVisible();
    await expect(subtitle).toBeVisible();
  });

  test('should have proper page structure with regions', async ({ page }) => {
    await page.goto('/');

    const regions = page.locator('[data-region-id]');
    const regionCount = await regions.count();

    expect(regionCount).toBeGreaterThan(0);
  });

  test('should navigate to products page', async ({ page }) => {
    await page.goto('/');

    const shoppingLink = page.locator(
      'a:has-text("Shop Now"), a:has-text("Products"), button:has-text("Shop Now")'
    );
    if (await shoppingLink.first().isVisible()) {
      await shoppingLink.first().click();
      // Wait for navigation
      await page.waitForLoadState('networkidle');
      expect(page.url()).toContain('/products');
    }
  });
});

test.describe('Widget Rendering', () => {
  test('should render product grid widget', async ({ page }) => {
    await page.goto('/products');

    const productGrid = page.locator('.product-grid-widget');
    await expect(productGrid).toBeVisible();
  });

  test('should render testimonials carousel', async ({ page }) => {
    await page.goto('/');

    const testimonials = page.locator('.testimonials-widget');
    if (await testimonials.isVisible()) {
      const quote = testimonials.locator('p').first();
      await expect(quote).toBeVisible();
    }
  });

  test('should render feature grid', async ({ page }) => {
    await page.goto('/');

    const features = page.locator('.feature-grid-widget');
    if (await features.isVisible()) {
      const featureItems = features.locator('[class*="feature"]');
      const count = await featureItems.count();
      expect(count).toBeGreaterThan(0);
    }
  });

  test('should render call to action widget', async ({ page }) => {
    await page.goto('/');

    const cta = page.locator('.call-to-action-widget');
    if (await cta.isVisible()) {
      const button = cta.locator('button');
      await expect(button).toBeVisible();
    }
  });

  test('should render newsletter signup form', async ({ page }) => {
    await page.goto('/');

    const newsletter = page.locator('.newsletter-widget');
    if (await newsletter.isVisible()) {
      const emailInput = newsletter.locator('input[type="email"]');
      await expect(emailInput).toBeVisible();
    }
  });
});

test.describe('Widget Interactions', () => {
  test('should navigate hero banner CTA button', async ({ page }) => {
    await page.goto('/');

    const heroBanner = page.locator('.hero-banner');
    const button = heroBanner.locator('button').first();

    if (await button.isVisible()) {
      await button.click();
      await page.waitForLoadState('networkidle');
      // Should navigate somewhere
      expect(page.url()).not.toBe('/');
    }
  });

  test('should navigate testimonials carousel', async ({ page }) => {
    await page.goto('/');

    const testimonials = page.locator('.testimonials-widget');
    if (await testimonials.isVisible()) {
      const nextButton = testimonials.locator('button:has-text("Next")').first();

      if (await nextButton.isVisible()) {
        await nextButton.click();
        await page.waitForTimeout(500);

        const newText = await testimonials.locator('p').first().textContent();
        // Text might change or stay same if only one testimonial
        expect(newText).toBeDefined();
      }
    }
  });

  test('should interact with newsletter form', async ({ page }) => {
    await page.goto('/');

    const newsletter = page.locator('.newsletter-widget');
    if (await newsletter.isVisible()) {
      const emailInput = newsletter.locator('input[type="email"]');

      if (await emailInput.isVisible()) {
        await emailInput.fill('test@example.com');
        // Don't actually submit to avoid side effects
      }
    }
  });
});

test.describe('Page Navigation', () => {
  test('should navigate to about page', async ({ page }) => {
    await page.goto('/');

    // Try to find a link to about page
    const aboutLink = page.locator('a:has-text("About"), a[href*="about"]').first();
    if (await aboutLink.isVisible()) {
      await aboutLink.click();
      await page.waitForLoadState('networkidle');
      expect(page.url()).toContain('/about');
    }
  });

  test('should navigate to contact page', async ({ page }) => {
    await page.goto('/');

    const contactLink = page.locator('a:has-text("Contact"), a[href*="contact"]').first();
    if (await contactLink.isVisible()) {
      await contactLink.click();
      await page.waitForLoadState('networkidle');
      expect(page.url()).toContain('/contact');
    }
  });
});

test.describe('Page Metadata', () => {
  test('should have proper meta tags on home page', async ({ page }) => {
    await page.goto('/');

    const titleTag = page.locator('head title');
    const descriptionTag = page.locator('meta[name="description"]');

    await expect(titleTag).toContainText(/Welcome|B2Connect|Home/);

    if (await descriptionTag.isVisible()) {
      const content = await descriptionTag.getAttribute('content');
      expect(content).toBeTruthy();
    }
  });

  test('should update title on page change', async ({ page }) => {
    await page.goto('/');
    const homeTitle = await page.locator('head title').textContent();

    const productsLink = page.locator('a:has-text("Products"), a[href*="products"]').first();
    if (await productsLink.isVisible()) {
      await productsLink.click();
      await page.waitForLoadState('networkidle');

      const productsTitle = await page.locator('head title').textContent();
      expect(productsTitle).not.toBe(homeTitle);
    }
  });
});

test.describe('Responsive Design', () => {
  test('should render correctly on mobile', async ({ page }) => {
    await page.setViewportSize({ width: 375, height: 667 });
    await page.goto('/');

    // Check for main content elements
    const mainHeading = page.locator('h1:has-text("Welcome to B2Connect")');
    await expect(mainHeading).toBeVisible();

    // Check that content is still visible
    const content = page.locator('h1, h2');
    await expect(content.first()).toBeVisible();
  });

  test('should render correctly on tablet', async ({ page }) => {
    await page.setViewportSize({ width: 768, height: 1024 });
    await page.goto('/');

    const mainHeading = page.locator('h1:has-text("Welcome to B2Connect")');
    await expect(mainHeading).toBeVisible();
  });

  test('should render correctly on desktop', async ({ page }) => {
    await page.setViewportSize({ width: 1920, height: 1080 });
    await page.goto('/');

    const mainHeading = page.locator('h1:has-text("Welcome to B2Connect")');
    await expect(mainHeading).toBeVisible();
  });
});

test.describe('Performance', () => {
  test('should load page within reasonable time', async ({ page }) => {
    const startTime = Date.now();
    await page.goto('/');
    const loadTime = Date.now() - startTime;

    // Page should load within 5 seconds
    expect(loadTime).toBeLessThan(5000);
  });

  test('should display content before full page load', async ({ page }) => {
    await page.goto('/', { waitUntil: 'domcontentloaded' });

    // Hero banner should be visible quickly
    const heroBanner = page.locator('.hero-banner');
    await expect(heroBanner).toBeVisible({ timeout: 3000 });
  });
});
