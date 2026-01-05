import { test, expect } from '@playwright/test';

test.describe('Product Search E2E Tests', () => {
  const baseUrl = process.env.STOREFRONT_URL || '';

  test.beforeEach(async ({ page }) => {
    // Navigate to StoreFront
    await page.goto(`${baseUrl}/products`);

    // Wait for page to be ready
    await page.waitForLoadState('networkidle');
  });

  test('should search for products by name', async ({ page }) => {
    // Arrange
    const searchQuery = 'jacket';

    // Act
    await page.fill('[data-testid="search-input"]', searchQuery);
    await page.click('[data-testid="search-button"]');

    // Wait for results to load
    await page.waitForSelector('[data-testid="product-card"]');

    // Assert
    const productCards = page.locator('[data-testid="product-card"]');
    const count = await productCards.count();

    expect(count).toBeGreaterThan(0);

    // Verify results contain search term
    const firstProduct = productCards.first();
    const productName = await firstProduct.locator('[data-testid="product-name"]').innerText();
    expect(productName.toLowerCase()).toContain(searchQuery.toLowerCase());
  });

  test('should filter products by price range', async ({ page }) => {
    // Arrange
    await page.fill('[data-testid="search-input"]', 'jacket');
    await page.click('[data-testid="search-button"]');
    await page.waitForSelector('[data-testid="product-card"]');

    // Act
    // Set price range filter
    await page.fill('[data-testid="price-min-input"]', '50');
    await page.fill('[data-testid="price-max-input"]', '200');
    await page.click('[data-testid="apply-filters-button"]');

    // Wait for filtered results
    await page.waitForTimeout(500);

    // Assert
    const productCards = page.locator('[data-testid="product-card"]');

    // Verify all products are within price range
    for (let i = 0; i < (await productCards.count()); i++) {
      const card = productCards.nth(i);
      const priceText = await card.locator('[data-testid="product-price"]').innerText();
      const price = parseFloat(priceText.replace(/[^0-9.-]+/g, ''));

      expect(price).toBeGreaterThanOrEqual(50);
      expect(price).toBeLessThanOrEqual(200);
    }
  });

  test('should filter products by category', async ({ page }) => {
    // Arrange
    await page.fill('[data-testid="search-input"]', 'jacket');
    await page.click('[data-testid="search-button"]');
    await page.waitForSelector('[data-testid="product-card"]');

    // Act
    // Click category filter
    await page.click('[data-testid="filter-category-clothing"]');
    await page.waitForTimeout(500);

    // Assert
    const productCards = page.locator('[data-testid="product-card"]');
    const count = await productCards.count();

    expect(count).toBeGreaterThan(0);

    // Verify category is active
    const categoryFilter = page.locator('[data-testid="filter-category-clothing"]');
    await expect(categoryFilter).toHaveClass(/active/);
  });

  test('should get autocomplete suggestions', async ({ page }) => {
    // Act
    const searchInput = page.locator('[data-testid="search-input"]');
    await searchInput.fill('bl');

    // Wait for suggestions
    await page.waitForSelector('[data-testid="search-suggestion"]', { timeout: 1000 });

    // Assert
    const suggestions = page.locator('[data-testid="search-suggestion"]');
    const suggestionCount = await suggestions.count();

    expect(suggestionCount).toBeGreaterThan(0);

    // Verify suggestions contain the query
    const firstSuggestion = await suggestions.first().innerText();
    expect(firstSuggestion.toLowerCase()).toContain('bl');
  });

  test('should select autocomplete suggestion', async ({ page }) => {
    // Arrange
    await page.fill('[data-testid="search-input"]', 'bl');
    await page.waitForSelector('[data-testid="search-suggestion"]');

    // Act
    await page.click('[data-testid="search-suggestion"]');

    // Wait for search results
    await page.waitForSelector('[data-testid="product-card"]');

    // Assert
    const productCards = page.locator('[data-testid="product-card"]');
    expect(await productCards.count()).toBeGreaterThan(0);

    // Verify search input is updated
    const searchInput = page.locator('[data-testid="search-input"]');
    const inputValue = await searchInput.inputValue();
    expect(inputValue.toLowerCase()).toContain('bl');
  });

  test('should sort search results', async ({ page }) => {
    // Arrange
    await page.fill('[data-testid="search-input"]', 'jacket');
    await page.click('[data-testid="search-button"]');
    await page.waitForSelector('[data-testid="product-card"]');

    // Act - Sort by price ascending
    await page.selectOption('[data-testid="sort-select"]', 'price_asc');
    await page.waitForTimeout(500);

    // Assert
    const priceElements = page.locator('[data-testid="product-price"]');
    const prices: number[] = [];

    for (let i = 0; i < Math.min(5, await priceElements.count()); i++) {
      const priceText = await priceElements.nth(i).innerText();
      const price = parseFloat(priceText.replace(/[^0-9.-]+/g, ''));
      prices.push(price);
    }

    // Verify prices are in ascending order
    for (let i = 1; i < prices.length; i++) {
      expect(prices[i]).toBeGreaterThanOrEqual(prices[i - 1]);
    }
  });

  test('should paginate through search results', async ({ page }) => {
    // Arrange
    await page.fill('[data-testid="search-input"]', 'jacket');
    await page.click('[data-testid="search-button"]');
    await page.waitForSelector('[data-testid="product-card"]');

    // Act - Go to next page
    await page.click('[data-testid="pagination-next"]');
    await page.waitForTimeout(500);

    // Assert
    const page2Count = await page.locator('[data-testid="product-card"]').count();
    expect(page2Count).toBeGreaterThan(0);

    // Verify pagination info
    const pageInfo = page.locator('[data-testid="pagination-info"]');
    await expect(pageInfo).toContainText('Page 2');
  });

  test('should view product details from search', async ({ page }) => {
    // Arrange
    await page.fill('[data-testid="search-input"]', 'jacket');
    await page.click('[data-testid="search-button"]');
    await page.waitForSelector('[data-testid="product-card"]');

    // Act
    const firstProductCard = page.locator('[data-testid="product-card"]').first();
    const productId = await firstProductCard.getAttribute('data-product-id');
    await firstProductCard.click();

    // Wait for product detail page
    await page.waitForLoadState('networkidle');

    // Assert
    expect(page.url()).toContain(`/products/${productId}`);

    const productName = page.locator('[data-testid="product-name"]');
    const productPrice = page.locator('[data-testid="product-price"]');
    const productDescription = page.locator('[data-testid="product-description"]');

    await expect(productName).toBeVisible();
    await expect(productPrice).toBeVisible();
    await expect(productDescription).toBeVisible();
  });

  test('should handle empty search results', async ({ page }) => {
    // Act
    await page.fill('[data-testid="search-input"]', 'nonexistentproduct12345');
    await page.click('[data-testid="search-button"]');
    await page.waitForTimeout(500);

    // Assert
    const emptyState = page.locator('[data-testid="empty-state"]');
    await expect(emptyState).toBeVisible();

    const emptyMessage = await emptyState.innerText();
    expect(emptyMessage).toContain('no results');
  });

  test('should handle search with special characters', async ({ page }) => {
    // Act
    await page.fill('[data-testid="search-input"]', 'jacket & shirt');
    await page.click('[data-testid="search-button"]');

    // Wait for results or empty state
    await page.waitForSelector('[data-testid="product-card"], [data-testid="empty-state"]');

    // Assert
    const pageContent = page.locator('body');
    await expect(pageContent).toBeVisible();
  });

  test('should apply multiple filters simultaneously', async ({ page }) => {
    // Arrange
    await page.fill('[data-testid="search-input"]', 'jacket');
    await page.click('[data-testid="search-button"]');
    await page.waitForSelector('[data-testid="product-card"]');

    // Act
    // Apply category filter
    await page.click('[data-testid="filter-category-clothing"]');

    // Apply color filter
    await page.click('[data-testid="filter-color-blue"]');

    // Apply price filter
    await page.fill('[data-testid="price-min-input"]', '50');
    await page.click('[data-testid="apply-filters-button"]');

    await page.waitForTimeout(500);

    // Assert
    const productCards = page.locator('[data-testid="product-card"]');
    const count = await productCards.count();

    expect(count).toBeGreaterThan(0);

    // Verify all filters are active
    await expect(page.locator('[data-testid="filter-category-clothing"]')).toHaveClass(/active/);
    await expect(page.locator('[data-testid="filter-color-blue"]')).toHaveClass(/active/);
  });

  test('should clear all filters', async ({ page }) => {
    // Arrange
    await page.fill('[data-testid="search-input"]', 'jacket');
    await page.click('[data-testid="search-button"]');
    await page.waitForSelector('[data-testid="product-card"]');

    // Apply filters
    await page.click('[data-testid="filter-category-clothing"]');
    await page.fill('[data-testid="price-min-input"]', '100');
    await page.click('[data-testid="apply-filters-button"]');
    await page.waitForTimeout(500);

    const filteredCount = await page.locator('[data-testid="product-card"]').count();

    // Act - Clear all filters
    await page.click('[data-testid="clear-filters-button"]');
    await page.waitForTimeout(500);

    // Assert
    const clearedCount = await page.locator('[data-testid="product-card"]').count();
    expect(clearedCount).toBeGreaterThanOrEqual(filteredCount);

    // Verify filters are inactive
    const categoryFilter = page.locator('[data-testid="filter-category-clothing"]');
    expect(await categoryFilter.getAttribute('class')).not.toContain('active');
  });

  test('should measure search performance', async ({ page }) => {
    // Arrange
    const startTime = Date.now();

    // Act
    await page.fill('[data-testid="search-input"]', 'jacket');
    await page.click('[data-testid="search-button"]');

    // Wait for results
    await page.waitForSelector('[data-testid="product-card"]');
    const endTime = Date.now();

    // Assert
    const duration = endTime - startTime;

    // Search should complete within 1 second (including UI render time)
    expect(duration).toBeLessThan(1000);

    // Log performance metric
    console.log(`Search completed in ${duration}ms`);
  });
});
