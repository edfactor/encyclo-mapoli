import type { Promise as PromiseType } from "@types/node";
import { ModuleRegistry } from "ag-grid-community";

/**
 * Register core ag-grid modules immediately at startup
 * These modules are loaded synchronously as they are needed for basic grid functionality
 */
export async function registerCoreGridModules() {
  const {
    ClientSideRowModelModule,
    PaginationModule,
    FilterToolPanelModule,
    StatusBarModule
  } = await import("ag-grid-community");

  ModuleRegistry.registerModules([
    ClientSideRowModelModule,
    PaginationModule,
    FilterToolPanelModule,
    StatusBarModule
  ]);
}

/**
 * Register advanced ag-grid modules lazily when needed
 * These modules are deferred to improve initial load time
 * Call this function when the application needs advanced grid features
 */
export async function registerAdvancedGridModules() {
  const {
    ClipboardModule,
    ExcelExportModule,
    ColumnsToolPanelModule,
    MasterDetailModule,
    SideBarModule,
    RangeSelectionModule,
    RowGroupingModule,
    RichSelectModule,
    RowGridModule,
    MultiFilterModule,
    AdvancedFilterModule
  } = await import("ag-grid-community");

  ModuleRegistry.registerModules([
    ClipboardModule,
    ExcelExportModule,
    ColumnsToolPanelModule,
    MasterDetailModule,
    SideBarModule,
    RangeSelectionModule,
    RowGroupingModule,
    RichSelectModule,
    RowGridModule,
    MultiFilterModule,
    AdvancedFilterModule
  ]);
}

/**
 * Initialize ag-grid modules
 * Registers core modules immediately, defers advanced modules to idle time
 * This should be called early in the application lifecycle (e.g., in App.tsx useEffect)
 */
export function initializeAgGrid(): void {
  // Register core modules immediately
  registerCoreGridModules().catch((err) => {
    console.error("Failed to register core ag-grid modules:", err);
  });

  // Defer advanced modules to idle time for faster initial load
  if (typeof window !== "undefined" && "requestIdleCallback" in window) {
    requestIdleCallback(
      () => {
        registerAdvancedGridModules().catch((err) => {
          console.error("Failed to register advanced ag-grid modules:", err);
        });
      },
      { timeout: 5000 }
    );
  } else {
    // Fallback for browsers without requestIdleCallback
    setTimeout(() => {
      registerAdvancedGridModules().catch((err) => {
        console.error("Failed to register advanced ag-grid modules:", err);
      });
    }, 2000);
  }
}
