import FullscreenIcon from "@mui/icons-material/Fullscreen";
import FullscreenExitIcon from "@mui/icons-material/FullscreenExit";
import { IconButton } from "@mui/material";
import { CellClickedEvent, ColDef, ICellRendererParams } from "ag-grid-community";
import { AgGridReact } from "ag-grid-react";
import React, { useEffect, useMemo, useRef } from "react";
import DSMPaginatedGrid from "../../../components/DSMPaginatedGrid/DSMPaginatedGrid";
import ReportSummary from "../../../components/ReportSummary";
import { GRID_KEYS } from "../../../constants";
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
  isGridExpanded?: boolean;
  onToggleExpand?: () => void;
  onCloseLeftPane?: () => void;
  onShowUnsavedChangesDialog?: () => void;
  onShowErrorDialog?: (title: string, message: string) => void;
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
  isGridExpanded = false,
  onToggleExpand,
  onCloseLeftPane: _onCloseLeftPane,
  onShowUnsavedChangesDialog,
  onShowErrorDialog
}) => {
  // Check if current navigation should be read-only
  const isReadOnly = useReadOnlyNavigation();

  // Local ref for grid API access (cell refresh)
  const localGridRef = useRef<AgGridReact | null>(null);

  const {
    pageNumber,
    pageSize,
    sortParams,
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
    onShowUnsavedChangesDialog,
    onShowErrorDialog
  });

  // Refresh grid cells when read-only status changes
  // This forces cell renderers to re-read isReadOnly from context
  useEffect(() => {
    const api = localGridRef.current?.api ?? gridRef.current?.api;
    if (api) {
      api.refreshCells({ force: true });
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

  if (!unForfeits?.response) return null;

  return (
    <>
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

      <DSMPaginatedGrid
        preferenceKey={GRID_KEYS.REHIRE_FORFEITURES}
        data={gridData ?? []}
        columnDefs={columnDefs}
        totalRecords={unForfeits.response.total || 0}
        isLoading={isFetching}
        pagination={{
          pageNumber,
          pageSize,
          sortParams,
          handlePageNumberChange: paginationHandlers.setPageNumber,
          handlePageSizeChange: paginationHandlers.setPageSize,
          handleSortChange: sortEventHandler
        }}
        onSortChange={sortEventHandler}
        header={<ReportSummary report={unForfeits} />}
        headerActions={
          onToggleExpand && (
            <IconButton
              onClick={onToggleExpand}
              sx={{ zIndex: 1 }}>
              {isGridExpanded ? <FullscreenExitIcon /> : <FullscreenIcon />}
            </IconButton>
          )
        }
        heightConfig={{
          mode: "content-aware",
          heightPercentage: isGridExpanded ? 0.85 : 0.4,
          isExpanded: isGridExpanded,
          rowHeight: 40
        }}
        gridOptions={{
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
            localGridRef.current = params as unknown as AgGridReact;
            onGridReady(params);
          },
          context: gridContext
        }}
      />
    </>
  );
};

export default UnForfeitGrid;
