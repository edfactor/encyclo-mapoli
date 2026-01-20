import { Print } from "@mui/icons-material";
import FullscreenIcon from "@mui/icons-material/Fullscreen";
import FullscreenExitIcon from "@mui/icons-material/FullscreenExit";
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Grid,
  IconButton,
  Tooltip,
  Typography
} from "@mui/material";
import { ColDef, SelectionChangedEvent } from "ag-grid-community";
import React, { useEffect, useRef, useState } from "react";
import { DSMPaginatedGrid } from "../../../components/DSMPaginatedGrid/DSMPaginatedGrid";
import DuplicateSsnGuard from "../../../components/DuplicateSsnGuard";
import { useGridPagination } from "../../../hooks/useGridPagination";
import type { AdhocProfLetter73Response } from "../../../reduxstore/api/AdhocProfLetter73Api";
import { useLazyGetAdhocProfLetter73Query } from "../../../reduxstore/api/AdhocProfLetter73Api";
import { AdhocProfLetter73FilterParams } from "./AdhocProfLetter73SearchFilter.tsx";
import { useAdhocProfLetter73Print } from "./hooks/useAdhocProfLetter73Print";

interface AdhocProfLetter73GridProps {
  filterParams?: AdhocProfLetter73FilterParams | null;
  onLoadingChange?: (isLoading: boolean) => void;
  isGridExpanded?: boolean;
  onToggleExpand?: () => void;
  // optional trigger used to request a fresh search from parent without unmounting
  searchTrigger?: number;
}

const AdhocProfLetter73Grid: React.FC<AdhocProfLetter73GridProps> = (props) => {
  const { filterParams, onLoadingChange, isGridExpanded = false, onToggleExpand, searchTrigger } = props;
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [columnDefs, setColumnDefs] = useState<ColDef[]>([]);
  const [rowData, setRowData] = useState<Record<string, unknown>[]>([]);
  const [selectedRows, setSelectedRows] = useState<Record<string, unknown>[]>([]);

  // Allow nullable filterParams; derive profitYear defensively
  const profitYear = filterParams?.profitYear?.getFullYear() || 0;

  // Keep last successful API response so we can display previous data while fetching
  const lastApiRef = useRef<AdhocProfLetter73Response | null>(null);

  const {
    handlePrint,
    isDownloading,
    isPrintDialogOpen,
    setIsPrintDialogOpen,
    printContent,
    printFormLetter,
    error: printError,
    clearError
  } = useAdhocProfLetter73Print(filterParams ?? null, selectedRows);

  const [trigger, { data: apiData, isFetching, error, isError }] = useLazyGetAdhocProfLetter73Query();

  const handleSelectionChanged = (event: SelectionChangedEvent) => {
    const selectedData = event.api.getSelectedRows();
    setSelectedRows(selectedData);
  };

  const { pageNumber, pageSize, sortParams, handlePageNumberChange, handlePageSizeChange, handleSortChange } =
    useGridPagination({
      initialPageSize: 25,
      initialSortBy: "BadgeNumber",
      initialSortDescending: false,
      persistenceKey: "ADHOC_PROF_LETTER73",
      onPaginationChange: (newPageNumber, newPageSize, newSortParams) => {
        if (profitYear > 0) {
          trigger({
            profitYear,
            DeMinimusValue: filterParams?.DeMinimusValue,
            skip: newPageNumber * newPageSize,
            take: newPageSize,
            sortBy: newSortParams.sortBy,
            isSortDescending: newSortParams.isSortDescending
          });
        }
      }
    });

  // Trigger API call when profitYear, DeMinimusValue, or searchTrigger changes (initial load and re-search)
  useEffect(() => {
    if (profitYear > 0) {
      const apiParams = {
        profitYear,
        DeMinimusValue: filterParams?.DeMinimusValue,
        skip: pageNumber * pageSize,
        take: pageSize,
        sortBy: sortParams.sortBy,
        isSortDescending: sortParams.isSortDescending
      };
      trigger(apiParams);
    }
  }, [profitYear, filterParams?.DeMinimusValue, trigger, pageNumber, pageSize, sortParams, searchTrigger]);

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

  // Generate columns dynamically from API response - server-side sorting
  // Build columns and update row data only when we receive a new API response.
  // Preserve the last successful response in `lastApiRef` so the UI can continue
  // showing previous results while a new fetch is in-flight.
  useEffect(() => {
    if (apiData && Array.isArray(apiData.results)) {
      // Save last response even when results are empty (explicit empty set)
      lastApiRef.current = apiData as AdhocProfLetter73Response;

      if (apiData.results.length > 0) {
        const sampleData = apiData.results[0];
        if (sampleData) {
          const cols: ColDef[] = Object.keys(sampleData).map((key) => {
            const baseCol: ColDef = {
              headerName: key
                .replace(/([A-Z])/g, " $1")
                .trim()
                .replace(/^./, (str) => str.toUpperCase()),
              field: key,
              sortable: true,
              filter: false,
              resizable: true
            };

            if (key.toLowerCase() === "factor") {
              baseCol.headerName = "Factor";
              baseCol.type = "rightAligned";
              baseCol.valueFormatter = (params) => {
                const value = params.value;
                if (value == null) return "N/A";
                const numValue = Number(value);
                return Number.isFinite(numValue) ? numValue.toFixed(4) : "N/A";
              };
            }

            if (key.toLowerCase() === "rmd") {
              baseCol.headerName = "RMD";
              baseCol.type = "rightAligned";
              baseCol.valueFormatter = (params) => {
                const value = params.value;
                if (value == null) return "N/A";
                const numValue = Number(value);
                if (!Number.isFinite(numValue)) return "N/A";
                return new Intl.NumberFormat("en-US", {
                  style: "currency",
                  currency: "USD",
                  minimumFractionDigits: 2,
                  maximumFractionDigits: 2
                }).format(numValue);
              };
            }

            if (key.toLowerCase() === "balance") {
              baseCol.headerName = "Balance";
              baseCol.type = "rightAligned";
              baseCol.valueFormatter = (params) => {
                const value = params.value;
                if (value == null) return "N/A";
                const numValue = Number(value);
                if (!Number.isFinite(numValue)) return "N/A";
                return new Intl.NumberFormat("en-US", {
                  style: "currency",
                  currency: "USD",
                  minimumFractionDigits: 2,
                  maximumFractionDigits: 2
                }).format(numValue);
              };
            }

            if (key.toLowerCase() === "paymentsinprofityear") {
              baseCol.headerName = "Payments In Profit Year";
              baseCol.type = "rightAligned";
              baseCol.valueFormatter = (params) => {
                const value = params.value;
                if (value == null) return "N/A";
                const numValue = Number(value);
                if (!Number.isFinite(numValue)) return "N/A";
                return new Intl.NumberFormat("en-US", {
                  style: "currency",
                  currency: "USD",
                  minimumFractionDigits: 2,
                  maximumFractionDigits: 2
                }).format(numValue);
              };
            }

            if (key.toLowerCase() === "suggestrmdcheckamount") {
              baseCol.headerName = "Suggest RMD Check Amount";
              baseCol.type = "rightAligned";
              baseCol.valueFormatter = (params) => {
                const value = params.value;
                if (value == null) return "N/A";
                const numValue = Number(value);
                if (!Number.isFinite(numValue)) return "N/A";
                return new Intl.NumberFormat("en-US", {
                  style: "currency",
                  currency: "USD",
                  minimumFractionDigits: 2,
                  maximumFractionDigits: 2
                }).format(numValue);
              };
            }

            return baseCol;
          });

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
          setRowData(apiData.results);
        }
      } else {
        // Explicit empty result set; clear grid
        setColumnDefs([]);
        setRowData([]);
      }
    }
  }, [apiData]);

  return (
    <DuplicateSsnGuard>
      {() => (
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
            <Grid sx={{ display: "flex", gap: 1 }}>
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

          {(() => {
            const displayApi = apiData ?? lastApiRef.current;
            const displayRows = apiData ? rowData : (displayApi?.results ?? []);

            // If we're fetching and have no prior data, show spinner
            if (isFetching && !displayApi) {
              return (
                <Box
                  display="flex"
                  justifyContent="center"
                  alignItems="center"
                  py={4}>
                  <CircularProgress />
                </Box>
              );
            }

            if (displayApi && !errorMessage) {
              return columnDefs.length > 0 ? (
                <DSMPaginatedGrid
                  preferenceKey="ADHOC_PROF_LETTER73"
                  data={displayRows}
                  columnDefs={columnDefs}
                  totalRecords={displayApi.total || displayRows.length}
                  isLoading={isFetching}
                  pagination={{
                    pageNumber,
                    pageSize,
                    sortParams,
                    handlePageNumberChange,
                    handlePageSizeChange,
                    handleSortChange
                  }}
                  onSortChange={(update) => {
                    handleSortChange({
                      sortBy: update.sortBy,
                      isSortDescending: update.isSortDescending
                    });
                  }}
                  heightConfig={{
                    mode: "content-aware",
                    heightPercentage: isGridExpanded ? 0.85 : 0.4
                  }}
                  gridOptions={{
                    rowSelection: "multiple",
                    onSelectionChanged: handleSelectionChanged
                  }}
                  showPagination={(displayRows?.length ?? 0) > 0}
                />
              ) : (
                <Box sx={{ padding: "24px" }}>
                  <Typography>No data available for the selected profit year.</Typography>
                </Box>
              );
            }

            return null;
          })()}

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
      )}
    </DuplicateSsnGuard>
  );
};

export default AdhocProfLetter73Grid;
