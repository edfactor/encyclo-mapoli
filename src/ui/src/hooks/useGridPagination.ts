import { useCallback, useEffect, useMemo, useRef, useState } from "react";

export interface GridPaginationState {
  pageNumber: number;
  pageSize: number;
  sortParams: {
    sortBy: string;
    isSortDescending: boolean;
  };
}

export interface GridPaginationActions {
  handlePaginationChange: (pageNumber: number, pageSize: number) => void;
  handleSortChange: (sortParams: any) => void;
  resetPagination: () => void;
}

export interface UseGridPaginationConfig {
  initialPageSize: number;
  initialSortBy: string;
  initialSortDescending?: boolean;
  onPaginationChange?: (pageNumber: number, pageSize: number, sortParams: any) => void;
}

/*
 * Hook for grid pagination state and handlers
 *
 */
export const useGridPagination = ({
  initialPageSize,
  initialSortBy,
  initialSortDescending = true,
  onPaginationChange
}: UseGridPaginationConfig): GridPaginationState & GridPaginationActions => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(initialPageSize);
  const [sortBy, setSortBy] = useState(initialSortBy);
  const [isSortDescending, setIsSortDescending] = useState(initialSortDescending);

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
    [sortBy, isSortDescending, pageSize]
  );

  const handleSortChange = useCallback(
    (newSortParams: any) => {
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
  }, [initialPageSize, initialSortBy, initialSortDescending]);

  return {
    ...pagination,
    handlePaginationChange,
    handleSortChange,
    resetPagination
  };
};
