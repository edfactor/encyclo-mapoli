import { expect, Page, Response } from "@playwright/test";

// Common selectors
const rolesCombobox = (page: Page) => page.getByRole("combobox", { name: "roles" });
const roleOptionCheckbox = (page: Page, roleName: string) =>
  page.getByRole("option", { name: roleName }).getByRole("checkbox");
const searchButton = (page: Page) => page.getByRole("button", { name: "SEARCH" });

/**
 * Ensures a role is checked or unchecked in the roles multiselect.
 */
export async function setRole(page: Page, roleName: string, checked = true): Promise<void> {
  await rolesCombobox(page).click();
  const cb = roleOptionCheckbox(page, roleName);
  const isChecked = await cb.isChecked();
  if (checked && !isChecked) await cb.check();
  if (!checked && isChecked) await cb.uncheck();
  // Close the dropdown by clicking outside
  await page.locator("body").click();
}

/**
 * Navigate to Master Inquiry landing page, impersonating Finance-Manager by default.
 */
export async function gotoMasterInquiry(page: Page, baseUrl: string): Promise<void> {
  // Go to app root; auth may redirect through Okta then back to our app
  await page.goto(baseUrl, { waitUntil: "domcontentloaded" });
  // Wait until any login callback completes and the main shell renders
  await page.waitForFunction(() => !location.pathname.includes("login/callback"), undefined, {
    timeout: 60000
  });
  // Wait for top nav to become available
  const inquiriesBtn = page.getByRole("button", { name: /INQUIRIES/i });
  await expect(inquiriesBtn).toBeVisible({ timeout: 60000 });
  // Set impersonation if roles combobox is available
  const rolesCombo = page.getByRole("combobox", { name: "roles" });
  if ((await rolesCombo.count()) > 0) {
    try {
      await expect(rolesCombo).toBeVisible({ timeout: 10000 });
      await setRole(page, "Finance-Manager", true);
      await page.reload({ waitUntil: "networkidle" });
      await expect(inquiriesBtn).toBeVisible({ timeout: 20000 });
    } catch {}
  }
  // Navigate via nav to Master Inquiry
  await inquiriesBtn.click();
  const masterInquiryLink = page.getByRole("link", { name: /MASTER INQUIRY/i });
  await expect(masterInquiryLink).toBeVisible({ timeout: 30000 });
  await masterInquiryLink.click();
  // Verify page heading
  await expect(page.getByRole("heading", { name: /MASTER INQUIRY/i })).toBeVisible({ timeout: 30000 });
}

/**
 * Clicks Search and resolves with the matching API response for master-inquiry search.
 * Uses a race-safe Promise.all to avoid missing fast responses.
 */
export async function triggerSearch(page: Page): Promise<Response> {
  const [response] = await Promise.all([
    page.waitForResponse(
      (resp) =>
        resp.url().includes("master-inquiry/search") && [200, 400, 401, 403, 404, 422, 500].includes(resp.status())
    ),
    searchButton(page).click()
  ]);
  return response;
}

/**
 * Triggers a search and asserts HTTP 200. Returns the API response for further inspection.
 */
export async function expectSearchOk(page: Page): Promise<Response> {
  await waitForMasterInquiryReady(page);
  const response = await triggerSearch(page);
  await expect(response.status(), "Expected search API to succeed").toBe(200);
  return response;
}

/** Returns parsed JSON body for the last search call. */
export async function searchAndGetJson<T = any>(page: Page): Promise<T> {
  const response = await expectSearchOk(page);
  return (await response.json()) as T;
}

/** Ensure the Master Inquiry form is ready for interaction */
export async function waitForMasterInquiryReady(page: Page): Promise<void> {
  // Search button should be visible and enabled
  const btn = searchButton(page);
  await expect(btn).toBeVisible({ timeout: 15000 });
  await expect(btn).toBeEnabled({ timeout: 15000 });
  // Fields should be enabled (they may start disabled during initial load)
  const nameInput = page.locator('input[name="name"]');
  const badgeInput = page.locator('input[name="badgeNumber"]');
  // It's okay if optional filters are disabled, but try to wait a bit for enablement
  try {
    await expect(nameInput).toBeEditable({ timeout: 5000 });
  } catch {}
  try {
    await expect(badgeInput).toBeEditable({ timeout: 5000 });
  } catch {}
}

export const selectors = {
  rolesCombobox,
  roleOptionCheckbox,
  searchButton
};
