import "@testing-library/jest-dom";
import { cleanup } from "@testing-library/react";
import { afterEach, beforeAll, vi } from "vitest";
import { ModuleRegistry } from "ag-grid-community";
import { ClientSideRowModelModule } from "ag-grid-community";
import React from "react";

// Register AG Grid modules globally for tests
// AG Grid v33+ requires explicit module registration
// See: https://www.ag-grid.com/react-data-grid/upgrading-to-ag-grid-33/#changes-to-modules/
ModuleRegistry.registerModules([ClientSideRowModelModule]);

// Configure base URL for RTK Query API calls in tests
beforeAll(() => {
  process.env.VITE_API_BASE_URL = "http://localhost:3000";
});

// Mock DOM methods not available in jsdom
Element.prototype.scrollIntoView = vi.fn();
HTMLElement.prototype.scrollIntoView = vi.fn();

// Mock components that make API calls to prevent real network requests during tests
vi.mock("../components/DuplicateSsnGuard", () => ({
  default: vi.fn(({ children }) => React.createElement(React.Fragment, null, children))
}));

// Mock React Hook Form globally to prevent form-related test failures
// Components using useForm, Controller, etc. will use these mocks
vi.mock("react-hook-form", async () => {
  const { createReactHookFormMock } = await import("./mocks/reactHookFormMock");
  return createReactHookFormMock();
});

// Cleanup after each test
afterEach(() => {
  cleanup();
});
