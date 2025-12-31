import CheckCircleOutlineIcon from "@mui/icons-material/CheckCircleOutline";
import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";
import { Box, Chip, Tooltip } from "@mui/material";
import { ICellRendererParams } from "ag-grid-community";
import React from "react";
import { ProfitSharingAdjustmentRowDto } from "../../../reduxstore/types";

type AdjustmentEligibilityStatus = "editable" | "reversed" | "locked";

/**
 * Determines the adjustment eligibility status of a row.
 */
const getAdjustmentEligibilityStatus = (data: ProfitSharingAdjustmentRowDto): AdjustmentEligibilityStatus => {
  if (data.hasBeenReversed) {
    return "reversed";
  }

  // Draft insert rows (profitDetailId is null) are editable
  if (data.profitDetailId == null) {
    return "editable";
  }

  // Existing persisted rows are locked (can only be adjusted via the Adjust button)
  return "locked";
};

/**
 * Returns a human-readable description for the status.
 */
const getStatusDescription = (data: ProfitSharingAdjustmentRowDto): string => {
  const status = getAdjustmentEligibilityStatus(data);

  switch (status) {
    case "reversed":
      return "This transaction has already been reversed";
    case "editable":
      return "This is a draft adjustment row - editable fields can be modified directly";
    case "locked":
      return "This is a persisted transaction - use the Adjust button to create a reversal";
    default:
      return "";
  }
};

interface AdjustmentStatusCellRendererProps extends ICellRendererParams {
  data: ProfitSharingAdjustmentRowDto;
}

export const AdjustmentStatusCellRenderer: React.FC<AdjustmentStatusCellRendererProps> = ({ data }) => {
  const status = getAdjustmentEligibilityStatus(data);
  const description = getStatusDescription(data);

  const cellStyle: React.CSSProperties = {
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    height: "42px",
    marginTop: "-1px"
  };

  // Show "Reversed" badge for already-reversed rows
  if (status === "reversed") {
    return (
      <Tooltip title={description}>
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

  // Show check icon for draft/editable rows
  if (status === "editable") {
    return (
      <Tooltip title={description}>
        <Box sx={cellStyle}>
          <CheckCircleOutlineIcon
            sx={{
              color: "success.main",
              fontSize: 20
            }}
          />
        </Box>
      </Tooltip>
    );
  }

  // Show info icon for locked (persisted) rows
  return (
    <Tooltip title={description}>
      <Box sx={cellStyle}>
        <ErrorOutlineIcon
          sx={{
            color: "info.main",
            fontSize: 20
          }}
        />
      </Box>
    </Tooltip>
  );
};

AdjustmentStatusCellRenderer.displayName = "AdjustmentStatusCellRenderer";
