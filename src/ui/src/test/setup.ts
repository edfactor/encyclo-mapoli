import "@testing-library/jest-dom";
import { cleanup } from "@testing-library/react";
import { afterEach, vi } from "vitest";
import { ModuleRegistry } from "ag-grid-community";
import { ClientSideRowModelModule } from "ag-grid-community";

// Register AG Grid modules globally for tests
// AG Grid v33+ requires explicit module registration
// See: https://www.ag-grid.com/react-data-grid/upgrading-to-ag-grid-33/#changes-to-modules/
ModuleRegistry.registerModules([ClientSideRowModelModule]);

// Mock DOM methods not available in jsdom
Element.prototype.scrollIntoView = vi.fn();
HTMLElement.prototype.scrollIntoView = vi.fn();

// Cleanup after each test
afterEach(() => {
  cleanup();
});
