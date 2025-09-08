import { test as setup } from "@playwright/test";
import { password, userName } from "./env.setup";

const authFile = "test-results/user.json";

setup("authenticate", async ({ page }) => {
  if (!userName || !password) {
    throw new Error("No username/password found for playwright");
  }
  // Perform authentication steps. Replace these actions with your own.
  await page.goto("https://marketbasket.okta.com/login/default");
  await page.getByLabel("Username").fill(userName);
  await page.getByLabel("Password").fill(password);
  await page.getByRole("button", { name: "Sign in" }).click();
  // Wait until the page receives the cookies.
  //
  // Sometimes login flow sets cookies in the process of several redirects.
  // Wait for the final URL to ensure that the cookies are actually set.
  await page.waitForURL("https://marketbasket.okta.com/app/UserHome?session_hint=AUTHENTICATED");

  // End of authentication steps.

  await page.context().storageState({ path: authFile });
});
