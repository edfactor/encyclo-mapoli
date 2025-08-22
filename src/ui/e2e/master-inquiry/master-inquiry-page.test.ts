import { test, expect } from "@playwright/test";
import { baseUrl } from "../env.setup";


test.describe("Master Inqiry landing page: ", () => {
    test.beforeEach(async ({ page }) => {
        await page.goto(baseUrl);
        await page.waitForLoadState("networkidle");
        await page.getByRole("button", { name: "INQUIRIES" }).click();
        await page.getByRole("link", { name: "MASTER INQUIRY" }).click();
    });
    test("page should load properly without any issue.", async ({ page }) => {
        await expect(page.getByRole("heading", { name: "MASTER INQUIRY (008-10)" })).toBeVisible();
    });

    test("click on search button without setting IMPERSONATE value", async ({page}) => {
        await page.getByRole("button",{name:"SEARCH"}).click();
        await page.waitForLoadState("networkidle");
        await expect(page.getByRole("paragraph",{name:"Search Failed"})).toBeVisible();
    });

    test("click on search button after setting IMPERSONATE value", async ({page}) => {
        await page.getByRole("combobox",{name:"roles"}).click();
        await page.getByRole('option', { name: 'Finance-Manager' }).getByRole('checkbox').check();
        await page.getByRole("button",{name:"SEARCH"}).dblclick();
        await page.waitForLoadState("networkidle");
        await expect(page.getByRole("presentation",{name:"Badge"}).isVisible())
    });
});