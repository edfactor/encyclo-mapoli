import { useCallback, useEffect, useReducer, useRef } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetEmployeeWagesForYearQuery } from "../../../reduxstore/api/YearsEndApi";
import { setEmployeeWagesForYearQueryParams } from "../../../reduxstore/slices/yearsEndSlice";
import { RootState } from "../../../reduxstore/store";
import useFiscalCloseProfitYear from "../../../hooks/useFiscalCloseProfitYear";
import { useGridPagination, SortParams } from "../../../hooks/useGridPagination";
import { initialState, ytdWagesReducer, selectShowData, selectHasResults } from "./useYTDWagesReducer";

export interface YTDWagesSearchParams {
  profitYear: number;
}

const useYTDWages = () => {
  const [state, dispatch] = useReducer(ytdWagesReducer, initialState);
  const reduxDispatch = useDispatch();

  const [triggerSearch, { isFetching: isSearching }] = useLazyGetEmployeeWagesForYearQuery();
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const fiscalCloseProfitYear = useFiscalCloseProfitYear();

  const handlePaginationChange = useCallback(
    (pageNumber: number, pageSize: number, sortParams: SortParams) => {
      if (fiscalCloseProfitYear && hasToken) {
        try {
          const request = {
            profitYear: fiscalCloseProfitYear,
            pagination: {
              skip: pageNumber * pageSize,
              take: pageSize,
              sortBy: sortParams.sortBy,
              isSortDescending: sortParams.isSortDescending
            },
            acceptHeader: "application/json"
          };

          triggerSearch(request, false)
            .unwrap()
            .then((result) => {
              dispatch({ type: "SEARCH_SUCCESS", payload: result });
              reduxDispatch(setEmployeeWagesForYearQueryParams(fiscalCloseProfitYear));
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
    [fiscalCloseProfitYear, hasToken, triggerSearch, reduxDispatch]
  );

  const pagination = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "storeNumber",
    initialSortDescending: false,
    onPaginationChange: handlePaginationChange
  });

  const executeSearch = useCallback(
    async (searchParams: YTDWagesSearchParams, source = "manual") => {
      if (!hasToken) return;

      dispatch({ type: "SEARCH_START", payload: { profitYear: searchParams.profitYear } });

      try {
        const request = {
          profitYear: searchParams.profitYear,
          pagination: {
            skip: pagination.pageNumber * pagination.pageSize,
            take: pagination.pageSize,
            sortBy: pagination.sortParams.sortBy,
            isSortDescending: pagination.sortParams.isSortDescending
          },
          acceptHeader: "application/json"
        };

        const result = await triggerSearch(request, false).unwrap();
        dispatch({ type: "SEARCH_SUCCESS", payload: result });
        reduxDispatch(setEmployeeWagesForYearQueryParams(searchParams.profitYear));
      } catch (error) {
        console.error("Search failed:", error);
        dispatch({ type: "SEARCH_ERROR" });
      }
    },
    [hasToken, pagination, triggerSearch, reduxDispatch]
  );

  const hasInitiallySearched = useRef(false);

  useEffect(() => {
    if (fiscalCloseProfitYear && !state.data && hasToken && !state.search.isLoading && !hasInitiallySearched.current) {
      hasInitiallySearched.current = true;
      executeSearch({ profitYear: fiscalCloseProfitYear }, "auto-initial");
    }
    // Note: executeSearch is intentionally excluded from dependencies to prevent infinite loops
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [fiscalCloseProfitYear, state.data, hasToken, state.search.isLoading]);

  return {
    searchResults: state.data,
    isSearching: isSearching || state.search.isLoading,
    pagination,
    showData: selectShowData(state),
    hasResults: selectHasResults(state),
    searchParams: state.search.profitYear ? { profitYear: state.search.profitYear } : null,

    executeSearch
  };
};

export default useYTDWages;
