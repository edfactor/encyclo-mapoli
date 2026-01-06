/**
 * Utility functions for persisting grid pagination state to localStorage.
 *
 * Note: Column state is handled separately by smart-ui-library's DSMGrid,
 * which stores it as a raw array directly under the GRID_KEY.
 * Pagination is stored under `${GRID_KEY}_pagination` to avoid conflicts.
 */

/**
 * Pagination state stored for each grid
 */
export interface PaginationState {
  pageNumber: number;
  pageSize: number;
  sortBy: string;
  isSortDescending: boolean;
}

const PAGINATION_SUFFIX = "_pagination";

const warnedInvalidPaginationKeys = new Set<string>();

const isValidGridPersistenceKey = (key: unknown): key is string =>
  typeof key === "string" && Boolean(key) && key !== "undefined" && key !== "null";

const warnIfInvalidKey = (key: unknown) => {
  if (process.env.NODE_ENV !== "development") {
    return;
  }

  if (isValidGridPersistenceKey(key)) {
    return;
  }

  const keyLabel = String(key);
  if (warnedInvalidPaginationKeys.has(keyLabel)) {
    return;
  }

  warnedInvalidPaginationKeys.add(keyLabel);
  console.warn(
    `[useGridPagination] Invalid persistenceKey detected (will skip localStorage pagination): "${keyLabel}". Investigate the call site passing persistenceKey.`,
    new Error("Invalid persistenceKey stack trace")
  );
};

/**
 * Load pagination state from localStorage
 * @param key - Unique key for the grid (from GRID_KEYS)
 * @returns The pagination state or null if not found
 */
export const loadPaginationState = (key: string): PaginationState | null => {
  if (!isValidGridPersistenceKey(key)) {
    warnIfInvalidKey(key);
    return null;
  }
  try {
    const stored = localStorage.getItem(`${key}${PAGINATION_SUFFIX}`);
    if (stored) {
      return JSON.parse(stored) as PaginationState;
    }
    return null;
  } catch (error) {
    console.warn(`Failed to load pagination state for key "${key}":`, error);
    return null;
  }
};

/**
 * Save pagination state to localStorage
 * @param key - Unique key for the grid
 * @param pagination - The pagination state to persist
 */
export const savePaginationState = (key: string, pagination: PaginationState): void => {
  if (!isValidGridPersistenceKey(key)) {
    warnIfInvalidKey(key);
    return;
  }
  try {
    localStorage.setItem(`${key}${PAGINATION_SUFFIX}`, JSON.stringify(pagination));
  } catch (error) {
    console.warn(`Failed to save pagination state for key "${key}":`, error);
  }
};

/**
 * Clear pagination state from localStorage
 * @param key - Unique key for the grid
 */
export const clearPaginationState = (key: string): void => {
  if (!isValidGridPersistenceKey(key)) {
    warnIfInvalidKey(key);
    return;
  }
  try {
    localStorage.removeItem(`${key}${PAGINATION_SUFFIX}`);
  } catch (error) {
    console.warn(`Failed to clear pagination state for key "${key}":`, error);
  }
};
