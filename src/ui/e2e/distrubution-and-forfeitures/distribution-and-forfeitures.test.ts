import { expect, test } from "@playwright/test";
import { baseUrl, impersonateRole } from "../env.setup";

test.describe("Distribution and Forfeitures: ", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");
    await impersonateRole(page, "Finance-Manager");

    // Wait for nav to be visible with reasonable timeout
    const navRegion = page.getByRole("navigation").first();
    if ((await navRegion.count()) > 0) {
      await navRegion.waitFor({ state: "visible", timeout: 5000 }).catch(() => {});
    }

    // Attempt to open drawer if necessary - simplified
    const drawerButton = page.locator('button[aria-label*="menu"], button.MuiIconButton-root').first();
    if ((await drawerButton.count()) > 0 && (await drawerButton.isVisible())) {
      await drawerButton.click().catch(() => {});
    }

    // Navigate to December Activities - direct click, no retry loop
    const decBtn = page.getByRole("button", { name: "December Activities" });
    await decBtn.waitFor({ state: "visible", timeout: 5000 });
    await decBtn.click();

    // Navigate to Distributions and Forfeitures
    await page.getByRole("button", { name: "Distributions and Forfeitures" }).click();
    await page.waitForLoadState("networkidle");
  });

  test("Page load successfully", async ({ page }) => {
    await expect(page.getByRole("heading", { name: "Distributions And Forfeitures (QPAY129)" })).toBeVisible();
    await page.getByTestId("searchButton").click();
    const [response] = await Promise.all([
      page.waitForResponse((resp) => resp.url().includes("yearend/distributions-and-forfeitures"))
    ]);
    await expect(response.status()).toBe(200);
  });

  test("checking pagination", async ({ page }) => {
    await page.getByTestId("searchButton").click();
    const [response] = await Promise.all([
      page.waitForResponse((resp) => resp.url().includes("yearend/distributions-and-forfeitures"))
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
        page.waitForResponse((resp) => resp.url().includes("yearend/distributions-and-forfeitures"))
      ]);
      await expect(response1.status()).toBe(200);
    }
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

  test("Click on Search button", async ({ page }) => {
    await page.getByTestId("searchButton").click();
    const [response] = await Promise.all([
      page.waitForResponse((resp) => resp.url().includes("yearend/distributions-and-forfeitures"))
    ]);
    await expect(response.status()).toBe(200);
  });

  test("changing status of distribution and forfeitures", async ({ page }) => {
    await page.getByRole("combobox").nth(1).click();
    await page.getByRole("option", { name: "Complete" }).click();
    const [response] = await Promise.all([page.waitForResponse((resp) => resp.url().includes("api/navigation"))]);
    const json = await response.json();
    await expect(json.isSuccessful).toBe(true);
  });
});
