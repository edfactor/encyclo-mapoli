import { expect, test } from "@playwright/test";
import { baseUrl, impersonateRole, navigateToPage } from "../env.setup";

test.describe("Profit Share Report (PAY426): ", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");
    await impersonateRole(page, "Finance-Manager");
    await navigateToPage(page, "December Activities", "Profit SHare Report");
  });

  test("Page render successfully", async ({ page }) => {
    await expect(page.getByRole("heading", { name: "Profit Share Report (PAY426)" })).toBeVisible();
  });

  test("changing status of PAY426", async ({ page }) => {
    await page.getByRole("combobox").nth(1).click();
    await page.getByRole("option", { name: "Complete" }).click();
    const [response] = await Promise.all([page.waitForResponse((resp) => resp.url().includes("api/navigation"))]);
    const json = await response.json();
    return;
  });
  test("Changing forfeit amount", async ({ page }) => {
    await page.getByRole("row").nth(10).click();
    await expect(page.getByRole("button", { name: "Filter" })).toBeVisible();
  });
});
