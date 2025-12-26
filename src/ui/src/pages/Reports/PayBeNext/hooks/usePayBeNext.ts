import { useCallback, useMemo, useReducer } from "react";
import { useSelector } from "react-redux";
import { GRID_KEYS } from "../../../../constants";
import { SortParams, useGridPagination } from "../../../../hooks/useGridPagination";
import { useLazyAdhocBeneficiariesReportQuery } from "../../../../reduxstore/api/AdhocApi";
import { RootState } from "../../../../reduxstore/store";
import { AdhocBeneficiariesReportRequest, BeneficiaryReportDto } from "../../../../reduxstore/types";
import {
  initialState,
  PayBeNextFormData,
  payBeNextReducer,
  selectHasResults,
  selectResults,
  selectShowData,
  selectTotalEndingBalance,
  selectTotalRecords
} from "./usePayBeNextReducer";

/**
 * Extended row type with expansion state
 */
export interface PayBeNextGridRow extends BeneficiaryReportDto {
  isExpandable: boolean;
  isExpanded: boolean;
  isDetail: boolean;
  parentId?: number;
}

/**
 * Custom hook for PayBeNext component logic
 */
const usePayBeNext = () => {
  const [state, dispatch] = useReducer(payBeNextReducer, initialState);

  const [triggerReport, { isFetching: isSearching }] = useLazyAdhocBeneficiariesReportQuery();
  const hasToken = !!useSelector((rootState: RootState) => rootState.security.token);

  /**
   * Handle pagination changes from useGridPagination
   */
  const handlePaginationChange = useCallback(
    (pageNumber: number, pageSize: number, sortParams: SortParams) => {
      if (hasToken) {
        try {
          const request: AdhocBeneficiariesReportRequest = {
            profitYear: state.formData.profitYear,
            isAlsoEmployee: state.formData.isAlsoEmployee,
            skip: pageNumber * pageSize,
            take: pageSize,
            sortBy: sortParams.sortBy,
            isSortDescending: sortParams.isSortDescending
          };

          triggerReport(request)
            .unwrap()
            .then((result) => {
              dispatch({ type: "SEARCH_SUCCESS", payload: result });
            })
            .catch((error) => {
              console.error("Pagination search failed:", error);
              dispatch({ type: "SEARCH_ERROR" });
            });
        } catch (error) {
          console.error("Pagination search failed:", error);
          dispatch({ type: "SEARCH_ERROR" });
        }
      }
    },
    [hasToken, state.formData.profitYear, state.formData.isAlsoEmployee, triggerReport]
  );

  const pagination = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "psnSuffix",
    initialSortDescending: true,
    onPaginationChange: handlePaginationChange,
    persistenceKey: GRID_KEYS.PAY_BE_NEXT
  });

  /**
   * Execute search with current form values
   */
  const executeSearch = useCallback(
    async (formData?: PayBeNextFormData) => {
      if (!hasToken) return;

      const searchData = formData ?? state.formData;

      dispatch({ type: "SEARCH_START" });

      // Update form data in state if new data provided
      if (formData) {
        dispatch({ type: "SET_FORM_DATA", payload: formData });
      }

      // Reset pagination on new search
      pagination.handlePageNumberChange(0);

      try {
        const request: AdhocBeneficiariesReportRequest = {
          profitYear: searchData.profitYear,
          isAlsoEmployee: searchData.isAlsoEmployee,
          skip: 0,
          take: pagination.pageSize,
          sortBy: pagination.sortParams.sortBy,
          isSortDescending: pagination.sortParams.isSortDescending
        };

        const result = await triggerReport(request).unwrap();
        dispatch({ type: "SEARCH_SUCCESS", payload: result });
      } catch (error) {
        console.error("Search failed:", error);
        dispatch({ type: "SEARCH_ERROR" });
      }
    },
    [hasToken, pagination, state.formData, triggerReport]
  );

  /**
   * Reset search to initial state
   */
  const resetSearch = useCallback(() => {
    dispatch({ type: "RESET_ALL" });
    pagination.handlePageNumberChange(0);
  }, [pagination]);

  /**
   * Toggle row expansion for master-detail view
   */
  const toggleRowExpansion = useCallback((rowKey: string) => {
    dispatch({ type: "TOGGLE_ROW_EXPANSION", payload: rowKey });
  }, []);

  /**
   * Create grid data with expandable rows
   */
  const gridData = useMemo((): PayBeNextGridRow[] => {
    const results = selectResults(state);
    if (!results || results.length === 0) return [];

    const rows: PayBeNextGridRow[] = [];

    for (const row of results) {
      const hasDetails = row.profitDetails && row.profitDetails.length > 0;
      const rowKey = `${row.badgeNumber}-${row.beneficiaryId}`;

      // Add main row
      rows.push({
        ...row,
        isExpandable: hasDetails ?? false,
        isExpanded: hasDetails ? Boolean(state.expandedRows[rowKey]) : false,
        isDetail: false
      });

      // Add detail rows if expanded
      if (hasDetails && state.expandedRows[rowKey]) {
        for (const detail of row.profitDetails ?? []) {
          rows.push({
            ...row,
            ...detail,
            isDetail: true,
            isExpandable: false,
            isExpanded: false,
            parentId: row.badgeNumber * 1000000 + row.beneficiaryId
          } as PayBeNextGridRow);
        }
      }
    }

    return rows;
  }, [state]);

  return {
    // State
    searchResults: state.data,
    formData: state.formData,
    isSearching: isSearching || state.search.isLoading,
    pagination,
    gridData,
    expandedRows: state.expandedRows,

    // Selectors
    showData: selectShowData(state),
    hasResults: selectHasResults(state),
    totalEndingBalance: selectTotalEndingBalance(state),
    totalRecords: selectTotalRecords(state),

    // Actions
    executeSearch,
    resetSearch,
    toggleRowExpansion
  };
};

export default usePayBeNext;
