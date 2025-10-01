import { test, expect } from "@playwright/test";
import { baseUrl, impersonateRole } from "../env.setup";


test.describe("Terminations: ", () => {
    test.beforeEach(async ({ page }) => {
        await page.goto(baseUrl);
        await page.waitForLoadState("networkidle");
    await impersonateRole(page, 'Finance-Manager');
        // Open the navigation drawer robustly. Try the standard empty-icon button first,
        // then the first MUI icon button as a fallback. Wait for the December Activities
        // button to appear before clicking it, to avoid intermittent timing issues.
        const emptyBtn = page.getByRole('button').filter({ hasText: /^$/ });
        if ((await emptyBtn.count()) > 0) {
            await emptyBtn.first().click();
        } else {
            // fallback: click the first icon button on the page
            const iconBtn = page.locator('button.MuiIconButton-root').first();
            if ((await iconBtn.count()) > 0) {
                await iconBtn.click();
            }
        }
        // wait for the December Activities button to be visible and clickable
        const decBtn = page.getByRole('button', { name: 'December Activities' });
        await decBtn.click();
        await page.getByRole('button', { name: 'Terminations' }).click();
    });

    test('Page render successfully',async ({page})=>{
        await expect(page.getByRole('heading', { name: 'Terminations (QPAY066)' })).toBeVisible();
    });

    test('click on search',async ({page})=>{
        await page.getByTestId('searchButton').click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/terminated-employees'))]);
        await expect(response.status()).toBe(200);
    });

    test('changing dates and click on search', async({page})=>{
        await page.locator('#beginningDate').fill('12/31/2018');
        await page.locator('#endingDate').fill('12/01/2024');
        await page.getByTestId('searchButton').click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/terminated-employees'))]);
        await expect(response.status()).toBe(200);
    });

    
    test('Pagination',async ({page})=>{
        await page.getByTestId('searchButton').click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/terminated-employees'))]);
        await expect(response.status()).toBe(200);
        await page.getByRole('button', { name: 'next page' }).click();
        const [response1] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/terminated-employees'))]);
        await expect(response1.status()).toBe(200);
        await page.getByRole('button', { name: 'last page' }).click();
        const [response2] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/terminated-employees'))]);
        await expect(response2.status()).toBe(200);
    });

    test('changing status of terminations', async ({page})=>{
        await page.getByRole('combobox').nth(2).click();
        await page.getByRole('option', { name: 'Complete' }).click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('api/navigation'))]);
        const json  = await response.json();
        await expect(json.isSuccessful).toBe(true);
    });

    test('Changing forfeit amount',async ({page})=>{
        await page.getByTestId('searchButton').click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/terminated-employees'))]);
        await expect(response.status()).toBe(200);
        const suggestedUnforfeit = await page.locator('[col-id="suggestedForfeit"]');
        const count = await suggestedUnforfeit.count();
        for(let i = 0; i < count; i++){
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


    test('click on save button on the Grid',async ({page})=>{
        await page.getByTestId('searchButton').click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/terminated-employees'))]);
        await expect(response.status()).toBe(200);
        const saveButtons = await page.locator('[col-id="saveButton"] button');
        const count = await saveButtons.count();
        for(let i = 0; i < count; i++){
            const isButtonEnabled = await saveButtons.nth(i).isEnabled();
            if(isButtonEnabled){
                await saveButtons.nth(i).click();
                const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/terminated-employees'))]);
                await expect(response.status()).toBe(200);
                break;
            }
        }
    });

});