import { defineConfig } from "vitest/config";
import react from "@vitejs/plugin-react";
import path from "path";

export default defineConfig({
  plugins: [react()],
  test: {
    globals: true,
    environment: "jsdom",
    environmentOptions: {
      jsdom: {
        // Suppress jsdom "Not implemented" warnings (navigation, etc.)
        pretendToBeVisual: true,
        resources: "usable"
      }
    },
    setupFiles: "./src/test/setup.ts",
    css: true,
    // Use 'dot' reporter for minimal output: dots for passes, details for failures, summary at end
    reporters: ["dot"],
    // Suppress all console output during tests (return false to hide, true to show)
    onConsoleLog: () => false,
    // Windows limitation: Reduce workers to avoid "too many open files" error
    // On Windows, limit to 1 worker; on Unix systems, use default (auto-detected based on CPU cores)
    threads: process.platform === "win32" ? 1 : true,
    // Disable file-level parallelism on Windows to reduce file descriptor pressure
    fileParallelism: process.platform === "win32" ? false : true,
    exclude: ["**/node_modules/**", "**/dist/**", "**/e2e/**", "**/.{idea,git,cache,output,temp}/**"],
    coverage: {
      provider: "v8",
      reporter: ["text", "json", "html"],
      // Ensure coverage directory exists and cleanup is handled properly
      reportsDirectory: "./coverage",
      cleanOnRerun: true,
      exclude: ["node_modules/", "src/test/", "e2e/", "**/*.d.ts", "**/*.config.*", "**/mockData", "**/types"]
    },
    // Handle server-side dependencies that need special resolution
    server: {
      deps: {
        inline: ["smart-ui-library"]
      }
    }
  },
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
      reduxstore: path.resolve(__dirname, "./src/reduxstore"),
      utils: path.resolve(__dirname, "./src/utils"),
      Okta: path.resolve(__dirname, "./src/Okta"),
      components: path.resolve(__dirname, "./src/components"),
      pages: path.resolve(__dirname, "./src/pages"),
      schemas: path.resolve(__dirname, "./src/schemas"),
      hooks: path.resolve(__dirname, "./src/hooks"),
      styles: path.resolve(__dirname, "./src/styles"),
      // Fix for ES module directory import issue with MUI date pickers
      "@mui/x-date-pickers/AdapterDateFnsV3": "@mui/x-date-pickers/AdapterDateFnsV3/index.js",
      "@mui/x-date-pickers/AdapterDayjs": "@mui/x-date-pickers/AdapterDayjs/index.js"
    },
    // Add conditions to help with ES module resolution
    conditions: ["node", "import", "module", "browser", "default"]
  }
});
