import { test, expect } from "@playwright/test";
import { baseUrl } from "../env.setup";


test.describe("Master Inqiry landing page: ", () => {
    test.beforeEach(async ({ page }) => {
        await page.goto(baseUrl);
        await page.waitForLoadState("networkidle");
        await page.getByRole("button", { name: "INQUIRIES" }).click();
        await page.getByRole("link", { name: "MASTER INQUIRY" }).click();
    });
    test("page should load properly without any issue.", async ({ page }) => {
        await expect(page.getByRole("heading", { name: "MASTER INQUIRY (008-10)" })).toBeVisible();
    });

    test("click on search button without setting IMPERSONATE value", async ({ page }) => {
        await page.getByRole("button", { name: "SEARCH" }).click();
        await page.waitForLoadState("networkidle");
        await expect(page.getByText("Search Failed")).toBeVisible();
    });

    test("click on search button after setting IMPERSONATE value", async ({ page }) => {
        await page.getByRole("combobox", { name: "roles" }).click();
        await page.getByRole('option', { name: 'Finance-Manager' }).getByRole('checkbox').check();
        await page.locator("body").click();
        await page.getByRole("button", { name: "SEARCH" }).dblclick();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('master-inquiry/search'))]);
        await expect(response.status()).toBe(200);
    });

    test("pagingation is working properly or not", async ({ page }) => {
        await page.getByRole("combobox", { name: "roles" }).click();
        await page.getByRole('option', { name: 'Finance-Manager' }).getByRole('checkbox').check();
        await page.locator("body").click();
        await page.getByRole("button", { name: "SEARCH" }).dblclick();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('master-inquiry/search'))]);
        await expect(response.status()).toBe(200);
        await page.getByRole('button', { name: 'next page' }).click();
        const [response1] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('master-inquiry/search'))]);
        const json = await response1.json();
        await expect(json.results.length).toBeGreaterThan(0);
        
    });
    test("enter Name and click search", async ({ page }) => {
        await page.getByRole("combobox", { name: "roles" }).click();
        await page.getByRole('option', { name: 'Finance-Manager' }).getByRole('checkbox').check();
        await page.locator("body").click();
        await page.locator('input[name="name"]').fill("evans");
        await page.getByRole("button", { name: "SEARCH" }).dblclick();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('master-inquiry/search'))]);
        await expect(response.status()).toBe(200);
    });

    test("enter Badge/Psn Number and click search", async ({ page }) => {
        await page.getByRole("combobox", { name: "roles" }).click();
        await page.getByRole('option', { name: 'Finance-Manager' }).getByRole('checkbox').check();
        await page.locator("body").click();
        await page.locator('input[name="badgeNumber"]').fill("706056")
        await page.getByRole("button", { name: "SEARCH" }).dblclick();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('master-inquiry/search'))]);
        await expect(response.status()).toBe(200);
    });

    test("select Payment Type  and click search", async ({ page }) => {
        await page.getByRole("combobox", { name: "roles" }).click();
        await page.getByRole('option', { name: 'Finance-Manager' }).getByRole('checkbox').check();
        await page.locator("body").click();
        await page.getByRole('radio', { name: 'Payoffs/Forfeit' }).check();
        await page.getByRole("button", { name: "SEARCH" }).dblclick();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('master-inquiry/search'))]);
        await expect(response.status()).toBe(200);
    });
});