import FullscreenIcon from "@mui/icons-material/Fullscreen";
import FullscreenExitIcon from "@mui/icons-material/FullscreenExit";
import { IconButton } from "@mui/material";
import { AgGridReact } from "ag-grid-react";
import { useEffect, useMemo, useRef } from "react";
import { numberToCurrency, TotalsGrid } from "smart-ui-library";
import DSMPaginatedGrid from "../../../components/DSMPaginatedGrid/DSMPaginatedGrid";
import ReportSummary from "../../../components/ReportSummary";
import { GRID_KEYS } from "../../../constants";
import { useReadOnlyNavigation } from "../../../hooks/useReadOnlyNavigation";
import { CalendarResponseDto } from "../../../reduxstore/types";
import { useTerminationGrid } from "./hooks/useTerminationGrid";
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
  onShowUnsavedChangesDialog?: () => void;
  isGridExpanded?: boolean;
  onToggleExpand?: () => void;
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
  onLoadingChange,
  onShowUnsavedChangesDialog,
  isGridExpanded = false,
  onToggleExpand
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

    termination,
    selectedProfitYear,

    selectionState,
    handleSave,
    handleBulkSave,
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
    onLoadingChange,
    isReadOnly,
    onShowUnsavedChangesDialog
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

  // Get main and detail columns and combine them into a single list
  const mainColumns = useMemo(() => GetTerminationColumns(), []);
  const detailColumns = useMemo(
    () =>
      GetDetailColumns(
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

  // Combine main and detail columns into a single flat list
  const columnDefs = useMemo(() => {
    return [...mainColumns, ...detailColumns];
  }, [mainColumns, detailColumns]);

  if (!termination?.response) return null;

  return (
    <DSMPaginatedGrid
      preferenceKey={GRID_KEYS.TERMINATION}
      data={gridData ?? []}
      columnDefs={columnDefs}
      totalRecords={termination.response.total || 0}
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
      header={<ReportSummary report={termination} />}
      headerActions={
        onToggleExpand && (
          <IconButton
            onClick={onToggleExpand}
            sx={{ zIndex: 1 }}>
            {isGridExpanded ? <FullscreenExitIcon /> : <FullscreenIcon />}
          </IconButton>
        )
      }
      beforeGrid={
        <div className="sticky top-0 z-10 flex bg-white">
          <TotalsGrid
            displayData={[[numberToCurrency(termination.totalEndingBalance || 0)]]}
            leftColumnHeaders={["Amount in Profit Sharing"]}
            topRowHeaders={[]}
          />
          <TotalsGrid
            displayData={[[numberToCurrency(termination.totalVested || 0)]]}
            leftColumnHeaders={["Vested Amount"]}
            topRowHeaders={[]}
          />
          <TotalsGrid
            displayData={[[numberToCurrency(termination.totalForfeit || 0)]]}
            leftColumnHeaders={["Total Forfeitures"]}
            topRowHeaders={[]}
          />
          <TotalsGrid
            displayData={[[numberToCurrency(termination.totalBeneficiaryAllocation || 0)]]}
            leftColumnHeaders={["Total Beneficiary Allocations"]}
            topRowHeaders={[]}
          />
        </div>
      }
      heightConfig={{
        mode: "content-aware",
        heightPercentage: isGridExpanded ? 0.85 : 0.4
      }}
      gridOptions={{
        onGridReady: (params) => {
          gridRef.current = params;
          localGridRef.current = params as unknown as AgGridReact;
          onGridReady(params);
        },
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
      className="relative"
    />
  );
};

export default TerminationGrid;
