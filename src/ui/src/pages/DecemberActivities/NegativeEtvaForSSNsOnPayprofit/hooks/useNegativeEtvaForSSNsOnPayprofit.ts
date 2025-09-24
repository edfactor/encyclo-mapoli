import { useCallback, useEffect, useReducer, useRef } from "react";
import { useSelector } from "react-redux";
import { useLazyGetNegativeEVTASSNQuery } from "../../../../reduxstore/api/YearsEndApi";
import { RootState } from "../../../../reduxstore/store";
import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import { useGridPagination } from "../../../../hooks/useGridPagination";
import {
  initialState,
  negativeEtvaForSSNsOnPayprofitReducer,
  selectShowData,
  selectHasResults
} from "./useNegativeEtvaForSSNsOnPayprofitReducer";

export interface NegativeEtvaForSSNsOnPayprofitSearchParams {
  profitYear: number;
}

const useNegativeEtvaForSSNsOnPayprofit = () => {
  const [state, dispatch] = useReducer(negativeEtvaForSSNsOnPayprofitReducer, initialState);

  const [triggerSearch, { isFetching: isSearching }] = useLazyGetNegativeEVTASSNQuery();
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const decemberFlowProfitYear = useDecemberFlowProfitYear();

  const handlePaginationChange = useCallback(
    (pageNumber: number, pageSize: number, sortParams: any) => {
      if (decemberFlowProfitYear && hasToken) {
        try {
          const request = {
            profitYear: decemberFlowProfitYear,
            pagination: {
              skip: pageNumber * pageSize,
              take: pageSize,
              sortBy: sortParams.sortBy,
              isSortDescending: sortParams.isSortDescending
            }
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
    initialSortDescending: false,
    onPaginationChange: handlePaginationChange
  });

  const executeSearch = useCallback(
    async (searchParams: NegativeEtvaForSSNsOnPayprofitSearchParams, source = "manual") => {
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
          }
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
  }, [decemberFlowProfitYear, state.data, hasToken, state.search.isLoading]);

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

export default useNegativeEtvaForSSNsOnPayprofit;