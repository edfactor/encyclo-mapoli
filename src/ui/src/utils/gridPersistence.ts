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

/**
 * Load pagination state from localStorage
 * @param key - Unique key for the grid (from GRID_KEYS)
 * @returns The pagination state or null if not found
 */
export const loadPaginationState = (key: string): PaginationState | null => {
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
  try {
    localStorage.removeItem(`${key}${PAGINATION_SUFFIX}`);
  } catch (error) {
    console.warn(`Failed to clear pagination state for key "${key}":`, error);
  }
};
