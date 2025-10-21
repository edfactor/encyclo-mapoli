import { CellClickedEvent, ColDef, ICellRendererParams } from "ag-grid-community";
import { useMemo } from "react";
import { DSMGrid, numberToCurrency, Pagination, TotalsGrid } from "smart-ui-library";
import  ReportSummary  from "../../../components/ReportSummary";
import { useDynamicGridHeight } from "../../../hooks/useDynamicGridHeight";
import { useReadOnlyNavigation } from "../../../hooks/useReadOnlyNavigation";
import { useTerminationGrid } from "../../../hooks/useTerminationGrid";
import { CalendarResponseDto } from "../../../reduxstore/types";
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
  onLoadingChange?: (isLoading: boolean) => void;
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
  onErrorOccurred,
  onLoadingChange
}) => {
  // Use dynamic grid height utility hook
  const gridMaxHeight = useDynamicGridHeight();

  // Check if current navigation should be read-only
  const isReadOnly = useReadOnlyNavigation();

  const {
    pageNumber,
    pageSize,
    gridData,

    termination,
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
  } = useTerminationGrid({
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
  });

  // Get main and detail columns
  const mainColumns = useMemo(() => GetTerminationColumns(), []);
  const detailColumns = useMemo(
    () =>
      GetDetailColumns(
        selectionState.addRowToSelection,
        selectionState.removeRowFromSelection,
        selectionState.selectedRowIds,
        selectedProfitYear,
        handleSave,
        handleBulkSave,
        isReadOnly
      ),
    [
      selectionState.addRowToSelection,
      selectionState.removeRowFromSelection,
      selectionState.selectedRowIds,
      selectedProfitYear,
      handleSave,
      handleBulkSave,
      isReadOnly
    ]
  );

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
  }, [mainColumns, detailColumns, handleRowExpansion]);

  // Row class for detail rows using Tailwind
  const getRowClass = (params: { data: { isDetail: boolean } }) => {
    return params.data.isDetail ? "bg-gray-100" : "";
  };

  return (
    <div className="relative">
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
            handleSortChanged={sortEventHandler}
            maxHeight={gridMaxHeight}
            isLoading={false}
            providedOptions={{
              onGridReady: (params) => {
                gridRef.current = params;
                onGridReady(params);
              },
              rowData: gridData,
              columnDefs: columnDefs,
              getRowClass: getRowClass,
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
              context: gridContext
            }}
          />

          {!!termination && termination.response.results.length > 0 && (
            <Pagination
              pageNumber={pageNumber}
              setPageNumber={paginationHandlers.setPageNumber}
              pageSize={pageSize}
              setPageSize={paginationHandlers.setPageSize}
              recordCount={termination.response.total || 0}
            />
          )}
        </>
      )}
    </div>
  );
};

export default TerminationGrid;
