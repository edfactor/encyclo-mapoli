import { Grid } from "@mui/material";
import { ColDef, ICellRendererParams } from "ag-grid-community";
import React, { useMemo } from "react";
import { DSMGrid, Pagination } from "smart-ui-library";
import ReportSummary from "../../../components/ReportSummary";
import { useUnForfeitGrid } from "../../../hooks/useUnForfeitGrid";
import { CalendarResponseDto } from "../../../reduxstore/types";
import { GetProfitDetailColumns } from "./RehireForfeituresProfitDetailGridColumns";
import { GetRehireForfeituresGridColumns } from "./RehireForfeituresGridColumns";

interface RehireForfeituresGridSearchProps {
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

const RehireForfeituresGrid: React.FC<RehireForfeituresGridSearchProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
  resetPageFlag,
  onUnsavedChanges,
  hasUnsavedChanges,
  shouldArchive,
  onArchiveHandled,
  setHasUnsavedChanges,
  fiscalCalendarYear
}) => {
  const {
    pageNumber,
    pageSize,
    gridData,
    isFetching,
    rehireForfeitures,
    selectedProfitYear,
    editState,
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
    fiscalCalendarYear
  });

  // Get the main and detail columns
  const mainColumns = useMemo(() => GetRehireForfeituresGridColumns(), []);
  const detailColumns = useMemo(
    () =>
      GetProfitDetailColumns(
        selectionState.addRowToSelection,
        selectionState.removeRowFromSelection,
        selectedProfitYear,
        handleSave,
        handleBulkSave
      ),
    [selectionState.addRowToSelection, selectionState.removeRowFromSelection, selectedProfitYear, handleSave, handleBulkSave]
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
      onCellClicked: (event: any) => {
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
            const hideInDetails = !detailColumns.some((detailCol) => detailCol.field === (column as any).field);
            if (hideInDetails) {
              return "";
            }
          }

          if ((column as any).cellRenderer) {
            return (column as any).cellRenderer(params);
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
              if ((column as any).cellRenderer) {
                return (column as any).cellRenderer(params);
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

      {rehireForfeitures?.response && (
        <>
          <Grid
            container
            justifyContent="space-between"
            alignItems="center"
            marginBottom={2}>
            <Grid>
              <ReportSummary report={rehireForfeitures} />
            </Grid>
          </Grid>

          <DSMGrid
            preferenceKey={"REHIRE-FORFEITURES"}
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            maxHeight={400}
            providedOptions={{
              rowData: gridData,
              columnDefs: columnDefs,
              getRowClass: (params: any) => (params.data.isDetail ? "detail-row" : ""),
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
              onGridReady: (params) => {
                gridRef.current = params;
                onGridReady(params);
              },
              context: gridContext
            }}
          />

          {!!rehireForfeitures && rehireForfeitures.response.results.length > 0 && (
            <Pagination
              pageNumber={pageNumber}
              setPageNumber={paginationHandlers.setPageNumber}
              pageSize={pageSize}
              setPageSize={paginationHandlers.setPageSize}
              recordCount={rehireForfeitures.response.total || 0}
            />
          )}
        </>
      )}
    </div>
  );
};

export default RehireForfeituresGrid;