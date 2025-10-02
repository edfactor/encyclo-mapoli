import { test, expect } from "@playwright/test";
import { baseUrl, impersonateRole } from "../env.setup";


test.describe("Pay Beneficiary Report: ", () => {
    test.beforeEach(async ({ page }) => {
        await page.goto(baseUrl);
        await page.waitForLoadState("networkidle");
        await impersonateRole(page, 'Finance-Manager');

        // Wait for a stable navigation region or the app header before interacting with nav buttons
        const navRegion = page.getByRole('navigation').first();
        if ((await navRegion.count()) > 0) {
            await navRegion.waitFor({ state: 'visible', timeout: 10000 }).catch(() => {});
        } else {
            // fallback: wait for any MUI app bar or a known container
            await page.locator('header, .MuiAppBar-root, .app-header').first().waitFor({ state: 'visible', timeout: 10000 }).catch(() => {});
        }

        // Ensure the drawer/icon button is clicked if present (some pages hide nav inside a drawer)
        const emptyBtn = page.getByRole('button').filter({ hasText: /^$/ });
        if ((await emptyBtn.count()) > 0) {
            await emptyBtn.first().click().catch(() => {});
        } else {
            const iconBtn = page.locator('button.MuiIconButton-root').first();
            if ((await iconBtn.count()) > 0) {
                await iconBtn.first().click().catch(() => {});
            }
        }

        const fiscalBtn = page.getByRole('button', { name: 'Fiscal Close' });
        await fiscalBtn.waitFor({ state: 'visible', timeout: 20000 });

        // retry clicking Fiscal Close a few times to avoid flakiness
        let clicked = false;
        for (let i = 0; i < 4 && !clicked; i++) {
            try {
                await fiscalBtn.click({ timeout: 5000 });
                clicked = true;
            } catch (e) {
                await page.waitForTimeout(500);
            }
        }
        if (!clicked) {
            // final fallback to click via text selector
            await page.click('button:has-text("Fiscal Close")', { timeout: 10000 }).catch(() => {});
        }

        await page.getByRole('button', { name: 'Pay Beneficiary Report' }).click();
    });

    test('page load successfully', async ({ page }) => {
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/payben-report'))]);
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
            resp.url().includes('yearend/payben-report'))]);
        await expect(response.status()).toBe(200);
        // Locate the select dropdown
        const pageSizeSelect = page.locator('select[aria-label="rows per page"]');

        // Select by value
        await pageSizeSelect.selectOption('10');

        const isNextPageDisabled = await page.getByRole('button', { name: 'next page' }).isDisabled();

        if (!isNextPageDisabled) {
            await page.getByRole('button', { name: 'next page' }).click();
            const [response1] = await Promise.all([page.waitForResponse((resp) =>
                resp.url().includes('yearend/payben-report'))]);
            await expect(response1.status()).toBe(200);
        }

    });

    test('clicking PSN link', async ({ page }) => {
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/payben-report'))]);
        await expect(response.status()).toBe(200);
        await page.getByRole('gridcell').nth(0).locator('a').click();
        await expect(page.url()).toContain("master-inquiry");

    });


    test('check sorting', async ({ page }) => {
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/payben-report'))]);
        await expect(response.status()).toBe(200);
        await page.locator('div:nth-child(3) > .ag-header-cell-comp-wrapper > .ag-cell-label-container > .ag-header-cell-label').click();
        const [response1] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('yearend/payben-report'))]);
        await expect(response1.status()).toBe(200);

    });
});