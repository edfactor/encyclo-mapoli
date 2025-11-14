import { GridApi } from "ag-grid-community";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  useLazyGetTerminationReportQuery,
  useUpdateForfeitureAdjustmentBulkMutation,
  useUpdateForfeitureAdjustmentMutation
} from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import {
  CalendarResponseDto,
  FilterableStartAndEndDateRequest,
  ForfeitureAdjustmentUpdateRequest
} from "reduxstore/types";
import { setMessage } from "smart-ui-library";
import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import { useEditState } from "../../../../hooks/useEditState";
import { SortParams, useGridPagination } from "../../../../hooks/useGridPagination";
import { useRowSelection } from "../../../../hooks/useRowSelection";
import { generateRowKey } from "../../../../utils/forfeitActivities/gridDataHelpers";
import {
  clearGridSelectionsForBadges,
  formatApiError,
  generateBulkSaveSuccessMessage,
  generateSaveSuccessMessage,
  getErrorMessage,
  getRowKeysForRequests,
  prepareBulkSaveRequests,
  prepareSaveRequest
} from "../../../../utils/forfeitActivities/saveOperationHelpers";
import { Messages } from "../../../../utils/messageDictonary";

interface TerminationSearchRequest {
  beginningDate?: string;
  endingDate?: string;
  profitYear?: number;
  excludeZeroBalance?: boolean;
  excludeZeroAndFullyVested?: boolean;
  vestedBalanceValue?: number | null;
  vestedBalanceOperator?: number | null;
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
  isReadOnly: boolean;
  onShowUnsavedChangesDialog?: () => void;
}

// Configuration for shared utilities
const ACTIVITY_CONFIG = {
  activityType: "termination" as const,
  rowKeyConfig: { type: "termination" as const }
};

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
  onLoadingChange,
  isReadOnly,
  onShowUnsavedChangesDialog
}: TerminationGridConfig) => {
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

  // Notify parent component of loading state changes
  useEffect(() => {
    if (onLoadingChange) {
      onLoadingChange(isFetching);
    }
  }, [isFetching, onLoadingChange]);

  // Create a request object based on current parameters
  const createRequest = useCallback(
    (
      skip: number,
      sortBy: string,
      isSortDescending: boolean,
      profitYear: number,
      pageSz: number
    ): (FilterableStartAndEndDateRequest & { archive?: boolean }) | null => {
      const base: FilterableStartAndEndDateRequest = searchParams
        ? {
            beginningDate: searchParams.beginningDate || "",
            endingDate: searchParams.endingDate || "",
            profitYear,
            pagination: { skip, take: pageSz, sortBy, isSortDescending },
            excludeZeroBalance: searchParams.excludeZeroBalance,
            excludeZeroAndFullyVested: searchParams.excludeZeroAndFullyVested,
            vestedBalanceValue: searchParams.vestedBalanceValue,
            vestedBalanceOperator: searchParams.vestedBalanceOperator
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
      archive?: boolean,
      excludeZeroAndFullyVested?: boolean,
      excludeAlreadyForfeited?: boolean,
      vestedBalanceValue?: number | null,
      vestedBalanceOperator?: number | null
    ) =>
      `${skip}|${pageSz}|${sortBy}|${isSortDescending}|${profitYear}|${beginningDate ?? ""}|${endingDate ?? ""}|${archive ? "1" : "0"}|${excludeZeroAndFullyVested ? "1" : "0"}|${excludeAlreadyForfeited ? "1" : "0"}|${vestedBalanceValue ?? ""}|${vestedBalanceOperator ?? ""}`,
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
          (params as FilterableStartAndEndDateRequest & { archive?: boolean }).archive,
          params.excludeZeroAndFullyVested,
          undefined,
          params.vestedBalanceValue,
          params.vestedBalanceOperator
        );

        // Allow re-search with same parameters (consistent with all other search pages)
        lastRequestKeyRef.current = key;

        try {
          await triggerSearch(params, false);
        } catch (error) {
          console.error("Error fetching termination report:", error);
        }

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

      // Capture the CURRENT page number from the ref (not the stale closure value)
      const actualCurrentPage = currentPageNumberRef.current;

      editState.addLoadingRow(rowId);

      try {
        // Transform request using shared helper (no transformation for termination, but keeps consistent)
        const transformedRequest = prepareSaveRequest(ACTIVITY_CONFIG, request);
        await updateForfeitureAdjustment(transformedRequest);

        // Generate row key using shared helper
        const rowKey = generateRowKey(ACTIVITY_CONFIG.rowKeyConfig, {
          badgeNumber: request.badgeNumber,
          profitYear: request.profitYear
        });
        editState.removeEditedValue(rowKey);

        // Check for remaining edits after removing this one
        const remainingEdits = Object.keys(editState.editedValues).filter((key) => key !== rowKey).length > 0;
        onUnsavedChanges(remainingEdits);

        // Generate success message using shared helper
        const successMessage = generateSaveSuccessMessage(
          ACTIVITY_CONFIG.activityType,
          name || "the selected employee",
          request.forfeitureAmount
        );

        // Refresh grid and show success message after data loads
        if (searchParams) {
          // Use the captured page number to stay on the same page
          const skip = actualCurrentPage * pageSize;
          const params = createRequest(
            skip,
            sortParams.sortBy,
            sortParams.isSortDescending,
            selectedProfitYear,
            pageSize
          );
          if (params) {
            await triggerSearch(params, false);

            // Restore the page number in pagination state
            handlePaginationChange(actualCurrentPage, pageSize);

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

        const errorMessage = formatApiError(error, getErrorMessage(ACTIVITY_CONFIG.activityType, "save"));
        dispatch(
          setMessage({
            key: "TerminationSave",
            message: {
              message: errorMessage,
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

  const handleBulkSave = useCallback(
    async (requests: ForfeitureAdjustmentUpdateRequest[], names: string[]) => {
      const badgeNumbers = requests.map((request) => request.badgeNumber);
      editState.addLoadingRows(badgeNumbers);

      // Capture the CURRENT page number from the ref (not the stale closure value)
      const actualCurrentPage = currentPageNumberRef.current;

      setIsBulkSaveInProgress(true);

      try {
        // Transform all requests using shared helper
        const transformedRequests = prepareBulkSaveRequests(ACTIVITY_CONFIG, requests);
        await updateForfeitureAdjustmentBulk(transformedRequests);

        // Generate row keys using shared helper
        const rowKeys = getRowKeysForRequests(ACTIVITY_CONFIG, requests);
        editState.clearEditedValues(rowKeys);
        selectionState.clearSelection();

        // Clear selections in grid using shared helper
        clearGridSelectionsForBadges(gridRef.current?.api, badgeNumbers);

        // Check for remaining edits
        const remainingEditKeys = Object.keys(editState.editedValues).filter((key) => !rowKeys.includes(key));
        onUnsavedChanges(remainingEditKeys.length > 0);

        // Generate bulk success message using shared helper
        const employeeNames = names.map((name) => name || "Unknown Employee");
        const bulkSuccessMessage = generateBulkSaveSuccessMessage(
          ACTIVITY_CONFIG.activityType,
          requests.length,
          employeeNames
        );

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

        const errorMessage = formatApiError(error, getErrorMessage(ACTIVITY_CONFIG.activityType, "bulkSave"));
        dispatch(
          setMessage({
            key: "TerminationSave",
            message: {
              message: errorMessage,
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

  // Build grid data with flat combined rows (one row per employee using yearDetail matching selectedProfitYear)
  const gridData = useMemo(() => {
    if (!termination?.response?.results) return [];

    // Flatten: create one row per employee combining master data with yearDetail for current profit year
    return termination.response.results
      .map((masterRow) => {
        const badgeNumber = masterRow.psn;

        // We return 1 year for each person.
        const matchingDetail = masterRow.yearDetails[0];

        return {
          // Master row fields
          psn: masterRow.psn,
          name: masterRow.name,
          badgeNumber, // For row key generation and save operations

          // Current year detail fields
          ...matchingDetail,

          // Metadata (mark as detail row for shared components)
          isDetail: true,
          parentId: badgeNumber,
          index: 0
        };
      })
      .filter((row) => row !== null); // Remove null entries
  }, [termination]);

  const paginationHandlers = {
    setPageNumber: (value: number) => {
      if (hasUnsavedChanges) {
        onShowUnsavedChangesDialog?.();
        return;
      }
      handlePaginationChange(value - 1, pageSize);
      setInitialSearchLoaded(true);
    },
    setPageSize: (value: number) => {
      if (hasUnsavedChanges) {
        onShowUnsavedChangesDialog?.();
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
