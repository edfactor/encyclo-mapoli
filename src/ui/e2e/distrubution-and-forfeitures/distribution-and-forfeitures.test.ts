import { expect, test } from "@playwright/test";
import { baseUrl, impersonateRole } from "../env.setup";

test.describe("Distribution and Forfeitures: ", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");
    await impersonateRole(page, "Finance-Manager");

    // Wait for nav to be visible
    const navRegion = page.getByRole("navigation").first();
    if ((await navRegion.count()) > 0) {
      await navRegion.waitFor({ state: "visible", timeout: 5000 }).catch(() => {});
    } else {
      await page
        .locator("header, .MuiAppBar-root, .app-header")
        .first()
        .waitFor({ state: "visible", timeout: 1000 })
        .catch(() => {});
    }

    // attempt to open drawer if necessary
    const emptyBtn = page.getByRole("button").filter({ hasText: /^$/ });
    if ((await emptyBtn.count()) > 0) {
      await emptyBtn
        .first()
        .click()
        .catch(() => {});
    } else {
      const iconBtn = page.locator("button.MuiIconButton-root").first();
      if ((await iconBtn.count()) > 0) {
        await iconBtn
          .first()
          .click()
          .catch(() => {});
      }
    }

    const decBtn = page.getByRole("button", { name: "December Activities" });
    await decBtn.waitFor({ state: "visible", timeout: 10000 });

    // retry click with small backoff
    let clicked = false;
    for (let i = 0; i < 4 && !clicked; i++) {
      try {
        await decBtn.click({ timeout: 1000 });
        clicked = true;
      } catch (e) {
        await page.waitForTimeout(500);
      }
    }
    if (!clicked) {
      await page.click('button:has-text("December Activities")', { timeout: 5000 }).catch(() => {});
    }

    await page.getByRole("button", { name: "Distributions and Forfeitures" }).click();
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
