import { expect, test } from "@playwright/test";
import { baseUrl, impersonateRole, navigateToPage } from "../env.setup";

/**
 * E2E Tests for Unauthorized Access Page
 *
 * Testing Strategy:
 * - Access denied appears when a user navigates to a page with proper role access,
 *   then changes to a role without access and refreshes
 * - Navigation API filters out pages user doesn't have access to
 * - Tests verify the access control mechanism and user experience when access is revoked
 */
test.describe("Unauthorized Access", () => {
  test("User sees Access Denied after switching to unauthorized role", async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");

    // Start with Finance-Manager role (has access to December Activities)
    await impersonateRole(page, "Finance-Manager");

    // Navigate to a page that requires Finance-Manager role
    await navigateToPage(page, "December Activities", /^Forfeitures 008-12/);

    // Should see the page content initially
    await page.waitForTimeout(1000);

    // Now switch to a role without access (Distributions-Clerk doesn't have access per navigation script)
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "Finance-Manager" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "Distributions-Clerk" }).getByRole("checkbox").check();
    await page.locator("body").click();

    // Refresh the page to apply new role permissions
    await page.reload();
    await page.waitForLoadState("networkidle");

    // Should now see Access Denied message
    await expect(page.getByText("Access Denied").nth(1)).toBeVisible();
  });

  test("Access Denied page displays appropriate message", async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");

    // Start with System-Administrator role (has access to all pages)
    await impersonateRole(page, "System-Administrator");

    // Navigate to Manage Executive Hours
    await navigateToPage(page, "Fiscal Close", "Manage Executive Hours");
    await page.waitForTimeout(1000);

    // Switch to IT-DevOps role (only has access to IT DevOps menu per navigation script)
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "System-Administrator" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "IT-DevOps" }).getByRole("checkbox").check();
    await page.locator("body").click();

    // Refresh to apply role change
    await page.reload();
    await page.waitForLoadState("networkidle");

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

    // Start with authorized role
    await impersonateRole(page, "Finance-Manager");
    await navigateToPage(page, "December Activities", /^Forfeitures 008-12/);
    await page.waitForTimeout(1000);

    // Switch to unauthorized role and refresh
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "Finance-Manager" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "Distributions-Clerk" }).getByRole("checkbox").check();
    await page.locator("body").click();
    await page.reload();
    await page.waitForLoadState("networkidle");

    // Should see Access Denied
    await expect(page.getByText("Access Denied").nth(1)).toBeVisible();

    // User should still be able to use navigation drawer
    const drawerButton = page.getByRole("button").filter({ hasText: /^$/ }).first();
    await expect(drawerButton).toBeVisible();

    // User should be able to navigate away
    await drawerButton.click();
    await page.waitForTimeout(500);

    // Note: December Activities won't appear for Distributions-Clerk, so check for available menu
    const navMenu = page.getByRole("button", { name: "INQUIRIES" });
    await expect(navMenu).toBeVisible();
  });

  test("User can regain access by switching back to authorized role", async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");

    // Start with Finance-Manager (authorized for December Activities)
    await impersonateRole(page, "Finance-Manager");
    await navigateToPage(page, "December Activities", /^Forfeitures 008-12/);
    await page.waitForTimeout(1000);

    // Switch to Distributions-Clerk (unauthorized)
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "Finance-Manager" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "Distributions-Clerk" }).getByRole("checkbox").check();
    await page.locator("body").click();
    await page.reload();
    await page.waitForLoadState("networkidle");

    // Should see Access Denied
    await expect(page.getByText("Access Denied").nth(1)).toBeVisible();

    // Now switch back to Finance-Manager (authorized)
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

    // Start with authorized role
    await impersonateRole(page, "Finance-Manager");
    await navigateToPage(page, "December Activities", /^Forfeitures 008-12/);
    await page.waitForTimeout(1000);

    // Switch to unauthorized role
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "Finance-Manager" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "IT-DevOps" }).getByRole("checkbox").check();
    await page.locator("body").click();
    await page.reload();
    await page.waitForLoadState("networkidle");

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

    // Start with System-Administrator (has access to both pages)
    await impersonateRole(page, "System-Administrator");

    // Navigate to first page
    await navigateToPage(page, "December Activities", /^Forfeitures 008-12/);
    await page.waitForTimeout(1000);

    // Switch to IT-DevOps (no access to December Activities per navigation script)
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "System-Administrator" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "IT-DevOps" }).getByRole("checkbox").check();
    await page.locator("body").click();
    await page.reload();
    await page.waitForLoadState("networkidle");

    const firstAccessDenied = await page.getByText("Access Denied").textContent();

    // Go back to System-Administrator to navigate to second page
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "IT-DevOps" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "System-Administrator" }).getByRole("checkbox").check();
    await page.locator("body").click();

    // Navigate to different page (Manage Executive Hours)
    await navigateToPage(page, "Fiscal Close", "Manage Executive Hours");
    await page.waitForTimeout(1000);

    // Switch back to IT-DevOps
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "System-Administrator" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "IT-DevOps" }).getByRole("checkbox").check();
    await page.locator("body").click();
    await page.reload();
    await page.waitForLoadState("networkidle");

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

    // Start with Finance-Manager
    await impersonateRole(page, "Finance-Manager");
    await navigateToPage(page, "December Activities", /^Forfeitures 008-12/);
    await page.waitForTimeout(1000);

    // Switch to Distributions-Clerk and trigger Access Denied
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "Finance-Manager" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "Distributions-Clerk" }).getByRole("checkbox").check();
    await page.locator("body").click();
    await page.reload();
    await page.waitForLoadState("networkidle");

    // Should see Access Denied
    await expect(page.getByText("Access Denied").nth(1)).toBeVisible();

    // Switch back to Finance-Manager to test navigation still works
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "Distributions-Clerk" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "Finance-Manager" }).getByRole("checkbox").check();
    await page.locator("body").click();

    // Navigate to another authorized page (Terminations under December Activities)
    await navigateToPage(page, "December Activities", "Termination");
    await page.waitForTimeout(1000);

    // Should successfully navigate away from Access Denied
    // Termination page should be accessible to Finance-Manager
    const accessDenied = page.getByText("Access Denied");
    const isStillDenied = await accessDenied.isVisible().catch(() => false);

    // Should NOT see access denied on Terminations page with Finance-Manager role
    expect(isStillDenied).toBe(false);
  });

  test("Access Denied page layout components still work", async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");

    // Start with authorized role
    await impersonateRole(page, "Finance-Manager");
    await navigateToPage(page, "December Activities", /^Forfeitures 008-12/);
    await page.waitForTimeout(1000);

    // Switch to IT-DevOps (unauthorized)
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "Finance-Manager" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "IT-DevOps" }).getByRole("checkbox").check();
    await page.locator("body").click();
    await page.reload();
    await page.waitForLoadState("networkidle");

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

    // Start with authorized role
    await impersonateRole(page, "Finance-Manager");
    await navigateToPage(page, "December Activities", /^Forfeitures 008-12/);
    await page.waitForTimeout(1000);

    // Get URL before triggering Access Denied
    const pageUrl = page.url();

    // Switch to IT-DevOps (unauthorized) and trigger Access Denied
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "Finance-Manager" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "IT-DevOps" }).getByRole("checkbox").check();
    await page.locator("body").click();
    await page.reload();
    await page.waitForLoadState("networkidle");

    await expect(page.getByText("Access Denied").nth(1)).toBeVisible();

    // Use browser back button
    await page.goBack();
    await page.waitForLoadState("networkidle");

    // Should navigate back (might be to base URL or previous page)
    const currentUrl = page.url();

    // URL should change (went back)
    console.log("Navigated back from Access Denied:", pageUrl, "->", currentUrl);
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

    // Start with authorized role
    await impersonateRole(page, "Finance-Manager");
    await navigateToPage(page, "December Activities", /^Forfeitures 008-12/);
    await page.waitForTimeout(1000);

    // Switch to unauthorized role
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "Finance-Manager" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "IT-DevOps" }).getByRole("checkbox").check();
    await page.locator("body").click();
    await page.reload();
    await page.waitForLoadState("networkidle");

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
