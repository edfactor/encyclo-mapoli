import { expect, test } from "@playwright/test";
import { baseUrl, impersonateRole, navigateToPage } from "../env.setup";

test.describe.serial("Pay 443: ", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");
    await impersonateRole(page, "Finance-Manager");
    await navigateToPage(page, "Fiscal Close", "Profit Share Forfeit");
  });

  test("page load successfully", async ({ page }) => {
    await expect(page.getByRole("heading", { name: "Forfeit (PAY443)" })).toBeVisible();
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

  test("click on search button", async ({ page }) => {
    await page.getByTestId("searchButton").click();
    const [response] = await Promise.all([
      page.waitForResponse((resp) => resp.url().includes("yearend/frozen/forfeitures-and-points"))
    ]);
    await expect(response.status()).toBe(200);
  });

  test("checking pagination", async ({ page }) => {
    await page.getByTestId("searchButton").click();
    const [response] = await Promise.all([
      page.waitForResponse((resp) => resp.url().includes("yearend/frozen/forfeitures-and-points"))
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
        page.waitForResponse((resp) => resp.url().includes("yearend/frozen/forfeitures-and-points"))
      ]);
      await expect(response1.status()).toBe(200);
    }
  });

  test("changing status of YTD Wages Extract", async ({ page }) => {
    const [response] = await Promise.all([
      page.waitForResponse((resp) => resp.url().includes("api/navigation")),
      (async () => {
        await page.getByRole("combobox").nth(1).click();
        await page.getByRole("option", { name: "Complete" }).click();
      })()
    ]);
    const json = await response.json();
    return;
  });
});
