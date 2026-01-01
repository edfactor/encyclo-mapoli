import { useCallback, useEffect, useReducer, useRef } from "react";
import { useSelector } from "react-redux";
import { GRID_KEYS } from "../../../../constants";
import { SortParams, useGridPagination } from "../../../../hooks/useGridPagination";
import { usePayBenReportMutation } from "../../../../reduxstore/api/YearsEndApi";
import { RootState } from "../../../../reduxstore/store";
import { PayBenReportRequest } from "../../../../types";
import { initialState, payBenReportReducer, selectHasResults, selectShowData } from "./usePayBenReportReducer";

const usePayBenReport = () => {
  const [state, dispatch] = useReducer(payBenReportReducer, initialState);

  const [triggerReport, { isLoading: isSearching }] = usePayBenReportMutation();
  const hasToken = !!useSelector((state: RootState) => state.security.token);

  const handlePaginationChange = useCallback(
    (pageNumber: number, pageSize: number, sortParams: SortParams) => {
      if (hasToken) {
        try {
          const request: PayBenReportRequest = {
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
    [hasToken, triggerReport]
  );

  const pagination = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "ssn",
    initialSortDescending: true,
    onPaginationChange: handlePaginationChange,
    persistenceKey: GRID_KEYS.PAY_BEN_REPORT
  });

  const executeSearch = useCallback(async () => {
    if (!hasToken) return;

    dispatch({ type: "SEARCH_START" });

    try {
      const request: PayBenReportRequest = {
        skip: pagination.pageNumber * pagination.pageSize,
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
  }, [hasToken, pagination, triggerReport]);

  const hasInitiallySearched = useRef(false);

  useEffect(() => {
    if (!state.data && hasToken && !state.search.isLoading && !hasInitiallySearched.current) {
      hasInitiallySearched.current = true;
      executeSearch();
    }
  }, [state.data, hasToken, state.search.isLoading, executeSearch]);

  return {
    searchResults: state.data,
    isSearching: isSearching || state.search.isLoading,
    pagination,
    showData: selectShowData(state),
    hasResults: selectHasResults(state),

    executeSearch
  };
};

export default usePayBenReport;
