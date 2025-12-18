import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";
import { Box, Checkbox, Chip, Tooltip } from "@mui/material";
import { ICellRendererParams } from "ag-grid-community";
import React from "react";
import { getReversalEligibilityStatus, ReversalEligibilityStatus, REVERSIBLE_PROFIT_CODES } from "./ReversalsGridColumns";

interface RowData {
  id: number;
  profitCodeId: number;
  monthToDate: number;
  yearToDate: number;
  isAlreadyReversed?: boolean;
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
const getIneligibilityReason = (data: {
  profitCodeId: number;
  monthToDate: number;
  yearToDate: number;
  isAlreadyReversed?: boolean;
}): string => {
  // Check if already reversed first
  if (data.isAlreadyReversed) {
    return "This transaction has already been reversed";
  }

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

    // January rule: if current month is January, transaction month must be > 1 and < 12
    const currentMonth = new Date().getMonth() + 1; // getMonth() is 0-based
    if (currentMonth === 1) {
      if (!(monthToDate > 1 && monthToDate < 12)) {
        return "Transaction not eligible for reversal in January";
      }
    }
  }

  return "This row cannot be reversed";
};

export const ReversalCheckboxCellRenderer: React.FC<ReversalCheckboxCellRendererProps> = ({ data, context }) => {
  const eligibilityStatus: ReversalEligibilityStatus = getReversalEligibilityStatus(data);

  const cellStyle: React.CSSProperties = {
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    height: "42px",
    marginTop: "-1px"
  };

  // Show "Reversed" badge for already-reversed rows
  if (eligibilityStatus === "already-reversed") {
    return (
      <Tooltip title="This transaction has already been reversed">
        <Box sx={cellStyle}>
          <Chip
            label="Reversed"
            size="small"
            sx={{
              backgroundColor: "#f5f5f5",
              color: "#666",
              fontSize: "0.7rem",
              height: "20px",
              "& .MuiChip-label": {
                padding: "0 6px"
              }
            }}
          />
        </Box>
      </Tooltip>
    );
  }

  // Show error icon for other ineligible rows
  if (eligibilityStatus === "ineligible") {
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
