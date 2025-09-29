import { test, expect } from "@playwright/test";
import { baseUrl } from "../env.setup";


test.describe("Forfeiture Adjustment (008-12): ", () => {
    test.beforeEach(async ({ page }) => {
        await page.goto(baseUrl);
        await page.waitForLoadState("networkidle");
        await page.getByRole("combobox", { name: "roles" }).click();
        await page.getByRole('option', { name: 'Finance-Manager' }).getByRole('checkbox').check();
        await page.locator("body").click();
        await page.reload();
        await page.waitForLoadState("networkidle");
        await page.getByRole('button').filter({ hasText: /^$/ }).click();
        await page.getByRole('button', { name: 'December Activities' }).click();
        await page.getByRole('button', { name: 'Forfeitures 008-12' }).click();
    });

    test('Page load successfully', async ({ page }) => {
        await expect(page.getByRole('heading', { name: 'Forfeitures Adjustment(008-12)' })).toBeVisible();
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

    test('changing status of forfeiture adjustment', async ({page})=>{
        await page.getByRole('combobox').nth(2).click();
        await page.getByRole('option', { name: 'Complete' }).click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('api/navigation'))]);
        const json  = await response.json();
        await expect(json.isSuccessful).toBe(true);
    });

    test('click on search button after adding badge number. ', async ({ page }) => {
        await page.getByRole('textbox', { name: 'Badge' }).fill('706222');
        await page.getByTestId('searchButton').click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/forfeiture-adjustments'))]);
        await expect(response.status()).toBe(200);
        

    });


    

    test('Add Forfeiture', async ({ page }) => {
        await page.getByRole('textbox', { name: 'Badge' }).fill('706222');
        await page.getByTestId('searchButton').click();
        const [response0] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/forfeiture-adjustments'))]);
        await expect(response0.status()).toBe(200);
        await page.getByRole('button', { name: 'ADD FORFEITURE' }).click();
        await page.getByRole('checkbox', { name: 'Class Action' }).check();
        await page.getByRole('spinbutton').fill('10');
        await page.getByRole('button', { name: 'SAVE' }).click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/forfeiture-adjustments/update'))]);
        await expect(response.status()).toBe(204);
        const [response1] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/forfeiture-adjustments'))]);
        await expect(response1.status()).toBe(200);
    });

    

});