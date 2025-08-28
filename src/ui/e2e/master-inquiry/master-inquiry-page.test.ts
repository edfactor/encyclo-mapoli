import { test, expect } from "@playwright/test";
import { baseUrl } from "../env.setup";


test.describe("Master Inqiry landing page: ", () => {
    test.beforeEach(async ({ page }) => {
        await page.goto(baseUrl);
        await page.waitForLoadState("networkidle");
        await page.getByRole("combobox", { name: "roles" }).click();
        await page.getByRole('option', { name: 'Finance-Manager' }).getByRole('checkbox').check();
        await page.locator("body").click();
        await page.reload();
        await page.waitForLoadState("networkidle");
        await page.getByRole("button", { name: "INQUIRIES" }).click();
        await page.getByRole("link", { name: "MASTER INQUIRY" }).click();
    });
    test("page should load properly without any issue.", async ({ page }) => {
        await expect(page.getByRole("heading", { name: "MASTER INQUIRY (008-10)" })).toBeVisible();
    });

    test("click on search button without setting IMPERSONATE value", async ({ page }) => {
        await page.getByRole("combobox", { name: "roles" }).click();
        await page.getByRole('option', { name: 'Finance-Manager' }).getByRole('checkbox').uncheck();
        await page.locator("body").click();
        await page.getByRole("button", { name: "SEARCH" }).click();
        await page.waitForLoadState("networkidle");
        await expect(page.getByText("Search Failed")).toBeVisible();
    });

    test("click on search button after setting IMPERSONATE value", async ({ page }) => {
        await page.getByRole("button", { name: "SEARCH" }).click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('master-inquiry/search'))]);
        await expect(response.status()).toBe(200);
    });

    test("pagingation is working properly or not", async ({ page }) => {
        
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
        await page.locator('input[name="name"]').fill("evans");
        await page.getByRole("button", { name: "SEARCH" }).click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('master-inquiry/search'))]);
        await expect(response.status()).toBe(200);
    });

    test("enter Badge/Psn Number and click search", async ({ page }) => {
        await page.locator('input[name="badgeNumber"]').fill("706056")
        await page.getByRole("button", { name: "SEARCH" }).click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('master-inquiry/search'))]);
        await expect(response.status()).toBe(200);
    });

    test("select Payment Type  and click search", async ({ page }) => {
        await page.getByRole('radio', { name: 'Payoffs/Forfeit' }).check();
        await page.getByRole("button", { name: "SEARCH" }).click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('master-inquiry/search'))]);
        await expect(response.status()).toBe(200);
    });

    test("select Member Type  and click search", async ({ page }) => {
        await page.getByRole('radio', { name: 'Beneficiaries' }).check();
        await page.getByRole("button", { name: "SEARCH" }).click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('master-inquiry/search'))]);
        await expect(response.status()).toBe(200);
    });

    test("select Beginning Month and Ending Month and click on Search", async ({ page }) => {
        await page.locator('#mui-component-select-startProfitMonth').click();
        await page.getByRole('option', { name: '2', exact: true }).click();
        await page.locator('#mui-component-select-endProfitMonth').click()
        await page.getByRole('option', { name: '10' }).click();
        await page.getByRole("button", { name: "SEARCH" }).click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('master-inquiry/search'))]);
        await expect(response.status()).toBe(200);
    });
    test("filling values in Name, Badge/PSN Number and then click on Reset button.", async ({ page }) => {
        await page.locator('input[name="badgeNumber"]').fill("706056");
        await page.locator('input[name="name"]').fill("evans");
        await page.getByRole("button", { name: "RESET" }).click();
        // Assert reset worked
        await expect(page.locator('input[name="badgeNumber"]')).toHaveValue("");
        await expect(page.locator('input[name="name"]')).toHaveValue("");
    });

    test("if we click on search button multiple times", async ({ page }) => {
        await page.locator('input[name="badgeNumber"]').fill("706056");
        await page.getByRole("button", { name: "SEARCH" }).click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('master-inquiry/search'))]);
        await expect(response.status()).toBe(200);
        await page.getByRole("button", { name: "SEARCH" }).click();
        const [response1] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('master-inquiry/search'))]);
            await page.getByRole("button", { name: "SEARCH" }).click();
        await expect(response1.status()).toBe(200);
        await page.getByRole("button", { name: "SEARCH" }).click();
        const [response2] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('master-inquiry/search'))]);
        await expect(response2.status()).toBe(200);
    });

    test("If we change IMPERSONATE to IT-OPERATIONS, it should MASK values", async ({ page }) => {
        await page.getByRole("combobox", { name: "roles" }).click();
        await page.getByRole('option', { name: 'Finance-Manager' }).getByRole('checkbox').uncheck();
        await page.locator("body").click();
        await page.getByRole("combobox", { name: "roles" }).click();
        await page.getByRole('option', { name: 'IT-Operations' }).getByRole('checkbox').check();
        await page.locator("body").click();
        await page.locator('input[name="badgeNumber"]').fill("706056");
        await page.getByRole("button", { name: "SEARCH" }).click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('master-inquiry/search'))]);
        const json = await response.json();
        await expect(json.results[0].firstName).toContain('X');
    });

    test("If we enter Name as Evans and click on Badge Number link in the Grid", async ({ page }) => {
        
        await page.locator('input[name="name"]').fill("evans");
        await page.getByRole("button", { name: "SEARCH" }).click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('master-inquiry/search'))]);
        await expect(response.status()).toBe(200);
        await page.getByRole('link', { name: '708248' }).click();
        await expect(page.url()).toContain('master-inquiry/708248');
    });
});