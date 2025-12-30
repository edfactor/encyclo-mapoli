import { useCallback, useState } from "react";
import { SortParams } from "../../../hooks/useGridPagination";

/**
 * Configuration for useBeneficiarySearch hook
 */
interface UseBeneficiarySearchConfig {
  defaultPageSize?: number;
  defaultSortBy?: string;
}

/**
 * Return type for useBeneficiarySearch hook
 */
export interface UseBeneficiarySearchReturn {
  // State
  pageNumber: number;
  pageSize: number;
  sortParams: SortParams;

  // Actions
  handlePaginationChange: (page: number, size: number) => void;
  handleSortChange: (sortParams: SortParams) => void;
  reset: () => void;
}

/**
 * useBeneficiarySearch - Encapsulates pagination and sort state for member search results
 *
 * This hook manages the pagination state for the member results grid.
 * It does NOT manage search execution - that remains in the component.
 * This allows the component to control when searches happen while the hook
 * manages pagination state cleanly.
 *
 * @param config - Optional configuration with default page size and sort field
 * @returns Pagination state and handlers
 *
 * @example
 * const search = useBeneficiarySearch({ defaultPageSize: 10 });
 *
 * // In component:
 * const handleSearch = (request) => {
 *   search.reset(); // Clear pagination on new search
 *   executeSearch(request);
 * };
 */
export const useBeneficiarySearch = (config?: UseBeneficiarySearchConfig): UseBeneficiarySearchReturn => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(config?.defaultPageSize ?? 10);
  const [sortParams, setSortParams] = useState<SortParams>({
    sortBy: config?.defaultSortBy ?? "fullName",
    isSortDescending: false
  });

  const handlePaginationChange = useCallback((page: number, size: number) => {
    setPageNumber(page);
    setPageSize(size);
  }, []);

  const handleSortChange = useCallback((newSortParams: SortParams) => {
    setSortParams(newSortParams);
  }, []);

  const reset = useCallback(() => {
    setPageNumber(0);
    setPageSize(config?.defaultPageSize ?? 10);
    setSortParams({
      sortBy: config?.defaultSortBy ?? "fullName",
      isSortDescending: false
    });
  }, [config?.defaultPageSize, config?.defaultSortBy]);

  return {
    pageNumber,
    pageSize,
    sortParams,
    handlePaginationChange,
    handleSortChange,
    reset
  };
};
