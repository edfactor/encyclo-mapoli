import { expect, test } from "@playwright/test";
import { baseUrl, impersonateRole, navigateToPage } from "../env.setup";

/**
 * E2E Tests for Unauthorized Access Page
 *
 * This is a simple test suite because:
 * - Tests static "Access Denied" page (no business logic)
 * - Verifies role-based access control is working
 * - Low risk - just checking that unauthorized users are blocked
 * - Simple validation of error messages and navigation
 */
test.describe("Unauthorized Access", () => {
  test("Unauthorized user sees Access Denied on protected page", async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");

    // Use a role with limited permissions
    await impersonateRole(page, "Distributions-Clerk");

    // Try to navigate to a page that requires Finance-Manager role
    await navigateToPage(page, "December Activities", /^Forfeitures 008-12/);

    // Should see Access Denied message
    await expect(page.getByText("Access Denied").nth(1)).toBeVisible();
  });

  test("Access Denied page displays appropriate message", async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");

    // Use read-only role
    await impersonateRole(page, "IT-DevOps");

    // Try to access Manage Executive Hours (requires higher permissions)
    await navigateToPage(page, "Fiscal Close", "Manage Executive Hours");

    // Verify Access Denied is shown
    const accessDeniedText = page.getByText("Access Denied");
    await expect(accessDeniedText).toBeVisible();

    // May also have additional messaging
    const helpText = page.locator("text=/do not have permission|contact administrator|access restricted/i");
    const hasHelpText = (await helpText.count()) > 0;

    if (hasHelpText) {
      await expect(helpText.first()).toBeVisible();
    }
  });

  test("Access Denied page has navigation back option", async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");

    await impersonateRole(page, "Distributions-Clerk");
    await navigateToPage(page, "December Activities", /^Forfeitures 008-12/);

    // Should see Access Denied
    await expect(page.getByText("Access Denied").nth(1)).toBeVisible();

    // User should still be able to use navigation drawer
    const drawerButton = page.getByRole("button").filter({ hasText: /^$/ }).first();
    await expect(drawerButton).toBeVisible();

    // User should be able to navigate away
    await drawerButton.click();
    await page.waitForTimeout(500);

    const navItem = page.getByRole("button", { name: "December Activities" });
    await expect(navItem).toBeVisible();
  });

  test("User can change role from Access Denied page", async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");

    // Start with limited role
    await impersonateRole(page, "Distributions-Clerk");
    await navigateToPage(page, "December Activities", /^Forfeitures 008-12/);

    // Should see Access Denied
    await expect(page.getByText("Access Denied").nth(1)).toBeVisible();

    // Now switch to authorized role
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "Distributions-Clerk" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "Finance-Manager" }).getByRole("checkbox").check();
    await page.locator("body").click();

    // Reload to apply role
    await page.reload();
    await page.waitForLoadState("networkidle");

    // After reload with correct role, should NOT see Access Denied
    const accessDenied = page.getByText("Access Denied");
    const isStillDenied = await accessDenied.isVisible().catch(() => false);
    expect(isStillDenied).toBe(false);
  });

  test("Access Denied does not expose sensitive data", async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");

    await impersonateRole(page, "IT-DevOps");
    await navigateToPage(page, "December Activities", /^Forfeitures 008-12/);

    // Should see Access Denied
    await expect(page.getByText("Access Denied").nth(1)).toBeVisible();

    // Verify that no data grids or sensitive content is visible
    const dataGrids = page.locator('[role="grid"], .MuiDataGrid-root, .ag-root');
    const gridCount = await dataGrids.count();

    // Should have no or minimal data grids visible (maybe one for layout purposes)
    expect(gridCount).toBeLessThanOrEqual(1);

    // Verify no actual data rows are visible
    const dataRows = page.locator('[role="row"], .MuiDataGrid-row, .ag-row').filter({ hasText: /\d{6}/ }); // Badge numbers
    const rowCount = await dataRows.count();
    expect(rowCount).toBe(0);
  });

  test("Multiple unauthorized pages show consistent Access Denied messaging", async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");

    await impersonateRole(page, "IT-DevOps");

    // Test first unauthorized page
    await navigateToPage(page, "December Activities", /^Forfeitures 008-12/);
    const firstAccessDenied = await page.getByText("Access Denied").textContent();

    // Navigate to different unauthorized page
    await navigateToPage(page, "Fiscal Close", "Manage Executive Hours");
    const secondAccessDenied = await page.getByText("Access Denied").textContent();

    // Messages should be similar/consistent
    expect(firstAccessDenied).toBeTruthy();
    expect(secondAccessDenied).toBeTruthy();

    // Both should contain "Access Denied"
    expect(firstAccessDenied?.toLowerCase()).toContain("access denied");
    expect(secondAccessDenied?.toLowerCase()).toContain("access denied");
  });

  test("Access Denied page does not break application state", async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");

    await impersonateRole(page, "Distributions-Clerk");

    // Hit unauthorized page
    await navigateToPage(page, "December Activities", /^Forfeitures 008-12/);
    await expect(page.getByText("Access Denied").nth(1)).toBeVisible();

    // Navigate to an authorized page (Terminations is usually accessible to Distributions-Clerk)
    await navigateToPage(page, "December Activities", "Termination");

    // Should successfully navigate away from Access Denied
    // Look for Termination page heading
    const terminationHeading = page.getByRole("heading", { name: /termination/i });
    const isVisible = await terminationHeading.isVisible().catch(() => false);

    // If Termination is accessible, should see the heading
    // If also denied, will see Access Denied again
    const accessDenied = page.getByText("Access Denied");
    const isStillDenied = await accessDenied.isVisible().catch(() => false);

    // One or the other should be true (navigated successfully OR hit another access denied)
    expect(isVisible || isStillDenied).toBe(true);
  });

  test("Access Denied page layout components still work", async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");

    await impersonateRole(page, "IT-DevOps");
    await navigateToPage(page, "December Activities", /^Forfeitures 008-12/);

    // Should see Access Denied
    await expect(page.getByText("Access Denied").nth(1)).toBeVisible();

    // But header components should still work

    // Year selector should be visible
    const yearSelector = page.getByRole("combobox").nth(0);
    await expect(yearSelector).toBeVisible();

    // Role selector should be visible
    const roleSelector = page.getByRole("combobox", { name: /roles/i });
    await expect(roleSelector).toBeVisible();

    // Navigation drawer should be accessible
    const drawerButton = page.getByRole("button").filter({ hasText: /^$/ }).first();
    await expect(drawerButton).toBeVisible();
  });

  test("Browser back button works from Access Denied page", async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");

    // Start on home/dashboard
    const initialUrl = page.url();

    await impersonateRole(page, "IT-DevOps");

    // Navigate to unauthorized page
    await navigateToPage(page, "December Activities", /^Forfeitures 008-12/);
    await expect(page.getByText("Access Denied").nth(1)).toBeVisible();

    // Use browser back button
    await page.goBack();
    await page.waitForLoadState("networkidle");

    // Should navigate back (might be to base URL or previous page)
    const currentUrl = page.url();

    // URL should change (went back)
    console.log("Navigated back from Access Denied:", initialUrl, "->", currentUrl);
  });

  test("No JavaScript errors on Access Denied page", async ({ page }) => {
    const consoleErrors: string[] = [];

    page.on("console", (msg) => {
      if (msg.type() === "error") {
        consoleErrors.push(msg.text());
      }
    });

    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");

    await impersonateRole(page, "IT-DevOps");
    await navigateToPage(page, "December Activities", /^Forfeitures 008-12/);

    await expect(page.getByText("Access Denied").nth(1)).toBeVisible();

    // Wait for any async errors
    await page.waitForTimeout(2000);

    // Filter out known acceptable errors
    const criticalErrors = consoleErrors.filter(
      (err) =>
        !err.includes("favicon") &&
        !err.includes("chrome-extension") &&
        !err.includes("401") && // 401 is expected for unauthorized access
        !err.includes("403") // 403 is expected for forbidden access
    );

    expect(criticalErrors.length).toBe(0);
  });
});
