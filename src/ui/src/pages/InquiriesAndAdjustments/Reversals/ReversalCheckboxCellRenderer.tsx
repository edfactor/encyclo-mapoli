import { Box, Checkbox, Tooltip } from "@mui/material";
import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";
import { ICellRendererParams } from "ag-grid-community";
import React from "react";
import { isRowReversible, REVERSIBLE_PROFIT_CODES } from "./ReversalsGridColumns";

interface RowData {
  id: number;
  profitCodeId: number;
  monthToDate: number;
  yearToDate: number;
}

interface GridContext {
  getSelectedIds: () => Set<number>;
  toggleSelection: (rowId: number) => void;
}

interface ReversalCheckboxCellRendererProps extends ICellRendererParams {
  data: RowData;
  context: GridContext;
}

// Determine why a row cannot be reversed
const getIneligibilityReason = (data: { profitCodeId: number; monthToDate: number; yearToDate: number }): string => {
  // Check profit code first
  if (!REVERSIBLE_PROFIT_CODES.includes(data.profitCodeId)) {
    return "Ineligible code for reversal";
  }

  // Check if transaction is too old (more than 2 months ago)
  const { monthToDate, yearToDate } = data;
  if (yearToDate && monthToDate) {
    const rowDate = new Date(yearToDate, monthToDate - 1);
    const twoMonthsAgo = new Date();
    twoMonthsAgo.setMonth(twoMonthsAgo.getMonth() - 2);

    if (rowDate < twoMonthsAgo) {
      return "Transaction too old for reversal";
    }
  }

  return "This row cannot be reversed";
};

export const ReversalCheckboxCellRenderer: React.FC<ReversalCheckboxCellRendererProps> = ({ data, context }) => {
  const isReversible = isRowReversible(data);

  const cellStyle: React.CSSProperties = {
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    height: "42px",
    marginTop: "-1px"
  };

  if (!isReversible) {
    const tooltipMessage = getIneligibilityReason(data);
    return (
      <Tooltip title={tooltipMessage}>
        <Box sx={cellStyle}>
          <ErrorOutlineIcon
            sx={{
              color: "text.primary",
              fontSize: 20
            }}
          />
        </Box>
      </Tooltip>
    );
  }

  // Get selection state from React context (via getter function for fresh value)
  const isSelected = context.getSelectedIds().has(data.id);

  const handleClick = (event: React.MouseEvent) => {
    event.stopPropagation();
    context.toggleSelection(data.id);
  };

  return (
    <Box sx={cellStyle}>
      <Checkbox
        checked={isSelected}
        onClick={handleClick}
        size="small"
        sx={{ padding: 0 }}
      />
    </Box>
  );
};

ReversalCheckboxCellRenderer.displayName = "ReversalCheckboxCellRenderer";
