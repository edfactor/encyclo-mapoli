import { expect, test } from "@playwright/test";
import { baseUrl } from "../env.setup";
import {
  expectSearchOk,
  gotoMasterInquiry,
  searchAndGetJson,
  setRole,
  waitForMasterInquiryReady
} from "../support/test-helpers";

test.describe("Master Inquiry landing page", () => {
  test.beforeEach(async ({ page }) => {
    await gotoMasterInquiry(page, baseUrl);
  });
  test("page should load properly without any issue", async ({ page }) => {
    await expect(page.getByRole("heading", { name: /MASTER INQUIRY/i })).toBeVisible();
  });

  test("click on search button without setting IMPERSONATE value", async ({ page }) => {
    await setRole(page, "Finance-Manager", false);
    // Try searching and expect UI error toast/dialog
    await page.getByRole("button", { name: "SEARCH" }).click();
    await expect(page.getByText(/Search Failed/i)).toBeVisible();
  });

  test("click on search button after setting IMPERSONATE value", async ({ page }) => {
    await expectSearchOk(page);
  });

  test("pagination works and returns results", async ({ page }) => {
    await expectSearchOk(page);
    await page.getByRole("button", { name: /next page/i }).click();
    const json: any = await searchAndGetJson(page);
    await expect(Array.isArray(json.results)).toBeTruthy();
    await expect(json.results.length).toBeGreaterThan(0);
  });
  test("enter Name and click search", async ({ page }) => {
    await page.locator('input[name="name"]').fill("evans");
    await expectSearchOk(page);
  });

  test("enter Badge/PSN Number and click search", async ({ page }) => {
    await page.locator('input[name="badgeNumber"]').fill("706056");
    await expectSearchOk(page);
  });

  test("select Payment Type and click search", async ({ page }) => {
    await page.getByRole("radio", { name: "Payoffs/Forfeit" }).check();
    await expectSearchOk(page);
  });

  test("select Member Type and click search", async ({ page }) => {
    await page.getByRole("radio", { name: "Beneficiaries" }).check();
    await expectSearchOk(page);
  });

  test("select Beginning Month and Ending Month and click on Search", async ({ page }) => {
    await page.locator("#mui-component-select-startProfitMonth").click();
    await page.getByRole("option", { name: "2", exact: true }).click();
    await page.locator("#mui-component-select-endProfitMonth").click();
    await page.getByRole("option", { name: "10" }).click();
    await expectSearchOk(page);
  });
  test("filling values in Name, Badge/PSN Number and then click on Reset button", async ({ page }) => {
    await waitForMasterInquiryReady(page);
    await page.locator('input[name="badgeNumber"]').fill("706056");
    await page.locator('input[name="name"]').fill("evans");
    await page.getByRole("button", { name: "RESET" }).click();
    // Assert reset worked
    await expect(page.locator('input[name="badgeNumber"]')).toHaveValue("");
    await expect(page.locator('input[name="name"]')).toHaveValue("");
  });

  test("search button is debounced/multiple clicks handled", async ({ page }) => {
    await page.locator('input[name="badgeNumber"]').fill("706056");
    // Fire a few quick clicks; ensure at least one successful response arrives.
    const responses = await Promise.all([expectSearchOk(page), expectSearchOk(page), expectSearchOk(page)]);
    for (const r of responses) {
      await expect(r.status()).toBe(200);
    }
  });

  test("If we change IMPERSONATE to IT-DEVOPS, it should MASK values", async ({ page }) => {
    await setRole(page, "Finance-Manager", false);
    await setRole(page, "IT-DevOps", true);
    await page.locator('input[name="badgeNumber"]').fill("706056");
    const json: any = await searchAndGetJson(page);
    await expect(json.results?.[0]?.firstName).toContain("X");
  });

  test("If we enter Name as Evans and click on Badge Number link in the Grid", async ({ page }) => {
    await page.locator('input[name="name"]').fill("evans");
    await expectSearchOk(page);
    await page.getByRole("link", { name: "708248" }).click();
    await expect(page).toHaveURL(/master-inquiry\/708248/);
  });
});
