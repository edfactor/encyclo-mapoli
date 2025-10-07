import { test, expect } from "@playwright/test";
import { baseUrl, impersonateRole } from "../env.setup";


test.describe("Control Sheet: ", () => {
    test.beforeEach(async ({ page }) => {
        await page.goto(baseUrl);
        await page.waitForLoadState("networkidle");
    await impersonateRole(page, 'Finance-Manager');
    // Open the side drawer / navigation by clicking the icon button (no accessible name)
    // This targets the MUI icon button used for the drawer toggle.
    await page.locator('button.MuiIconButton-root').first().click();
    // Ensure the Fiscal Close button is present before interacting
    await page.getByRole('button', { name: 'Fiscal Close' }).waitFor({ state: 'visible', timeout: 10000 });
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
});