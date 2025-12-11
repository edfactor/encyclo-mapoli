/**
 * Utility functions for persisting grid pagination state to localStorage.
 * Used by useGridPagination hook when a persistenceKey is provided.
 */

export interface PersistedGridState {
  pageNumber: number;
  pageSize: number;
  sortBy: string;
  isSortDescending: boolean;
}

const STORAGE_PREFIX = "gridPagination_";

/**
 * Load persisted grid state from localStorage
 * @param key - Unique key for the grid (e.g., "TERMINATION", "PROFALL")
 * @returns The persisted state or null if not found/invalid
 */
export const loadGridState = (key: string): PersistedGridState | null => {
  try {
    const stored = localStorage.getItem(`${STORAGE_PREFIX}${key}`);
    if (stored) {
      const parsed = JSON.parse(stored) as PersistedGridState;
      // Validate the parsed object has required fields
      if (
        typeof parsed.pageNumber === "number" &&
        typeof parsed.pageSize === "number" &&
        typeof parsed.sortBy === "string" &&
        typeof parsed.isSortDescending === "boolean"
      ) {
        return parsed;
      }
    }
  } catch (error) {
    console.warn(`Failed to load persisted grid state for key "${key}":`, error);
  }
  return null;
};

/**
 * Save grid state to localStorage
 * @param key - Unique key for the grid
 * @param state - The grid state to persist
 */
export const saveGridState = (key: string, state: PersistedGridState): void => {
  try {
    localStorage.setItem(`${STORAGE_PREFIX}${key}`, JSON.stringify(state));
  } catch (error) {
    console.warn(`Failed to save grid state for key "${key}":`, error);
  }
};

/**
 * Clear persisted grid state from localStorage
 * @param key - Unique key for the grid
 */
export const clearGridState = (key: string): void => {
  try {
    localStorage.removeItem(`${STORAGE_PREFIX}${key}`);
  } catch (error) {
    console.warn(`Failed to clear grid state for key "${key}":`, error);
  }
};
