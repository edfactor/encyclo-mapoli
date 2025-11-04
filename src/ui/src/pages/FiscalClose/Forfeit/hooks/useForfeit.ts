import { useCallback, useEffect, useReducer, useRef } from "react";
import { useSelector } from "react-redux";
import useFiscalCloseProfitYear from "../../../../hooks/useFiscalCloseProfitYear";
import { SortParams, useGridPagination } from "../../../../hooks/useGridPagination";
import { useLazyGetForfeituresAndPointsQuery } from "../../../../reduxstore/api/YearsEndApi";
import { RootState } from "../../../../reduxstore/store";
import { forfeitReducer, initialState, selectHasResults, selectShowData } from "./useForfeitReducer";

export interface ForfeitSearchParams {
  profitYear: number;
}

const useForfeit = () => {
  const [state, dispatch] = useReducer(forfeitReducer, initialState);

  const [triggerSearch, { isFetching: isSearching }] = useLazyGetForfeituresAndPointsQuery();
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const fiscalCloseProfitYear = useFiscalCloseProfitYear();

  const handlePaginationChange = useCallback(
    (pageNumber: number, pageSize: number, sortParams: SortParams) => {
      if (fiscalCloseProfitYear && hasToken && state.search.hasSearched) {
        try {
          const request = {
            profitYear: fiscalCloseProfitYear,
            useFrozenData: true,
            archive: false, // Always use archive=false for normal pagination
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
    [fiscalCloseProfitYear, hasToken, state.search.hasSearched, triggerSearch]
  );

  const pagination = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "badgeNumber",
    initialSortDescending: false,
    onPaginationChange: handlePaginationChange
  });

  const executeSearch = useCallback(
    async (searchParams: ForfeitSearchParams) => {
      if (!hasToken) return;

      dispatch({ type: "SEARCH_START", payload: { profitYear: searchParams.profitYear } });
      dispatch({ type: "RESET_PAGINATION" });

      try {
        const request = {
          profitYear: searchParams.profitYear,
          useFrozenData: true,
          archive: false, // Always use archive=false for search button
          pagination: {
            skip: 0,
            take: 25,
            sortBy: "badgeNumber",
            isSortDescending: true
          }
        };

        const result = await triggerSearch(request, false).unwrap();
        dispatch({ type: "SEARCH_SUCCESS", payload: result });
      } catch (error) {
        console.error("Search failed:", error);
        dispatch({ type: "SEARCH_ERROR" });
      }
    },
    [hasToken, triggerSearch]
  );

  const handleStatusChange = useCallback((_newStatus: string, statusName?: string) => {
    // When status is set to "Complete", trigger archiving
    if (statusName === "Complete") {
      dispatch({ type: "TRIGGER_ARCHIVE" });
    }
  }, []);

  const handleReset = useCallback(() => {
    dispatch({ type: "RESET_ALL" });
    pagination.resetPagination();
  }, [pagination]);

  // Handle archive request when shouldArchive flag is set
  useEffect(() => {
    if (state.archive.shouldArchive && state.search.hasSearched && fiscalCloseProfitYear) {
      dispatch({ type: "ARCHIVE_START" });

      const request = {
        profitYear: fiscalCloseProfitYear,
        useFrozenData: true,
        archive: true, // ONLY set to true when Complete status selected
        pagination: {
          skip: pagination.pageNumber * pagination.pageSize,
          take: pagination.pageSize,
          sortBy: pagination.sortParams.sortBy,
          isSortDescending: pagination.sortParams.isSortDescending
        }
      };

      triggerSearch(request, false)
        .unwrap()
        .then(() => {
          dispatch({ type: "ARCHIVE_COMPLETE" });
        })
        .catch((error) => {
          console.error("Archive request failed:", error);
          dispatch({ type: "ARCHIVE_COMPLETE" });
        });
    }
  }, [
    state.archive.shouldArchive,
    state.search.hasSearched,
    fiscalCloseProfitYear,
    pagination.pageNumber,
    pagination.pageSize,
    pagination.sortParams.sortBy,
    pagination.sortParams.isSortDescending,
    triggerSearch
  ]);

  const hasInitiallySearched = useRef(false);

  // Auto-search on mount if profit year is available
  useEffect(() => {
    if (fiscalCloseProfitYear && !state.data && hasToken && !state.search.isLoading && !hasInitiallySearched.current) {
      hasInitiallySearched.current = true;
      executeSearch({ profitYear: fiscalCloseProfitYear });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [fiscalCloseProfitYear, state.data, hasToken, state.search.isLoading]);

  return {
    searchResults: state.data,
    isSearching: isSearching || state.search.isLoading,
    pagination,
    showData: selectShowData(state),
    hasResults: selectHasResults(state),
    searchParams: state.search.profitYear ? { profitYear: state.search.profitYear } : null,
    shouldArchive: state.archive.shouldArchive,

    executeSearch,
    handleStatusChange,
    handleReset
  };
};

export default useForfeit;
