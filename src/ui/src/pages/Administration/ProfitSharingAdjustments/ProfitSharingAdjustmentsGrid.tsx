import { Box, Button, Typography } from "@mui/material";
import { CellValueChangedEvent, GridApi, SelectionChangedEvent } from "ag-grid-community";
import React, { memo, useMemo, useRef } from "react";
import { DSMGrid } from "smart-ui-library";
import { GRID_KEYS } from "../../../constants";
import { ProfitSharingAdjustmentRowDto } from "../../../reduxstore/types";
import { GetProfitSharingAdjustmentsGridColumns } from "./ProfitSharingAdjustmentsGridColumns";

interface ProfitSharingAdjustmentsGridProps {
  rowData: ProfitSharingAdjustmentRowDto[];
  isLoading: boolean;
  selectedRow: ProfitSharingAdjustmentRowDto | null;
  hasUnsavedChanges: boolean;
  onRowSelection: (row: ProfitSharingAdjustmentRowDto | null) => void;
  onClearSelection: () => void;
  onAdjust: () => void;
  onSave: () => void;
  onDiscard: () => void;
  onCellValueChanged: (row: ProfitSharingAdjustmentRowDto, field: string, oldValue: unknown) => boolean;
}

const ProfitSharingAdjustmentsGrid: React.FC<ProfitSharingAdjustmentsGridProps> = memo(
  ({
    rowData,
    isLoading,
    selectedRow,
    hasUnsavedChanges,
    onRowSelection,
    onClearSelection,
    onAdjust,
    onSave,
    onDiscard,
    onCellValueChanged
  }) => {
    const gridApiRef = useRef<GridApi | null>(null);

    const columnDefs = useMemo(() => GetProfitSharingAdjustmentsGridColumns(), []);

    const handleCellValueChanged = (event: CellValueChangedEvent) => {
      const row = event.data as ProfitSharingAdjustmentRowDto | undefined;
      if (!row || !event.colDef.field) {
        return;
      }

      const success = onCellValueChanged(row, event.colDef.field, event.oldValue);

      if (!success) {
        // Refresh cells to revert the change visually
        event.api.refreshCells({ force: true });
      }
    };

    const handleSelectionChanged = (event: SelectionChangedEvent<ProfitSharingAdjustmentRowDto>) => {
      const selected = event.api.getSelectedNodes().map((n) => n.data ?? null)[0] ?? null;
      onRowSelection(selected);
    };

    return (
      <>
        {/* Action Buttons */}
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            gap: 2,
            px: 1,
            mb: 2,
            flexWrap: "wrap"
          }}>
          <Box sx={{ display: "flex", gap: 2, alignItems: "center" }}>
            <Button
              variant="outlined"
              disabled={!selectedRow || isLoading}
              onClick={onClearSelection}>
              Clear selection
            </Button>

            <Button
              variant="contained"
              disabled={
                !selectedRow ||
                selectedRow.profitDetailId == null ||
                selectedRow.hasBeenReversed ||
                isLoading ||
                rowData.length === 0
              }
              title={selectedRow?.hasBeenReversed ? "This row has already been reversed" : ""}
              onClick={onAdjust}>
              Adjust…
            </Button>
          </Box>

          <Box sx={{ display: "flex", gap: 2 }}>
            <Button
              variant="contained"
              disabled={!hasUnsavedChanges || isLoading}
              onClick={onSave}>
              Save
            </Button>
            <Button
              variant="outlined"
              disabled={!hasUnsavedChanges || isLoading}
              onClick={onDiscard}>
              Discard
            </Button>
          </Box>
        </Box>

        {/* Grid */}
        <DSMGrid
          preferenceKey={GRID_KEYS.PROFIT_SHARING_ADJUSTMENTS}
          isLoading={isLoading}
          providedOptions={{
            onGridReady: (params) => {
              gridApiRef.current = params.api as GridApi;
            },
            rowData,
            columnDefs,
            suppressMultiSort: true,
            stopEditingWhenCellsLoseFocus: true,
            enterNavigatesVertically: true,
            enterNavigatesVerticallyAfterEdit: true,
            rowSelection: {
              mode: "singleRow",
              checkboxes: true,
              enableClickSelection: false
            },
            onSelectionChanged: handleSelectionChanged as (event: unknown) => void,
            onCellValueChanged: handleCellValueChanged,
            getRowStyle: (params) => {
              const data = params.data as ProfitSharingAdjustmentRowDto | undefined;
              if (data?.hasBeenReversed) {
                return { backgroundColor: "#f5f5f5", color: "#9e9e9e" };
              }
              return undefined;
            }
          }}
        />
      </>
    );
  }
);

ProfitSharingAdjustmentsGrid.displayName = "ProfitSharingAdjustmentsGrid";

export default ProfitSharingAdjustmentsGrid;
