import { useCallback, useEffect, useReducer, useRef } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetEmployeeWagesForYearQuery } from "../../../reduxstore/api/YearsEndApi";
import { setEmployeeWagesForYearQueryParams } from "../../../reduxstore/slices/yearsEndSlice";
import { RootState } from "../../../reduxstore/store";
import { ISortParams } from "smart-ui-library";
import useFiscalCloseProfitYear from "../../../hooks/useFiscalCloseProfitYear";
import {
  initialState,
  ytdWagesReducer,
  selectShowData,
  selectHasResults
} from "./useYTDWagesReducer";

export interface YTDWagesSearchParams {
  profitYear: number;
}

const useYTDWages = () => {
  const [state, dispatch] = useReducer(ytdWagesReducer, initialState);
  const reduxDispatch = useDispatch();

  const [triggerSearch, { isFetching: isSearching }] = useLazyGetEmployeeWagesForYearQuery();
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const fiscalCloseProfitYear = useFiscalCloseProfitYear();

  const executeSearch = useCallback(
    async (searchParams: YTDWagesSearchParams, source = "manual") => {
      if (!hasToken) return;

      dispatch({ type: "SEARCH_START", payload: { profitYear: searchParams.profitYear } });

      try {
        const request = {
          profitYear: searchParams.profitYear,
          pagination: {
            skip: state.pagination.pageNumber * state.pagination.pageSize,
            take: state.pagination.pageSize,
            sortBy: state.pagination.sortParams.sortBy,
            isSortDescending: state.pagination.sortParams.isSortDescending
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
    [hasToken, state.pagination, triggerSearch, reduxDispatch]
  );

  const handlePaginationChange = useCallback(
    async (pageNumber: number, pageSize: number, sortParams: ISortParams) => {
      // Always update pagination state
      dispatch({
        type: "SET_PAGINATION",
        payload: { pageNumber, pageSize, sortParams }
      });

      // Only make API call if this actually changes the effective pagination
      const currentEffectivePagination = {
        skip: state.pagination.pageNumber * state.pagination.pageSize,
        take: state.pagination.pageSize,
        sortBy: state.pagination.sortParams.sortBy,
        isSortDescending: state.pagination.sortParams.isSortDescending
      };

      const newEffectivePagination = {
        skip: pageNumber * pageSize,
        take: pageSize,
        sortBy: sortParams.sortBy,
        isSortDescending: sortParams.isSortDescending
      };

      const paginationChanged = JSON.stringify(currentEffectivePagination) !== JSON.stringify(newEffectivePagination);

      if (!paginationChanged) {
        return;
      }

      // Re-execute search with new pagination using fiscalCloseProfitYear
      if (fiscalCloseProfitYear && hasToken) {
        try {
          const request = {
            profitYear: fiscalCloseProfitYear,
            pagination: newEffectivePagination,
            acceptHeader: "application/json"
          };

          const result = await triggerSearch(request, false).unwrap();
          dispatch({ type: "SEARCH_SUCCESS", payload: result });
        } catch (error) {
          console.error("Pagination search failed:", error);
          dispatch({ type: "SEARCH_ERROR" });
        }
      }
    },
    [fiscalCloseProfitYear, hasToken, triggerSearch, state.pagination]
  );

  const handleSortChange = useCallback(
    (sortParams: ISortParams) => {
      handlePaginationChange(state.pagination.pageNumber, state.pagination.pageSize, sortParams);
    },
    [handlePaginationChange, state.pagination.pageNumber, state.pagination.pageSize]
  );

  const resetSearch = useCallback(() => {
    dispatch({ type: "RESET_ALL" });
  }, []);


  // Auto-execute search when fiscal year is available and no data has been loaded yet
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
    // State
    searchResults: state.data,
    isSearching: isSearching || state.search.isLoading,
    pagination: state.pagination,
    showData: selectShowData(state),
    hasResults: selectHasResults(state),
    searchParams: state.search.profitYear ? { profitYear: state.search.profitYear } : null,

    // Actions
    executeSearch,
    handlePaginationChange,
    handleSortChange,
    resetSearch
  };
};

export default useYTDWages;