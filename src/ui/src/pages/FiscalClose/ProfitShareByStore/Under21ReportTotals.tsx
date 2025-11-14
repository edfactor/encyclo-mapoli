import { Box, Typography } from "@mui/material";
import React from "react";
import { ProfitSharingUnder21ReportResponse } from "reduxstore/types";
import { TotalsGrid } from "smart-ui-library";

interface Under21ReportTotalsProps {
  totals: ProfitSharingUnder21ReportResponse | null;
  isLoading: boolean;
  title?: string;
}

const Under21ReportTotals: React.FC<Under21ReportTotalsProps> = ({
  totals,
  isLoading,
  title = "UNDER 21 REPORT (QPAY066-UNDR21)"
}) => {
  if (!totals && !isLoading) return null;

  const employeeCountsData = [
    // Row 1: 100% Vested
    [
      totals?.activeTotals.totalVested || 0,
      totals?.terminatedTotals.totalVested || 0,
      totals?.inactiveTotals.totalVested || 0
    ],
    // Row 2: Partially Vested
    [
      totals?.activeTotals.partiallyVested || 0,
      totals?.terminatedTotals.partiallyVested || 0,
      totals?.inactiveTotals.partiallyVested || 0
    ],
    // Row 3: 1-2 PS Years
    [
      totals?.activeTotals.partiallyVestedButLessThanThreeYears || 0,
      totals?.terminatedTotals.partiallyVestedButLessThanThreeYears || 0,
      totals?.inactiveTotals.partiallyVestedButLessThanThreeYears || 0
    ],
    // Row 4: Totals (only computing the active column total)
    [totals ? totals.totalUnder21 : 0, "", ""]
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

export default Under21ReportTotals;
