import { useCallback, useEffect, useReducer, useRef } from "react";
import { useSelector } from "react-redux";
import { useLazyPayBenReportQuery } from "../../../../reduxstore/api/YearsEndApi";
import { RootState } from "../../../../reduxstore/store";
import { PayBenReportRequest } from "../../../../types";
import { useGridPagination } from "../../../../hooks/useGridPagination";
import { initialState, payBenReportReducer, selectShowData, selectHasResults } from "./usePayBenReportReducer";

const usePayBenReport = () => {
  const [state, dispatch] = useReducer(payBenReportReducer, initialState);

  const [triggerReport, { isFetching: isSearching }] = useLazyPayBenReportQuery();
  const hasToken = !!useSelector((state: RootState) => state.security.token);

  const handlePaginationChange = useCallback(
    (pageNumber: number, pageSize: number, sortParams: any) => {
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
    onPaginationChange: handlePaginationChange
  });

  const executeSearch = useCallback(
    async (source = "manual") => {
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
    },
    [hasToken, pagination, triggerReport]
  );

  const hasInitiallySearched = useRef(false);

  useEffect(() => {
    if (!state.data && hasToken && !state.search.isLoading && !hasInitiallySearched.current) {
      hasInitiallySearched.current = true;
      executeSearch("auto-initial");
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
