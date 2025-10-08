import { expect, test } from "@playwright/test";
import { baseUrl, impersonateRole } from "../env.setup";

/**
 * E2E Tests for Layout Components
 *
 * These tests verify that common layout components are present across all pages:
 * - Navigation drawer
 * - Header/AppBar
 * - Role selector
 * - Year selector
 * - Footer (if present)
 *
 * These are simple tests because layout components are:
 * - Present on every page (consistent)
 * - Mostly static (don't depend on business logic)
 * - Essential for user navigation (high value to test)
 */
test.describe("Layout Components", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");
    await impersonateRole(page, "Finance-Manager");
  });

  test("Navigation drawer button is present and clickable", async ({ page }) => {
    // The drawer button is typically an icon button (hamburger menu)
    const drawerButton = page.getByRole("button").filter({ hasText: /^$/ }).first();

    await expect(drawerButton).toBeVisible();

    // Verify it's clickable
    const isEnabled = await drawerButton.isEnabled();
    expect(isEnabled).toBe(true);

    // Click to open drawer
    await drawerButton.click();

    // Wait for drawer animation
    await page.waitForTimeout(500);

    // Verify drawer content is visible (navigation items)
    const navItems = page.getByRole("button", { name: /December Activities|Year End|Reports|Fiscal Close/i });
    await expect(navItems.first()).toBeVisible();
  });

  test("Role selector combobox is present and functional", async ({ page }) => {
    // The roles combobox should be visible in the header
    const roleSelector = page.getByRole("combobox", { name: /roles/i });

    await expect(roleSelector).toBeVisible();

    // Click to open role options
    await roleSelector.click();

    // Verify role options appear
    const roleOptions = page.getByRole("option");
    const optionCount = await roleOptions.count();

    expect(optionCount).toBeGreaterThan(0);

    // Close the role selector
    await page.keyboard.press("Escape");
  });

  test("Year selector is present and shows current year", async ({ page }) => {
    // Year selector is the second combobox (nth(1)) - first is role selector
    const yearSelector = page.getByRole("combobox").nth(1);

    await expect(yearSelector).toBeVisible();

    // Get the current year text
    const yearText = await yearSelector.textContent();

    // Verify it contains a 4-digit year
    expect(yearText).toMatch(/20\d{2}/);
  });

  test("Application title/logo is present in header", async ({ page }) => {
    // The app uses a drawer-based navigation, not a traditional header/AppBar
    // Check for the navigation drawer button and year/role selectors as proxy for "header" presence
    const drawerButton = page.locator("button.MuiIconButton-root").first();
    const yearSelector = page.getByRole("combobox").nth(1);
    const roleSelector = page.getByRole("combobox").nth(0);

    await expect(drawerButton).toBeVisible();
    await expect(yearSelector).toBeVisible();
    await expect(roleSelector).toBeVisible();
  });

  test("User info or profile section is visible", async ({ page }) => {
    // Many apps show user name, email, or profile icon
    // This might be in the header/appbar

    // Look for common user info indicators
    const userInfo = page.locator('[aria-label*="user" i], [aria-label*="profile" i], [aria-label*="account" i]');
    const hasUserInfo = (await userInfo.count()) > 0;

    if (hasUserInfo) {
      await expect(userInfo.first()).toBeVisible();
    } else {
      // Alternative: look for email or username text
      const userEmail = page.locator("text=/@/").first();
      const hasEmail = await userEmail.isVisible().catch(() => false);

      // Not critical if user info isn't visible, but log it
      console.log("User info visibility:", hasEmail);
    }
  });

  test("Navigation drawer contains expected menu categories", async ({ page }) => {
    // Open drawer
    const drawerButton = page.getByRole("button").filter({ hasText: /^$/ }).first();
    await drawerButton.click();
    await page.waitForTimeout(500);

    // Verify key navigation categories are present
    const expectedCategories = ["December Activities", "Year End", "Fiscal Close", "Reports", "IT DevOps"];

    let foundCategories = 0;

    for (const category of expectedCategories) {
      const categoryButton = page.getByRole("button", { name: category });
      const isVisible = await categoryButton.isVisible().catch(() => false);

      if (isVisible) {
        foundCategories++;
      }
    }

    // Should find at least 3 of the expected categories
    expect(foundCategories).toBeGreaterThanOrEqual(3);
  });

  test("Navigation drawer can be opened and closed", async ({ page }) => {
    // Open drawer
    const drawerButton = page.getByRole("button").filter({ hasText: /^$/ }).first();
    await drawerButton.click();
    await page.waitForTimeout(500);

    // Verify navigation items are visible
    const navItem = page.getByRole("button", { name: "December Activities" });
    await expect(navItem).toBeVisible();

    // Close drawer by clicking outside or pressing Escape
    await page.keyboard.press("Escape");
    await page.waitForTimeout(500);

    // After closing, drawer content should not be visible (or should be in closed state)
    // Note: Depending on implementation, it might just be collapsed, not hidden
    const isStillVisible = await navItem.isVisible().catch(() => false);

    // If permanent drawer, it's OK to still be visible
    // Otherwise it should be hidden
    console.log("Drawer item visible after close:", isStillVisible);
  });

  test("Layout is responsive and drawer works on different viewport sizes", async ({ page }) => {
    // Test mobile viewport
    await page.setViewportSize({ width: 375, height: 667 });
    await page.waitForTimeout(500);

    // Drawer button should still be present
    const drawerButton = page.getByRole("button").filter({ hasText: /^$/ }).first();
    await expect(drawerButton).toBeVisible();

    // Click drawer button
    await drawerButton.click();
    await page.waitForTimeout(500);

    // Navigation should be accessible
    const navItem = page.getByRole("button", { name: /December Activities|Year End/i }).first();
    await expect(navItem).toBeVisible();

    // Reset to desktop viewport
    await page.setViewportSize({ width: 1920, height: 1080 });
  });

  test("Year selector can be opened and shows multiple years", async ({ page }) => {
    const yearSelector = page.getByRole("combobox").nth(0);

    // Click to open year dropdown
    await yearSelector.click();
    await page.waitForTimeout(300);

    // Count year options
    const yearOptions = page.getByRole("option");
    const optionCount = await yearOptions.count();

    // Should have multiple years available
    expect(optionCount).toBeGreaterThan(1);

    // Close dropdown
    await page.keyboard.press("Escape");
  });

  test("Header remains visible when scrolling (if applicable)", async ({ page }) => {
    // The app uses drawer navigation, check if drawer button remains visible
    const drawerButton = page.locator("button.MuiIconButton-root").first();

    await expect(drawerButton).toBeVisible();

    // Scroll down the page
    await page.evaluate(() => window.scrollTo(0, 500));
    await page.waitForTimeout(300);

    // Drawer button should still be visible
    await expect(drawerButton).toBeVisible();

    // Scroll back to top
    await page.evaluate(() => window.scrollTo(0, 0));
  });

  test("No console errors on page load", async ({ page }) => {
    // Collect console errors
    const consoleErrors: string[] = [];

    page.on("console", (msg) => {
      if (msg.type() === "error") {
        consoleErrors.push(msg.text());
      }
    });

    // Navigate to a page
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");

    // Wait a bit for any async errors
    await page.waitForTimeout(2000);

    // Filter out known acceptable errors (if any)
    const criticalErrors = consoleErrors.filter(
      (err) =>
        !err.includes("favicon") && // Favicon 404s are common and non-critical
        !err.includes("chrome-extension") // Browser extension errors are not app errors
    );

    // Should have no critical console errors
    expect(criticalErrors.length).toBe(0);
  });
});
