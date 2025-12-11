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

// Polyfill for requestSubmit which is not implemented in jsdom
// See: https://github.com/jsdom/jsdom/issues/3117
// Note: JSDOM defines requestSubmit but throws "Not implemented", so we unconditionally override
HTMLFormElement.prototype.requestSubmit = function (submitter?: HTMLElement) {
  if (submitter) {
    if (!(submitter instanceof HTMLElement)) {
      throw new TypeError("The specified element is not of type HTMLElement");
    }
    if ((submitter as HTMLButtonElement).type !== "submit") {
      throw new TypeError("The specified element is not a submit button");
    }
    if ((submitter as HTMLButtonElement).form !== this) {
      throw new DOMException("The specified element is not owned by this form element", "NotFoundError");
    }
    submitter.click();
  } else {
    const submitEvent = new Event("submit", { bubbles: true, cancelable: true });
    this.dispatchEvent(submitEvent);
  }
};

// Suppress JSDOM "Not implemented" console.error warnings that can't be polyfilled
// These warnings are logged by JSDOM before our polyfills can take effect
const originalConsoleError = console.error;
console.error = (...args: unknown[]) => {
  const message = args[0];
  if (typeof message === "string" && message.includes("Not implemented: HTMLFormElement")) {
    return; // Suppress JSDOM "Not implemented" warnings
  }
  originalConsoleError.apply(console, args);
};

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
