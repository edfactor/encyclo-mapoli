import { useCallback, useEffect, useReducer, useRef } from "react";
import { useSelector } from "react-redux";
import { GRID_KEYS } from "../../../../constants";
import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import { SortParams, useGridPagination } from "../../../../hooks/useGridPagination";
import { useLazyGetDemographicBadgesNotInPayprofitQuery } from "../../../../reduxstore/api/YearsEndApi";
import { RootState } from "../../../../reduxstore/store";
import {
  demographicBadgesNotInPayprofitReducer,
  initialState,
  selectHasResults,
  selectShowData
} from "./useDemographicBadgesNotInPayprofitReducer";

export interface DemographicBadgesNotInPayprofitSearchParams {
  profitYear: number;
}

const useDemographicBadgesNotInPayprofit = () => {
  const [state, dispatch] = useReducer(demographicBadgesNotInPayprofitReducer, initialState);

  const [triggerSearch, { isFetching: isSearching }] = useLazyGetDemographicBadgesNotInPayprofitQuery();
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const decemberFlowProfitYear = useDecemberFlowProfitYear();

  const handlePaginationChange = useCallback(
    (pageNumber: number, pageSize: number, sortParams: SortParams) => {
      if (decemberFlowProfitYear && hasToken) {
        try {
          // Handle empty sortBy case - set default
          if (sortParams.sortBy === "") {
            sortParams.sortBy = "badgeNumber";
            sortParams.isSortDescending = true;
          }

          const request = {
            pagination: {
              skip: pageNumber * pageSize,
              take: pageSize,
              sortBy: sortParams.sortBy,
              isSortDescending: sortParams.isSortDescending
            },
            profitYear: decemberFlowProfitYear
          };

          triggerSearch(request, false)
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
    [decemberFlowProfitYear, hasToken, triggerSearch]
  );

  const pagination = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "badgeNumber",
    initialSortDescending: true,
    persistenceKey: GRID_KEYS.DEMOGRAPHIC_BADGES,
    onPaginationChange: handlePaginationChange
  });

  const executeSearch = useCallback(
    async (searchParams: DemographicBadgesNotInPayprofitSearchParams, _source = "manual") => {
      if (!hasToken) return;

      dispatch({ type: "SEARCH_START", payload: { profitYear: searchParams.profitYear } });

      try {
        const request = {
          pagination: {
            skip: pagination.pageNumber * pagination.pageSize,
            take: pagination.pageSize,
            sortBy: pagination.sortParams.sortBy,
            isSortDescending: pagination.sortParams.isSortDescending
          },
          profitYear: searchParams.profitYear
        };

        const result = await triggerSearch(request, false).unwrap();
        dispatch({ type: "SEARCH_SUCCESS", payload: result });
      } catch (error) {
        console.error("Search failed:", error);
        dispatch({ type: "SEARCH_ERROR" });
      }
    },
    [hasToken, pagination, triggerSearch]
  );

  const hasInitiallySearched = useRef(false);

  useEffect(() => {
    if (decemberFlowProfitYear && !state.data && hasToken && !state.search.isLoading && !hasInitiallySearched.current) {
      hasInitiallySearched.current = true;
      executeSearch({ profitYear: decemberFlowProfitYear }, "auto-initial");
    }
  }, [decemberFlowProfitYear, state.data, hasToken, state.search.isLoading, executeSearch]);

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

export default useDemographicBadgesNotInPayprofit;
