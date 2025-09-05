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
