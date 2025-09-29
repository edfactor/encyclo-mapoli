import { expect, test } from "@playwright/test";
import { baseUrl } from "../env.setup";

test.describe("Demographic Freeze: ", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto(baseUrl);
    await page.waitForLoadState("networkidle");
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "IT-DevOps" }).getByRole("checkbox").check();
    await page.locator("body").click();
    await page.reload();
    await page.waitForLoadState("networkidle");
    await page.getByRole("button", { name: "IT DEVOPS" }).click();
    await page.getByRole("link", { name: "Demographic Freeze" }).click();
  });

  test("Demographic freeze page load successfully", async ({ page }) => {
    const [response] = await Promise.all([page.waitForResponse((resp) => resp.url().includes("/itdevops/frozen"))]);
    await expect(response.status()).toBe(200);
  });

  test("check with impersonate role other than IT-DevOps", async ({ page }) => {
    await page.getByRole("combobox", { name: "roles" }).click();
    await page.getByRole("option", { name: "IT-DevOps" }).getByRole("checkbox").uncheck();
    await page.getByRole("option", { name: "Finance-Manager" }).getByRole("checkbox").check();
    await page.locator("body").click();
    await page.reload();
    await page.waitForLoadState("networkidle");
    await expect(page.getByText("Access Denied").nth(1)).toBeVisible();
  });

  test("filling profit year less then current year-2", async ({ page }) => {
    const currentYear = new Date().getFullYear() - 2;
    await page.getByRole("textbox", { name: "YYYY", exact: true }).fill(currentYear.toString());
    await expect(page.getByText("Year must be current or previous year")).toBeVisible();
  });

  test("filling As of Date greater than current date", async ({ page }) => {
    // Get current date/time + 20 mins
    const now = new Date();
    now.setMinutes(now.getMinutes() + 20);

    const currentYear = now.getFullYear().toString();
    const currentMonth = (now.getMonth() + 1).toString().padStart(2, "0");
    const currentDay = (now.getDate() + 1).toString().padStart(2, "0");

    // format time in 24-hour format for input[type="time"]
    const hours = now.getHours().toString().padStart(2, "0");
    const minutes = now.getMinutes().toString().padStart(2, "0");
    const formattedTime = `${hours}:${minutes}`; // e.g. "17:45"

    await page.getByRole("textbox", { name: "MM/DD/YYYY" }).fill(`${currentMonth}/${currentDay}/${currentYear}`);

    await page.locator("#asOfTime").fill(formattedTime);

    await expect(page.getByRole("button", { name: "CREATE FREEZE POINT" })).toBeDisabled();
  });

  test("add values and click on CREATE FREEZE POINT", async ({ page }) => {
    // Get current date/time + 20 mins
    const now = new Date();
    now.setMinutes(now.getMinutes() + 20);

    const currentYear = now.getFullYear().toString();
    const currentMonth = (now.getMonth() + 1).toString().padStart(2, "0");
    const currentDay = now.getDate().toString().padStart(2, "0");

    // format time in 24-hour format for input[type="time"]
    const hours = now.getHours().toString().padStart(2, "0");
    const minutes = now.getMinutes().toString().padStart(2, "0");
    const formattedTime = `${hours}:${minutes}`; // e.g. "17:45"

    await page.getByRole("textbox", { name: "MM/DD/YYYY" }).fill(`${currentMonth}/${currentDay}/${currentYear}`);

    await page.locator("#asOfTime").fill(formattedTime);

    await page.getByRole("button", { name: "CREATE FREEZE POINT" }).click();
    const [response] = await Promise.all([page.waitForResponse((resp) => resp.url().includes("itdevops/freeze"))]);
    await expect(response.status()).toBe(200);
  });

  test("after creating freeze the fields are cleared and create is disabled", async ({ page }) => {
    // Get current date/time + 20 mins
    const now = new Date();
    now.setMinutes(now.getMinutes() + 20);

    const currentYear = now.getFullYear().toString();
    const currentMonth = (now.getMonth() + 1).toString().padStart(2, "0");
    const currentDay = now.getDate().toString().padStart(2, "0");

    const hours = now.getHours().toString().padStart(2, "0");
    const minutes = now.getMinutes().toString().padStart(2, "0");
    const formattedTime = `${hours}:${minutes}`;

    // Fill the form
    await page.getByRole("textbox", { name: "MM/DD/YYYY" }).fill(`${currentMonth}/${currentDay}/${currentYear}`);
    await page.locator("#asOfTime").fill(formattedTime);

    // Trigger the freeze
    await page.getByRole("button", { name: "CREATE FREEZE POINT" }).click();
    const [response] = await Promise.all([page.waitForResponse((resp) => resp.url().includes("itdevops/freeze"))]);
    await expect(response.status()).toBe(200);

    // After success the form inputs should be cleared and the create button should be disabled
    const dateInput = page.getByRole("textbox", { name: "MM/DD/YYYY" });
    const timeInput = page.locator("#asOfTime");
    const createBtn = page.getByRole("button", { name: "CREATE FREEZE POINT" });

    await expect(dateInput).toHaveValue("");
    await expect(timeInput).toHaveValue("");
    await expect(createBtn).toBeDisabled();
  });

  test("checking pagination", async ({ page }) => {
    // Locate the select dropdown
    const pageSizeSelect = page.locator('select[aria-label="rows per page"]');

    // Select by value
    await pageSizeSelect.selectOption("10");
    const isDisabled = await page.getByRole("button", { name: "next page" }).isDisabled();
    if (!isDisabled) {
      await page.getByRole("button", { name: "next page" }).click();
      const [response] = await Promise.all([page.waitForResponse((resp) => resp.url().includes("/itdevops/frozen"))]);
      await expect(response.status()).toBe(200);
    }
  });
});
