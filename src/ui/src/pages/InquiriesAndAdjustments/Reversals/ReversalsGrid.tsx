import ReplayIcon from "@mui/icons-material/Replay";
import { Box, Button, Grid, Typography } from "@mui/material";
import { GridApi, IRowNode } from "ag-grid-community";
import React, { memo, useCallback, useEffect, useMemo, useRef, useState } from "react";
import { DSMGrid, Pagination } from "smart-ui-library";
import { GRID_KEYS } from "../../../constants";
import { GetReversalsGridColumns, isRowReversible } from "./ReversalsGridColumns";

export interface ProfitDetailRow {
  id: number;
  profitYear: number;
  profitYearIteration: number;
  profitCodeId: number;
  profitCodeName: string;
  contribution: number;
  earnings: number;
  forfeiture: number;
  payment: number;
  monthToDate: number;
  yearToDate: number;
  currentHoursYear: number;
  currentIncomeYear: number;
  federalTaxes: number;
  stateTaxes: number;
  taxCode: string;
  commentTypeName: string;
  commentRelatedCheckNumber: string;
  employmentStatus: string;
  /** Indicates whether this profit detail record has already been reversed */
  isAlreadyReversed?: boolean;
}

interface ReversalsGridProps {
  profitData: {
    results: ProfitDetailRow[];
    total: number;
  } | null;
  isLoading: boolean;
  onReverse: (selectedRows: ProfitDetailRow[]) => void;
  pageNumber: number;
  pageSize: number;
  onPaginationChange: (pageNumber: number, pageSize: number) => void;
}

const ReversalsGrid: React.FC<ReversalsGridProps> = memo(
  ({ profitData, isLoading, onReverse, pageNumber, pageSize, onPaginationChange }) => {
    const [selectedIds, setSelectedIds] = useState<Set<number>>(new Set());
    const selectedIdsRef = useRef<Set<number>>(selectedIds);
    const gridApiRef = useRef<GridApi | null>(null);

    // Keep ref in sync with state
    selectedIdsRef.current = selectedIds;

    // Redraw rows whenever selectedIds changes to force cell renderer re-creation
    useEffect(() => {
      if (gridApiRef.current) {
        gridApiRef.current.redrawRows();
      }
    }, [selectedIds]);

    const columnDefs = useMemo(() => GetReversalsGridColumns(), []);

    // Determine if a row can be selected for reversal
    const isRowSelectable = useCallback((node: IRowNode) => {
      const data = node.data as ProfitDetailRow | undefined;
      if (!data) return false;
      return isRowReversible(data);
    }, []);

    // Toggle selection for a row (called from cell renderer via context)
    const toggleSelection = useCallback((rowId: number) => {
      setSelectedIds((prev) => {
        const newSet = new Set(prev);
        if (newSet.has(rowId)) {
          newSet.delete(rowId);
        } else {
          newSet.add(rowId);
        }
        selectedIdsRef.current = newSet;
        return newSet;
      });
    }, []);

    // Get selected rows data for the reverse action
    const selectedRows = useMemo(() => {
      if (!profitData) return [];
      return profitData.results.filter((row) => selectedIds.has(row.id));
    }, [profitData, selectedIds]);

    const handleReverseClick = useCallback(() => {
      if (selectedRows.length > 0) {
        onReverse(selectedRows);
        // Clear selection after reverse
        selectedIdsRef.current = new Set();
        setSelectedIds(new Set());
      }
    }, [selectedRows, onReverse]);

    const handlePageChange = useCallback(
      (newPageNumber: number) => {
        onPaginationChange(newPageNumber - 1, pageSize);
      },
      [onPaginationChange, pageSize]
    );

    const handlePageSizeChange = useCallback(
      (newPageSize: number) => {
        onPaginationChange(0, newPageSize);
      },
      [onPaginationChange]
    );

    // Grid context - use ref so cell renderer always gets current value
    const gridContext = useMemo(
      () => ({
        getSelectedIds: () => selectedIdsRef.current,
        toggleSelection
      }),
      [toggleSelection]
    );

    if (!profitData) {
      return null;
    }

    return (
      <>
        <style>
          {`
            .row-already-reversed {
              background-color: #f5f5f5 !important;
              color: #888 !important;
            }
            .row-already-reversed:hover {
              background-color: #e8e8e8 !important;
            }
          `}
        </style>
        <Grid
          container
          width="100%"
          spacing={2}>
          {/* Reverse Button - always visible, disabled when no selection */}
          <Grid size={{ xs: 12 }}>
            <Box sx={{ display: "flex", justifyContent: "flex-end", mb: 1 }}>
              <Button
                variant="outlined"
                color="success"
                startIcon={<ReplayIcon />}
                onClick={handleReverseClick}
                disabled={selectedRows.length === 0}
                sx={{
                  fontWeight: "bold",
                  textTransform: "none"
                }}>
                REVERSE{selectedRows.length > 0 ? ` (${selectedRows.length})` : ""}
              </Button>
            </Box>
          </Grid>

          {/* Grid Title */}
          <Grid size={{ xs: 12 }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5", marginY: "8px" }}>
              Transactions
            </Typography>
          </Grid>

          {/* Grid */}
          <Grid size={{ xs: 12 }}>
            <DSMGrid
              preferenceKey={GRID_KEYS.REVERSALS}
              isLoading={isLoading}
              providedOptions={{
                rowData: profitData.results,
                columnDefs: columnDefs,
                context: gridContext,
                rowSelection: {
                  mode: "multiRow",
                  checkboxes: false,
                  headerCheckbox: false,
                  enableClickSelection: false,
                  isRowSelectable: isRowSelectable
                },
                suppressRowClickSelection: true,
                onGridReady: (params) => {
                  gridApiRef.current = params.api;
                },
                getRowClass: (params) => {
                  // Apply gray styling to already-reversed rows
                  const data = params.data as ProfitDetailRow | undefined;
                  if (data?.isAlreadyReversed) {
                    return "row-already-reversed";
                  }
                  return undefined;
                }
              }}
            />
          </Grid>

          {/* Pagination */}
          <Grid size={{ xs: 12 }}>
            <Pagination
              pageNumber={pageNumber}
              setPageNumber={handlePageChange}
              pageSize={pageSize}
              setPageSize={handlePageSizeChange}
              recordCount={profitData.total}
            />
          </Grid>
        </Grid>
      </>
    );
  }
);

ReversalsGrid.displayName = "ReversalsGrid";

export default ReversalsGrid;
