import { Alert, Box, Button, CircularProgress, Dialog, DialogActions, DialogContent, DialogTitle, Grid, IconButton, Tooltip, Typography } from "@mui/material";
import { Print } from "@mui/icons-material";
import FullscreenIcon from "@mui/icons-material/Fullscreen";
import FullscreenExitIcon from "@mui/icons-material/FullscreenExit";
import { ColDef, SelectionChangedEvent } from "ag-grid-community";
import React, { useEffect, useState } from "react";
import { DSMGrid, Pagination } from "smart-ui-library";
import { useDynamicGridHeight } from "../../../hooks/useDynamicGridHeight";
import { useGridPagination } from "../../../hooks/useGridPagination";
import { useLazyGetAdhocProfLetter73Query } from "../../../reduxstore/api/AdhocProfLetter73Api";
import { AdhocProfLetter73FilterParams } from "./AdhocProfLetter73SearchFilter.tsx";
import { useAdhocProfLetter73Print } from "./hooks/useAdhocProfLetter73Print";

interface AdhocProfLetter73GridProps {
  filterParams: AdhocProfLetter73FilterParams;
  onLoadingChange?: (isLoading: boolean) => void;
  isGridExpanded?: boolean;
  onToggleExpand?: () => void;
}

const AdhocProfLetter73Grid: React.FC<AdhocProfLetter73GridProps> = ({ 
  filterParams, 
  onLoadingChange,
  isGridExpanded = false,
  onToggleExpand
}) => {
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [columnDefs, setColumnDefs] = useState<ColDef[]>([]);
  const [rowData, setRowData] = useState<Record<string, unknown>[]>([]);
  const [selectedRows, setSelectedRows] = useState<Record<string, unknown>[]>([]);

  const profitYear = filterParams.profitYear?.getFullYear() || 0;

  const {
    handlePrint,
    isDownloading,
    isPrintDialogOpen,
    setIsPrintDialogOpen,
    printContent,
    printFormLetter,
    error: printError,
    clearError
  } = useAdhocProfLetter73Print(filterParams, selectedRows);

  const [trigger, { data: apiData, isFetching, error, isError }] = useLazyGetAdhocProfLetter73Query();

  const handleSelectionChanged = (event: SelectionChangedEvent) => {
    const selectedData = event.api.getSelectedRows();
    setSelectedRows(selectedData);
  };

  // Use dynamic grid height utility hook - increase height when expanded
  const gridMaxHeight = useDynamicGridHeight({ heightPercentage: isGridExpanded ? 0.85 : 0.4 });

  // Pagination hook
  const { pageNumber, pageSize, handlePaginationChange } = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "",
    initialSortDescending: false,
    onPaginationChange: () => {
      // Pagination handled client-side for now
    }
  });

  // Trigger API call when profitYear or pagination changes
  useEffect(() => {
    if (profitYear > 0) {
      trigger({ profitYear });
    }
  }, [profitYear, pageNumber, pageSize, trigger]);

  useEffect(() => {
    onLoadingChange?.(isFetching);
  }, [isFetching, onLoadingChange]);

  // Handle API errors
  useEffect(() => {
    if (isError && error) {
      console.error("AdhocProfLetter73 API error:", error);
      setErrorMessage("Failed to load data. Please try again.");
    } else if (!isFetching && apiData) {
      setErrorMessage(null);
    }
  }, [isError, error, isFetching, apiData]);

  // Generate columns dynamically from API response and apply client-side pagination
  useEffect(() => {
    if (apiData?.response?.results && Array.isArray(apiData.response.results) && apiData.response.results.length > 0) {
      // Get first row to determine structure
      const sampleData = apiData.response.results[0];
      
      if (sampleData) {
        const cols: ColDef[] = Object.keys(sampleData).map((key) => ({
          headerName: key.replace(/([A-Z])/g, ' $1').trim().replace(/^./, str => str.toUpperCase()),
          field: key,
          sortable: true,
          filter: true,
          resizable: true
        }));
        
        // Add Print checkbox column
        cols.push({
          headerName: "Print",
          field: "print",
          colId: "print",
          checkboxSelection: true,
          headerCheckboxSelection: true,
          width: 95,
          maxWidth: 95,
          minWidth: 95,
          pinned: "right",
          lockPosition: "right",
          suppressSizeToFit: true,
          suppressAutoSize: true,
          suppressColumnsToolPanel: true,
          suppressMovable: true
        });
        
        setColumnDefs(cols);
        
        // Apply client-side pagination
        const startIndex = pageNumber * pageSize;
        const endIndex = startIndex + pageSize;
        const paginatedData = apiData.response.results.slice(startIndex, endIndex);
        
        setRowData(paginatedData);
      }
    }
  }, [apiData, pageNumber, pageSize]);

  return (
    <div className="relative">
      {printError && (
        <Box sx={{ padding: "0 24px", marginBottom: "16px" }}>
          <Alert 
            severity="error" 
            onClose={clearError}>
            {printError}
          </Alert>
        </Box>
      )}
      <Grid
        container
        justifyContent="space-between"
        alignItems="center"
        sx={{ padding: "0 24px" }}>
        <Grid>
          <Typography
            variant="h2"
            sx={{ color: "#0258A5" }}>
            Adhoc Prof Letter 73
          </Typography>
        </Grid>
        <Grid sx={{ display: 'flex', gap: 1 }}>
          <Tooltip 
            title={selectedRows.length === 0 ? "You must check at least one box" : "Print Prof Letter 73"} 
            placement="top">
            <span>
              <Button
                variant="outlined"
                color="primary"
                size="medium"
                startIcon={isDownloading ? <CircularProgress size={20} /> : <Print />}
                onClick={handlePrint}
                disabled={isDownloading || selectedRows.length === 0}
                sx={{ marginLeft: 2, marginRight: "20px" }}>
                {isDownloading ? "Generating..." : "Print"}
              </Button>
            </span>
          </Tooltip>
          {onToggleExpand && (
            <IconButton
              onClick={onToggleExpand}
              aria-label={isGridExpanded ? "Exit fullscreen" : "Enter fullscreen"}
              sx={{ zIndex: 1 }}>
              {isGridExpanded ? <FullscreenExitIcon /> : <FullscreenIcon />}
            </IconButton>
          )}
        </Grid>
      </Grid>

      {errorMessage && (
        <Box sx={{ padding: "24px", color: "error.main" }}>
          <Typography color="error">{errorMessage}</Typography>
        </Box>
      )}

      {isFetching ? (
        <Box
          display="flex"
          justifyContent="center"
          alignItems="center"
          py={4}>
          <CircularProgress />
        </Box>
      ) : apiData && !errorMessage && columnDefs.length > 0 ? (
        <>
          <DSMGrid
            preferenceKey="ADHOC_PROF_LETTER73"
            isLoading={isFetching}
            maxHeight={gridMaxHeight}
            providedOptions={{
              rowData: rowData,
              columnDefs: columnDefs,
              rowSelection: "multiple",
              onSelectionChanged: handleSelectionChanged
            }}
          />
          {rowData.length > 0 && (
            <Pagination
              pageNumber={pageNumber}
              setPageNumber={(value: number) => {
                handlePaginationChange(value - 1, pageSize);
              }}
              pageSize={pageSize}
              setPageSize={(value: number) => {
                handlePaginationChange(0, value);
              }}
              recordCount={apiData.response?.total || rowData.length}
            />
          )}
        </>
      ) : null}

      <Dialog
        open={isPrintDialogOpen}
        onClose={() => setIsPrintDialogOpen(false)}
        maxWidth="lg"
        fullWidth>
        <DialogTitle>Print Preview - Prof Letter 73</DialogTitle>
        <DialogContent>
          <pre style={{ whiteSpace: "pre-wrap", fontFamily: "monospace", fontSize: "12px" }}>{printContent}</pre>
        </DialogContent>
        <DialogActions sx={{ paddingRight: "25px" }}>
          <Button onClick={() => setIsPrintDialogOpen(false)}>Close</Button>
          <Button
            onClick={() => printFormLetter(printContent)}
            variant="contained">
            Print
          </Button>
        </DialogActions>
      </Dialog>
    </div>
  );
};

export default AdhocProfLetter73Grid;
