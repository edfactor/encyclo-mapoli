import { GridApi } from "ag-grid-community";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  useLazyGetTerminationReportQuery,
  useUpdateForfeitureAdjustmentBulkMutation,
  useUpdateForfeitureAdjustmentMutation
} from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { CalendarResponseDto, ForfeitureAdjustmentUpdateRequest, StartAndEndDateRequest } from "reduxstore/types";
import { formatNumberWithComma, setMessage } from "smart-ui-library";
import { Messages } from "../utils/messageDictonary";
import useDecemberFlowProfitYear from "./useDecemberFlowProfitYear";
import { useEditState } from "./useEditState";
import { SortParams, useGridPagination } from "./useGridPagination";
import { useRowSelection } from "./useRowSelection";

interface TerminationSearchRequest {
  beginningDate?: string;
  endingDate?: string;
  profitYear?: number;
  pagination?: {
    skip: number;
    take: number;
    sortBy: string;
    isSortDescending: boolean;
  };
}

interface TerminationGridConfig {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  searchParams: TerminationSearchRequest | null;
  resetPageFlag: boolean;
  onUnsavedChanges: (hasChanges: boolean) => void;
  hasUnsavedChanges: boolean;
  fiscalData: CalendarResponseDto | null;
  shouldArchive?: boolean;
  onArchiveHandled?: () => void;
  onErrorOccurred?: () => void;
  onLoadingChange?: (isLoading: boolean) => void;
}

export const useTerminationGrid = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
  searchParams,
  resetPageFlag,
  onUnsavedChanges,
  hasUnsavedChanges,
  fiscalData,
  shouldArchive,
  onArchiveHandled,
  onErrorOccurred,
  onLoadingChange
}: TerminationGridConfig) => {
  const [expandedRows, setExpandedRows] = useState<Record<string, boolean>>({});
  const [pendingSuccessMessage, setPendingSuccessMessage] = useState<string | null>(null);
  const [isPendingBulkMessage, setIsPendingBulkMessage] = useState<boolean>(false);
  const [isBulkSaveInProgress, setIsBulkSaveInProgress] = useState<boolean>(false);

  const selectedProfitYear = useDecemberFlowProfitYear();
  const { termination } = useSelector((state: RootState) => state.yearsEnd);
  const dispatch = useDispatch();

  const [triggerSearch, { isFetching }] = useLazyGetTerminationReportQuery();
  const [updateForfeitureAdjustmentBulk] = useUpdateForfeitureAdjustmentBulkMutation();
  const [updateForfeitureAdjustment] = useUpdateForfeitureAdjustmentMutation();

  const editState = useEditState();
  const selectionState = useRowSelection();
  const gridRef = useRef<{ api: GridApi } | null>(null);
  const prevTermination = useRef<typeof termination | null>(null);
  const lastRequestKeyRef = useRef<string | null>(null);

  // Create a request object based on current parameters
  const createRequest = useCallback(
    (
      skip: number,
      sortBy: string,
      isSortDescending: boolean,
      profitYear: number,
      pageSz: number
    ): (StartAndEndDateRequest & { archive?: boolean }) | null => {
      const base: StartAndEndDateRequest = searchParams
        ? {
            ...searchParams,
            profitYear,
            pagination: { skip, take: pageSz, sortBy, isSortDescending }
          }
        : {
            beginningDate: fiscalData?.fiscalBeginDate || "",
            endingDate: fiscalData?.fiscalEndDate || "",
            profitYear,
            pagination: { skip, take: pageSz, sortBy, isSortDescending }
          };

      if (!base.beginningDate || !base.endingDate) return null;

      return shouldArchive ? { ...base, archive: true } : base;
    },
    [searchParams, fiscalData?.fiscalBeginDate, fiscalData?.fiscalEndDate, shouldArchive]
  );

  const { pageNumber, pageSize, sortParams, handlePaginationChange, handleSortChange, resetPagination } =
    useGridPagination({
      initialPageSize: 25,
      initialSortBy: "name",
      initialSortDescending: false,
      onPaginationChange: useCallback(
        async (pageNum: number, pageSz: number, sortPrms: SortParams) => {
          if (initialSearchLoaded && searchParams) {
            const params = createRequest(
              pageNum * pageSz,
              sortPrms.sortBy,
              sortPrms.isSortDescending,
              selectedProfitYear,
              pageSz
            );
            if (params) {
              await triggerSearch(params, false);
            }
          }
        },
        [initialSearchLoaded, searchParams, createRequest, selectedProfitYear, triggerSearch]
      )
    });

  const onGridReady = useCallback((_params: { api: GridApi }) => {
    // Grid is now ready - gridRef will be set by the component
  }, []);

  // Effect to show success message after grid finishes loading
  useEffect(() => {
    if (!isFetching && pendingSuccessMessage) {
      const messageTemplate = isPendingBulkMessage
        ? Messages.TerminationBulkSaveSuccess
        : Messages.TerminationSaveSuccess;
      dispatch(
        setMessage({
          ...messageTemplate,
          message: {
            ...messageTemplate.message,
            message: pendingSuccessMessage
          }
        })
      );
      setPendingSuccessMessage(null);
      setIsPendingBulkMessage(false);
    }
  }, [isFetching, pendingSuccessMessage, isPendingBulkMessage, dispatch]);

  // Notify parent component about loading state changes
  useEffect(() => {
    onLoadingChange?.(isFetching);
  }, [isFetching, onLoadingChange]);

  // Need a useEffect to reset the page number when total count changes (new search, not pagination)
  // Don't reset during or immediately after bulk save operations to preserve grid position
  useEffect(() => {
    const prevTotal = prevTermination.current?.response?.total;
    const currentTotal = termination?.response?.total;

    // Skip if this is the initial load (previous was undefined/null OR transitioning from 0 to non-zero)
    const isInitialLoad =
      !prevTermination.current || (prevTotal === 0 && currentTotal !== undefined && currentTotal > 0);

    if (
      !isBulkSaveInProgress &&
      !isInitialLoad &&
      termination !== prevTermination.current &&
      currentTotal !== undefined &&
      currentTotal !== prevTotal
    ) {
      resetPagination();
    }
    prevTermination.current = termination;
  }, [termination, resetPagination, isBulkSaveInProgress]);

  // Initialize expandedRows when data is loaded
  useEffect(() => {
    if (termination?.response?.results && termination.response.results.length > 0) {
      const initialExpandState: Record<string, boolean> = {};
      termination.response.results.forEach((row) => {
        if (row.yearDetails && row.yearDetails.length > 0) {
          initialExpandState[row.badgeNumber.toString()] = true;
        }
      });
      setExpandedRows(initialExpandState);
    }
  }, [termination?.response?.results]);

  // Refresh the grid when loading state changes
  useEffect(() => {
    if (gridRef.current?.api) {
      gridRef.current.api.refreshCells({
        force: true,
        suppressFlash: false
      });
    }
  }, [editState.loadingRowIds]);

  // Helper to build a unique key for current request inputs
  const buildRequestKey = useCallback(
    (
      skip: number,
      sortBy: string,
      isSortDescending: boolean,
      profitYear: number,
      pageSz: number,
      beginningDate?: string,
      endingDate?: string,
      archive?: boolean
    ) =>
      `${skip}|${pageSz}|${sortBy}|${isSortDescending}|${profitYear}|${beginningDate ?? ""}|${endingDate ?? ""}|${archive ? "1" : "0"}`,
    []
  );

  // Unified effect to handle all data fetching scenarios
  useEffect(() => {
    if (!initialSearchLoaded || !searchParams) return;

    const executeSearch = async () => {
      const params = createRequest(
        pageNumber * pageSize,
        sortParams.sortBy,
        sortParams.isSortDescending,
        selectedProfitYear,
        pageSize
      );

      if (params) {
        const key = buildRequestKey(
          pageNumber * pageSize,
          sortParams.sortBy,
          sortParams.isSortDescending,
          selectedProfitYear,
          pageSize,
          params.beginningDate,
          params.endingDate,
          (params as StartAndEndDateRequest & { archive?: boolean }).archive
        );

        // Prevent duplicate requests
        if (lastRequestKeyRef.current === key) {
          if (shouldArchive) {
            onArchiveHandled?.();
          }
          return;
        }

        lastRequestKeyRef.current = key;
        await triggerSearch(params, false);

        // Clear archive flag after successful search
        if (shouldArchive) {
          onArchiveHandled?.();
        }
      }
    };

    executeSearch();
  }, [
    searchParams,
    pageNumber,
    pageSize,
    sortParams,
    selectedProfitYear,
    shouldArchive,
    triggerSearch,
    createRequest,
    initialSearchLoaded,
    buildRequestKey,
    onArchiveHandled
  ]);

  // Ref to always have current pageNumber (avoid stale closure)
  const currentPageNumberRef = useRef(pageNumber);
  useEffect(() => {
    currentPageNumberRef.current = pageNumber;
  }, [pageNumber]);

  const handleSave = useCallback(
    async (request: ForfeitureAdjustmentUpdateRequest, name: string) => {
      const rowId = request.badgeNumber;
      editState.addLoadingRow(rowId);

      try {
        await updateForfeitureAdjustment(request);
        const rowKey = `${request.badgeNumber}-${request.profitYear}`;
        editState.removeEditedValue(rowKey);

        // Check for remaining edits after removing this one
        const remainingEdits = Object.keys(editState.editedValues).filter((key) => key !== rowKey).length > 0;
        onUnsavedChanges(remainingEdits);

        // Prepare success message
        const employeeName = name || "the selected employee";
        const successMessage = `The forfeiture adjustment of amount $${formatNumberWithComma(request.forfeitureAmount)} for ${employeeName} saved successfully`;

        // Refresh grid and show success message after data loads
        if (searchParams) {
          const params = createRequest(
            pageNumber * pageSize,
            sortParams.sortBy,
            sortParams.isSortDescending,
            selectedProfitYear,
            pageSize
          );
          if (params) {
            await triggerSearch(params, false);
            setPendingSuccessMessage(successMessage);
          }
        } else {
          dispatch(
            setMessage({
              ...Messages.TerminationSaveSuccess,
              message: {
                ...Messages.TerminationSaveSuccess.message,
                message: successMessage
              }
            })
          );
        }
      } catch (error) {
        console.error("Failed to save forfeiture adjustment:", error);

        dispatch(
          setMessage({
            key: "TerminationSave",
            message: {
              message: `Failed to save adjustment for ${name || "employee"}.`,
              type: "error"
            }
          })
        );

        if (onErrorOccurred) {
          onErrorOccurred();
        }
      } finally {
        editState.removeLoadingRow(rowId);
      }
    },
    [
      updateForfeitureAdjustment,
      editState,
      onUnsavedChanges,
      searchParams,
      pageNumber,
      pageSize,
      sortParams,
      selectedProfitYear,
      createRequest,
      triggerSearch,
      dispatch,
      onErrorOccurred
    ]
  );

  const handleBulkSave = useCallback(
    async (requests: ForfeitureAdjustmentUpdateRequest[], names: string[]) => {
      const badgeNumbers = requests.map((request) => request.badgeNumber);
      editState.addLoadingRows(badgeNumbers);

      // Capture the CURRENT page number from the ref (not the stale closure value)
      const actualCurrentPage = currentPageNumberRef.current;

      setIsBulkSaveInProgress(true);

      try {
        await updateForfeitureAdjustmentBulk(requests);

        const rowKeys = requests.map((request) => `${request.badgeNumber}-${request.profitYear}`);
        editState.clearEditedValues(rowKeys);
        selectionState.clearSelection();

        // Check for remaining edits
        const remainingEditKeys = Object.keys(editState.editedValues).filter((key) => !rowKeys.includes(key));
        onUnsavedChanges(remainingEditKeys.length > 0);

        // Prepare bulk success message
        const employeeNames = names.map((name) => name || "Unknown Employee");
        const bulkSuccessMessage = `Members affected: ${employeeNames.join("; ")}`;

        if (searchParams) {
          // Use the captured page number to stay on the same page
          const skip = actualCurrentPage * pageSize;
          const request = createRequest(
            skip,
            sortParams.sortBy,
            sortParams.isSortDescending,
            selectedProfitYear,
            pageSize
          );
          if (request) {
            await triggerSearch(request, false);

            // Restore the page number in pagination state
            handlePaginationChange(actualCurrentPage, pageSize);

            setPendingSuccessMessage(bulkSuccessMessage);
            setIsPendingBulkMessage(true);
          }
        }
      } catch (error) {
        console.error("Failed to save forfeiture adjustments:", error);

        dispatch(
          setMessage({
            key: "TerminationSave",
            message: {
              message: `Failed to save bulk adjustments.`,
              type: "error"
            }
          })
        );

        if (onErrorOccurred) {
          onErrorOccurred();
        }
      } finally {
        editState.removeLoadingRows(badgeNumbers);
        // Reset the flag after a small delay to ensure the Redux state update completes first
        setTimeout(() => setIsBulkSaveInProgress(false), 100);
      }
    },
    [
      updateForfeitureAdjustmentBulk,
      editState,
      selectionState,
      onUnsavedChanges,
      searchParams,
      pageSize,
      sortParams,
      selectedProfitYear,
      createRequest,
      triggerSearch,
      handlePaginationChange,
      dispatch,
      onErrorOccurred
    ]
  );

  // Reset page number to 0 when resetPageFlag changes (from search button)
  // Track the previous value to detect actual changes, not just re-renders
  const prevResetPageFlag = useRef<boolean>(resetPageFlag);
  useEffect(() => {
    // Only reset if the flag actually toggled (changed value)
    const flagChanged = prevResetPageFlag.current !== resetPageFlag;

    if (flagChanged && !isBulkSaveInProgress) {
      resetPagination();
    }

    prevResetPageFlag.current = resetPageFlag;
  }, [resetPageFlag, resetPagination, isBulkSaveInProgress]);

  // Track unsaved changes based on edit state only
  useEffect(() => {
    onUnsavedChanges(editState.hasAnyEdits);
  }, [editState.hasAnyEdits, onUnsavedChanges]);

  // Sort handler that immediately triggers a search with the new sort parameters
  const sortEventHandler = (update: SortParams) => {
    handleSortChange(update);
  };

  // Handle row expansion toggle
  const handleRowExpansion = (badgeNumber: string) => {
    setExpandedRows((prev) => ({
      ...prev,
      [badgeNumber]: !prev[badgeNumber]
    }));
  };

  // Build grid data with expandable rows
  const gridData = useMemo(() => {
    if (!termination?.response?.results) return [];
    const rows = [];
    for (const row of termination.response.results) {
      const hasDetails = row.yearDetails && row.yearDetails.length > 0;

      // Add main row
      rows.push({
        ...row,
        isExpandable: hasDetails,
        isExpanded: hasDetails && Boolean(expandedRows[row.badgeNumber.toString()]),
        isDetail: false
      });

      // Add detail rows if expanded
      if (hasDetails && expandedRows[row.badgeNumber.toString()]) {
        let index = 0;
        for (const detail of row.yearDetails) {
          rows.push({
            ...row,
            ...detail,
            isDetail: true,
            isExpandable: false,
            isExpanded: false,
            parentId: row.badgeNumber,
            index: index
          });
          index++;
        }
      }
    }

    return rows;
  }, [termination, expandedRows]);

  const paginationHandlers = {
    setPageNumber: (value: number) => {
      if (hasUnsavedChanges) {
        alert("Please save your changes.");
        return;
      }
      handlePaginationChange(value - 1, pageSize);
      setInitialSearchLoaded(true);
    },
    setPageSize: (value: number) => {
      if (hasUnsavedChanges) {
        alert("Please save your changes.");
        return;
      }
      handlePaginationChange(0, value);
      setInitialSearchLoaded(true);
    }
  };

  return {
    // State
    pageNumber,
    pageSize,
    sortParams,
    expandedRows,
    gridData,
    isFetching,
    termination,
    selectedProfitYear,

    // Edit and selection state
    editState,
    selectionState,

    // Handlers
    handleSave,
    handleBulkSave,
    handleRowExpansion,
    sortEventHandler,
    onGridReady,

    // Pagination
    paginationHandlers,

    // Refs
    gridRef,

    // Grid context
    gridContext: {
      editedValues: editState.editedValues,
      updateEditedValue: editState.updateEditedValue,
      loadingRowIds: editState.loadingRowIds
    }
  };
};
