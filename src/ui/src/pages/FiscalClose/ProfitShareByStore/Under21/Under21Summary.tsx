import { Box, Typography } from "@mui/material";
import React from "react";
import { Under21TotalsResponse } from "reduxstore/types";
import { TotalsGrid } from "smart-ui-library";

interface Under21SummaryProps {
  totals: Under21TotalsResponse | null;
  isLoading: boolean;
  title?: string;
}

const Under21Summary: React.FC<Under21SummaryProps> = ({
  totals,
  isLoading,
  title = "UNDER 21 REPORT (QPAY066TA-UNDR21)"
}) => {
  if (!totals && !isLoading) return null;

  const employeeCountsData = [
    // Row 1: 100% Vested
    [
      totals?.numberOfActiveUnder21With100PctVested || 0,
      totals?.numberOfTerminatedUnder21With100PctVested || 0,
      totals?.numberOfInActiveUnder21With100PctVested || 0
    ],
    // Row 2: Partially Vested
    [
      totals?.numberOfActiveUnder21With20to80PctVested || 0,
      totals?.numberOfTerminatedUnder21With20to80PctVested || 0,
      totals?.numberOfInActiveUnder21With20to80PctVested || 0
    ],
    // Row 3: 1-2 PS Years
    [
      totals?.numberOfActiveUnder21With1to2Years || 0,
      totals?.numberOfTerminatedUnder21With1to2Years || 0,
      totals?.numberOfInActiveUnder21With1to2Years || 0
    ],
    // Row 4: Totals (only computing the active column total)
    [
      totals
        ? totals.numberOfActiveUnder21With1to2Years +
          totals.numberOfActiveUnder21With20to80PctVested +
          totals.numberOfActiveUnder21With100PctVested
        : 0,
      "",
      ""
    ]
  ];

  return (
    <Box sx={{ marginBottom: 3 }}>
      <Typography
        variant="h6"
        sx={{ marginBottom: 2, color: "#0258A5" }}>
        {title}
      </Typography>

      <div className="mb-4">
        <TotalsGrid
          displayData={employeeCountsData}
          leftColumnHeaders={[
            "Under 21 100% Vested",
            "Under 21 Partially Vested",
            "Under 21 with 1-2 PS Years",
            "Total Under 21"
          ]}
          topRowHeaders={["", "Active Employees", "Terminated Employees", "Inactive Employees"]}
          tablePadding="0px"
        />
      </div>
    </Box>
  );
};

export default Under21Summary;
