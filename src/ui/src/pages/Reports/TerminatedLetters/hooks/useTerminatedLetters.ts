import { useCallback, useReducer, useRef, useState } from "react";
import {
  useLazyGetTerminatedLettersDownloadQuery,
  useLazyGetTerminatedLettersReportQuery
} from "reduxstore/api/AdhocApi";
import { TerminatedLettersDetail, TerminatedLettersResponse } from "reduxstore/types";
import { GRID_KEYS } from "../../../../constants";
import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import { SortParams, useGridPagination } from "../../../../hooks/useGridPagination";
import { useMissiveAlerts } from "../../../../hooks/useMissiveAlerts";
import { ServiceErrorResponse } from "../../../../types/errors/errors";

interface SearchParams {
  beginningDate: string;
  endingDate: string;
}

interface TerminatedLettersState {
  search: {
    isSearching: boolean;
    searchCompleted: boolean;
    searchParams: SearchParams | null;
    error: string | null;
  };
  report: {
    data: TerminatedLettersResponse | null;
    isLoading: boolean;
    error: string | null;
  };
  selectedRows: TerminatedLettersDetail[];
}

type TerminatedLettersAction =
  | { type: "SEARCH_START"; payload: { params: SearchParams } }
  | { type: "SEARCH_SUCCESS"; payload: { data: TerminatedLettersResponse } }
  | { type: "SEARCH_FAILURE"; payload: { error: string } }
  | { type: "SEARCH_RESET" }
  | { type: "REPORT_FETCH_START" }
  | { type: "REPORT_FETCH_SUCCESS"; payload: { data: TerminatedLettersResponse } }
  | { type: "REPORT_FETCH_FAILURE"; payload: { error: string } }
  | { type: "SET_SELECTED_ROWS"; payload: { rows: TerminatedLettersDetail[] } }
  | { type: "CLEAR_SELECTED_ROWS" };

const initialState: TerminatedLettersState = {
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
  },
  selectedRows: []
};

const terminatedLettersReducer = (
  state: TerminatedLettersState,
  action: TerminatedLettersAction
): TerminatedLettersState => {
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
        },
        selectedRows: [] // Clear selection on new search
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

    case "SET_SELECTED_ROWS":
      return {
        ...state,
        selectedRows: action.payload.rows
      };

    case "CLEAR_SELECTED_ROWS":
      return {
        ...state,
        selectedRows: []
      };

    default:
      return state;
  }
};

export const useTerminatedLetters = () => {
  const [state, dispatch] = useReducer(terminatedLettersReducer, initialState);
  const [triggerSearch] = useLazyGetTerminatedLettersReportQuery();
  const [triggerDownload, { isFetching: isDownloading }] = useLazyGetTerminatedLettersDownloadQuery();
  const { addAlert, clearAlerts } = useMissiveAlerts();
  const profitYear = useDecemberFlowProfitYear();

  // Print dialog state
  const [isPrintDialogOpen, setIsPrintDialogOpen] = useState(false);
  const [printContent, setPrintContent] = useState<string>("");
  const [isXerox, setIsXerox] = useState(false);

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
        const errorMsg = serviceError?.data?.detail || "Failed to fetch terminated letters report";
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
    initialPageSize: 50,
    initialSortBy: "fullName",
    initialSortDescending: false,
    persistenceKey: GRID_KEYS.TERMINATED_LETTERS,
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
            take: 50,
            sortBy: "fullName",
            isSortDescending: false
          }
        }).unwrap();

        dispatch({ type: "SEARCH_SUCCESS", payload: { data } });

        // Reset pagination to first page on new search
        paginationRef.current.resetPagination();

        return true;
      } catch (error) {
        console.error("Terminated letters search failed:", error);
        const serviceError = error as ServiceErrorResponse;
        const errorMsg = serviceError?.data?.detail || "Failed to search terminated letters";
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

  const setSelectedRows = useCallback((rows: TerminatedLettersDetail[]) => {
    dispatch({ type: "SET_SELECTED_ROWS", payload: { rows } });
  }, []);

  const clearSelectedRows = useCallback(() => {
    dispatch({ type: "CLEAR_SELECTED_ROWS" });
  }, []);

  const handlePrint = useCallback(async () => {
    if (state.selectedRows.length === 0) return;

    const currentSearchParams = searchParamsRef.current;
    const currentProfitYear = profitYearRef.current;

    if (!currentSearchParams) return;

    const badgeNumbers = state.selectedRows.map((row) => row.badgeNumber);

    try {
      const result = await triggerDownload({
        profitYear: currentProfitYear || 0,
        badgeNumbers: badgeNumbers,
        beginningDate: currentSearchParams.beginningDate,
        endingDate: currentSearchParams.endingDate,
        isXerox: isXerox,
        pagination: {
          skip: 0,
          take: 999999,
          sortBy: "fullName",
          isSortDescending: false
        }
      });

      if (result.data) {
        // Convert blob to string
        const text = await (result.data as Blob).text();
        setPrintContent(text);
        setIsPrintDialogOpen(true);
      }
    } catch (error) {
      console.error("Error downloading terminated letters:", error);
      addAlert({
        id: 999,
        severity: "Error",
        message: "Print Failed",
        description: "Failed to generate print content"
      });
    }
  }, [state.selectedRows, triggerDownload, addAlert, isXerox]);

  const printTerminatedLetters = useCallback((content: string, title = "Print Preview") => {
    const printWindow = window.open("", "_blank");
    if (printWindow) {
      printWindow.document.write(`
        <html>
          <head>
            <title>${title}</title>
            <style>
              body {
                font-family: monospace;
                font-size: 12px;
                white-space: pre-wrap;
                margin: 20px;
              }
              @media print {
                body { margin: 0; }
                @page {
                  margin: 0;
                  size: auto;
                }
              }
            </style>
          </head>
          <body>
            ${content.replace(/\n/g, "<br>")}
          </body>
        </html>
      `);
      printWindow.document.close();
      printWindow.focus();
      printWindow.print();
      printWindow.close();
    }
  }, []);

  return {
    // State
    isSearching: state.search.isSearching,
    searchCompleted: state.search.searchCompleted,
    searchParams: state.search.searchParams,
    searchError: state.search.error,
    reportData: state.report.data,
    isLoadingReport: state.report.isLoading,
    reportError: state.report.error,
    selectedRows: state.selectedRows,

    // Actions
    executeSearch,
    resetSearch,
    setSelectedRows,
    clearSelectedRows,

    // Print functionality
    handlePrint,
    isDownloading,
    isPrintDialogOpen,
    setIsPrintDialogOpen,
    printContent,
    printTerminatedLetters,
    isXerox,
    setIsXerox,

    // Pagination
    gridPagination
  };
};

export default useTerminatedLetters;
