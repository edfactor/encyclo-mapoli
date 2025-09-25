import { test, expect } from "@playwright/test";
import { baseUrl } from "../env.setup";


test.describe("Control Sheet: ", () => {
    test.beforeEach(async ({ page }) => {
        await page.goto(baseUrl);
        await page.waitForLoadState("networkidle");
        await page.getByRole("combobox", { name: "roles" }).click();
        await page.getByRole('option', { name: 'Finance-Manager' }).getByRole('checkbox').check();
        await page.locator("body").click();
        await page.reload();
        await page.waitForLoadState("networkidle");
        await page.getByRole('button').filter({ hasText: /^$/ }).click();
        await page.getByRole('button', { name: 'Fiscal Close' }).click();
        await page.getByRole('button', { name: 'Prof Control Sheet' }).click();
    });

    test('page load successfully', async ({ page }) => {
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/post-frozen/control-sheet'))]);
        await expect(response.status()).toBe(200);
    });

    test('check access on other roles.', async ({ page }) => {
        await page.getByRole("combobox", { name: "roles" }).click();
        await page.getByRole('option', { name: 'Finance-Manager' }).getByRole('checkbox').uncheck();
        await page.getByRole('option', { name: 'Distributions-Clerk' }).getByRole('checkbox').check();
        await page.locator("body").click();
        await page.reload();
        await page.waitForLoadState("networkidle");
        await expect(page.getByText('Access Denied').nth(1)).toBeVisible();
    });

    test('change profit year and see if control sheet API works fine.', async ({page})=>{
        await page.getByRole('combobox').nth(1).click();
        await page.getByRole('option').nth(1).click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/post-frozen/control-sheet'))]);
        await expect(response.status()).toBe(200);
        await page.getByRole('combobox').nth(1).click();
        await page.getByRole('option').nth(2).click();
        const [response1] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/post-frozen/control-sheet'))]);
        await expect(response1.status()).toBe(200);
    });
});