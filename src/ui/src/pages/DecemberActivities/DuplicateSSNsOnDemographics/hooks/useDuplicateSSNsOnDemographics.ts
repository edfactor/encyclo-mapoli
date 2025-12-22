import { useCallback, useEffect, useReducer, useRef } from "react";
import { useSelector } from "react-redux";
import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import { GRID_KEYS } from "../../../../constants";
import { SortParams, useGridPagination } from "../../../../hooks/useGridPagination";
import { useLazyGetDuplicateSSNsQuery } from "../../../../reduxstore/api/YearsEndApi";
import { RootState } from "../../../../reduxstore/store";
import {
  duplicateSSNsOnDemographicsReducer,
  initialState,
  selectHasResults,
  selectShowData
} from "./useDuplicateSSNsOnDemographicsReducer";

export interface DuplicateSSNsOnDemographicsSearchParams {
  profitYear: number;
}

const useDuplicateSSNsOnDemographics = () => {
  const [state, dispatch] = useReducer(duplicateSSNsOnDemographicsReducer, initialState);

  const [triggerSearch, { isFetching: isSearching }] = useLazyGetDuplicateSSNsQuery();
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const decemberFlowProfitYear = useDecemberFlowProfitYear();

  const handlePaginationChange = useCallback(
    (pageNumber: number, pageSize: number, sortParams: SortParams) => {
      if (decemberFlowProfitYear && hasToken) {
        try {
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
    initialSortBy: "name",
    initialSortDescending: false,
    persistenceKey: GRID_KEYS.DUPLICATE_SSNS,
    onPaginationChange: handlePaginationChange
  });

  const executeSearch = useCallback(
    async (searchParams: DuplicateSSNsOnDemographicsSearchParams, _source = "manual") => {
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
    // eslint-disable-next-line react-hooks/exhaustive-deps
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

export default useDuplicateSSNsOnDemographics;
