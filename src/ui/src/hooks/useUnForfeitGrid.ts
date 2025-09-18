import { GridApi } from "ag-grid-community";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  useLazyGetRehireForfeituresQuery,
  useUpdateForfeitureAdjustmentBulkMutation,
  useUpdateForfeitureAdjustmentMutation
} from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { CalendarResponseDto, ForfeitureAdjustmentUpdateRequest, StartAndEndDateRequest } from "reduxstore/types";
import { formatNumberWithComma, ISortParams, setMessage } from "smart-ui-library";
import useDecemberFlowProfitYear from "./useDecemberFlowProfitYear";
import { useEditState } from "./useEditState";
import { useRowSelection } from "./useRowSelection";
import { Messages } from "../utils/messageDictonary";

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
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "fullName",
    isSortDescending: false
  });
  const [expandedRows, setExpandedRows] = useState<Record<string, boolean>>({});
  const [gridApi, setGridApi] = useState<GridApi | null>(null);
  const [pendingSuccessMessage, setPendingSuccessMessage] = useState<string | null>(null);
  const [isPendingBulkMessage, setIsPendingBulkMessage] = useState<boolean>(false);

  const selectedProfitYear = useDecemberFlowProfitYear();
  const { rehireForfeitures, rehireForfeituresQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const dispatch = useDispatch();

  const [triggerSearch, { isFetching }] = useLazyGetRehireForfeituresQuery();
  const [updateForfeitureAdjustmentBulk] = useUpdateForfeitureAdjustmentBulkMutation();
  const [updateForfeitureAdjustment] = useUpdateForfeitureAdjustmentMutation();

  const editState = useEditState();
  const selectionState = useRowSelection();
  const gridRef = useRef<any>(null);
  const prevRehireForfeitures = useRef<any>(null);

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
  useEffect(() => {
    if (
      rehireForfeitures !== prevRehireForfeitures.current &&
      rehireForfeitures?.response?.total !== undefined &&
      rehireForfeitures.response.total !== prevRehireForfeitures.current?.response?.total
    ) {
      setPageNumber(0);
    }
    prevRehireForfeitures.current = rehireForfeitures;
  }, [rehireForfeitures]);

  // Create a request object based on current parameters
  const createRequest = useCallback(
    (
      skip: number,
      sortBy: string,
      isSortDescending: boolean
    ): (StartAndEndDateRequest & { archive?: boolean }) | null => {
      const baseRequest: StartAndEndDateRequest = {
        beginningDate: rehireForfeituresQueryParams?.beginningDate || fiscalCalendarYear?.fiscalBeginDate || "",
        endingDate: rehireForfeituresQueryParams?.endingDate || fiscalCalendarYear?.fiscalEndDate || "",
        profitYear: selectedProfitYear,
        pagination: { skip, take: pageSize, sortBy, isSortDescending }
      };

      if (!baseRequest.beginningDate || !baseRequest.endingDate) return null;

      const finalRequest = shouldArchive ? { ...baseRequest, archive: true } : baseRequest;
      return finalRequest;
    },
    [
      rehireForfeituresQueryParams,
      fiscalCalendarYear?.fiscalBeginDate,
      fiscalCalendarYear?.fiscalEndDate,
      pageSize,
      shouldArchive,
      selectedProfitYear
    ]
  );

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

          if (rehireForfeituresQueryParams) {
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
      rehireForfeituresQueryParams,
      pageNumber,
      pageSize,
      sortParams,
      createRequest,
      triggerSearch,
      gridApi
    ]
  );

  const handleBulkSave = useCallback(
    async (requests: ForfeitureAdjustmentUpdateRequest[], names: string[]) => {
      const badgeNumbers = requests.map((request) => request.badgeNumber);
      editState.addLoadingRows(badgeNumbers);

      try {
        await updateForfeitureAdjustmentBulk(requests);
        const rowKeys = requests.map(request => `${request.badgeNumber}-${request.profitYear}`);
        editState.clearEditedValues(rowKeys);
        selectionState.clearSelection();

        const employeeNames = names.map((name) => name || "Unknown Employee");
        const bulkSuccessMessage = `Members affected: ${employeeNames.join("; ")}`;

        if (rehireForfeituresQueryParams) {
          const request = createRequest(
            pageNumber * pageSize,
            sortParams.sortBy,
            sortParams.isSortDescending
          );
          if (request) {
            await triggerSearch(request, false);
            setPendingSuccessMessage(bulkSuccessMessage);
            setIsPendingBulkMessage(true);
          }
        }
      } catch (error) {
        console.error("Failed to save forfeiture adjustments:", error);
        alert("Failed to save one or more adjustments. Please try again.");
      } finally {
        editState.removeLoadingRows(badgeNumbers);
      }
    },
    [
      updateForfeitureAdjustmentBulk,
      editState,
      selectionState,
      rehireForfeituresQueryParams,
      pageNumber,
      pageSize,
      sortParams,
      createRequest,
      triggerSearch
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

  // Reset page number to 0 when resetPageFlag changes
  useEffect(() => {
    setPageNumber(0);
  }, [resetPageFlag]);

  useEffect(() => {
    const hasChanges = selectionState.selectedRowIds.length > 0 || Object.keys(editState.editedValues).length > 0;
    onUnsavedChanges(hasChanges);
    setHasUnsavedChanges(hasChanges);
  }, [selectionState.selectedRowIds, editState.editedValues, onUnsavedChanges, setHasUnsavedChanges]);

  // Initialize expandedRows when data is loaded
  useEffect(() => {
    if (rehireForfeitures?.response?.results && rehireForfeitures.response.results.length > 0) {
      const initialExpandState: Record<string, boolean> = {};
      rehireForfeitures.response.results.forEach((row: any) => {
        const hasDetails = row.details && row.details.length > 0;
        if (hasDetails) {
          initialExpandState[row.badgeNumber.toString()] = true;
        }
      });
      setExpandedRows(initialExpandState);
    }
  }, [rehireForfeitures?.response?.results]);

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
  const sortEventHandler = (update: ISortParams) => {
    setSortParams(update);
    setPageNumber(0);
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
    if (!rehireForfeitures?.response?.results) return [];

    const rows = [];

    for (const row of rehireForfeitures.response.results) {
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
  }, [rehireForfeitures, expandedRows]);

  const paginationHandlers = {
    setPageNumber: (value: number) => {
      if (hasUnsavedChanges) {
        alert("Please save your changes.");
        return;
      }
      setPageNumber(value - 1);
      setInitialSearchLoaded(true);
    },
    setPageSize: (value: number) => {
      if (hasUnsavedChanges) {
        alert("Please save your changes.");
        return;
      }
      setPageSize(value);
      setPageNumber(0);
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
    rehireForfeitures,
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