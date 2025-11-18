import { useCallback, useEffect, useReducer, useRef } from "react";
import { useSelector } from "react-redux";
import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import { SortParams, useGridPagination } from "../../../../hooks/useGridPagination";
import { useLazyGetDuplicateNamesAndBirthdaysQuery } from "../../../../reduxstore/api/YearsEndApi";
import { RootState } from "../../../../reduxstore/store";
import {
    duplicateNamesAndBirthdaysReducer,
    initialState,
    selectHasResults,
    selectShowData
} from "./useDuplicateNamesAndBirthdaysReducer";

export interface DuplicateNamesAndBirthdaysSearchParams {
  profitYear: number;
  includeFictionalSsnPairs?: boolean;
}

const useDuplicateNamesAndBirthdays = (includeFictionalSsnPairs: boolean = false) => {
  const [state, dispatch] = useReducer(duplicateNamesAndBirthdaysReducer, initialState);

  const [triggerSearch, { isFetching: isSearching }] = useLazyGetDuplicateNamesAndBirthdaysQuery();
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const decemberFlowProfitYear = useDecemberFlowProfitYear();

  const handlePaginationChange = useCallback(
    (pageNumber: number, pageSize: number, sortParams: SortParams) => {
      if (decemberFlowProfitYear && hasToken) {
        try {
          const request = {
            profitYear: decemberFlowProfitYear,
            includeFictionalSsnPairs,
            pagination: {
              skip: pageNumber * pageSize,
              take: pageSize,
              sortBy: sortParams.sortBy,
              isSortDescending: sortParams.isSortDescending,
              profitYear: decemberFlowProfitYear
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
    [decemberFlowProfitYear, hasToken, triggerSearch, includeFictionalSsnPairs]
  );

  const pagination = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "name",
    initialSortDescending: false,
    onPaginationChange: handlePaginationChange
  });

  const executeSearch = useCallback(
    async (searchParams: DuplicateNamesAndBirthdaysSearchParams, _source = "manual") => {
      if (!hasToken) return;

      dispatch({ type: "SEARCH_START", payload: { profitYear: searchParams.profitYear } });

      try {
        const request = {
          profitYear: searchParams.profitYear,
          includeFictionalSsnPairs: searchParams.includeFictionalSsnPairs ?? includeFictionalSsnPairs,
          pagination: {
            skip: pagination.pageNumber * pagination.pageSize,
            take: pagination.pageSize,
            sortBy: pagination.sortParams.sortBy,
            isSortDescending: pagination.sortParams.isSortDescending,
            profitYear: searchParams.profitYear
          }
        };

        const result = await triggerSearch(request, false).unwrap();
        dispatch({ type: "SEARCH_SUCCESS", payload: result });
      } catch (error) {
        console.error("Search failed:", error);
        dispatch({ type: "SEARCH_ERROR" });
      }
    },
    [hasToken, pagination, triggerSearch, includeFictionalSsnPairs]
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

export default useDuplicateNamesAndBirthdays;
