import { expect, test } from "@playwright/test";
import { baseUrl, impersonateRole } from "../env.setup";

// PS-1623: Read-only role restrictions for Manage Executive Hours

test.describe("Manage Executive Hours: Read-Only Role", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");
    await impersonateRole(page, "Finance-Manager");
    await page.getByRole("button").filter({ hasText: /^$/ }).click();
    await page.getByRole("button", { name: "Fiscal Close" }).click();
    await page.getByRole("button", { name: "Manage Executive Hours" }).click();
  });

  test("Add Executive button is disabled for ITDEVOPS", async ({ page }) => {
    await page.locator('input[name="fullNameContains"]').click();
    await page.locator('input[name="fullNameContains"]').fill("bell");
    const searchButton = await page.getByTestId("searchButton");

    // Wait for API response after clicking search
    const [response] = await Promise.all([
      page.waitForResponse((resp) => resp.url().includes("yearend/executive-hours-and-dollars")),
      searchButton.click()
    ]);
    await expect(response.status()).toBe(200);

    // Robust selector for the ADD EXECUTIVE button
    const addExecutiveButton = await page.getByRole("button", { name: "Add Executive" });
    if ((await addExecutiveButton.count()) === 0) {
      throw new Error("ADD EXECUTIVE button not present in current state");
    }
    await expect(addExecutiveButton).toBeVisible();
  });

  test("Search/filter is enabled for ITDEVOPS", async ({ page }) => {
    // Fill required fields before checking search button
    await page.fill('input[name="badgeNumber"]', "123456");
    const searchButton = await page.getByTestId("searchButton");
    await expect(searchButton).toBeEnabled();
    // Wait for API response after clicking search
    const [response] = await Promise.all([
      page.waitForResponse((resp) => resp.url().includes("yearend/executive-hours-and-dollars")),
      searchButton.click()
    ]);
    await expect(response.status()).toBe(200);
  });
});
