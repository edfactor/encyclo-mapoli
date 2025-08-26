import { test, expect } from "@playwright/test";
import { baseUrl } from "../env.setup";

test.describe("landing page: ", () =>
  test("test welcome banner", async ({ page }) => {
    // example of logging in playwright.
    // page.on("console", (msg) => console.log(msg.text()));
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");
    await expect(page.locator("div.username")).toBeVisible();
  }));
