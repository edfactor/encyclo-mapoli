import { fixupPluginRules } from "@eslint/compat";
import js from "@eslint/js";
import tsPlugin from "@typescript-eslint/eslint-plugin";
import tsParser from "@typescript-eslint/parser";
import reactHooks from "eslint-plugin-react-hooks";
import reactRefresh from "eslint-plugin-react-refresh";
import { defineConfig, globalIgnores } from "eslint/config";
import globals from "globals";
import path from "node:path";
import { fileURLToPath } from "node:url";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

export default defineConfig([
  globalIgnores(["**/dist", "**/.eslintrc.cjs", "**/coverage", "**/node_modules", "**/.env*", "**/build"]),

  // Main TypeScript/React files
  {
    files: ["src/**/*.{ts,tsx}"],
    plugins: {
      "react-refresh": reactRefresh,
      "react-hooks": fixupPluginRules(reactHooks),
      "@typescript-eslint": fixupPluginRules(tsPlugin)
    },

    languageOptions: {
      globals: {
        ...globals.browser,
        React: "readonly",
        RequestInfo: "readonly",
        RequestInit: "readonly",
        NodeJS: "readonly",
        process: "readonly"
      },
      parser: tsParser,
      ecmaVersion: 2024
    },

    rules: {
      ...js.configs.recommended.rules,

      // Disable JS version of no-unused-vars, use TypeScript version instead
      "no-unused-vars": "off",

      // React Compiler - STRICT MODE
      "react-refresh/only-export-components": [
        "error",
        {
          allowConstantExport: false
        }
      ],

      // React Hooks - STRICT (enforced as errors)
      "react-hooks/rules-of-hooks": "error",
      "react-hooks/exhaustive-deps": "error",

      // TypeScript - STRICT
      "@typescript-eslint/no-unused-vars": [
        "error",
        {
          argsIgnorePattern: "^_",
          varsIgnorePattern: "^_",
          caughtErrorsIgnorePattern: "^_",
          destructuredArrayIgnorePattern: "^_"
        }
      ],
      "@typescript-eslint/no-explicit-any": "error",
      "@typescript-eslint/ban-ts-comment": "error",
      "@typescript-eslint/no-require-imports": "error"
    }
  },

  // Test files with vitest/jest globals
  {
    files: [
      "src/**/*.{test,spec}.{ts,tsx}",
      "**/__test__/**/*.{ts,tsx}",
      "**/__tests__/**/*.{ts,tsx}",
      "src/test/**/*.{ts,tsx}",
      "e2e/**/*.ts"
    ],
    plugins: {
      "@typescript-eslint": fixupPluginRules(tsPlugin)
    },
    languageOptions: {
      globals: {
        ...globals.browser,
        ...globals.node,
        describe: "readonly",
        it: "readonly",
        test: "readonly",
        expect: "readonly",
        beforeEach: "readonly",
        afterEach: "readonly",
        beforeAll: "readonly",
        afterAll: "readonly",
        jest: "readonly",
        vi: "readonly",
        BlobPart: "readonly",
        RequestInfo: "readonly",
        RequestInit: "readonly",
        NodeJS: "readonly",
        process: "readonly"
      },
      parser: tsParser
    },
    rules: {
      "@typescript-eslint/no-unused-vars": [
        "error",
        {
          argsIgnorePattern: "^_",
          varsIgnorePattern: "^_",
          caughtErrorsIgnorePattern: "^_"
        }
      ]
    }
  },

  // Config files with node globals
  {
    files: ["*.config.{js,ts}", "vite.config.ts", "vitest.config.ts", "tailwind.config.js"],
    plugins: {
      "@typescript-eslint": fixupPluginRules(tsPlugin)
    },
    languageOptions: {
      globals: {
        ...globals.node,
        process: "readonly",
        __dirname: "readonly",
        __filename: "readonly"
      },
      parser: tsParser
    },
    rules: {
      "@typescript-eslint/no-unused-vars": [
        "error",
        {
          argsIgnorePattern: "^_",
          varsIgnorePattern: "^_",
          caughtErrorsIgnorePattern: "^_"
        }
      ]
    }
  },

  // CommonJS files
  {
    files: ["**/*.cjs"],
    languageOptions: {
      globals: {
        ...globals.node
      }
    }
  }
]);
