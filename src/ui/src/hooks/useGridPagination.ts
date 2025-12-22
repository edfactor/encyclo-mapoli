import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { clearPaginationState, loadPaginationState, savePaginationState } from "../utils/gridPersistence";

export interface SortParams {
  sortBy: string;
  isSortDescending: boolean;
}

export interface GridPaginationState {
  pageNumber: number;
  pageSize: number;
  sortParams: SortParams;
}

export interface GridPaginationActions {
  handlePaginationChange: (pageNumber: number, pageSize: number) => void;
  /** Use this for setPageNumber prop - avoids stale closure issues with pageSize */
  handlePageNumberChange: (pageNumber: number) => void;
  /** Use this for setPageSize prop - automatically resets to page 0 */
  handlePageSizeChange: (pageSize: number) => void;
  handleSortChange: (sortParams: SortParams) => void;
  resetPagination: () => void;
  clearPersistedState: () => void;
}

export interface UseGridPaginationConfig {
  initialPageSize: number;
  initialSortBy: string;
  initialSortDescending?: boolean;
  onPaginationChange?: (pageNumber: number, pageSize: number, sortParams: SortParams) => void;
  /**
   * Optional key for persisting pagination state to localStorage.
   * If provided, pagination state (page number, page size, sort) will be
   * saved and restored across sessions.
   * Use the same key as DSMGrid's preferenceKey for consistency.
   */
  persistenceKey?: string;
}

/*
 * Hook for grid pagination state and handlers.
 * Optionally persists state to localStorage when persistenceKey is provided.
 */
export const useGridPagination = ({
  initialPageSize,
  initialSortBy,
  initialSortDescending = true,
  onPaginationChange,
  persistenceKey
}: UseGridPaginationConfig): GridPaginationState & GridPaginationActions => {
  // Load persisted state if persistenceKey is provided
  const persistedState = persistenceKey ? loadPaginationState(persistenceKey) : null;

  // Debug: Log what we're loading from localStorage
  if (persistenceKey && process.env.NODE_ENV === "development") {
    console.log(`[useGridPagination] ${persistenceKey}: loaded`, persistedState);
  }

  const [pageNumber, setPageNumber] = useState(persistedState?.pageNumber ?? 0);
  const [pageSize, setPageSize] = useState(persistedState?.pageSize ?? initialPageSize);
  const [sortBy, setSortBy] = useState(persistedState?.sortBy ?? initialSortBy);
  const [isSortDescending, setIsSortDescending] = useState(persistedState?.isSortDescending ?? initialSortDescending);

  // Store the callback in a ref to make it stable
  const callbackRef = useRef(onPaginationChange);
  useEffect(() => {
    callbackRef.current = onPaginationChange;
  }, [onPaginationChange]);

  // Track current pageSize in a ref to avoid stale closure issues
  const pageSizeRef = useRef(pageSize);
  pageSizeRef.current = pageSize;

  const sortParams = useMemo(
    () => ({
      sortBy,
      isSortDescending
    }),
    [sortBy, isSortDescending]
  );

  const pagination = useMemo(
    () => ({
      pageNumber,
      pageSize,
      sortParams
    }),
    [pageNumber, pageSize, sortParams]
  );

  // Persist state to localStorage when it changes (if persistenceKey is provided)
  useEffect(() => {
    if (persistenceKey) {
      savePaginationState(persistenceKey, {
        pageNumber,
        pageSize,
        sortBy,
        isSortDescending
      });
    }
  }, [persistenceKey, pageNumber, pageSize, sortBy, isSortDescending]);

  const handlePaginationChange = useCallback(
    (newPageNumber: number, newPageSize: number) => {
      setPageNumber(newPageNumber);
      setPageSize(newPageSize);
      pageSizeRef.current = newPageSize;

      if (typeof callbackRef.current === "function") {
        callbackRef.current(newPageNumber, newPageSize, { sortBy, isSortDescending });
      }
    },
    [sortBy, isSortDescending]
  );

  // Separate handler for page number changes - uses ref to get current pageSize
  // This avoids stale closure issues when Pagination component calls both
  // setPageSize and setPageNumber synchronously
  const handlePageNumberChange = useCallback(
    (newPageNumber: number) => {
      setPageNumber(newPageNumber);

      if (typeof callbackRef.current === "function") {
        callbackRef.current(newPageNumber, pageSizeRef.current, { sortBy, isSortDescending });
      }
    },
    [sortBy, isSortDescending]
  );

  // Separate handler for page size changes - always resets to page 0
  const handlePageSizeChange = useCallback(
    (newPageSize: number) => {
      setPageNumber(0);
      setPageSize(newPageSize);
      pageSizeRef.current = newPageSize;

      if (typeof callbackRef.current === "function") {
        callbackRef.current(0, newPageSize, { sortBy, isSortDescending });
      }
    },
    [sortBy, isSortDescending]
  );

  const handleSortChange = useCallback(
    (newSortParams: SortParams) => {
      setSortBy(newSortParams.sortBy);
      setIsSortDescending(newSortParams.isSortDescending);

      if (callbackRef.current) {
        callbackRef.current(pageNumber, pageSize, newSortParams);
      }
    },
    [pageNumber, pageSize]
  );

  const resetPagination = useCallback(() => {
    setPageNumber(0);
    setPageSize(initialPageSize);
    setSortBy(initialSortBy);
    setIsSortDescending(initialSortDescending);

    // Clear persisted pagination state when resetting (preserves column state)
    if (persistenceKey) {
      clearPaginationState(persistenceKey);
    }
  }, [initialPageSize, initialSortBy, initialSortDescending, persistenceKey]);

  const clearPersistedState = useCallback(() => {
    if (persistenceKey) {
      clearPaginationState(persistenceKey);
    }
  }, [persistenceKey]);

  return {
    ...pagination,
    handlePaginationChange,
    handlePageNumberChange,
    handlePageSizeChange,
    handleSortChange,
    resetPagination,
    clearPersistedState
  };
};
