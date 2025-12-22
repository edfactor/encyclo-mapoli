import { useCallback, useReducer, useRef } from "react";
import { useLazyGetRecentlyTerminatedReportQuery } from "reduxstore/api/AdhocApi";
import { RecentlyTerminatedResponse } from "reduxstore/types";
import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import { GRID_KEYS } from "../../../../constants";
import { SortParams, useGridPagination } from "../../../../hooks/useGridPagination";
import { useMissiveAlerts } from "../../../../hooks/useMissiveAlerts";
import { ServiceErrorResponse } from "../../../../types/errors/errors";

interface SearchParams {
  beginningDate: string;
  endingDate: string;
}

interface RecentlyTerminatedState {
  search: {
    isSearching: boolean;
    searchCompleted: boolean;
    searchParams: SearchParams | null;
    error: string | null;
  };
  report: {
    data: RecentlyTerminatedResponse | null;
    isLoading: boolean;
    error: string | null;
  };
}

type RecentlyTerminatedAction =
  | { type: "SEARCH_START"; payload: { params: SearchParams } }
  | { type: "SEARCH_SUCCESS"; payload: { data: RecentlyTerminatedResponse } }
  | { type: "SEARCH_FAILURE"; payload: { error: string } }
  | { type: "SEARCH_RESET" }
  | { type: "REPORT_FETCH_START" }
  | { type: "REPORT_FETCH_SUCCESS"; payload: { data: RecentlyTerminatedResponse } }
  | { type: "REPORT_FETCH_FAILURE"; payload: { error: string } };

const initialState: RecentlyTerminatedState = {
  search: {
    isSearching: false,
    searchCompleted: false,
    searchParams: null,
    error: null
  },
  report: {
    data: null,
    isLoading: false,
    error: null
  }
};

const recentlyTerminatedReducer = (
  state: RecentlyTerminatedState,
  action: RecentlyTerminatedAction
): RecentlyTerminatedState => {
  switch (action.type) {
    case "SEARCH_START":
      return {
        ...state,
        search: {
          ...state.search,
          isSearching: true,
          searchCompleted: false,
          searchParams: action.payload.params,
          error: null
        }
      };

    case "SEARCH_SUCCESS":
      return {
        ...state,
        search: {
          ...state.search,
          isSearching: false,
          searchCompleted: true,
          error: null
        },
        report: {
          ...state.report,
          data: action.payload.data,
          isLoading: false
        }
      };

    case "SEARCH_FAILURE":
      return {
        ...state,
        search: {
          ...state.search,
          isSearching: false,
          searchCompleted: false,
          error: action.payload.error
        },
        report: {
          ...state.report,
          isLoading: false,
          error: action.payload.error
        }
      };

    case "SEARCH_RESET":
      return initialState;

    case "REPORT_FETCH_START":
      return {
        ...state,
        report: {
          ...state.report,
          isLoading: true,
          error: null
        }
      };

    case "REPORT_FETCH_SUCCESS":
      return {
        ...state,
        report: {
          ...state.report,
          data: action.payload.data,
          isLoading: false,
          error: null
        }
      };

    case "REPORT_FETCH_FAILURE":
      return {
        ...state,
        report: {
          ...state.report,
          isLoading: false,
          error: action.payload.error
        }
      };

    default:
      return state;
  }
};

export const useRecentlyTerminated = () => {
  const [state, dispatch] = useReducer(recentlyTerminatedReducer, initialState);
  const [triggerSearch] = useLazyGetRecentlyTerminatedReportQuery();
  const { addAlert, clearAlerts } = useMissiveAlerts();
  const profitYear = useDecemberFlowProfitYear();

  // Use refs to prevent infinite loops in useGridPagination
  const searchParamsRef = useRef(state.search.searchParams);
  const profitYearRef = useRef(profitYear);

  // Keep refs updated
  searchParamsRef.current = state.search.searchParams;
  profitYearRef.current = profitYear;

  const handleReportPaginationChange = useCallback(
    async (pageNumber: number, pageSize: number, sortParams: SortParams) => {
      const currentSearchParams = searchParamsRef.current;
      const currentProfitYear = profitYearRef.current;

      if (!currentSearchParams) return;

      dispatch({ type: "REPORT_FETCH_START" });

      try {
        const data = await triggerSearch({
          profitYear: currentProfitYear || 0,
          beginningDate: currentSearchParams.beginningDate,
          endingDate: currentSearchParams.endingDate,
          pagination: {
            skip: pageNumber * pageSize,
            take: pageSize,
            sortBy: sortParams.sortBy,
            isSortDescending: sortParams.isSortDescending
          }
        }).unwrap();

        dispatch({ type: "REPORT_FETCH_SUCCESS", payload: { data } });
      } catch (error) {
        const serviceError = error as ServiceErrorResponse;
        const errorMsg = serviceError?.data?.detail || "Failed to fetch recently terminated report";
        dispatch({ type: "REPORT_FETCH_FAILURE", payload: { error: errorMsg } });
        addAlert({
          id: 999,
          severity: "Error",
          message: "Report Failed",
          description: errorMsg
        });
      }
    },
    [triggerSearch, addAlert]
  );

  const gridPagination = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "fullName, terminationDate",
    initialSortDescending: false,
    persistenceKey: GRID_KEYS.RECENTLY_TERMINATED,
    onPaginationChange: handleReportPaginationChange
  });

  // Store pagination ref to avoid dependency issues
  const paginationRef = useRef(gridPagination);
  paginationRef.current = gridPagination;

  const executeSearch = useCallback(
    async (beginningDate: string, endingDate: string) => {
      try {
        const params = { beginningDate, endingDate };
        dispatch({ type: "SEARCH_START", payload: { params } });
        clearAlerts();

        const data = await triggerSearch({
          profitYear: profitYear || 0,
          beginningDate,
          endingDate,
          pagination: {
            skip: 0,
            take: 25,
            sortBy: "fullName, terminationDate",
            isSortDescending: false
          }
        }).unwrap();

        dispatch({ type: "SEARCH_SUCCESS", payload: { data } });

        // Reset pagination to first page on new search
        paginationRef.current.resetPagination();

        return true;
      } catch (error) {
        console.error("Recently terminated search failed:", error);
        const serviceError = error as ServiceErrorResponse;
        const errorMsg = serviceError?.data?.detail || "Failed to search recently terminated employees";
        dispatch({ type: "SEARCH_FAILURE", payload: { error: errorMsg } });

        addAlert({
          id: 999,
          severity: "Error",
          message: "Search Failed",
          description: errorMsg
        });

        return false;
      }
    },
    [triggerSearch, profitYear, addAlert, clearAlerts]
  );

  const resetSearch = useCallback(() => {
    dispatch({ type: "SEARCH_RESET" });
    clearAlerts();
    paginationRef.current.resetPagination();
  }, [clearAlerts]);

  return {
    // State
    isSearching: state.search.isSearching,
    searchCompleted: state.search.searchCompleted,
    searchParams: state.search.searchParams,
    searchError: state.search.error,
    reportData: state.report.data,
    isLoadingReport: state.report.isLoading,
    reportError: state.report.error,

    // Actions
    executeSearch,
    resetSearch,

    // Pagination
    gridPagination
  };
};

export default useRecentlyTerminated;
