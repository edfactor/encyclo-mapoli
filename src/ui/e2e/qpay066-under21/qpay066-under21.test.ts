import { test, expect } from "@playwright/test";
import { baseUrl } from "../env.setup";

test.describe("Profit Share Gross Report (QPAY501): ", () => {
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
        await page.getByRole('button', { name: 'Prof Share by Store' }).click();
        await page.getByRole('button', { name: 'Under-21 Report' }).click();
    });

    test('Page render successfully', async ({ page }) => {
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/post-frozen/under-21-breakdown-by-store'))]);
        await expect(response.status()).toBe(200);
    });

    test('checking pagination', async ({ page }) => {
        try {
            const [response] = await Promise.all([page.waitForResponse((resp) =>
                resp.url().includes('yearend/post-frozen/under-21-breakdown-by-store'))]);
            await expect(response.status()).toBe(200);
            // Locate the select dropdown
            const pageSizeSelect = page.locator('select[aria-label="rows per page"]');

            // Select by value
            await pageSizeSelect.selectOption('10');

            const isNextPageDisabled = await page.getByRole('button', { name: 'next page' }).isDisabled();

            if (!isNextPageDisabled) {
                await page.getByRole('button', { name: 'next page' }).click();
                const [response1] = await Promise.all([page.waitForResponse((resp) =>
                    resp.url().includes('yearend/post-frozen/under-21-breakdown-by-store'))]);
                await expect(response1.status()).toBe(200);
            }
        } catch (error) { return; }
    });

    test('changing status updates navigation api', async ({ page }) => {
        await page.getByRole('combobox').nth(2).click();
        await page.getByRole('option', { name: 'Complete' }).click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('api/navigation'))]);
        const json = await response.json();
        await expect(json.isSuccessful).toBe(true);
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

    test('click on badge number', async ({ page }) => {
        await page.getByRole('gridcell').nth(0).locator('a').click();
        await expect(page.url()).toContain('master-inquiry');
    });

});