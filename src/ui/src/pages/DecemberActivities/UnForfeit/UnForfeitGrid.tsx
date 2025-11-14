import { Grid } from "@mui/material";
import { CellClickedEvent, ColDef, ICellRendererParams } from "ag-grid-community";
import React, { useEffect, useMemo } from "react";
import { DSMGrid, Pagination } from "smart-ui-library";
import ReportSummary from "../../../components/ReportSummary";
import { useDynamicGridHeight } from "../../../hooks/useDynamicGridHeight";
import { useReadOnlyNavigation } from "../../../hooks/useReadOnlyNavigation";
import { CalendarResponseDto } from "../../../reduxstore/types";
import { UnForfeitGridColumns } from "./UnForfeitGridColumns";
import { GetProfitDetailColumns } from "./UnForfeitProfitDetailGridColumns";
import { useUnForfeitGrid } from "./useUnForfeitGrid";

interface UnForfeitGridSearchProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  resetPageFlag: boolean;
  onUnsavedChanges: (hasChanges: boolean) => void;
  hasUnsavedChanges: boolean;
  shouldArchive: boolean;
  onArchiveHandled?: () => void;
  setHasUnsavedChanges: (hasChanges: boolean) => void;
  fiscalCalendarYear: CalendarResponseDto | null;
  onShowUnsavedChangesDialog?: () => void;
}

const UnForfeitGrid: React.FC<UnForfeitGridSearchProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
  resetPageFlag,
  onUnsavedChanges,
  hasUnsavedChanges,
  shouldArchive,
  onArchiveHandled,
  setHasUnsavedChanges,
  fiscalCalendarYear,
  onShowUnsavedChangesDialog
}) => {
  // Use dynamic grid height utility hook
  const gridMaxHeight = useDynamicGridHeight();

  // Check if current navigation should be read-only
  const isReadOnly = useReadOnlyNavigation();

  const {
    pageNumber,
    pageSize,
    gridData,
    isFetching,

    unForfeits,
    selectedProfitYear,

    selectionState,
    handleSave,
    handleBulkSave,
    handleRowExpansion,
    sortEventHandler,
    onGridReady,
    paginationHandlers,
    gridRef,
    gridContext
  } = useUnForfeitGrid({
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
    onShowUnsavedChangesDialog
  });

  // Refresh grid cells when read-only status changes
  // This forces cell renderers to re-read isReadOnly from context
  useEffect(() => {
    if (gridRef.current?.api) {
      gridRef.current.api.refreshCells({ force: true });
    }
    // gridRef is a ref and doesn't need to be in the dependency array
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isReadOnly]);

  // Get the main and detail columns
  const mainColumns = useMemo(() => UnForfeitGridColumns(), []);
  const detailColumns = useMemo(
    () =>
      GetProfitDetailColumns(
        selectionState.addRowToSelection,
        selectionState.removeRowFromSelection,
        selectedProfitYear,
        handleSave,
        handleBulkSave,
        isReadOnly
      ),
    [
      selectionState.addRowToSelection,
      selectionState.removeRowFromSelection,
      selectedProfitYear,
      handleSave,
      handleBulkSave,
      isReadOnly
    ]
  );

  // Create column definitions with expand/collapse functionality and combine main/detail columns
  const columnDefs = useMemo(() => {
    // Add an expansion column
    const expansionColumn = {
      headerName: "",
      field: "isExpandable",
      width: 50,
      cellRenderer: (params: ICellRendererParams) => {
        if (params.data && !params.data.isDetail && params.data.isExpandable) {
          return params.data.isExpanded ? "▼" : "►";
        }
        return "";
      },
      onCellClicked: (event: CellClickedEvent) => {
        if (event.data && !event.data.isDetail && event.data.isExpandable) {
          handleRowExpansion(event.data.badgeNumber.toString());
        }
      },
      suppressSizeToFit: true,
      suppressAutoSize: true,
      lockVisible: true,
      lockPosition: true,
      pinned: "left" as const
    } as ColDef;

    // For main columns, hide content for detail rows unless the same field exists in detail columns
    const visibleColumns = mainColumns.map((column) => {
      return {
        ...column,
        cellRenderer: (params: ICellRendererParams) => {
          if (params.data?.isDetail) {
            const hideInDetails = !detailColumns.some((detailCol) => detailCol.field === (column as ColDef).field);
            if (hideInDetails) {
              return "";
            }
          }

          if ((column as ColDef).cellRenderer) {
            return (column as ColDef).cellRenderer(params);
          }
          return params.valueFormatted ? params.valueFormatted : params.value;
        }
      } as ColDef;
    });

    // Add detail-specific columns that only appear for detail rows
    const detailOnlyColumns = detailColumns
      .filter((detailCol) => !mainColumns.some((mainCol) => mainCol.field === detailCol.field))
      .map(
        (column) =>
          ({
            ...column,
            cellRenderer: (params: ICellRendererParams) => {
              if (!params.data?.isDetail) {
                return "";
              }
              if ((column as ColDef).cellRenderer) {
                return (column as ColDef).cellRenderer(params);
              }
              return params.valueFormatted ? params.valueFormatted : params.value;
            }
          }) as ColDef
      );

    const finalColumns = [expansionColumn, ...visibleColumns, ...detailOnlyColumns];

    return finalColumns;
  }, [mainColumns, detailColumns, handleRowExpansion]);

  return (
    <div>
      <style>
        {`
          .detail-row {
            background-color: #f5f5f5;
          }
          .invalid-cell {
            background-color: #fff6f6;
          }
        `}
      </style>

      {unForfeits?.response && (
        <>
          <Grid
            container
            justifyContent="space-between"
            alignItems="center"
            marginBottom={2}>
            <Grid>
              <ReportSummary report={unForfeits} />
            </Grid>
          </Grid>

          <DSMGrid
            preferenceKey={"REHIRE-FORFEITURES"}
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            maxHeight={gridMaxHeight}
            providedOptions={{
              rowData: gridData,
              columnDefs: columnDefs,
              getRowClass: (params) => ((params.data as { isDetail?: boolean })?.isDetail ? "detail-row" : ""),
              rowSelection: {
                mode: "multiRow",
                checkboxes: false,
                headerCheckbox: false,
                enableClickSelection: false
              },
              rowHeight: 40,
              suppressMultiSort: true,
              defaultColDef: {
                resizable: true
              },
              onGridReady: (params) => {
                gridRef.current = params;
                onGridReady(params);
              },
              context: gridContext
            }}
          />

          {!!unForfeits && unForfeits.response.results.length > 0 && (
            <Pagination
              pageNumber={pageNumber}
              setPageNumber={paginationHandlers.setPageNumber}
              pageSize={pageSize}
              setPageSize={paginationHandlers.setPageSize}
              recordCount={unForfeits.response.total || 0}
            />
          )}
        </>
      )}
    </div>
  );
};

export default UnForfeitGrid;
