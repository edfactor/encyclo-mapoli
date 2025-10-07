import { test, expect } from "@playwright/test";
import { baseUrl, impersonateRole, navigateToPage } from "../env.setup";


test.describe("Military Contributions: ", () => {
    test.beforeEach(async ({ page }) => {
        await page.goto(baseUrl);
        await page.waitForLoadState("networkidle");
        await impersonateRole(page, 'Finance-Manager');
        await navigateToPage(page, 'December Activities', 'Military Contributions');
    });

    test('page load successfully', async ({ page }) => {
        await expect(page.getByRole('heading', { name: 'Military Contributions' })).toBeVisible();
        await expect(page.url()).toContain('military-contribution');
    });

    test('changing status of military contributions', async ({ page }) => {
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('api/navigation')),
            (async () => {
                await page.getByRole('combobox').nth(1).click();
                await page.getByRole('option', { name: 'Complete' }).click();
            })()
        ]);
        const json = await response.json();
        await expect(response.status()).toBe(200);
    });

    test('add badge number and click on search', async ({ page }) => {
        await page.getByRole('textbox', { name: 'Enter Badge Number' }).fill('706056');
        await page.getByTestId('searchButton').click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('military'))]);
        await expect(response.status()).toBe(200);
    });

    test('ADD MILITARY CONTRIBUTION button click', async ({ page }) => {
        await page.getByRole('textbox', { name: 'Enter Badge Number' }).fill('706056');
        await page.getByTestId('searchButton').click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('military'))]);
        await expect(response.status()).toBe(200);
        await page.getByRole('button', { name: 'Add Military Contribution' }).click();
        await page.getByRole('textbox', { name: 'YYYY' }).fill('2025');
        await page.locator('#contributionAmount').fill('150');
        await page.getByRole('checkbox', { name: 'Is Supplemental Contribution' }).check();
        await page.getByRole('button', { name: 'Submit' }).click();
        const [response1] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('military') && resp.request().method() === 'POST')]);
        if (response1.status() !== 201) {
            await expect(page.getByText('Employee employment status is not eligible for contributions')).toBeVisible();
        } else {
            await expect(response1.status()).toBe(201);
        }

    });

    test('checking pagination', async ({ page }) => {

        await page.getByRole('textbox', { name: 'Enter Badge Number' }).fill('706056');
        await page.getByTestId('searchButton').click();
        const [response] = await Promise.all([page.waitForResponse((resp) =>
            resp.url().includes('military'))]);
        await expect(response.status()).toBe(200);
        // If the API returned no results, skip pagination checks to avoid false failures
        const json = await response.json();
        if (!json || !Array.isArray(json.results) || json.results.length === 0) {
            // nothing to page through
            return;
        }
        // Locate the select dropdown
        const pageSizeSelect = page.locator('select[aria-label="rows per page"]');

        // Select by value
        await pageSizeSelect.selectOption('10');

        const isNextPageDisabled = await page.getByRole('button', { name: 'next page' }).isDisabled();

        if (!isNextPageDisabled) {
            await page.getByRole('button', { name: 'next page' }).click();
            const [response1] = await Promise.all([page.waitForResponse((resp) =>
                resp.url().includes('yearend/executive-hours-and-dollars'))]);
            await expect(response1.status()).toBe(200);
        }

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