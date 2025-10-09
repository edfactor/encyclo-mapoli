import type { Page } from "@playwright/test";
import dotenv from "dotenv";
import path, { dirname } from "path";
import { fileURLToPath } from "url";

const playWrightEnv = "../.playwright.env";
const localBaseUrl = "http://localhost:3100";
const qaBaseUrl = "https://ps.qa.demoulas.net";

// if no user name / password defined, loading them for .playwright.env file, which is in gitignore.
// if you do not have the file (.playwright.env), please ask one developer.
if (!process.env.USER_NAME || !process.env.PASSWORD) {
  // Get the file path
  const fileName = fileURLToPath(import.meta.url);
  // Get the directory path
  const rootDirectory = dirname(fileName);
  // Load .env file before tests run
  const envConfig = dotenv.config({ path: path.resolve(rootDirectory, playWrightEnv) });
  if (envConfig.error) {
    throw new Error(envConfig.error.message);
  }
}

export const userName = process.env.USER_NAME;
export const password = process.env.PASSWORD;
export const baseUrl = process.env.QA == "true" ? qaBaseUrl : localBaseUrl;

export async function impersonateRole(page: Page, roleName: string) {
  // Try multiple ways to find the roles combobox. Some builds expose it as "Roles" or
  // with an aria-label. Use case-insensitive match for robustness.
  const candidateLocators = [
    page.getByRole("combobox", { name: /roles/i }),
    page.locator('[aria-label="roles"]'),
    page.locator('[aria-label="Roles"]'),
    page.locator('select[aria-label="roles"]'),
    page.locator('select[name="roles"]'),
    page.locator('div[role="combobox"] >> text=/roles/i')
  ];

  // Try to find the combobox, retrying a few times and attempting to open the side drawer
  let combo: any = null;
  const maxAttempts = 6; // increase attempts for slower or loaded environments
  for (let attempt = 1; attempt <= maxAttempts; attempt++) {
    for (const loc of candidateLocators) {
      try {
        // increased wait to discover the element under load
        await loc.waitFor({ state: "visible", timeout: 3000 });
        combo = loc;
        break;
      } catch (e) {
        // ignore and try next locator
      }
    }

    if (combo) break;

    // If not found, try opening the side drawer (many pages hide the roles combobox inside it)
    try {
      // prefer a specific drawer toggle if present
      const drawerSelectors = [
        'button[aria-label="Open Navigation"]',
        'button[data-testid="drawer-toggle"]',
        "button.MuiIconButton-root"
      ];
      for (const sel of drawerSelectors) {
        const btn = page.locator(sel).first();
        if ((await btn.count()) > 0) {
          await btn.click({ timeout: 2500 }).catch(() => {});
          break;
        }
      }

      // small backoff to allow DOM to update
      await page.waitForTimeout(700);
    } catch (e) {
      // swallow and continue attempts
    }
  }

  if (!combo) {
    const message =
      "impersonateRole: roles combobox not found after retries. Ensure the page exposes a combobox with accessible name 'roles' or check the app state.";
    // eslint-disable-next-line no-console
    console.error(message);
    throw new Error(message);
  }

  try {
    // Perform a normal click if possible with a short timeout
    await combo.click({ timeout: 5000 });

    // Find the option's checkbox and check it if not already checked
    const optionCheckbox = page.getByRole("option", { name: roleName }).getByRole("checkbox");
    await optionCheckbox.waitFor({ state: "visible", timeout: 5000 });
    const checked = await optionCheckbox.isChecked().catch(() => false);
    if (!checked) {
      await optionCheckbox.check({ timeout: 5000 });
    }

    await page.locator("body").click();
    await page.reload();
    await page.waitForLoadState("networkidle");
    return;
  } catch (err) {
    // Retry with force clicks but keep short timeouts to avoid global test timeout hangs
    try {
      await combo.click({ force: true, timeout: 3000 });
      const opt = page.getByRole("option", { name: roleName });
      // try a couple of times to find the option; sometimes the options render after the combobox opens
      let optionFound = false;
      for (let i = 0; i < 3; i++) {
        try {
          await opt.waitFor({ state: "visible", timeout: 1000 });
          optionFound = true;
          break;
        } catch (e) {
          // small wait and retry
          await page.waitForTimeout(300);
        }
      }
      if (optionFound) {
        await opt.getByRole("checkbox").check({ force: true, timeout: 2000 });
      } else {
        throw new Error(`Option for role '${roleName}' not found`);
      }
      await page.locator("body").click({ force: true, timeout: 2000 });
      await page.reload();
      await page.waitForLoadState("networkidle");
      return;
    } catch (err2) {
      // Expose a helpful error for debugging flakiness in CI
      // eslint-disable-next-line no-console
      console.error("impersonateRole failed", err2);
      throw err2;
    }
  }
}

/**
 * Set the page status dropdown to a specific value (e.g., "In Progress", "Complete").
 * This is typically the second combobox on the page (index 1).
 * If the year is frozen or status dropdown is not available, this function will skip gracefully.
 * @param page - Playwright Page object
 * @param status - Status to select (e.g., "In Progress", "Complete")
 */
export async function setPageStatus(page: Page, status: string) {
  try {
    // Wait for page to be ready
    await page.waitForLoadState("networkidle");

    // The status dropdown is typically the second combobox on the page
    const statusDropdown = page.getByRole("combobox").nth(1);

    // Check if dropdown exists and is visible (it might be hidden if year is frozen)
    const isVisible = await statusDropdown.isVisible().catch(() => false);

    if (!isVisible) {
      console.warn(`Status dropdown not visible - year might be frozen. Skipping status change.`);
      return; // Gracefully skip if not available
    }

    // Click to open dropdown
    await statusDropdown.click({ timeout: 3000 });

    // Wait a moment for options to appear
    await page.waitForTimeout(300);

    // Select the desired status
    await page.getByRole("option", { name: status }).click();

    // Wait for any status change API calls to complete
    await page.waitForTimeout(500);
  } catch (error) {
    // If status dropdown is not found or not available, log a warning but don't fail
    // This allows tests to proceed even when year is frozen
    console.warn(`Could not set page status to "${status}": ${error}`);
    // Don't throw - let the test continue
  }
}

/**
 * Navigate to a page through the navigation drawer with retry logic.
 * Handles race conditions in parallel test execution.
 *
 * @param page - Playwright page object
 * @param categoryName - Top-level navigation category (e.g., "December Activities", "Year End")
 * @param pageName - Specific page name (can be string or regex)
 * @param maxRetries - Maximum number of retry attempts (default: 3)
 */
export async function navigateToPage(
  page: Page,
  categoryName: string,
  pageName: string | RegExp,
  maxRetries: number = 3
): Promise<void> {
  for (let attempt = 1; attempt <= maxRetries; attempt++) {
    try {
      // Step 1: Open the drawer if needed (empty button pattern)
      const emptyBtn = page.getByRole("button").filter({ hasText: /^$/ });
      const emptyBtnCount = await emptyBtn.count();
      if (emptyBtnCount > 0) {
        await emptyBtn.first().click({ timeout: 5000 });
        // Wait for drawer animation
        await page.waitForTimeout(500);
      }

      // Step 2: Click on the category
      const categoryBtn = page.getByRole("button", { name: categoryName });
      await categoryBtn.waitFor({ state: "visible", timeout: 10000 });
      await categoryBtn.click({ timeout: 5000 });

      // Wait for submenu to expand
      await page.waitForTimeout(500);

      // Step 3: Click on the specific page
      const pageBtn =
        typeof pageName === "string"
          ? page.getByRole("button", { name: pageName })
          : page.getByRole("button", { name: pageName });

      await pageBtn.waitFor({ state: "visible", timeout: 10000 });
      await pageBtn.click({ timeout: 5000 });

      // Wait for navigation to complete
      await page.waitForLoadState("networkidle", { timeout: 15000 });

      console.log(`Successfully navigated to ${categoryName} > ${pageName} on attempt ${attempt}`);
      return; // Success!
    } catch (error) {
      console.warn(`Navigation attempt ${attempt} failed:`, error);

      if (attempt === maxRetries) {
        throw new Error(`Failed to navigate to ${categoryName} > ${pageName} after ${maxRetries} attempts`);
      }

      // Wait before retry and try to reset state
      await page.waitForTimeout(2000);

      // Try to close any open drawers/menus before retry
      try {
        await page.keyboard.press("Escape");
        await page.waitForTimeout(500);
      } catch (e) {
        // Ignore errors when pressing Escape
      }
    }
  }
}
