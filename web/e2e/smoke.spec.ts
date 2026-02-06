import { test, expect } from '@playwright/test';

test.describe('MyWeekendsLeft Smoke Tests', () => {
  test('homepage loads and displays calculator', async ({ page }) => {
    await page.goto('/');

    // Verify header is present
    await expect(page.getByRole('heading', { name: 'MyWeekendsLeft' })).toBeVisible();

    // Verify hero text is present
    await expect(page.getByText('Make Every Weekend Count')).toBeVisible();

    // Verify calculator form elements are present
    await expect(page.getByLabel('Your Age')).toBeVisible();
    await expect(page.getByText('Male')).toBeVisible();
    await expect(page.getByText('Female')).toBeVisible();
    await expect(page.getByLabel('Country')).toBeVisible();
    await expect(page.getByRole('button', { name: 'Calculate My Weekends' })).toBeVisible();
  });

  test('can calculate weekends and see results', async ({ page }) => {
    await page.goto('/');

    // Fill in the form
    await page.getByLabel('Your Age').fill('35');
    await page.getByText('Male').click();

    // Submit the form
    await page.getByRole('button', { name: 'Calculate My Weekends' }).click();

    // Wait for results (skeleton should appear then results)
    await expect(page.getByText('weekends remaining')).toBeVisible({ timeout: 15000 });

    // Verify key result elements are present
    await expect(page.getByText('Your Life in Weekends')).toBeVisible();
    await expect(page.getByText('Make your weekends count:')).toBeVisible();
    await expect(page.getByRole('button', { name: 'Calculate Again' })).toBeVisible();
  });

  test('can toggle dark mode', async ({ page }) => {
    await page.goto('/');

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

    // Calculate first time
    await page.getByLabel('Your Age').fill('40');
    await page.getByRole('button', { name: 'Calculate My Weekends' }).click();
    await expect(page.getByText('weekends remaining')).toBeVisible({ timeout: 15000 });

    // Click calculate again
    await page.getByRole('button', { name: 'Calculate Again' }).click();

    // Verify we're back to calculator
    await expect(page.getByRole('button', { name: 'Calculate My Weekends' })).toBeVisible();
    await expect(page.getByText('Make Every Weekend Count')).toBeVisible();
  });

  test('API version is displayed in footer', async ({ page }) => {
    await page.goto('/');

    // Wait a bit for the version to load
    await page.waitForTimeout(2000);

    // Check footer contains API version
    const footer = page.locator('footer');
    await expect(footer).toContainText('API v');
  });

  test('GitHub link is present', async ({ page }) => {
    await page.goto('/');

    // Verify GitHub link exists and points to correct repo
    const githubLink = page.getByRole('link', { name: /github/i }).or(
      page.locator('a[href*="github.com/jjbutler74/MyWeekendsLeft"]')
    );
    await expect(githubLink).toBeVisible();
  });
});
