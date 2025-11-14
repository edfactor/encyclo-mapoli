import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { SortParams, GridPaginationState, GridPaginationActions, UseGridPaginationConfig } from "../../../../hooks/useGridPagination";

const STORAGE_KEY = "payBenReport_gridState";

interface PersistedGridState {
  pageNumber: number;
  pageSize: number;
  sortBy: string;
  isSortDescending: boolean;
}

/**
 * Enhanced grid pagination hook that persists state in session storage
 * This ensures sorting and pagination state is preserved when navigating away and back
 */
export const usePersistedGridPagination = ({
  initialPageSize,
  initialSortBy,
  initialSortDescending = true,
  onPaginationChange
}: UseGridPaginationConfig): GridPaginationState & GridPaginationActions & { clearPersistedState: () => void } => {
  // Load persisted state from session storage
  const loadPersistedState = useCallback((): PersistedGridState | null => {
    try {
      const stored = sessionStorage.getItem(STORAGE_KEY);
      if (stored) {
        return JSON.parse(stored);
      }
    } catch (error) {
      console.warn("Failed to load persisted grid state:", error);
    }
    return null;
  }, []);

  // Save state to session storage
  const savePersistedState = useCallback((state: PersistedGridState) => {
    try {
      sessionStorage.setItem(STORAGE_KEY, JSON.stringify(state));
    } catch (error) {
      console.warn("Failed to save grid state:", error);
    }
  }, []);

  // Initialize state from persisted data or defaults
  const persistedState = loadPersistedState();
  
  // Debug logging for development
  if (persistedState) {
    console.log("PayBenReport: Loaded persisted grid state:", persistedState);
  }
  
  const [pageNumber, setPageNumber] = useState(persistedState?.pageNumber ?? 0);
  const [pageSize, setPageSize] = useState(persistedState?.pageSize ?? initialPageSize);
  const [sortBy, setSortBy] = useState(persistedState?.sortBy ?? initialSortBy);
  const [isSortDescending, setIsSortDescending] = useState(
    persistedState?.isSortDescending ?? initialSortDescending
  );

  // Store the callback in a ref to make it stable
  const callbackRef = useRef(onPaginationChange);
  useEffect(() => {
    callbackRef.current = onPaginationChange;
  }, [onPaginationChange]);

  // Prevent duplicate calls
  const isUpdatingRef = useRef(false);

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

  // Persist state whenever it changes
  useEffect(() => {
    const state: PersistedGridState = {
      pageNumber,
      pageSize,
      sortBy,
      isSortDescending
    };
    console.log("PayBenReport: Persisting grid state:", state);
    savePersistedState(state);
  }, [pageNumber, pageSize, sortBy, isSortDescending, savePersistedState]);

  const handlePaginationChange = useCallback(
    (newPageNumber: number, newPageSize: number) => {
      // Prevent re-entrant calls
      if (isUpdatingRef.current) {
        return;
      }

      isUpdatingRef.current = true;
      setPageNumber(newPageNumber);
      setPageSize(newPageSize);

      if (typeof callbackRef.current === "function") {
        // Use setTimeout to defer callback and prevent immediate re-entry
        setTimeout(() => {
          callbackRef.current!(newPageNumber, newPageSize, { sortBy, isSortDescending });
          isUpdatingRef.current = false;
        }, 0);
      } else {
        isUpdatingRef.current = false;
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
    
    // Clear persisted state when resetting
    try {
      sessionStorage.removeItem(STORAGE_KEY);
    } catch (error) {
      console.warn("Failed to clear persisted grid state:", error);
    }
  }, [initialPageSize, initialSortBy, initialSortDescending]);

  const clearPersistedState = useCallback(() => {
    try {
      sessionStorage.removeItem(STORAGE_KEY);
    } catch (error) {
      console.warn("Failed to clear persisted grid state:", error);
    }
  }, []);

  return {
    ...pagination,
    handlePaginationChange,
    handleSortChange,
    resetPagination,
    clearPersistedState
  };
};