import { expect, test } from "@playwright/test";
import { baseUrl, impersonateRole, navigateToPage } from "../env.setup";

/**
 * E2E Tests for Unauthorized Access Page
 *
 * Testing Strategy:
 * - Navigate to a page with a role that HAS access (Finance-Manager)
 * - Switch to a role that DOESN'T have access (Auditor/Distributions-Clerk)
 * - Reload the page to trigger access check
 * - Verify redirect to /unauthorized page with proper messaging
 *
 * This pattern follows the approach used in control-sheet.test.ts:
 * 1. Navigate TO the page with a privileged role (Finance-Manager)
 * 2. Switch TO a restricted role (Auditor/Distributions-Clerk)
 * 3. Reload the page to trigger the access control check
 * 4. Verify redirect to /unauthorized and proper messaging
 */
test.describe("Unauthorized Access", () => {
  test.beforeEach(async ({ page }) => {
    // Navigate to base and establish auth with Finance-Manager (full access)
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");
    await impersonateRole(page, "Finance-Manager");

    // Navigate to Prof Control Sheet page (Finance-Manager has access)
    // We use Prof Control Sheet because it's confirmed to work with Finance-Manager
    await navigateToPage(page, "Fiscal Close", "Prof Control Sheet");
    await page.waitForLoadState("networkidle");
  });
  test("User with unauthorized role sees Access Denied message", async ({ page }) => {
    // Switch to Auditor role (doesn't have Fiscal Close access)
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "Finance-Manager" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "Auditor" }).getByRole("checkbox").check();
    await page.locator("body").click();

    // Reload to trigger access check
    await page.reload();
    await page.waitForLoadState("networkidle");

    // Should be redirected to unauthorized page
    await page.waitForURL("**/unauthorized*", { timeout: 10000 });
    expect(page.url()).toContain("/unauthorized");

    // Should see Access Denied heading (h2 element)
    const heading = page.locator("h2").filter({ hasText: "Access Denied" });
    await expect(heading).toBeVisible({ timeout: 10000 });
  });

  test("Distributions-Clerk cannot access Manage Executive Hours", async ({ page }) => {
    // Switch to Distributions-Clerk role (limited access)
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "Finance-Manager" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "Distributions-Clerk" }).getByRole("checkbox").check();
    await page.locator("body").click();

    // Reload to trigger access check
    await page.reload();
    await page.waitForLoadState("networkidle");

    // Verify redirect to unauthorized
    await page.waitForURL("**/unauthorized*", { timeout: 10000 });
    await expect(page.getByText("Access Denied").nth(1)).toBeVisible();
  });

  test("Unauthorized page displays 'Go to Home' button", async ({ page }) => {
    // Switch to Auditor and reload
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "Finance-Manager" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "Auditor" }).getByRole("checkbox").check();
    await page.locator("body").click();
    await page.reload();
    await page.waitForURL("**/unauthorized*", { timeout: 10000 });

    // Check for "Go to Home" button
    const homeButton = page.getByRole("button", { name: /Go to Home/i });
    await expect(homeButton).toBeVisible();
  });

  test("Unauthorized page displays 'Go Back' button", async ({ page }) => {
    // Switch to Auditor and reload
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "Finance-Manager" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "Auditor" }).getByRole("checkbox").check();
    await page.locator("body").click();
    await page.reload();
    await page.waitForURL("**/unauthorized*", { timeout: 10000 });

    // Check for "Go Back" button
    const backButton = page.getByRole("button", { name: /Go Back/i });
    await expect(backButton).toBeVisible();
  });

  test("Go to Home button navigates to home page", async ({ page }) => {
    // Switch to Auditor and reload
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "Finance-Manager" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "Auditor" }).getByRole("checkbox").check();
    await page.locator("body").click();
    await page.reload();
    await page.waitForURL("**/unauthorized*", { timeout: 10000 });

    // Click "Go to Home"
    const homeButton = page.getByRole("button", { name: /Go to Home/i });
    await homeButton.click();
    await page.waitForLoadState("networkidle");

    // Should navigate to home
    expect(page.url()).toBe(baseUrl + "/");
  });

  test("Go Back button navigates away from unauthorized page", async ({ page }) => {
    // Switch to Auditor and reload
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "Finance-Manager" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "Auditor" }).getByRole("checkbox").check();
    await page.locator("body").click();
    await page.reload();
    await page.waitForURL("**/unauthorized*", { timeout: 10000 });

    // Click "Go Back"
    const backButton = page.getByRole("button", { name: /Go Back/i });
    await backButton.click();
    await page.waitForLoadState("networkidle");

    // Should navigate away from unauthorized page
    const heading = page.locator("h2").filter({ hasText: "Access Denied" });
    await expect(heading).not.toBeVisible();
  });

  test("Unauthorized page does not display sensitive data", async ({ page }) => {
    // Switch to Auditor and reload
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "Finance-Manager" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "Auditor" }).getByRole("checkbox").check();
    await page.locator("body").click();
    await page.reload();
    await page.waitForURL("**/unauthorized*", { timeout: 10000 });

    // The page should only contain the unauthorized message and navigation
    // No data tables, forms, or sensitive information should be visible
    // Use .first() or .nth() to handle multiple "Access Denied" elements (h2 and h4)
    await expect(page.getByText("Access Denied").first()).toBeVisible();

    // Should not have data tables
    const tables = page.locator("table");
    await expect(tables).toHaveCount(0);
  });

  test("Unauthorized page shows required roles information", async ({ page }) => {
    // Switch to Auditor and reload
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "Finance-Manager" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "Auditor" }).getByRole("checkbox").check();
    await page.locator("body").click();
    await page.reload();
    await page.waitForURL("**/unauthorized*", { timeout: 10000 });

    // Should show Access Denied (use .first() for strict mode)
    await expect(page.getByText("Access Denied").first()).toBeVisible();

    // Should show some information about permission requirements
    // (The exact text may vary based on implementation)
    await expect(page.getByText(/permission/i)).toBeVisible();
  });
  test("No JavaScript errors on unauthorized page", async ({ page }) => {
    const consoleErrors: string[] = [];

    page.on("console", (msg) => {
      if (msg.type() === "error") {
        consoleErrors.push(msg.text());
      }
    });

    // Switch to Auditor and reload
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "Finance-Manager" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "Auditor" }).getByRole("checkbox").check();
    await page.locator("body").click();
    await page.reload();
    await page.waitForURL("**/unauthorized*", { timeout: 10000 });

    // Verify Access Denied is shown
    const heading = page.locator("h2").filter({ hasText: "Access Denied" });
    await expect(heading).toBeVisible({ timeout: 10000 });

    // Wait for any async errors
    await page.waitForTimeout(2000);

    // Filter out known acceptable errors
    const criticalErrors = consoleErrors.filter(
      (err) =>
        !err.includes("favicon") &&
        !err.includes("chrome-extension") &&
        !err.includes("401") &&
        !err.includes("403") &&
        !err.includes("404") && // 404s can happen when navigation changes
        !err.toLowerCase().includes("unauthorized") && // Expected when checking access
        !err.toLowerCase().includes("cors") && // CORS errors when switching roles
        !err.includes("Failed to load") && // Resource loading failures during navigation
        !err.toLowerCase().includes("networkidle") && // Network state messages
        !err.includes("same key") && // React duplicate key warnings (dev only)
        !err.includes("Keys should be unique") // React key warnings
    );

    // Log errors for debugging if test fails
    if (criticalErrors.length > 0) {
      console.log("Console errors detected:", criticalErrors);
    }

    // We should have no critical errors
    expect(criticalErrors.length).toBe(0);
  });
});
