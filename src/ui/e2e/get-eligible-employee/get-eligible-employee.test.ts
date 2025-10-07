import { test, expect } from "@playwright/test";
import { baseUrl, impersonateRole } from "../env.setup";

test.describe("Manage Executive Hours: ", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");
    await impersonateRole(page, "Finance-Manager");
    await page.getByRole("button").filter({ hasText: /^$/ }).click();
    await page.getByRole("button", { name: "Fiscal Close" }).click();
    await page.getByRole("button", { name: "Get Eligible Employees" }).click();
  });

  test("page load successfully", async ({ page }) => {
    const [response] = await Promise.all([
      page.waitForResponse((resp) => resp.url().includes("yearend/eligible-employees"))
    ]);
    await expect(response.status()).toBe(200);
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

  test("checking pagination", async ({ page }) => {
    const [response] = await Promise.all([
      page.waitForResponse((resp) => resp.url().includes("yearend/eligible-employees"))
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
        page.waitForResponse((resp) => resp.url().includes("yearend/eligible-employees"))
      ]);
      await expect(response1.status()).toBe(200);
    }
  });

  test("changing status of Get Eligible Employees", async ({ page }) => {
    // Trigger the selection and wait for the API response together to avoid race conditions
    const [response] = await Promise.all([
      page.waitForResponse((resp) => resp.url().includes("api/navigation") && resp.status() === 200, {
        timeout: 20000
      }),
      (async () => {
        await page.getByRole("combobox").nth(1).click();
        await page.getByRole("option", { name: "Complete" }).click();
      })()
    ]);
    const json = await response.json().catch(() => undefined);
    await expect(json).toBeTruthy();
  });

  test("click on badge number", async ({ page }) => {
    await page.getByRole("gridcell").nth(0).locator("a").click();
    await expect(page.url()).toContain("master-inquiry");
  });
});
