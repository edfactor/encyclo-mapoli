import { useCallback, useEffect, useReducer, useRef } from "react";
import { useDispatch, useSelector } from "react-redux";
import { GRID_KEYS } from "../../../../constants";
import { SortParams, useGridPagination } from "../../../../hooks/useGridPagination";
import { useLazyGetEmployeeWagesForYearQuery } from "../../../../reduxstore/api/YearsEndApi";
import { setEmployeeWagesForYearQueryParams } from "../../../../reduxstore/slices/yearsEndSlice";
import { RootState } from "../../../../reduxstore/store";
import { initialState, selectHasResults, selectShowData, ytdWagesReducer } from "./useYTDWagesReducer";

export interface YTDWagesSearchParams {
  profitYear: number;
  useFrozenData?: boolean;
  archive?: boolean;
}

export interface UseYTDWagesOptions {
  defaultUseFrozenData?: boolean;
  profitYear?: number; // Optional - won't search until this is available
}

const useYTDWages = (options: UseYTDWagesOptions) => {
  const { defaultUseFrozenData = true, profitYear } = options;
  const [state, dispatch] = useReducer(ytdWagesReducer, initialState);
  const reduxDispatch = useDispatch();

  const [triggerSearch, { isFetching: isSearching }] = useLazyGetEmployeeWagesForYearQuery();
  const hasToken = !!useSelector((state: RootState) => state.security.token);

  const handlePaginationChange = useCallback(
    (pageNumber: number, pageSize: number, sortParams: SortParams) => {
      if (profitYear && hasToken) {
        try {
          const request = {
            profitYear: profitYear,
            useFrozenData: state.search.useFrozenData !== undefined ? state.search.useFrozenData : defaultUseFrozenData,
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
              reduxDispatch(setEmployeeWagesForYearQueryParams(profitYear));
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
    [profitYear, hasToken, triggerSearch, reduxDispatch, state.search.useFrozenData, defaultUseFrozenData]
  );

  const pagination = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "storeNumber",
    initialSortDescending: false,
    persistenceKey: GRID_KEYS.YTD_WAGES,
    onPaginationChange: handlePaginationChange
  });

  const executeSearch = useCallback(
    async (searchParams: YTDWagesSearchParams, _source?: string) => {
      if (!hasToken) return;

      dispatch({
        type: "SEARCH_START",
        payload: { profitYear: searchParams.profitYear, useFrozenData: searchParams.useFrozenData }
      });

      try {
        const request = {
          profitYear: searchParams.profitYear,
          useFrozenData: searchParams.useFrozenData !== undefined ? searchParams.useFrozenData : defaultUseFrozenData,
          archive: searchParams.archive,
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
    [hasToken, pagination, triggerSearch, reduxDispatch, defaultUseFrozenData]
  );

  const hasInitiallySearched = useRef(false);

  useEffect(() => {
    if (profitYear && !state.data && hasToken && !state.search.isLoading && !hasInitiallySearched.current) {
      console.log("[useYTDWages] Running initial search with profitYear:", profitYear);
      hasInitiallySearched.current = true;
      executeSearch({ profitYear: profitYear, useFrozenData: defaultUseFrozenData }, "auto-initial");
    }
    // Note: executeSearch is intentionally excluded from dependencies to prevent infinite loops
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [profitYear, state.data, hasToken, state.search.isLoading]);

  // Cleanup function to prevent AbortController errors on unmount
  useEffect(() => {
    return () => {
      // Reset the search flag on unmount to allow re-initialization
      hasInitiallySearched.current = false;
    };
  }, []);

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
