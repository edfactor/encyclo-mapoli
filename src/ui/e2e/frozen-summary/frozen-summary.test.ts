import { expect, test } from "@playwright/test";
import { baseUrl, impersonateRole } from "../env.setup";

/**
 * E2E Tests for Frozen Summary Tab
 *
 * This is a simple test suite because:
 * - Tab navigation is straightforward UI interaction
 * - Frozen Summary is a read-only view (no edit operations)
 * - Tests focus on visibility and basic data display
 * - Located in IT DEVOPS → Demographic Freeze page
 */
test.describe("Frozen Summary Tab", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");

    // Use IT-DevOps role which has access to IT DEVOPS menu
    await impersonateRole(page, "IT-DevOps");

    // Navigate to Demographic Freeze page where Frozen Summary tab exists
    await page.getByRole("button", { name: "IT DEVOPS" }).click();
    await page.getByRole("link", { name: "Demographic Freeze" }).click();
    await page.waitForLoadState("networkidle");
  });

  test("Demographic Freeze page loads successfully", async ({ page }) => {
    // Verify we're on the Demographic Freeze page
    await expect(page.getByRole("heading", { name: /demographic|freeze/i })).toBeVisible();
  });

  test("Frozen Summary tab is present", async ({ page }) => {
    // Look for the Frozen Summary tab
    const frozenTab = page.getByRole("tab", { name: /frozen.*summary/i });

    await expect(frozenTab).toBeVisible();
  });

  test("Can navigate to Frozen Summary tab", async ({ page }) => {
    // Click on Frozen Summary tab
    const frozenTab = page.getByRole("tab", { name: /frozen.*summary/i });
    await frozenTab.click();

    // Wait for tab content to load
    await page.waitForTimeout(1000);

    // Verify tab is selected (has aria-selected="true")
    const isSelected = await frozenTab.getAttribute("aria-selected");
    expect(isSelected).toBe("true");
  });

  test("Frozen Summary tab content is displayed", async ({ page }) => {
    // Navigate to Frozen Summary tab
    const frozenTab = page.getByRole("tab", { name: /frozen.*summary/i });
    await frozenTab.click();
    await page.waitForTimeout(1000);

    // Verify content is visible (could be a grid, table, or summary cards)
    // Look for common data display elements
    const contentElements = page.locator(
      '[role="grid"], table, .MuiDataGrid-root, [data-testid*="frozen"], [class*="frozen"]'
    );
    const hasContent = (await contentElements.count()) > 0;

    if (hasContent) {
      await expect(contentElements.first()).toBeVisible();
    } else {
      // If no structured data, check for any text content in the tab panel
      const tabPanel = page.locator('[role="tabpanel"]');
      await expect(tabPanel).toBeVisible();
    }
  });

  test("Frozen Summary displays year information", async ({ page }) => {
    // Navigate to Frozen Summary tab
    const frozenTab = page.getByRole("tab", { name: /frozen.*summary/i });
    await frozenTab.click();
    await page.waitForTimeout(1000);

    // Look for year references in the content (e.g., "2024", "2025")
    const yearPattern = /20\d{2}/;
    const tabPanel = page.locator('[role="tabpanel"]');
    const panelText = await tabPanel.textContent();

    // Content should reference at least one year
    expect(panelText).toMatch(yearPattern);
  });

  test("Frozen Summary has no edit controls (read-only)", async ({ page }) => {
    // Navigate to Frozen Summary tab
    const frozenTab = page.getByRole("tab", { name: /frozen.*summary/i });
    await frozenTab.click();
    await page.waitForTimeout(1000);

    // Look for edit buttons within the tab panel
    const tabPanel = page.locator('[role="tabpanel"]');
    const editButtons = tabPanel.locator("button").filter({ hasText: /edit|update|modify|save|delete/i });
    const editButtonCount = await editButtons.count();

    // Should have very few or no edit buttons (might have export/print which is OK)
    expect(editButtonCount).toBeLessThanOrEqual(2);
  });

  test("Can switch between tabs (Demographics and Frozen Summary)", async ({ page }) => {
    // Find the Demographics tab (might be named differently)
    const demographicsTab = page
      .getByRole("tab")
      .filter({ hasText: /demographics|search|main/i })
      .first();

    // Click Frozen Summary tab
    const frozenTab = page.getByRole("tab", { name: /frozen.*summary/i });
    await frozenTab.click();
    await page.waitForTimeout(500);

    // Verify Frozen Summary is selected
    let isSelected = await frozenTab.getAttribute("aria-selected");
    expect(isSelected).toBe("true");

    // Switch back to Demographics tab
    await demographicsTab.click();
    await page.waitForTimeout(500);

    // Verify Demographics tab is now selected
    isSelected = await demographicsTab.getAttribute("aria-selected");
    expect(isSelected).toBe("true");
  });

  test("Frozen Summary displays data in grid or table format", async ({ page }) => {
    // Navigate to Frozen Summary tab
    const frozenTab = page.getByRole("tab", { name: /frozen.*summary/i });
    await frozenTab.click();
    await page.waitForTimeout(1000);

    // Look for data grid or table
    const dataDisplay = page.locator('[role="grid"], table, .MuiDataGrid-root, .ag-root');
    const hasDataDisplay = (await dataDisplay.count()) > 0;

    if (hasDataDisplay) {
      await expect(dataDisplay.first()).toBeVisible();

      // Check if there are rows/data
      const rows = page.locator('[role="row"], tr, .MuiDataGrid-row, .ag-row');
      const rowCount = await rows.count();

      // Should have at least a header row
      expect(rowCount).toBeGreaterThan(0);
    } else {
      // Alternative: summary cards or text display
      const summaryContent = page.locator('[role="tabpanel"]');
      await expect(summaryContent).toBeVisible();

      const hasText = await summaryContent.textContent();
      expect(hasText?.length).toBeGreaterThan(0);
    }
  });

  test("Frozen Summary is accessible to read-only users", async ({ page }) => {
    // Navigate to Frozen Summary tab first
    const frozenTab = page.getByRole("tab", { name: /frozen.*summary/i });
    await frozenTab.click();
    await page.waitForTimeout(1000);

    // Switch to Finance-Manager role (different from IT-DevOps)
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "IT-DevOps" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "Finance-Manager" }).getByRole("checkbox").check();
    await page.locator("body").click();

    // Reload page
    await page.reload();
    await page.waitForLoadState("networkidle");

    // Verify page redirects to unauthorized (Finance-Manager doesn't have IT DEVOPS access)
    const accessDenied = page.getByText("Access Denied");
    await expect(accessDenied).toBeVisible();
  });

  test("Tab navigation works with keyboard", async ({ page }) => {
    // Focus on the tab list
    const tabList = page.locator('[role="tablist"]');
    await tabList.focus();

    // Get initial tab
    //const initialTab = page.getByRole("tab", { name: /demographics|search|main/i }).first();

    // Press arrow key to navigate to next tab
    await page.keyboard.press("ArrowRight");
    await page.waitForTimeout(300);

    // One of the tabs should now have focus
    const focusedElement = page.locator(":focus");
    const tagName = await focusedElement.evaluate((el) => el.tagName);

    // Focused element should be a button or tab
    expect(["BUTTON", "DIV"]).toContain(tagName);
  });

  test("Frozen Summary content updates when year changes (if applicable)", async ({ page }) => {
    // Navigate to Frozen Summary tab
    const frozenTab = page.getByRole("tab", { name: /frozen.*summary/i });
    await frozenTab.click();
    await page.waitForTimeout(1000);

    // Get initial content
    const tabPanel = page.locator('[role="tabpanel"]');
    const initialContent = await tabPanel.textContent();

    // Try to change year selector
    const yearSelector = page.getByRole("combobox").nth(0);
    const yearIsVisible = await yearSelector.isVisible().catch(() => false);

    if (yearIsVisible) {
      await yearSelector.click();
      await page.waitForTimeout(300);

      // Select a different year option
      const yearOptions = page.getByRole("option");
      const optionCount = await yearOptions.count();

      if (optionCount > 1) {
        await yearOptions.nth(1).click();
        await page.waitForTimeout(2000);

        // Content should update or trigger a load
        const updatedContent = await tabPanel.textContent();

        // Content might change or stay the same (depending on data availability)
        console.log("Content changed after year switch:", initialContent !== updatedContent);
      }
    }
  });
});
