import { useCallback, useEffect, useReducer, useRef, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetUpdateSummaryQuery, useUpdateEnrollmentMutation } from "reduxstore/api/YearsEndApi";
import { setMessage } from "reduxstore/slices/messageSlice";
import { RootState } from "reduxstore/store";
import { UpdateSummaryResponse } from "reduxstore/types";
import useFiscalCloseProfitYear from "../../../../hooks/useFiscalCloseProfitYear";
import { SortParams, useGridPagination } from "../../../../hooks/useGridPagination";
import { ServiceErrorResponse } from "../../../../types/errors/errors";

interface NavigationItem {
  id: number;
  statusName?: string;
  items?: NavigationItem[];
  [key: string]: unknown;
}

interface PayMasterUpdateState {
  search: {
    isSearching: boolean;
    searchCompleted: boolean;
    profitYear: number | null;
    error: string | null;
  };
  summary: {
    data: UpdateSummaryResponse | null;
    isLoading: boolean;
    error: string | null;
  };
  currentStatus: string | null;
}

type PayMasterUpdateAction =
  | { type: "SEARCH_START"; payload: { profitYear: number } }
  | { type: "SEARCH_SUCCESS"; payload: { data: UpdateSummaryResponse } }
  | { type: "SEARCH_FAILURE"; payload: { error: string } }
  | { type: "SEARCH_RESET" }
  | { type: "SUMMARY_FETCH_START" }
  | { type: "SUMMARY_FETCH_SUCCESS"; payload: { data: UpdateSummaryResponse } }
  | { type: "SUMMARY_FETCH_FAILURE"; payload: { error: string } }
  | { type: "SET_CURRENT_STATUS"; payload: { status: string | null } };

const initialState: PayMasterUpdateState = {
  search: {
    isSearching: false,
    searchCompleted: false,
    profitYear: null,
    error: null
  },
  summary: {
    data: null,
    isLoading: false,
    error: null
  },
  currentStatus: null
};

const payMasterUpdateReducer = (state: PayMasterUpdateState, action: PayMasterUpdateAction): PayMasterUpdateState => {
  switch (action.type) {
    case "SEARCH_START":
      return {
        ...state,
        search: {
          ...state.search,
          isSearching: true,
          searchCompleted: false,
          profitYear: action.payload.profitYear,
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
        summary: {
          ...state.summary,
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
        summary: {
          ...state.summary,
          isLoading: false,
          error: action.payload.error
        }
      };

    case "SEARCH_RESET":
      return {
        ...initialState,
        currentStatus: state.currentStatus // Preserve status
      };

    case "SUMMARY_FETCH_START":
      return {
        ...state,
        summary: {
          ...state.summary,
          isLoading: true,
          error: null
        }
      };

    case "SUMMARY_FETCH_SUCCESS":
      return {
        ...state,
        summary: {
          ...state.summary,
          data: action.payload.data,
          isLoading: false,
          error: null
        }
      };

    case "SUMMARY_FETCH_FAILURE":
      return {
        ...state,
        summary: {
          ...state.summary,
          isLoading: false,
          error: action.payload.error
        }
      };

    case "SET_CURRENT_STATUS":
      return {
        ...state,
        currentStatus: action.payload.status
      };

    default:
      return state;
  }
};

export const usePayMasterUpdate = () => {
  const [state, dispatch] = useReducer(payMasterUpdateReducer, initialState);
  const reduxDispatch = useDispatch();
  const [triggerGetSummary] = useLazyGetUpdateSummaryQuery();
  const [updateEnrollment, { isLoading: isUpdating }] = useUpdateEnrollmentMutation();
  const fiscalCloseProfitYear = useFiscalCloseProfitYear();
  const navigationList = useSelector((state: RootState) => state.navigation.navigationData);

  // Modal state
  const [isModalOpen, setIsModalOpen] = useState(false);

  // Use refs to prevent infinite loops in useGridPagination
  const profitYearRef = useRef(state.search.profitYear);

  // Keep refs updated
  profitYearRef.current = state.search.profitYear;

  // Initialize current status from navigation state
  useEffect(() => {
    const currentNavigationId = parseInt(localStorage.getItem("navigationId") ?? "");

    const getNavigationObjectBasedOnId = (
      navigationArray: NavigationItem[] | undefined,
      id: number | undefined
    ): NavigationItem | undefined => {
      if (navigationArray) {
        for (const item of navigationArray) {
          if (item.id === id) {
            return item;
          }
          if (item.items && item.items.length > 0) {
            const found = getNavigationObjectBasedOnId(item.items, id);
            if (found) {
              return found;
            }
          }
        }
      }
      return undefined;
    };

    const obj = getNavigationObjectBasedOnId(navigationList?.navigation, currentNavigationId ?? undefined);
    if (obj) {
      dispatch({ type: "SET_CURRENT_STATUS", payload: { status: obj.statusName || null } });
    }
  }, [navigationList]);

  const handleSummaryPaginationChange = useCallback(
    async (pageNumber: number, pageSize: number, sortParams: SortParams) => {
      const currentProfitYear = profitYearRef.current ?? fiscalCloseProfitYear;

      if (!currentProfitYear) return;

      dispatch({ type: "SUMMARY_FETCH_START" });

      try {
        const data = await triggerGetSummary({
          profitYear: currentProfitYear,
          pagination: {
            skip: pageNumber * pageSize,
            take: pageSize,
            sortBy: sortParams.sortBy,
            isSortDescending: sortParams.isSortDescending
          }
        }).unwrap();

        dispatch({ type: "SUMMARY_FETCH_SUCCESS", payload: { data } });
      } catch (error) {
        const serviceError = error as ServiceErrorResponse;
        const errorMsg = serviceError?.data?.detail || "Failed to fetch pay master update summary";
        dispatch({ type: "SUMMARY_FETCH_FAILURE", payload: { error: errorMsg } });
      }
    },
    [triggerGetSummary, fiscalCloseProfitYear]
  );

  const gridPagination = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "name",
    initialSortDescending: false,
    onPaginationChange: handleSummaryPaginationChange
  });

  // Store pagination ref to avoid dependency issues
  const paginationRef = useRef(gridPagination);
  paginationRef.current = gridPagination;

  const executeSearch = useCallback(
    async (profitYear: number, archive: boolean = false) => {
      try {
        dispatch({ type: "SEARCH_START", payload: { profitYear } });

        const data = await triggerGetSummary({
          profitYear,
          pagination: {
            skip: 0,
            take: 25,
            sortBy: "name",
            isSortDescending: false
          },
          archive
        }).unwrap();

        dispatch({ type: "SEARCH_SUCCESS", payload: { data } });

        // Reset pagination to first page on new search
        paginationRef.current.resetPagination();

        return true;
      } catch (error) {
        console.error("Pay master update search failed:", error);
        const serviceError = error as ServiceErrorResponse;
        const errorMsg = serviceError?.data?.detail || "Failed to search pay master update";
        dispatch({ type: "SEARCH_FAILURE", payload: { error: errorMsg } });

        return false;
      }
    },
    [triggerGetSummary]
  );

  const resetSearch = useCallback(() => {
    dispatch({ type: "SEARCH_RESET" });
    paginationRef.current.resetPagination();
  }, []);

  const handleStatusChange = useCallback(
    (newStatus: string, statusName?: string) => {
      // Only trigger archive when status is changing TO "Complete" (not already "Complete")
      if (statusName === "Complete" && state.currentStatus !== "Complete") {
        dispatch({ type: "SET_CURRENT_STATUS", payload: { status: "Complete" } });
        // Trigger getUpdateSummary with archive=true
        executeSearch(fiscalCloseProfitYear, true);
      } else {
        dispatch({ type: "SET_CURRENT_STATUS", payload: { status: statusName || null } });
      }
    },
    [state.currentStatus, fiscalCloseProfitYear, executeSearch]
  );

  const handleUpdate = useCallback(async () => {
    try {
      await updateEnrollment({
        ProfitYearRequest: fiscalCloseProfitYear ?? 0
      }).unwrap();
      reduxDispatch(
        setMessage({
          key: "UpdateEnrollment",
          message: {
            type: "success",
            title: "Enrollment Updated",
            message: "Enrollment has been successfully updated."
          }
        })
      );
    } catch (error: unknown) {
      reduxDispatch(
        setMessage({
          key: "UpdateEnrollment",
          message: {
            type: "error",
            title: "Update Enrollment Failed",
            message: "Failed to update enrollment. Please try again."
          }
        })
      );
      console.error("Update enrollment failed:", error);
    } finally {
      setIsModalOpen(false);
    }
  }, [fiscalCloseProfitYear, updateEnrollment, reduxDispatch]);

  const handleCancel = useCallback(() => {
    setIsModalOpen(false);
  }, []);

  return {
    // State
    isSearching: state.search.isSearching,
    searchCompleted: state.search.searchCompleted,
    searchError: state.search.error,
    summaryData: state.summary.data,
    isLoadingSummary: state.summary.isLoading,
    summaryError: state.summary.error,
    currentStatus: state.currentStatus,

    // Actions
    executeSearch,
    resetSearch,
    handleStatusChange,

    // Update enrollment
    handleUpdate,
    isUpdating,
    isModalOpen,
    setIsModalOpen,
    handleCancel,

    // Pagination
    gridPagination
  };
};

export default usePayMasterUpdate;
