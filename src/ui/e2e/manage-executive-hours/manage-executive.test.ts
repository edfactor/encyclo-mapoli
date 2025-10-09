import { expect, test } from "@playwright/test";
import { baseUrl, impersonateRole, navigateToPage, setPageStatus } from "../env.setup";

test.describe("Manage Executive Hours: ", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");
    await impersonateRole(page, "Finance-Manager");

    // Use the new navigation helper with retry logic
    await navigateToPage(page, "Fiscal Close", "Manage Executive Hours");

    // Set page status to "In Progress" to enable search functionality
    await setPageStatus(page, "In Progress");
  });

  test("page load successfully", async ({ page }) => {
    await expect(page.getByRole("heading", { name: "Manage Executive Hours and" })).toBeVisible();
  });

  test("check access on other roles.", async ({ page }) => {
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "Finance-Manager" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "Distributions-Clerk" }).getByRole("checkbox").check();
    await page.locator("body").click();
    await page.reload();
    await page.waitForLoadState("networkidle");
    await expect(page.getByText("Access Denied").nth(1)).toBeVisible();
  });

  test("add badge number and click on search", async ({ page }) => {
    await page.locator('input[name="badgeNumber"]').fill("706056");
    await page.getByTestId("searchButton").click();
    const [response] = await Promise.all([
      page.waitForResponse((resp) => resp.url().includes("yearend/executive-hours-and-dollars"))
    ]);
    await expect(response.status()).toBe(200);
  });

  test("add full name and click on search", async ({ page }) => {
    await page.locator('input[name="fullNameContains"]').fill("EVANS, ZOE");
    await page.getByTestId("searchButton").click();
    const [response] = await Promise.all([
      page.waitForResponse((resp) => resp.url().includes("yearend/executive-hours-and-dollars"))
    ]);
    await expect(response.status()).toBe(200);
  });

  test("check the checkbox `Has Executive Hours and Dollars` with badge and click on search", async ({ page }) => {
    // Add a badge number to ensure search criteria is valid
    await page.locator('input[name="badgeNumber"]').fill("706056");

    // Check the "Has Executive Hours and Dollars" checkbox
    const checkbox = page
      .locator("div")
      .filter({ hasText: /^Has Executive Hours and Dollars$/ })
      .getByRole("checkbox");
    await checkbox.waitFor({ state: "visible", timeout: 10000 });
    await checkbox.check();

    // Verify checkbox is checked
    await expect(checkbox).toBeChecked();

    // Click search and wait for API response
    await page.getByTestId("searchButton").click();

    const [response] = await Promise.all([
      page.waitForResponse((resp) => resp.url().includes("yearend/executive-hours-and-dollars"))
    ]);

    await expect(response.status()).toBe(200);
  });

  test("check the checkbox `Monthly payroll` and click on search", async ({ page }) => {
    await page
      .locator("div")
      .filter({ hasText: /^Monthly Payroll$/ })
      .getByRole("checkbox")
      .check();
    await page.getByTestId("searchButton").click();
    const [response] = await Promise.all([
      page.waitForResponse((resp) => resp.url().includes("yearend/executive-hours-and-dollars"))
    ]);
    await expect(response.status()).toBe(200);
  });

  test("checking pagination", async ({ page }) => {
    await page
      .locator("div")
      .filter({ hasText: /^Has Executive Hours and Dollars$/ })
      .getByRole("checkbox")
      .check();
    await page.getByTestId("searchButton").click();
    const [response] = await Promise.all([
      page.waitForResponse((resp) => resp.url().includes("yearend/executive-hours-and-dollars"))
    ]);
    await expect(response.status()).toBe(200);
    // Locate the select dropdown
    const pageSizeSelect = page.locator('select[aria-label="rows per page"]');

    // Select by value
    await pageSizeSelect.selectOption("10");
    const isNextPageDisabled = await page.getByRole("button", { name: "next page" }).isDisabled();

    if (!isNextPageDisabled) {
      await page.getByRole("button", { name: "next page" }).click();
      const [response1] = await Promise.all([
        page.waitForResponse((resp) => resp.url().includes("yearend/executive-hours-and-dollars"))
      ]);
      await expect(response1.status()).toBe(200);
    }
  });

  test("update executive hours from the grid", async ({ page }) => {
    const [response] = await Promise.all([
      page.waitForResponse((resp) => resp.url().includes("yearend/executive-hours-and-dollars")),

      (async () => {
        await page.locator('input[name="badgeNumber"]').fill("706056");
        await page.getByTestId("searchButton").click();
      })()
    ]);
    await expect(response.status()).toBe(200);

    const [response1] = await Promise.all([
      page.waitForResponse((resp) => resp.url().includes("yearend/executive-hours-and-dollars")),
      (async () => {
        await page.getByRole("gridcell").nth(4).dblclick();
        const randomValue = String(Math.floor(Math.random() * 100) + 1);
        await page.getByRole("textbox", { name: "Input Editor" }).fill(randomValue);
        await page.getByRole("gridcell").nth(5).click();
        await page.getByRole("button", { name: "Save", exact: true }).click();
      })()
    ]);
    await expect(response1.status()).toBe(204);
  });
});
