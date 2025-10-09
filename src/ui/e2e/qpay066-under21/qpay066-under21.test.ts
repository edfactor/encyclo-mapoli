import { test, expect } from "@playwright/test";
import { baseUrl } from "../env.setup";

test.describe("Profit Share Gross Report (QPAY501): ", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "Finance-Manager" }).getByRole("checkbox").check();
    await page.locator("body").click();
    await page.reload();
    await page.waitForLoadState("networkidle");
    await page.getByRole("button").filter({ hasText: /^$/ }).click();
    await page.getByRole("button", { name: "Fiscal Close" }).click();
    await page.getByRole("button", { name: "Prof Share by Store" }).click();
    await page.getByRole("button", { name: "Under-21 Report" }).click();
  });

  test("Page render successfully", async ({ page }) => {
    const [response] = await Promise.all([
      page.waitForResponse((resp) => resp.url().includes("yearend/post-frozen/under-21-breakdown-by-store"))
    ]);
    await expect(response.status()).toBe(200);
  });

  test("changing status updates navigation api", async ({ page }) => {
    await page.getByRole("combobox").nth(1).click();
    await page.getByRole("option", { name: "Complete" }).click();
    const [response] = await Promise.all([page.waitForResponse((resp) => resp.url().includes("api/navigation"))]);
    const json = await response.json();
    await expect(json.isSuccessful).toBe(true);
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

  test("click on badge number", async ({ page }) => {
    await page.getByRole("gridcell").nth(0).locator("a").click();
    await expect(page.url()).toContain("master-inquiry");
  });
});
