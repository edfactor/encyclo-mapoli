import { test, expect } from "@playwright/test";
import { baseUrl, impersonateRole, navigateToPage } from "../env.setup";


test.describe("Control Sheet: ", () => {
    test.beforeEach(async ({ page }) => {
        await page.goto(baseUrl);
        await page.waitForLoadState("networkidle");
        await impersonateRole(page, 'Finance-Manager');
        await navigateToPage(page, 'Fiscal Close', 'Prof Control Sheet');
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
});