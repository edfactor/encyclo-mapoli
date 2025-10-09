import { expect, test } from "@playwright/test";
import { baseUrl, impersonateRole, navigateToPage } from "../env.setup";

test.describe("Clean-up Reports: ", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");
    await impersonateRole(page, "Finance-Manager");
    await navigateToPage(page, "December Activities", "Clean up Reports");
  });

  test("Demographic Badges Not In PayProfit", async ({ page }) => {
    const [response] = await Promise.all([
      page.waitForResponse((resp) => resp.url().includes("yearend/demographic-badges-not-in-payprofit")),
      (async () => {
        await page.getByRole("button", { name: "Demographic Badges Not In PayProfit" }).click();
      })()
    ]);
    await expect(response.status()).toBe(200);
  });

  test("Duplicate SSNs in Demographics", async ({ page }) => {
    await page.getByRole("button", { name: "Duplicate SSNs in Demographics" }).click();
    const [response] = await Promise.all([
      page.waitForResponse((resp) => resp.url().includes("yearend/duplicate-ssns"))
    ]);
    await expect(response.status()).toBe(200);
  });

  test("Negative ETVA", async ({ page }) => {
    await page.getByRole("button", { name: "Negative ETVA" }).click();
    const [response] = await Promise.all([
      page.waitForResponse((resp) => resp.url().includes("yearend/negative-evta-ssn"))
    ]);
    await expect(response.status()).toBe(200);
  });

  test("Duplicate Names and Birthdays", async ({ page }) => {
    await page.getByRole("button", { name: "Duplicate Names and Birthdays" }).click();
    const [response] = await Promise.all([
      page.waitForResponse((resp) => resp.url().includes("yearend/duplicate-names-and-birthdays"))
    ]);
    await expect(response.status()).toBe(200);
  });
});
