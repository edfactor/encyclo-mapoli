import { test, expect } from "@playwright/test";
import { baseUrl } from "../env.setup";


test.describe("Master Inqiry landing page: ", () => {
    test.beforeEach(async ({ page }) => {
        await page.goto(baseUrl);
        await page.waitForLoadState("networkidle");
        await page.getByRole("button", { name: "INQUIRIES" }).click();
        await page.getByRole("link", { name: "MASTER INQUIRY" }).click();
        await page.getByRole("combobox", { name: "roles" }).click();
        await page.getByRole('option', { name: 'Finance-Manager' }).getByRole('checkbox').check();
        await page.locator("body").click();
    });

    test('Demographic Badges Not In PayProfit',async ({page})=>{
        await page.getByRole('button').filter({ hasText: /^$/ }).click();
        await page.getByRole('button', { name: 'December Activities' }).click();
        await page.getByRole('button', { name: 'Clean up Reports' }).click();
        await page.getByRole('button', { name: 'Demographic Badges Not In PayProfit' }).click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/demographic-badges-not-in-payprofit'))]);
        await expect(response.status()).toBe(200);
    });

});