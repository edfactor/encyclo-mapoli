import "@testing-library/jest-dom";
import { cleanup } from "@testing-library/react";
import { afterEach, beforeAll, vi } from "vitest";
import { ModuleRegistry } from "ag-grid-community";
import { ClientSideRowModelModule } from "ag-grid-community";
import React from "react";

// Intercept stderr to suppress jsdom "Not implemented" warnings
const originalStderrWrite = process.stderr.write.bind(process.stderr);
process.stderr.write = function (
  chunk: string | Uint8Array,
  encodingOrCallback?: string | ((err?: Error) => void),
  callback?: (err?: Error) => void
): boolean {
  const message = typeof chunk === "string" ? chunk : chunk.toString();
  if (message.includes("Not implemented:")) {
    return true; // Suppress jsdom warnings
  }
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  return originalStderrWrite(chunk, encodingOrCallback as any, callback);
};

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

// Mock window.location to prevent "Not implemented: navigation" jsdom warnings
// jsdom doesn't implement navigation, so we stub assign/replace to no-op
Object.defineProperty(window, "location", {
  value: {
    ...window.location,
    assign: vi.fn(),
    replace: vi.fn(),
    reload: vi.fn()
  },
  writable: true
});

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

// Suppress all console output during tests to reduce noise
// Tests should assert behavior, not rely on console output
// To restore in a specific test: vi.restoreAllMocks()
vi.spyOn(console, "log").mockImplementation(() => {});
vi.spyOn(console, "info").mockImplementation(() => {});
vi.spyOn(console, "warn").mockImplementation(() => {});
vi.spyOn(console, "error").mockImplementation(() => {});
vi.spyOn(console, "debug").mockImplementation(() => {});
vi.spyOn(console, "trace").mockImplementation(() => {});

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
