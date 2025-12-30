import { GridApi } from "ag-grid-community";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  useUpdateForfeitureAdjustmentBulkMutation,
  useUpdateForfeitureAdjustmentMutation
} from "reduxstore/api/AdhocApi";
import { useLazyGetUnForfeitsQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { CalendarResponseDto, ForfeitureAdjustmentUpdateRequest, StartAndEndDateRequest } from "reduxstore/types";
import { setMessage } from "smart-ui-library";
import { GRID_KEYS } from "../../../constants";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { useEditState } from "../../../hooks/useEditState";
import { SortParams, useGridPagination } from "../../../hooks/useGridPagination";
import { useRowSelection } from "../../../hooks/useRowSelection";
import { flattenMasterDetailData, generateRowKey } from "../../../utils/forfeitActivities/gridDataHelpers";
import {
  clearGridSelectionsForBadges,
  formatApiError,
  generateBulkSaveSuccessMessage,
  generateSaveSuccessMessage,
  getErrorMessage,
  getRowKeysForRequests,
  prepareBulkSaveRequests,
  prepareSaveRequest
} from "../../../utils/forfeitActivities/saveOperationHelpers";
import { Messages } from "../../../utils/messageDictonary";

interface UnForfeitGridConfig {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  resetPageFlag: boolean;
  onUnsavedChanges: (hasChanges: boolean) => void;
  hasUnsavedChanges: boolean;
  shouldArchive: boolean;
  onArchiveHandled?: () => void;
  setHasUnsavedChanges: (hasChanges: boolean) => void;
  fiscalCalendarYear: CalendarResponseDto | null;
  isReadOnly: boolean;
  onShowUnsavedChangesDialog?: () => void;
  onShowErrorDialog?: (title: string, message: string) => void;
}

// Configuration for shared utilities
const ACTIVITY_CONFIG = {
  activityType: "unforfeit" as const,
  rowKeyConfig: { type: "unforfeit" as const }
};

export const useUnForfeitGrid = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
  resetPageFlag,
  onUnsavedChanges,
  hasUnsavedChanges,
  shouldArchive,
  onArchiveHandled,
  setHasUnsavedChanges,
  fiscalCalendarYear,
  isReadOnly,
  onShowUnsavedChangesDialog,
  onShowErrorDialog
}: UnForfeitGridConfig) => {
  const [expandedRows, setExpandedRows] = useState<Record<string, boolean>>({});
  const [gridApi, setGridApi] = useState<GridApi | null>(null);
  const [pendingSuccessMessage, setPendingSuccessMessage] = useState<string | null>(null);
  const [isPendingBulkMessage, setIsPendingBulkMessage] = useState<boolean>(false);
  const [isBulkSaveInProgress, setIsBulkSaveInProgress] = useState<boolean>(false);

  const selectedProfitYear = useDecemberFlowProfitYear();
  const { unForfeits, unForfeitsQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const dispatch = useDispatch();

  const [triggerSearch, { isFetching }] = useLazyGetUnForfeitsQuery();
  const [updateForfeitureAdjustmentBulk] = useUpdateForfeitureAdjustmentBulkMutation();
  const [updateForfeitureAdjustment] = useUpdateForfeitureAdjustmentMutation();

  const editState = useEditState();
  const selectionState = useRowSelection();
  const gridRef = useRef<{ api: GridApi } | null>(null);
  const prevUnForfeits = useRef<typeof unForfeits | null>(null);

  // Create a request object based on current parameters
  const createRequest = useCallback(
    (
      skip: number,
      sortBy: string,
      isSortDescending: boolean,
      take: number
    ): (StartAndEndDateRequest & { archive?: boolean }) | null => {
      const baseRequest: StartAndEndDateRequest = {
        beginningDate: unForfeitsQueryParams?.beginningDate || fiscalCalendarYear?.fiscalBeginDate || "",
        endingDate: unForfeitsQueryParams?.endingDate || fiscalCalendarYear?.fiscalEndDate || "",
        excludeZeroBalance: unForfeitsQueryParams?.excludeZeroBalance || false,
        profitYear: selectedProfitYear,
        pagination: { skip, take, sortBy, isSortDescending }
      };

      if (!baseRequest.beginningDate || !baseRequest.endingDate) return null;

      const finalRequest = shouldArchive ? { ...baseRequest, archive: true } : baseRequest;
      return finalRequest;
    },
    [
      unForfeitsQueryParams,
      fiscalCalendarYear?.fiscalBeginDate,
      fiscalCalendarYear?.fiscalEndDate,
      shouldArchive,
      selectedProfitYear
    ]
  );

  const { pageNumber, pageSize, sortParams, handlePaginationChange, handleSortChange, resetPagination } =
    useGridPagination({
      initialPageSize: 25,
      initialSortBy: "fullName",
      initialSortDescending: false,
      persistenceKey: GRID_KEYS.REHIRE_FORFEITURES
    });

  const isPaginationChangeRef = useRef<boolean>(false);
  const currentPageSizeRef = useRef<number>(pageSize);

  // Update the ref whenever pageSize changes
  useEffect(() => {
    currentPageSizeRef.current = pageSize;
  }, [pageSize]);

  const onGridReady = useCallback((params: { api: GridApi }) => {
    setGridApi(params.api);
  }, []);

  // Effect to show success message after grid finishes loading
  useEffect(() => {
    if (!isFetching && pendingSuccessMessage) {
      const messageTemplate = isPendingBulkMessage ? Messages.UnforfeitBulkSaveSuccess : Messages.UnforfeitSaveSuccess;
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

  // Need a useEffect to reset the page number when total count changes (new search, not pagination)
  // Don't reset during or immediately after bulk save operations to preserve grid position
  useEffect(() => {
    const prevTotal = prevUnForfeits.current?.response?.total;
    const currentTotal = unForfeits?.response?.total;

    // Skip if this is the initial load (previous was undefined/null OR transitioning from 0 to non-zero)
    const isInitialLoad =
      !prevUnForfeits.current || (prevTotal === 0 && currentTotal !== undefined && currentTotal > 0);

    if (
      !isBulkSaveInProgress &&
      !isInitialLoad &&
      !isPaginationChangeRef.current &&
      unForfeits !== prevUnForfeits.current &&
      currentTotal !== undefined &&
      currentTotal !== prevTotal
    ) {
      resetPagination();
    }
    prevUnForfeits.current = unForfeits;
  }, [unForfeits, resetPagination, isBulkSaveInProgress]);

  const performSearch = useCallback(
    async (skip: number, sortBy: string, isSortDescending: boolean, take: number = 25) => {
      const request = createRequest(skip, sortBy, isSortDescending, take);
      if (request) {
        await triggerSearch(request, false);
      }
    },
    [createRequest, triggerSearch]
  );

  const handleSave = useCallback(
    async (request: ForfeitureAdjustmentUpdateRequest, name: string) => {
      const rowId = request.badgeNumber;

      // Capture the CURRENT page number from the ref (not the stale closure value)
      const actualCurrentPage = currentPageNumberRef.current;

      // Clear the selection immediately for the saved item
      const currentGridApi = gridRef.current?.api || gridApi;
      if (currentGridApi) {
        currentGridApi.forEachNode((node) => {
          if (node.data?.badgeNumber === request.badgeNumber && node.data?.isDetail && node.isSelected()) {
            const nodeId = Number(node.id);
            selectionState.removeRowFromSelection(nodeId);
            node.setSelected(false);
          }
        });
      }

      editState.addLoadingRow(rowId);

      try {
        // Transform request using shared helper
        const transformedRequest = prepareSaveRequest(request);
        const result = await updateForfeitureAdjustment({ ...transformedRequest, suppressAllToastErrors: true });

        if (result?.error) {
          onShowErrorDialog?.(
            "Class Action Forfeit",
            "Save failed. One or more unforfeits were related to a class action forfeit."
          );
        } else {
          // Generate row key using shared helper (uses profitDetailId for unforfeit)
          const rowKey = generateRowKey(ACTIVITY_CONFIG.rowKeyConfig, {
            badgeNumber: request.badgeNumber,
            profitYear: request.profitYear,
            profitDetailId: request.offsettingProfitDetailId
          });
          editState.removeEditedValue(rowKey);

          // Generate success message using shared helper
          const successMessage = generateSaveSuccessMessage(
            ACTIVITY_CONFIG.activityType,
            name || "the selected employee",
            request.forfeitureAmount
          );

          if (unForfeitsQueryParams) {
            // Use the captured page number to stay on the same page
            const skip = actualCurrentPage * pageSize;
            const searchRequest = createRequest(skip, sortParams.sortBy, sortParams.isSortDescending, pageSize);
            if (searchRequest) {
              await triggerSearch(searchRequest, false);

              // Restore the page number in pagination state
              handlePaginationChange(actualCurrentPage, pageSize);

              setPendingSuccessMessage(successMessage);
            }
          }
        }
      } catch (error) {
        console.error("Failed to save forfeiture adjustment:", error);
        const errorMessage = formatApiError(error, getErrorMessage(ACTIVITY_CONFIG.activityType, "save"));
        onShowErrorDialog?.("Save Failed", errorMessage);
      } finally {
        editState.removeLoadingRow(rowId);
      }
    },
    [
      updateForfeitureAdjustment,
      editState,
      selectionState,
      unForfeitsQueryParams,
      pageSize,
      sortParams,
      createRequest,
      triggerSearch,
      handlePaginationChange,
      gridApi,
      onShowErrorDialog
    ]
  );

  // Ref to preserve the page number during bulk save before any resets occur
  const pageNumberAtBulkSaveRef = useRef<number | null>(null);

  // Ref to always have current pageNumber (avoid stale closure)
  const currentPageNumberRef = useRef(pageNumber);
  useEffect(() => {
    currentPageNumberRef.current = pageNumber;
  }, [pageNumber]);

  const handleBulkSave = useCallback(
    async (requests: ForfeitureAdjustmentUpdateRequest[], names: string[]) => {
      const badgeNumbers = requests.map((request) => request.badgeNumber);
      editState.addLoadingRows(badgeNumbers);

      // Capture the CURRENT page number from the ref (not the stale closure value)
      // This is critical because the callback's pageNumber can be stale
      const actualCurrentPage = currentPageNumberRef.current;
      pageNumberAtBulkSaveRef.current = actualCurrentPage;

      setIsBulkSaveInProgress(true);

      try {
        // Transform all requests using shared helper
        const transformedRequests = prepareBulkSaveRequests(requests);
        await updateForfeitureAdjustmentBulk(transformedRequests);

        // Generate row keys using shared helper (uses profitDetailId for unforfeit)
        const rowKeys = getRowKeysForRequests(ACTIVITY_CONFIG, requests);
        editState.clearEditedValues(rowKeys);
        selectionState.clearSelection();

        // Clear selections in grid using shared helper
        clearGridSelectionsForBadges(gridRef.current?.api ?? gridApi ?? undefined, badgeNumbers);

        // Generate bulk success message using shared helper
        const employeeNames = names.map((name) => name || "Unknown Employee");
        const bulkSuccessMessage = generateBulkSaveSuccessMessage(
          ACTIVITY_CONFIG.activityType,
          requests.length,
          employeeNames
        );

        if (unForfeitsQueryParams) {
          // Use the captured page number, not the current one (which may have been reset)
          const savedPageNumber = pageNumberAtBulkSaveRef.current ?? 0;
          const skip = savedPageNumber * pageSize;
          const request = createRequest(skip, sortParams.sortBy, sortParams.isSortDescending, pageSize);
          if (request) {
            await triggerSearch(request, false);

            // Restore the page number in pagination state
            handlePaginationChange(savedPageNumber, pageSize);

            setPendingSuccessMessage(bulkSuccessMessage);
            setIsPendingBulkMessage(true);
          }
        }
      } catch (error) {
        console.error("Failed to save forfeiture adjustments:", error);
        const errorMessage = formatApiError(error, getErrorMessage(ACTIVITY_CONFIG.activityType, "bulkSave"));
        onShowErrorDialog?.("Bulk Save Failed", errorMessage);
      } finally {
        editState.removeLoadingRows(badgeNumbers);
        pageNumberAtBulkSaveRef.current = null;
        // Reset the flag after a small delay to ensure the Redux state update completes first
        setTimeout(() => setIsBulkSaveInProgress(false), 100);
      }
    },
    [
      updateForfeitureAdjustmentBulk,
      editState,
      selectionState,
      unForfeitsQueryParams,
      pageSize,
      sortParams,
      createRequest,
      triggerSearch,
      handlePaginationChange,
      gridApi,
      onShowErrorDialog
    ]
  );

  // Effect to handle initial load and pagination changes
  useEffect(() => {
    if (initialSearchLoaded && !isPaginationChangeRef.current) {
      performSearch(pageNumber * pageSize, sortParams.sortBy, sortParams.isSortDescending, pageSize);
    }
  }, [initialSearchLoaded, pageNumber, pageSize, sortParams, performSearch]);

  // Effect to handle archive mode search
  useEffect(() => {
    if (!shouldArchive) return;

    let cancelled = false;
    const run = async () => {
      await performSearch(pageNumber * pageSize, sortParams.sortBy, sortParams.isSortDescending, pageSize);
      if (!cancelled) {
        onArchiveHandled?.();
      }
    };

    run();

    return () => {
      cancelled = true;
    };
  }, [
    shouldArchive,
    pageNumber,
    pageSize,
    sortParams,
    performSearch,
    fiscalCalendarYear?.fiscalBeginDate,
    fiscalCalendarYear?.fiscalEndDate,
    selectedProfitYear,
    onArchiveHandled
  ]);

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

  useEffect(() => {
    const hasChanges = selectionState.selectedRowIds.length > 0 || Object.keys(editState.editedValues).length > 0;
    onUnsavedChanges(hasChanges);
    setHasUnsavedChanges(hasChanges);
  }, [selectionState.selectedRowIds, editState.editedValues, onUnsavedChanges, setHasUnsavedChanges]);

  // Initialize expandedRows when data is loaded
  useEffect(() => {
    if (unForfeits?.response?.results && unForfeits.response.results.length > 0) {
      const initialExpandState: Record<string, boolean> = {};
      unForfeits.response.results.forEach((row) => {
        const hasDetails = row.details && row.details.length > 0;
        if (hasDetails) {
          initialExpandState[row.badgeNumber.toString()] = true;
        }
      });
      setExpandedRows(initialExpandState);
    }
  }, [unForfeits?.response?.results]);

  // Refresh the grid when loading state changes
  useEffect(() => {
    if (gridRef.current?.api) {
      gridRef.current.api.refreshCells({
        force: true,
        suppressFlash: false
      });
    }
  }, [editState.loadingRowIds]);

  // Sort handler that immediately triggers a search with the new sort parameters
  const sortEventHandler = (update: SortParams) => {
    // Update the sort state first
    handleSortChange(update);
    // Reset to page 0 and perform search with new sort parameters
    handlePaginationChange(0, pageSize);
    performSearch(0, update.sortBy, update.isSortDescending, pageSize);
  };

  // Handle row expansion toggle
  const handleRowExpansion = (badgeNumber: string) => {
    setExpandedRows((prev) => ({
      ...prev,
      [badgeNumber]: !prev[badgeNumber]
    }));
  };

  // Create the grid data with expandable rows using shared helper
  const gridData = useMemo(() => {
    if (!unForfeits?.response?.results) return [];

    // Convert expandedRows Record<string, boolean> to Set<string>
    const expandedRowsSet = new Set(
      Object.entries(expandedRows)
        .filter(([_, isExpanded]) => isExpanded)
        .map(([key, _]) => key)
    );

    return flattenMasterDetailData(unForfeits.response.results, expandedRowsSet, {
      getKey: (row) => row.badgeNumber.toString(),
      getDetails: (row) => {
        // For unforfeit, we need to merge parent data with detail data
        return (row.details || []).map((detail, index) => ({
          ...row,
          ...detail,
          parentId: row.badgeNumber,
          index
        }));
      },
      hasDetails: (row) => Boolean(row.details && row.details.length > 0)
    });
  }, [unForfeits, expandedRows]);

  const paginationHandlers = useMemo(
    () => ({
      setPageNumber: async (value: number) => {
        if (hasUnsavedChanges) {
          onShowUnsavedChangesDialog?.();
          return;
        }

        // If we're in the middle of a pagination change (like page size change),
        // just update the state without triggering a search
        if (isPaginationChangeRef.current) {
          // DSMPaginatedGrid already passes 0-based page number
          const currentPageSizeValue = currentPageSizeRef.current;
          handlePaginationChange(value, currentPageSizeValue);
          return;
        }

        // Normal page number change - trigger search
        // DSMPaginatedGrid already passes 0-based page number (no need to subtract 1)
        const currentPageSizeValue = currentPageSizeRef.current;
        handlePaginationChange(value, currentPageSizeValue);
        try {
          await performSearch(
            value * currentPageSizeValue,
            sortParams.sortBy,
            sortParams.isSortDescending,
            currentPageSizeValue
          );
        } finally {
          // Don't set isPaginationChangeRef here since we're not the one who set it
        }
        setInitialSearchLoaded(true);
      },
      setPageSize: async (value: number) => {
        if (hasUnsavedChanges) {
          onShowUnsavedChangesDialog?.();
          return;
        }
        isPaginationChangeRef.current = true;
        currentPageSizeRef.current = value; // Update the ref immediately
        handlePaginationChange(0, value);
        try {
          await performSearch(0, sortParams.sortBy, sortParams.isSortDescending, value);
        } finally {
          // Use requestAnimationFrame to clear flag after all synchronous updates
          requestAnimationFrame(() => {
            setTimeout(() => {
              isPaginationChangeRef.current = false;
            }, 10);
          });
        }
        setInitialSearchLoaded(true);
      }
    }),
    [
      hasUnsavedChanges,
      onShowUnsavedChangesDialog,
      handlePaginationChange,
      performSearch,
      sortParams.sortBy,
      sortParams.isSortDescending,
      setInitialSearchLoaded
    ]
  );

  return {
    // State
    pageNumber,
    pageSize,
    sortParams,
    expandedRows,
    gridData,
    isFetching,
    unForfeits,
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
      loadingRowIds: editState.loadingRowIds,
      isReadOnly
    }
  };
};
