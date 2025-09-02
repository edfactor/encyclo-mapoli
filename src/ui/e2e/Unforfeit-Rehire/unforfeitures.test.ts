import { test, expect } from "@playwright/test";
import { baseUrl } from "../env.setup";


test.describe("Unforfeitures landing page: ", () => {
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
        await page.getByRole('button', { name: 'Unforfeit' }).click();
    });


    test('Unforfeit page is successfully loading', async ({page})=>{
        await expect(page.getByRole('heading', { name: 'UnForfeit (QPREV-PROF)' })).toBeVisible();
        await expect(page.url()).toContain("unforfeitures");
    });

    test('click on search without changing anything', async ({page})=>{
        await page.getByTestId('searchButton').click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/unforfeitures'))]);
        await expect(response.status()).toBe(200);
    });

    test('pagination is working properly', async ({page})=>{
        await page.getByTestId('searchButton').click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/unforfeitures'))]);
        await expect(response.status()).toBe(200);
        await page.getByRole('button', { name: 'next page' }).click();
        const [response1] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/unforfeitures'))]);
        await expect(response1.status()).toBe(200);

        await page.getByRole('button', { name: 'last page' }).click();
        const [response2] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/unforfeitures'))]);
        await expect(response2.status()).toBe(200);
    });

    test('check Exclude employee with no current balance and click on search', async ({page})=>{
        await page.getByRole('checkbox', { name: 'Exclude employees with no current balance and no vested balance' }).check();
        await page.getByTestId('searchButton').click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/unforfeitures'))]);
        await expect(response.status()).toBe(200);
    });

    test('changing status of unforfeit', async ({page})=>{
        await page.getByRole('combobox').nth(2).click();
        await page.getByRole('option', { name: 'Complete' }).click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('api/navigation'))]);
        const json  = await response.json();
        await expect(json.isSuccessful).toBe(true);
    });

    test('changing suggested unforfeit amount', async ({page})=>{
        await page.getByTestId('searchButton').click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/unforfeitures'))]);
        await expect(response.status()).toBe(200);
        const suggestedUnforfeit = await page.locator('[col-id="suggestedUnforfeit"]');
        const count = await suggestedUnforfeit.count();
        for(var i = 0; i < count; i++){
            const innerText = await suggestedUnforfeit.nth(i).innerText();
            const numericValue = Number(innerText.replace(/[^0-9.]/g, ""));
            if(innerText.length>0 && !isNaN(numericValue) && numericValue>0){
                await suggestedUnforfeit.nth(i).dblclick();
                const numberInput = await suggestedUnforfeit.nth(i).locator('input[type="number"]');
                await numberInput.fill(`${numericValue+1}`);
                await expect(page.getByTestId('ErrorOutlineIcon')).toBeVisible();
                break;
            }
        }
        
    });




});