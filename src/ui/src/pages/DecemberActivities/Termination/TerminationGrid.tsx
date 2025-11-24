import { useEffect, useMemo } from "react";
import { Grid, IconButton } from "@mui/material";
import FullscreenIcon from "@mui/icons-material/Fullscreen";
import FullscreenExitIcon from "@mui/icons-material/FullscreenExit";
import { DSMGrid, numberToCurrency, Pagination, TotalsGrid } from "smart-ui-library";
import ReportSummary from "../../../components/ReportSummary";
import { useDynamicGridHeight } from "../../../hooks/useDynamicGridHeight";
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
  // Use dynamic grid height utility hook
  const gridMaxHeight = useDynamicGridHeight({
    heightPercentage: isGridExpanded ? 0.85 : 0.4
  });

  // Check if current navigation should be read-only
  const isReadOnly = useReadOnlyNavigation();

  const {
    pageNumber,
    pageSize,
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
    if (gridRef.current?.api) {
      gridRef.current.api.refreshCells({ force: true });
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

  return (
    <div className="relative">
      {termination?.response && (
        <>
          <Grid
            container
            justifyContent="space-between"
            alignItems="center"
            marginBottom={2}>
            <Grid>
              <ReportSummary report={termination} />
            </Grid>
            <Grid style={{ display: 'flex', gap: 8 }}>
              {onToggleExpand && (
                <IconButton
                  onClick={onToggleExpand}
                  sx={{ zIndex: 1 }}>
                  {isGridExpanded ? <FullscreenExitIcon /> : <FullscreenIcon />}
                </IconButton>
              )}
            </Grid>
          </Grid>

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
            preferenceKey={"TERMINATION"}
            handleSortChanged={sortEventHandler}
            maxHeight={gridMaxHeight}
            isLoading={isFetching}
            providedOptions={{
              onGridReady: (params) => {
                gridRef.current = params;
                onGridReady(params);
              },
              rowData: gridData,
              columnDefs: columnDefs,
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
