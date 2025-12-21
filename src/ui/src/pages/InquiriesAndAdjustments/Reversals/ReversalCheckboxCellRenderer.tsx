import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";
import { Box, Checkbox, Chip, Tooltip } from "@mui/material";
import { ICellRendererParams } from "ag-grid-community";
import React from "react";
import {
  getIneligibilityReason,
  getReversalEligibilityStatus,
  ReversalEligibilityStatus
} from "./ReversalsGridColumns";

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
