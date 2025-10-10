import { expect, test } from "@playwright/test";
import { baseUrl, impersonateRole, navigateToPage } from "../env.setup";

/**
 * E2E Tests for Unauthorized Access Page
 *
 * Testing Strategy:
 * - Tests the static "Access Denied" page by navigating directly to /unauthorized
 * - Verifies that the unauthorized page renders properly with query parameters
 * - Verifies that users can navigate away from the access denied page
 * - Note: Actual authorization enforcement is handled server-side and by navigation API
 *   These tests focus on the unauthorized page UX, not the authorization logic itself
 */
test.describe("Unauthorized Access", () => {
  test("Unauthorized page displays with required roles message", async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");

    // Navigate directly to unauthorized page with query params
    await page.goto(`${baseUrl}/unauthorized?requiredRoles=Finance-Manager&page=Forfeitures`);
    await page.waitForLoadState("networkidle");

    // Should see Access Denied heading (h2, not the Page component h4)
    const heading = page.locator("h2").filter({ hasText: "Access Denied" });
    await expect(heading).toBeVisible();

    // Should see information about required roles
    await expect(page.getByText(/Finance-Manager/)).toBeVisible();
  });

  test("Unauthorized page displays navigation restriction message", async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");

    // Navigate directly to unauthorized page with navigation restriction reason
    await page.goto(`${baseUrl}/unauthorized?page=some-restricted-page&reason=navigation_restricted`);
    await page.waitForLoadState("networkidle");

    // Should see Access Denied heading
    const heading = page.locator("h2").filter({ hasText: "Access Denied" });
    await expect(heading).toBeVisible();

    // Should see message about navigation restriction
    await expect(page.getByText(/not included in your navigation/i)).toBeVisible();
  });

  test("Unauthorized page has navigation controls available", async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");

    // Navigate to unauthorized page
    await page.goto(`${baseUrl}/unauthorized?requiredRoles=Finance-Manager&page=Forfeitures`);
    await page.waitForLoadState("networkidle");

    // Should see Access Denied
    const heading = page.locator("h2").filter({ hasText: "Access Denied" });
    await expect(heading).toBeVisible();

    // Should have "Go to Home" button
    const homeButton = page.getByRole("button", { name: /go to home/i });
    await expect(homeButton).toBeVisible();

    // Should have "Go Back" button
    const backButton = page.getByRole("button", { name: /go back/i });
    await expect(backButton).toBeVisible();
  });

  test("Go to Home button navigates to home page", async ({ page }) => {
    await page.goto(`${baseUrl}/unauthorized?requiredRoles=Finance-Manager&page=Forfeitures`);
    await page.waitForLoadState("networkidle");

    // Click "Go to Home" button
    const homeButton = page.getByRole("button", { name: /go to home/i });
    await homeButton.click();
    await page.waitForLoadState("networkidle");

    // Should navigate to home page
    expect(page.url()).toBe(`${baseUrl}/`);
  });

  test("Unauthorized page does not expose sensitive data", async ({ page }) => {
    await page.goto(`${baseUrl}/unauthorized?requiredRoles=Finance-Manager&page=Forfeitures`);
    await page.waitForLoadState("networkidle");

    // Verify that no data grids or sensitive content is visible
    const dataGrids = page.locator('[role="grid"], .MuiDataGrid-root, .ag-root');
    const gridCount = await dataGrids.count();

    // Should have no or minimal data grids visible
    expect(gridCount).toBeLessThanOrEqual(1);

    // Verify no actual data rows with badge numbers are visible
    const dataRows = page.locator('[role="row"], .MuiDataGrid-row, .ag-row').filter({ hasText: /\d{6}/ }); 
    const rowCount = await dataRows.count();
    expect(rowCount).toBe(0);
  });

  test("Unauthorized page shows consistent messaging for different pages", async ({ page }) => {
    // Test first page
    await page.goto(`${baseUrl}/unauthorized?requiredRoles=Finance-Manager&page=Forfeitures`);
    await page.waitForLoadState("networkidle");
    const firstHeading = page.locator("h2").filter({ hasText: "Access Denied" });
    await expect(firstHeading).toBeVisible();

    // Test second page
    await page.goto(`${baseUrl}/unauthorized?requiredRoles=System-Administrator&page=Executive%20Hours`);
    await page.waitForLoadState("networkidle");
    const secondHeading = page.locator("h2").filter({ hasText: "Access Denied" });
    await expect(secondHeading).toBeVisible();

    // Both should show "Access Denied"
    expect(await firstHeading.textContent()).toContain("Access Denied");
    expect(await secondHeading.textContent()).toContain("Access Denied");
  });

  test("Unauthorized page layout components still work", async ({ page }) => {
    await page.goto(`${baseUrl}/unauthorized?requiredRoles=Finance-Manager&page=Forfeitures`);
    await page.waitForLoadState("networkidle");

    // Header components should still be visible and functional
    // Year selector should be visible
    const yearSelector = page.getByRole("combobox").nth(0);
    const yearExists = await yearSelector.count();
    expect(yearExists).toBeGreaterThan(0);

    // Navigation drawer toggle should be accessible
    const drawerButton = page.getByRole("button").filter({ hasText: /^$/ }).first();
    const drawerExists = await drawerButton.count();
    expect(drawerExists).toBeGreaterThan(0);
  });

  test("Browser back button works from unauthorized page", async ({ page }) => {
    // Start on home page
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");
    
    // Navigate to unauthorized page
    await page.goto(`${baseUrl}/unauthorized?requiredRoles=Finance-Manager&page=Forfeitures`);
    await page.waitForLoadState("networkidle");

    // Use browser back button
    await page.goBack();
    await page.waitForLoadState("networkidle");

    // Should navigate back to home
    expect(page.url()).toBe(`${baseUrl}/`);
  });

  test("No JavaScript errors on unauthorized page", async ({ page }) => {
    const consoleErrors: string[] = [];

    page.on("console", (msg) => {
      if (msg.type() === "error") {
        consoleErrors.push(msg.text());
      }
    });

    await page.goto(`${baseUrl}/unauthorized?requiredRoles=Finance-Manager&page=Forfeitures`);
    await page.waitForLoadState("networkidle");

    // Wait for any async errors
    await page.waitForTimeout(2000);

    // Filter out known acceptable errors
    const criticalErrors = consoleErrors.filter(
      (err) =>
        !err.includes("favicon") &&
        !err.includes("chrome-extension") &&
        !err.includes("401") &&
        !err.includes("403")
    );

    expect(criticalErrors.length).toBe(0);
  });
});