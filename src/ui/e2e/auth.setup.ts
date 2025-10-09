import { test as setup } from "@playwright/test";
import { password, userName } from "./env.setup";

const authFile = "test-results/user.json";

setup("authenticate", async ({ page }) => {
  if (!userName || !password) {
    throw new Error("No username/password found for playwright");
  }
  // Perform authentication steps. Replace these actions with your own.
  await page.goto("https://marketbasket.okta.com/login/default");
  // Robust typing: click, clear, then type with a small delay to avoid input concatenation
  const userInput = page.getByLabel("Username");
  await userInput.click();
  await userInput.fill("");
  await userInput.type(userName, { delay: 50 });
  // Press Tab to move focus reliably to the password field
  await page.keyboard.press("Tab");

  const passwordInput = page.getByLabel("Password");
  // Ensure password field has focus and is empty, then type
  await passwordInput.click();
  await passwordInput.fill("");
  await passwordInput.type(password, { delay: 50 });

  // Verify inputs to catch intermittent concatenation early
  const typedUser = await userInput.inputValue();
  const typedPass = await passwordInput.inputValue();
  if (typedUser !== userName || typedPass !== password) {
    throw new Error(`Auth typing verification failed: user='${typedUser}' passLen=${typedPass.length}`);
  }

  await page.getByRole("button", { name: "Sign in" }).click();
  // Wait until the page receives the cookies.
  //
  // Sometimes login flow sets cookies in the process of several redirects.
  // Wait for the final URL to ensure that the cookies are actually set.
  await page.waitForURL("https://marketbasket.okta.com/app/UserHome?session_hint=AUTHENTICATED");

  // End of authentication steps.

  await page.context().storageState({ path: authFile });
});
