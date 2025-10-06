import { test, expect } from "@playwright/test";
import { baseUrl, impersonateRole } from "../env.setup";


test.describe("Manage Executive Hours: ", () => {
    test.beforeEach(async ({ page }) => {
        await page.goto(baseUrl);
        await page.waitForLoadState("networkidle");
    await impersonateRole(page, 'Finance-Manager');
        // Open navigation drawer robustly: try the empty-text button first, then fall back
        // to the first MUI icon button. Wait for 'Fiscal Close' to be visible before clicking.
        const emptyBtn = page.getByRole('button').filter({ hasText: /^$/ });
        if ((await emptyBtn.count()) > 0) {
            await emptyBtn.first().click();
        } else {
            const iconBtn = page.locator('button.MuiIconButton-root').first();
            if ((await iconBtn.count()) > 0) {
                await iconBtn.click();
            }
        }
        const fiscalCloseBtn = page.getByRole('button', { name: 'Fiscal Close' });
        await fiscalCloseBtn.click();
        await page.getByRole('button', { name: 'YTD Wages Extract' }).click();
    });

    test('page load successfully', async ({ page }) => {
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/wages-current-year'))]);
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


    test('checking pagination', async ({ page }) => {
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/wages-current-year'))]);
        await expect(response.status()).toBe(200);
        // Locate the select dropdown
        const pageSizeSelect = page.locator('select[aria-label="rows per page"]');

        // Select by value
        await pageSizeSelect.selectOption('10');

        const isNextPageDisabled = await page.getByRole('button', { name: 'next page' }).isDisabled();

        if (!isNextPageDisabled) {
            await page.getByRole('button', { name: 'next page' }).click();
            const [response1] = await Promise.all([page.waitForResponse((resp) =>
                resp.url().includes('yearend/wages-current-year'))]);
            await expect(response1.status()).toBe(200);
        }

    });

    test('changing status of YTD Wages Extract', async ({ page }) => {
        await page.getByRole('combobox').nth(1).click();
        await page.getByRole('option', { name: 'Complete' }).click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('api/navigation'))]);
        const json = await response.json();
        await expect(json.isSuccessful).toBe(true);
    });
});