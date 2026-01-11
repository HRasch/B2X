import { test, expect } from '@playwright/test';
import * as fs from 'fs';

test.describe('Debug Page Content', () => {
  test('should capture page HTML', async ({ page }) => {
    await page.goto('/');

    // Wait for page to load
    await page.waitForLoadState('networkidle');

    // Get the full HTML
    const html = await page.content();

    // Save to file
    fs.writeFileSync('debug-page-content.html', html);

    // Log key elements
    console.log('=== Page Title ===');
    console.log(await page.title());

    console.log('\n=== Looking for language-switcher ===');
    const switcher = page.locator('[data-testid="language-switcher"]');
    const switcherCount = await switcher.count();
    console.log(`Found ${switcherCount} elements with data-testid="language-switcher"`);

    console.log('\n=== Looking for navbar ===');
    const navbar = page.locator('.navbar');
    const navbarCount = await navbar.count();
    console.log(`Found ${navbarCount} elements with class="navbar"`);

    console.log('\n=== Looking for any li elements in nav ===');
    const navLi = page.locator('nav li');
    const liCount = await navLi.count();
    console.log(`Found ${liCount} li elements in nav`);

    console.log('\n=== Body inner HTML (first 2000 chars) ===');
    const bodyHtml = await page.locator('body').innerHTML();
    console.log(bodyHtml.substring(0, 2000));

    // Check for errors on the page
    console.log('\n=== Checking for error messages ===');
    const errorElements = await page.locator('.error, [class*="error"], #error').count();
    console.log(`Found ${errorElements} error-related elements`);

    // Check if app mounted
    console.log('\n=== Checking #app ===');
    const app = page.locator('#app');
    const appCount = await app.count();
    console.log(`Found ${appCount} #app elements`);

    if (appCount > 0) {
      const appHtml = await app.innerHTML();
      console.log(`#app innerHTML (first 1000 chars): ${appHtml.substring(0, 1000)}`);
    }

    expect(true).toBe(true); // Always pass for debug
  });
});
