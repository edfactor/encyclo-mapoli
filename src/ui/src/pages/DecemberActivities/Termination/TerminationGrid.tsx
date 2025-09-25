import { CellClickedEvent, ColDef, ICellRendererParams } from "ag-grid-community";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  useLazyGetTerminationReportQuery,
  useUpdateForfeitureAdjustmentBulkMutation,
  useUpdateForfeitureAdjustmentMutation
} from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { CalendarResponseDto, ForfeitureAdjustmentUpdateRequest, StartAndEndDateRequest } from "reduxstore/types";
import {
  DSMGrid,
  formatNumberWithComma,
  numberToCurrency,
  Pagination,
  setMessage
} from "smart-ui-library";
import { ReportSummary } from "../../../components/ReportSummary";
import { TotalsGrid } from "../../../components/TotalsGrid/TotalsGrid";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { useEditState } from "../../../hooks/useEditState";
import { useGridPagination } from "../../../hooks/useGridPagination";
import { useRowSelection } from "../../../hooks/useRowSelection";
import { Messages } from "../../../utils/messageDictonary";
import { TerminationSearchRequest } from "./Termination";
import { GetDetailColumns } from "./TerminationDetailsGridColumns";
import { GetTerminationColumns } from "./TerminationGridColumns";

interface TerminationGridSearchProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  searchParams: TerminationSearchRequest | null;
  resetPageFlag: boolean;
  onUnsavedChanges: (hasChanges: boolean) => void;
  hasUnsavedChanges: boolean;
  fiscalData: CalendarResponseDto | null;
  shouldArchive?: boolean;
  onArchiveHandled?: () => void;
  onErrorOccurred?: () => void; // Add this prop
}

const TerminationGrid: React.FC<TerminationGridSearchProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
  searchParams,
  resetPageFlag,
  onUnsavedChanges,
  hasUnsavedChanges,
  fiscalData,
  shouldArchive,
  onArchiveHandled,
  onErrorOccurred
}) => {
  const [expandedRows, setExpandedRows] = useState<Record<string, boolean>>({});
  const { termination } = useSelector((state: RootState) => state.yearsEnd);
  const dispatch = useDispatch();
  const [triggerSearch, { isFetching }] = useLazyGetTerminationReportQuery();
  const [pendingSuccessMessage, setPendingSuccessMessage] = useState<string | null>(null);
  const [isPendingBulkMessage, setIsPendingBulkMessage] = useState<boolean>(false);
  const selectedProfitYear = useDecemberFlowProfitYear();
  // fiscalData is now passed from parent to avoid timing issues on refresh
  const [updateForfeitureAdjustmentBulk, { isLoading: isBulkSaving }] = useUpdateForfeitureAdjustmentBulkMutation();
  const [updateForfeitureAdjustment, { isLoading: isSingleSaving }] = useUpdateForfeitureAdjustmentMutation();
  const lastRequestKeyRef = useRef<string | null>(null);

  // Use separate hooks for edit and selection state
  const editState = useEditState();
  const selectionState = useRowSelection();

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

  const { pageNumber, pageSize, sortParams, handlePaginationChange, handleSortChange, resetPagination } = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "badgeNumber",
    initialSortDescending: true,
    onPaginationChange: useCallback(async (pageNum: number, pageSz: number, sortPrms: any) => {
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
    }, [initialSearchLoaded, searchParams, createRequest, selectedProfitYear, triggerSearch])
  });

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

  // Reusable function to refresh grid after save operations
  const refreshGridAfterSave = useCallback((successMessage: string, isBulk = false) => {
    if (searchParams) {
      setPendingSuccessMessage(successMessage);
      setIsPendingBulkMessage(isBulk);
      // The unified useEffect will handle the actual API call
    } else {
      // If no search params, show message immediately
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
  }, [searchParams, dispatch]);

  // Add state to track current scroll position
  const [gridScrollPosition, setGridScrollPosition] = useState<{ top: number, left: number } | null>(null);
  
  // Add reference to track if we're currently restoring scroll position
  const isRestoringScrollRef = useRef(false);

  // Add state to track when to refresh data after save
  const [shouldRefreshData, setShouldRefreshData] = useState(false);
  const [saveSuccessType, setSaveSuccessType] = useState<'single' | 'bulk' | null>(null);
  const [successMessages, setSuccessMessages] = useState<{ message: string, type: 'single' | 'bulk' }[]>([]);

  // Define a function to refresh the data after save
  const refreshDataAfterSave = useCallback((successMessage: string, isBulk: boolean) => {
    if (searchParams) {
      // Store success message to display after refresh
      setSuccessMessages(prev => [...prev, { 
        message: successMessage, 
        type: isBulk ? 'bulk' : 'single' 
      }]);
      
      // Set flag to trigger refresh in the useEffect
      setShouldRefreshData(true);
      setSaveSuccessType(isBulk ? 'bulk' : 'single');
    } else {
      // If no search params, just show message immediately
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
  }, [searchParams, dispatch]);

  // Effect to refresh data after save
  useEffect(() => {
    const refreshData = async () => {
      if (shouldRefreshData && searchParams) {
        try {
          // Store current scroll position before refreshing data
          if (gridRef.current?.api) {
            try {
              // Get the scrollable viewport container - use proper AG Grid API methods
              const gridBodyViewport = document.querySelector('.ag-body-viewport');
              if (gridBodyViewport) {
                setGridScrollPosition({
                  top: gridBodyViewport.scrollTop,
                  left: gridBodyViewport.scrollLeft
                });
                
                // Log for debugging
                console.log('Saving scroll position:', gridBodyViewport.scrollTop, gridBodyViewport.scrollLeft);
              }
            } catch (err) {
              console.error("Error saving scroll position:", err);
            }
          }
          
          // Construct search params with current pagination and sort
          const params = createRequest(
            pageNumber * pageSize,
            sortParams.sortBy,
            sortParams.isSortDescending,
            selectedProfitYear
          );
          
          if (params) {
            // Perform the search to get fresh data
            await triggerSearch(params, false);
            
            // Show success messages after refresh
            if (successMessages.length > 0) {
              // Get the most recent message
              const latestMessage = successMessages[successMessages.length - 1];
              
              // Show appropriate message based on type
              const messageTemplate = latestMessage.type === 'bulk' 
                ? Messages.TerminationBulkSaveSuccess 
                : Messages.TerminationSaveSuccess;
                
              dispatch(
                setMessage({
                  ...messageTemplate,
                  message: {
                    ...messageTemplate.message,
                    message: latestMessage.message
                  }
                })
              );
              
              // Clear messages after displaying
              setSuccessMessages([]);
            }
          }
        } catch (error) {
          console.error("Error refreshing termination data:", error);
          if (onErrorOccurred) {
            onErrorOccurred();
          }
        } finally {
          // Reset refresh flag
          setShouldRefreshData(false);
          setSaveSuccessType(null);
        }
      }
    };
    
    refreshData();
  }, [shouldRefreshData, searchParams, pageNumber, pageSize, sortParams, triggerSearch, dispatch, successMessages, onErrorOccurred, selectedProfitYear, createRequest]);

  // Add effect to restore scroll position after data refresh
  useEffect(() => {
    const restoreScrollPosition = () => {
      if (gridRef.current?.api && gridScrollPosition && !isFetching && !isRestoringScrollRef.current) {
        // Set flag to prevent multiple scroll attempts
        isRestoringScrollRef.current = true;
        
        // Need to wait for the grid to fully render after data load
        setTimeout(() => {
          try {
            // Get the scrollable viewport container - use DOM selector instead of API method
            const gridBodyViewport = document.querySelector('.ag-body-viewport');
            if (gridBodyViewport) {
              // Set the scroll position directly
              gridBodyViewport.scrollTop = gridScrollPosition.top;
              gridBodyViewport.scrollLeft = gridScrollPosition.left;
              
              // Log for debugging
              console.log('Restored scroll position:', gridScrollPosition);
            }
          } catch (err) {
            console.error('Error restoring scroll position:', err);
          } finally {
            // Reset scroll position state
            setGridScrollPosition(null);
            
            // Reset the flag after a longer delay to ensure the scroll has been applied
            setTimeout(() => {
              isRestoringScrollRef.current = false;
            }, 300);
          }
        }, 250); // Increased delay to ensure grid is fully rendered
      }
    };
    
    restoreScrollPosition();
  }, [gridScrollPosition, isFetching]);

  const handleSave = useCallback(
    async (request: ForfeitureAdjustmentUpdateRequest, name: string) => {
      const rowId = request.badgeNumber; // Use badgeNumber as unique identifier
      editState.addLoadingRow(rowId);

      try {
        await updateForfeitureAdjustment(request);
        const rowKey = `${request.badgeNumber}-${request.profitYear}`;
        editState.removeEditedValue(rowKey);
        // Check for remaining edits after removing this one
        const remainingEdits = Object.keys(editState.editedValues).filter(key => key !== rowKey).length > 0;
        onUnsavedChanges(remainingEdits);

        // Prepare success message and refresh grid
        const employeeName = name || "the selected employee";
        const successMessage = `The forfeiture adjustment of amount $${formatNumberWithComma(request.forfeitureAmount)} for ${employeeName} saved successfully`;
        
        // Use the new function to refresh data and show success message
        refreshDataAfterSave(successMessage, false);
      } catch (error) {
        console.error("Failed to save forfeiture adjustment:", error);
        
        // Show error message
        dispatch(
          setMessage({
            key: "TerminationSave",
            message: {
              message: `Failed to save adjustment for ${name || "employee"}.`,
              type: "error"
            }
          })
        );
        
        // Call the passed-in error handler instead of internal scrollToTop
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
      refreshDataAfterSave,
      dispatch,
      onErrorOccurred
    ]
  );

  // Reset page number to 0 when resetPageFlag changes
  useEffect(() => {
    resetPagination();
  }, [resetPageFlag, resetPagination]);

  // Track unsaved changes based on edit state only
  useEffect(() => {
    onUnsavedChanges(editState.hasAnyEdits);
  }, [editState.hasAnyEdits, onUnsavedChanges]);

  // Refresh the grid when loading state changes
  const gridRef = useRef<any>(null);
  useEffect(() => {
    if (gridRef.current?.api) {
      gridRef.current.api.refreshCells({
        force: true,
        suppressFlash: false
      });
    }
  }, [editState.loadingRowIds]);

  // Initialize expandedRows when data is loaded
  useEffect(() => {
    if (termination?.response?.results && termination.response.results.length > 0) {
      // Only reset if badgeNumbers have changed
      const badgeNumbers = termination.response.results.map((row: any) => row.badgeNumber).join(",");
      const prevBadgeNumbers = Object.keys(expandedRows).join(",");
      if (badgeNumbers !== prevBadgeNumbers) {
        const initialExpandState: Record<string, boolean> = {};
        termination.response.results.forEach((row: any) => {
          // Set to TRUE to auto-expand rows with details!
          if (row.yearDetails && row.yearDetails.length > 0) {
            initialExpandState[row.badgeNumber] = true;
          }
        });
        setExpandedRows(initialExpandState);
      }
    }
  }, [termination?.response?.results]);

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
    // Don't load data until search button is clicked
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
          (params as any).beginningDate,
          (params as any).endingDate,
          (params as any).archive
        );

        // Prevent duplicate requests
        if (lastRequestKeyRef.current === key) {
          // If shouldArchive was true, clear it since we've already handled it
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
    shouldArchive, // Include shouldArchive in dependencies
    triggerSearch,
    createRequest,
    initialSearchLoaded,
    buildRequestKey,
    onArchiveHandled
  ]);

  const handleRowExpansion = (badgeNumber: string) => {
    setExpandedRows((prev) => ({
      ...prev,
      [badgeNumber]: !prev[badgeNumber]
    }));
  };

  const addRowToSelectedRows = selectionState.addRowToSelection;
  const removeRowFromSelectedRows = selectionState.removeRowFromSelection;
  const updateEditedValue = editState.updateEditedValue;

  const handleBulkSave = useCallback(
    async (requests: ForfeitureAdjustmentUpdateRequest[], names: string[]) => {
      // Add all affected badge numbers to loading state
      const badgeNumbers = requests.map((request) => request.badgeNumber);
      editState.addLoadingRows(badgeNumbers);

      try {
        // Call API to save bulk changes
        await updateForfeitureAdjustmentBulk(requests);

        // Clear edited values for saved requests
        const rowKeys = requests.map(request => `${request.badgeNumber}-${request.profitYear}`);
        editState.clearEditedValues(rowKeys);

        // Clear selection
        selectionState.clearSelection();

        // Check for remaining edits
        const remainingEditKeys = Object.keys(editState.editedValues).filter(key => !rowKeys.includes(key));
        onUnsavedChanges(remainingEditKeys.length > 0);

        // Prepare bulk success message and refresh grid
        const employeeNames = names.map(name => name || "Unknown Employee");
        const bulkSuccessMessage = `Members affected: ${employeeNames.join("; ")}`;
        
        // Use the new function to refresh data and show success message
        refreshDataAfterSave(bulkSuccessMessage, true);
      } catch (error) {
        console.error("Failed to save forfeiture adjustments:", error);
        
        // Show error message
        dispatch(
          setMessage({
            key: "TerminationSave",
            message: {
              message: `Failed to save bulk adjustments.`,
              type: "error"
            }
          })
        );
        
        // Call the passed-in error handler instead of internal scrollToTop
        if (onErrorOccurred) {
          onErrorOccurred();
        }
      } finally {
        // Remove all affected badge numbers from loading state
        editState.removeLoadingRows(badgeNumbers);
      }
    },
    [
      updateForfeitureAdjustmentBulk,
      editState,
      selectionState,
      onUnsavedChanges,
      refreshDataAfterSave,
      dispatch,
      onErrorOccurred
    ]
  );

  // Get main and detail columns
  const mainColumns = useMemo(() => GetTerminationColumns(), []);
  const detailColumns = useMemo(
    () =>
      GetDetailColumns(
        addRowToSelectedRows,
        removeRowFromSelectedRows,
        selectionState.selectedRowIds,
        selectedProfitYear,
        handleSave,
        handleBulkSave
      ),
    [addRowToSelectedRows, removeRowFromSelectedRows, selectionState.selectedRowIds, selectedProfitYear, handleSave, handleBulkSave]
  );

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
        isExpanded: hasDetails && Boolean(expandedRows[row.badgeNumber])
      });

      // Add detail rows if expanded
      if (hasDetails && expandedRows[row.badgeNumber]) {
        for (const detail of row.yearDetails) {
          // Create a base detail row with all parent properties to prevent undefined values
          // and then override with detail properties
          const detailRow = {
            // Copy all parent row properties first
            ...row,
            // Then add detail properties, which will override any duplicate fields
            ...detail,
            // Add special properties for UI handling
            isDetail: true,
            parentId: row.badgeNumber
          };

          rows.push(detailRow);
        }
      }
    }

    return rows;
  }, [termination, expandedRows]);

  // Compose columns: show main columns for parent, detail columns for detail
  const columnDefs = useMemo(() => {
    // Add an expansion column as the first column
    const expansionColumn = {
      headerName: "",
      field: "isExpandable",
      width: 50,
      cellRenderer: (params: ICellRendererParams) => {
        if (!params.data.isDetail && params.data.isExpandable) {
          return params.data.isExpanded ? "▼" : "►";
        }
        return "";
      },
      onCellClicked: (params: CellClickedEvent) => {
        if (!params.data.isDetail && params.data.isExpandable) {
          handleRowExpansion(params.data.badgeNumber);
        }
      },
      suppressSizeToFit: true,
      suppressAutoSize: true,
      lockVisible: true,
      lockPosition: true,
      pinned: "left"
    } as ColDef;

    // Determine which columns to display based on whether it's a detail row
    const visibleColumns = mainColumns.map((column) => {
      return {
        ...column,
        cellRenderer: (params: ICellRendererParams) => {
          // For detail rows, either hide the column or show a specific value
          if (params.data.isDetail) {
            // Check if this main column should be hidden in detail rows
            const hideInDetails = !detailColumns.some((detailCol) => detailCol.field === column.field);

            if (hideInDetails) {
              return ""; // Hide this column's content for detail rows
            }
          }

          // Use the default renderer for this column if available
          if (column.cellRenderer) {
            return column.cellRenderer(params);
          }

          // Otherwise just return the field value
          return params.valueFormatted ? params.valueFormatted : params.value;
        }
      };
    });

    // Add detail-specific columns that only appear for detail rows
    const detailOnlyColumns = detailColumns
      .filter((detailCol) => !mainColumns.some((mainCol) => mainCol.field === detailCol.field))
      .map((column) => {
        return {
          ...column,
          cellRenderer: (params: ICellRendererParams) => {
            // Only show content for detail rows
            if (!params.data.isDetail) {
              return "";
            }

            // Use the default renderer for this column if available
            if (column.cellRenderer) {
              return column.cellRenderer(params);
            }

            // Otherwise just return the field value
            return params.valueFormatted ? params.valueFormatted : params.value;
          }
        };
      });

    // Combine all columns
    return [expansionColumn, ...visibleColumns, ...detailOnlyColumns];
  }, [mainColumns, detailColumns]);

  // Row class for detail rows using Tailwind
  const getRowClass = (params: { data: { isDetail: boolean } }) => {
    return params.data.isDetail ? "bg-gray-100" : "";
  };

  const sortEventHandler = (update: any) => {
    handleSortChange(update);
  };

  return (
    <div className="relative">
      {/* Show loading overlay when fetching data */}
      {(isFetching) && (
        <div className="absolute inset-0 w-full h-full bg-white/60 z-[1000] flex items-center justify-center">
          <div
            className="spinner-border w-12 h-12"
            role="status">
            <span className="visually-hidden">Loading...</span>
          </div>
        </div>
      )}

      {(isBulkSaving || isSingleSaving) && (
        <div className="absolute inset-0 w-full h-full bg-white/60 z-[1000] flex items-center justify-center">
          <div
            className="spinner-border w-12 h-12"
            role="status">
            <span className="visually-hidden">Saving...</span>
          </div>
        </div>
      )}
      
      {termination?.response && (
        <>
          <ReportSummary report={termination} />
          
          <div className="sticky top-0 z-10 flex bg-white">
            <TotalsGrid
              displayData={[[numberToCurrency(termination.totalEndingBalance || 0)]]}
              leftColumnHeaders={["Amount in Profit Sharing"]}
              topRowHeaders={[]}></TotalsGrid>
            <TotalsGrid
              displayData={[[numberToCurrency(termination.totalVested || 0)]]}
              leftColumnHeaders={["Vested Amount"]}
              topRowHeaders={[]}></TotalsGrid>
            <TotalsGrid
              displayData={[[numberToCurrency(termination.totalForfeit || 0)]]}
              leftColumnHeaders={["Total Forfeitures"]}
              topRowHeaders={[]}></TotalsGrid>
            <TotalsGrid
              displayData={[[numberToCurrency(termination.totalBeneficiaryAllocation || 0)]]}
              leftColumnHeaders={["Total Beneficiary Allocations"]}
              topRowHeaders={[]}></TotalsGrid>
          </div>

          <DSMGrid
            preferenceKey={"QPREV-PROF"}
            handleSortChanged={sortEventHandler}
            maxHeight={400}
            isLoading={isFetching}
            providedOptions={{
              onGridReady: (params) => {
                gridRef.current = params;
              },
              rowData: gridData,
              columnDefs: columnDefs,
              getRowClass: getRowClass,
              rowSelection: {
                mode: "multiRow",
                checkboxes: true,
                headerCheckbox: true,
                enableClickSelection: false
              },
              rowHeight: 40,
              suppressMultiSort: true,
              defaultColDef: {
                resizable: true
              },
              context: {
                editedValues: editState.editedValues,
                updateEditedValue,
                loadingRowIds: editState.loadingRowIds
              }
            }}
          />

          {!!termination && termination.response.results.length > 0 && (
            <Pagination
              pageNumber={pageNumber}
              setPageNumber={(value: number) => {
                if (hasUnsavedChanges) {
                  alert("Please save your changes.");
                  return;
                }
                handlePaginationChange(value - 1, pageSize);
                setInitialSearchLoaded(true);
              }}
              pageSize={pageSize}
              setPageSize={(value: number) => {
                if (hasUnsavedChanges) {
                  alert("Please save your changes.");
                  return;
                }
                handlePaginationChange(0, value);
                setInitialSearchLoaded(true);
              }}
              recordCount={termination.response.total || 0}
            />
          )}
        </>
      )}
    </div>
  );
};

export default TerminationGrid;
