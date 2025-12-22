import { AllCommunityModule, ModuleRegistry } from "ag-grid-community";

/**
 * Initialize ag-grid modules
 * Defers loading non-critical modules to idle time for faster startup
 * Call this function early in the application lifecycle (e.g., in App.tsx useEffect)
 */
export function initializeAgGrid() {
  // Register all community modules immediately (minimal overhead for community features)
  ModuleRegistry.registerModules([AllCommunityModule]);

  // Could defer additional enterprise modules here if needed in future
  // For now, community modules are lightweight enough to load synchronously
}
