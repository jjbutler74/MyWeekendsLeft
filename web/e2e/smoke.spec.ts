import { test, expect } from '@playwright/test';

test.describe('MyWeekendsLeft Smoke Tests', () => {
  test('homepage loads and displays calculator', async ({ page }) => {
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    // Verify header is present
    await expect(page.getByRole('heading', { name: 'MyWeekendsLeft' })).toBeVisible();

    // Verify hero text is present (has fade-in animation, allow time)
    await expect(page.getByText('Make Every Weekend Count')).toBeVisible({ timeout: 10000 });

    // Verify calculator form elements are present
    await expect(page.getByLabel('Your Age')).toBeVisible();
    await expect(page.getByRole('button', { name: 'Male', exact: true })).toBeVisible();
    await expect(page.getByRole('button', { name: 'Female', exact: true })).toBeVisible();
    await expect(page.getByLabel('Country')).toBeVisible();
    await expect(page.getByRole('button', { name: 'Calculate My Weekends' })).toBeVisible();
  });

  test('can calculate weekends and see results', async ({ page }) => {
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    // Fill in the form
    await page.getByLabel('Your Age').fill('35');
    await page.getByRole('button', { name: 'Male', exact: true }).click();

    // Submit the form
    await page.getByRole('button', { name: 'Calculate My Weekends' }).click();

    // Wait for results (API call + count-up animation)
    await expect(page.getByText('weekends remaining')).toBeVisible({ timeout: 30000 });

    // Verify key result elements are present (appear after animation delay)
    await expect(page.getByText('Your Life in Weekends')).toBeVisible({ timeout: 5000 });
    await expect(page.getByText('Make your weekends count:')).toBeVisible();
    await expect(page.getByRole('button', { name: 'Calculate Again' })).toBeVisible();
  });

  test('can toggle dark mode', async ({ page }) => {
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    // Get dark mode toggle button
    const darkModeToggle = page.getByRole('button', { name: /switch to dark mode/i });
    await expect(darkModeToggle).toBeVisible();

    // Click to enable dark mode
    await darkModeToggle.click();

    // Verify the html element has dark class
    await expect(page.locator('html')).toHaveClass(/dark/);

    // Toggle back to light mode
    const lightModeToggle = page.getByRole('button', { name: /switch to light mode/i });
    await lightModeToggle.click();

    // Verify dark class is removed
    await expect(page.locator('html')).not.toHaveClass(/dark/);
  });

  test('can reset and calculate again', async ({ page }) => {
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    // Calculate first time
    await page.getByLabel('Your Age').fill('40');
    await page.getByRole('button', { name: 'Calculate My Weekends' }).click();
    await expect(page.getByText('weekends remaining')).toBeVisible({ timeout: 30000 });

    // Wait for Calculate Again button (appears after count-up animation)
    await expect(page.getByRole('button', { name: 'Calculate Again' })).toBeVisible({ timeout: 5000 });
    await page.getByRole('button', { name: 'Calculate Again' }).click();

    // Verify we're back to calculator
    await expect(page.getByRole('button', { name: 'Calculate My Weekends' })).toBeVisible();
    await expect(page.getByText('Make Every Weekend Count')).toBeVisible({ timeout: 10000 });
  });

  test('API version is displayed in footer', async ({ page }) => {
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    // Check footer contains API version
    const footer = page.locator('footer');
    await expect(footer).toContainText('API v', { timeout: 10000 });
  });

  test('GitHub link is present', async ({ page }) => {
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    // Verify GitHub link exists and points to correct repo
    const githubLink = page.locator('a[href*="github.com/jjbutler74/MyWeekendsLeft"]');
    await expect(githubLink).toBeVisible();
  });
});
