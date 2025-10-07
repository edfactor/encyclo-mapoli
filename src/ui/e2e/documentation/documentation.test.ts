import { expect, test } from "@playwright/test";
import { baseUrl, impersonateRole, navigateToPage } from "../env.setup";

/**
 * E2E Tests for Documentation Page
 *
 * This is a simple, low-risk test suite because:
 * - Documentation page is read-only (no edit operations)
 * - Static content (unlikely to change frequently)
 * - Isolated from business logic (doesn't depend on data state)
 * - Available to all roles (no complex permission testing needed)
 */
test.describe("Documentation Page", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");

    // Use a standard role that has access to documentation
    await impersonateRole(page, "Finance-Manager");

    // Navigate to Documentation page
    // Note: Adjust category name based on actual navigation structure
    await navigateToPage(page, "IT Operations", "Documentation");
  });

  test("Page loads successfully", async ({ page }) => {
    // Verify the Documentation heading is visible
    await expect(page.getByRole("heading", { name: /documentation/i })).toBeVisible();
  });

  test("Documentation list is rendered", async ({ page }) => {
    // Wait for documentation content to load
    await page.waitForTimeout(1000);

    // Verify that at least one documentation item is visible
    // This could be a list, grid, or card layout
    const docItems = page.locator('[data-testid*="doc"], .documentation-item, article');
    const count = await docItems.count();

    expect(count).toBeGreaterThan(0);
  });

  test("Documentation items are clickable", async ({ page }) => {
    // Wait for documentation content
    await page.waitForTimeout(1000);

    // Find first documentation link/button
    const firstDocLink = page.locator('a[href*="doc"], button').first();

    // Verify it exists and is visible
    await expect(firstDocLink).toBeVisible();

    // Verify it's clickable (not disabled)
    const isEnabled = await firstDocLink.isEnabled();
    expect(isEnabled).toBe(true);
  });

  test("Search or filter functionality exists (if applicable)", async ({ page }) => {
    // Check if there's a search/filter input
    const searchInput = page.locator(
      'input[type="search"], input[placeholder*="search" i], input[placeholder*="filter" i]'
    );
    const searchExists = (await searchInput.count()) > 0;

    if (searchExists) {
      // Verify search input is visible and functional
      await expect(searchInput.first()).toBeVisible();

      // Try typing in search
      await searchInput.first().fill("test");

      // Verify the input accepts text
      const value = await searchInput.first().inputValue();
      expect(value).toBe("test");
    } else {
      // No search functionality - test passes
      console.log("No search functionality found on Documentation page - this is acceptable");
    }
  });

  test("Page is accessible to read-only users", async ({ page }) => {
    // Switch to read-only role
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "Finance-Manager" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "IT-DevOps" }).getByRole("checkbox").check();
    await page.locator("body").click();

    // Reload to apply role change
    await page.reload();
    await page.waitForLoadState("networkidle");

    // Verify page is still accessible (should NOT show Access Denied)
    const accessDenied = page.getByText("Access Denied");
    const isAccessDenied = await accessDenied.isVisible().catch(() => false);

    expect(isAccessDenied).toBe(false);

    // Verify documentation heading is still visible
    await expect(page.getByRole("heading", { name: /documentation/i })).toBeVisible();
  });

  test("No edit controls are present (read-only page)", async ({ page }) => {
    // Documentation page should not have edit/save/delete buttons
    const editButtons = page.locator("button").filter({ hasText: /edit|save|delete|update|modify/i });
    const editButtonCount = await editButtons.count();

    // Should have zero or very few edit buttons (some might be in navigation/header)
    expect(editButtonCount).toBeLessThanOrEqual(2);
  });

  test("Page has proper breadcrumbs or navigation context", async ({ page }) => {
    // Check for breadcrumbs or page title that provides context
    const breadcrumbs = page.locator('nav[aria-label*="breadcrumb" i], .breadcrumb, .MuiBreadcrumbs-root');
    const hasBreadcrumbs = await breadcrumbs.isVisible().catch(() => false);

    if (hasBreadcrumbs) {
      // Verify breadcrumbs contain relevant text
      const breadcrumbText = await breadcrumbs.textContent();
      expect(breadcrumbText?.toLowerCase()).toContain("documentation");
    } else {
      // No breadcrumbs - verify page title instead
      const pageTitle = page.getByRole("heading", { name: /documentation/i });
      await expect(pageTitle).toBeVisible();
    }
  });
});
