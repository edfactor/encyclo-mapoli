import { useState, useCallback, useMemo } from "react";

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

  // Create stable sortParams object
  const sortParams = useMemo(() => ({
    sortBy,
    isSortDescending
  }), [sortBy, isSortDescending]);

  // Create stable pagination object
  const pagination = useMemo(() => ({
    pageNumber,
    pageSize,
    sortParams
  }), [pageNumber, pageSize, sortParams]);

  const handlePaginationChange = useCallback(
    (newPageNumber: number, newPageSize: number) => {
      setPageNumber(newPageNumber);
      setPageSize(newPageSize);

      if (onPaginationChange) {
        onPaginationChange(newPageNumber, newPageSize, sortParams);
      }
    },
    [onPaginationChange, sortParams]
  );

  const handleSortChange = useCallback(
    (newSortParams: any) => {
      setSortBy(newSortParams.sortBy);
      setIsSortDescending(newSortParams.isSortDescending);

      if (onPaginationChange) {
        onPaginationChange(pageNumber, pageSize, newSortParams);
      }
    },
    [onPaginationChange, pageNumber, pageSize]
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
