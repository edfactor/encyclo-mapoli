import { test, expect } from "@playwright/test";
import { baseUrl, impersonateRole } from "../env.setup";


test.describe.serial("Pay 443: ", () => {
    test.beforeEach(async ({ page }) => {
        await page.goto(baseUrl);
        await page.waitForLoadState("networkidle");
        await impersonateRole(page, 'Finance-Manager');
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
        // wait for the Fiscal Close button to be visible and clickable
        const decBtn = page.getByRole('button', { name: 'Fiscal Close' });

        // If there is a global backdrop/overlay, wait for it to hide first (reduces flakiness)
        const backdrop = page.locator('.MuiBackdrop-root');
        if ((await backdrop.count()) > 0) {
            // wait up to 5s for any blocking backdrop to disappear
            await backdrop.first().waitFor({ state: 'hidden', timeout: 5000 }).catch(() => { /* ignore timeout */ });
        }

        // Wait for the button to appear and be enabled, then scroll into view before clicking
        await decBtn.waitFor({ state: 'visible', timeout: 10000 });
        await expect(decBtn).toBeVisible();
        await decBtn.scrollIntoViewIfNeeded();
        await expect(decBtn).toBeEnabled();

        // Try clicking with a small retry to handle transient flakiness when running tests in bulk
        let clicked = false;
        for (let attempt = 0; attempt < 3 && !clicked; attempt++) {
            try {
                await decBtn.click({ timeout: 5000 });
                clicked = true;
            } catch (err) {
                // brief backoff then retry
                await page.waitForTimeout(250);
            }
        }

        if (!clicked) {
            // final attempt using the page click as a fallback
            await page.click('button:has-text("Fiscal Close")', { timeout: 5000 });
        }

        await page.getByRole('button', { name: 'Profit Share Forfeit' }).click();
    });

    test('page load successfully', async ({ page }) => {
        await expect(page.getByRole('heading', { name: 'Forfeit (PAY443)' })).toBeVisible();
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

    test('click on search button', async ({ page }) => {
        await page.getByTestId('searchButton').click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/frozen/forfeitures-and-points'))]);
        await expect(response.status()).toBe(200);
    })


    test('checking pagination', async ({ page }) => {
        await page.getByTestId('searchButton').click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/frozen/forfeitures-and-points'))]);
        await expect(response.status()).toBe(200);
        // Locate the select dropdown
        const pageSizeSelect = page.locator('select[aria-label="rows per page"]');

        // Select by value
        await pageSizeSelect.selectOption('10');

        const isNextPageDisabled = await page.getByRole('button', { name: 'next page' }).isDisabled();

        if (!isNextPageDisabled) {
            await page.getByRole('button', { name: 'next page' }).click();
            const [response1] = await Promise.all([page.waitForResponse((resp) =>
                resp.url().includes('yearend/frozen/forfeitures-and-points'))]);
            await expect(response1.status()).toBe(200);
        }

    });

    test('changing status of YTD Wages Extract', async ({ page }) => {
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('api/navigation')),
            (async () => {
                await page.getByRole('combobox').nth(1).click();
                await page.getByRole('option', { name: 'Complete' }).click();
            })()
        ]);
        const json = await response.json();
        return;
    });
});