import { GridApi } from "ag-grid-community";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  useLazyGetUnForfeitsQuery,
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
}

export const useUnForfeitGrid = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
  resetPageFlag,
  onUnsavedChanges,
  hasUnsavedChanges,
  shouldArchive,
  onArchiveHandled,
  setHasUnsavedChanges,
  fiscalCalendarYear
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
      isSortDescending: boolean
    ): (StartAndEndDateRequest & { archive?: boolean }) | null => {
      const baseRequest: StartAndEndDateRequest = {
        beginningDate: unForfeitsQueryParams?.beginningDate || fiscalCalendarYear?.fiscalBeginDate || "",
        endingDate: unForfeitsQueryParams?.endingDate || fiscalCalendarYear?.fiscalEndDate || "",
        profitYear: selectedProfitYear,
        pagination: { skip, take: 25, sortBy, isSortDescending } // Use fixed pageSize for now, will be updated dynamically
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

  const { pageNumber, pageSize, sortParams, handlePaginationChange, resetPagination } =
    useGridPagination({
      initialPageSize: 25,
      initialSortBy: "fullName",
      initialSortDescending: false,
      onPaginationChange: useCallback(
        async (pageNum: number, pageSz: number, sortPrms: SortParams) => {
          if (initialSearchLoaded) {
            const request = createRequest(pageNum * pageSz, sortPrms.sortBy, sortPrms.isSortDescending);
            if (request && request.pagination) {
              // Update the pageSize in the request
              request.pagination.take = pageSz;
              await triggerSearch(request, false);
            }
          }
        },
        [initialSearchLoaded, createRequest, triggerSearch]
      )
    });

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
      unForfeits !== prevUnForfeits.current &&
      currentTotal !== undefined &&
      currentTotal !== prevTotal
    ) {
      resetPagination();
    }
    prevUnForfeits.current = unForfeits;
  }, [unForfeits, resetPagination, isBulkSaveInProgress]);

  const performSearch = useCallback(
    async (skip: number, sortBy: string, isSortDescending: boolean) => {
      const request = createRequest(skip, sortBy, isSortDescending);
      if (request) {
        await triggerSearch(request, false);
      }
    },
    [createRequest, triggerSearch]
  );

  const handleSave = useCallback(
    async (request: ForfeitureAdjustmentUpdateRequest, name: string) => {
      const rowId = request.badgeNumber;

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
        const result = await updateForfeitureAdjustment({ ...request, suppressAllToastErrors: true });
        if (result?.error) {
          alert("Save failed. One or more unforfeits were related to a class action forfeit.");
        } else {
          const rowKey = `${request.badgeNumber}-${request.profitYear}`;
          editState.removeEditedValue(rowKey);

          const employeeName = name || "the selected employee";
          const successMessage = `The unforfeiture amount of $${formatNumberWithComma(request.forfeitureAmount)} for ${employeeName} saved successfully`;

          if (unForfeitsQueryParams) {
            const searchRequest = createRequest(pageNumber * pageSize, sortParams.sortBy, sortParams.isSortDescending);
            if (searchRequest) {
              await triggerSearch(searchRequest, false);
              setPendingSuccessMessage(successMessage);
            }
          }
        }
      } catch (error) {
        console.error("Failed to save forfeiture adjustment:", error);
        alert("Failed to save adjustment. Please try again.");
      } finally {
        editState.removeLoadingRow(rowId);
      }
    },
    [
      updateForfeitureAdjustment,
      editState,
      selectionState,
      unForfeitsQueryParams,
      pageNumber,
      pageSize,
      sortParams,
      createRequest,
      triggerSearch,
      gridApi
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
        await updateForfeitureAdjustmentBulk(requests);
        const rowKeys = requests.map((request) => `${request.badgeNumber}-${request.profitYear}`);
        editState.clearEditedValues(rowKeys);
        selectionState.clearSelection();

        const employeeNames = names.map((name) => name || "Unknown Employee");
        const bulkSuccessMessage = `Members affected: ${employeeNames.join("; ")}`;

        if (unForfeitsQueryParams) {
          // Use the captured page number, not the current one (which may have been reset)
          const savedPageNumber = pageNumberAtBulkSaveRef.current ?? 0;
          const skip = savedPageNumber * pageSize;
          const request = createRequest(skip, sortParams.sortBy, sortParams.isSortDescending);
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
        alert("Failed to save one or more adjustments. Please try again.");
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
      handlePaginationChange
    ]
  );

  // Effect to handle initial load and pagination changes
  useEffect(() => {
    if (initialSearchLoaded) {
      performSearch(pageNumber * pageSize, sortParams.sortBy, sortParams.isSortDescending);
    }
  }, [initialSearchLoaded, pageNumber, pageSize, sortParams, performSearch]);

  // Effect to handle archive mode search
  useEffect(() => {
    if (!shouldArchive) return;

    let cancelled = false;
    const run = async () => {
      await performSearch(pageNumber * pageSize, sortParams.sortBy, sortParams.isSortDescending);
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
    // Reset to page 0 and perform search with new sort parameters
    handlePaginationChange(0, pageSize);
    performSearch(0, update.sortBy, update.isSortDescending);
  };

  // Handle row expansion toggle
  const handleRowExpansion = (badgeNumber: string) => {
    setExpandedRows((prev) => ({
      ...prev,
      [badgeNumber]: !prev[badgeNumber]
    }));
  };

  // Create the grid data with expandable rows
  const gridData = useMemo(() => {
    if (!unForfeits?.response?.results) return [];

    const rows = [];

    for (const row of unForfeits.response.results) {
      const hasDetails = row.details && row.details.length > 0;

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
        for (const detail of row.details) {
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
  }, [unForfeits, expandedRows]);

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
      loadingRowIds: editState.loadingRowIds
    }
  };
};
