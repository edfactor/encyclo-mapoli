import React, { useMemo } from "react";
import { Typography, Table, TableBody, TableCell, TableContainer, TableRow } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { DSMGrid } from "smart-ui-library";
import { FilterParams } from "reduxstore/types";
import { numberToCurrency } from "smart-ui-library";
import presets from "./presets";
import { GetQPAY066AdHocGridColumns } from "./QPAY066AdHocGridColumns";

interface ReportGridProps {
  params: FilterParams;
  onLoadingChange?: (isLoading: boolean) => void;
}

// Dummy data for demonstration
const dummyData = [
  {
    badge: 47425,
    name: "Wilkins, A...",
    beginningBalance: 4781.67,
    beneficiaryAllocation: 0.0,
    distributionAmount: 0.0,
    forfeit: 0.0,
    endingBalance: 4781.67,
    vestingBalance: 4781.67,
    dateTerm: "XX/XX/X...",
    ytdHours: 427,
    years: "XX",
    vested: "XX%",
    age: "XX"
  },
  {
    badge: 82424,
    name: "Potts, Aria",
    beginningBalance: 2221.96,
    beneficiaryAllocation: 0.0,
    distributionAmount: 500.0,
    forfeit: 0.0,
    endingBalance: 1721.96,
    vestingBalance: 1721.96,
    dateTerm: "XX/XX/X...",
    ytdHours: 241,
    years: "XX",
    vested: "XX%",
    age: "XX"
  },
  {
    badge: 85744,
    name: "Lewis, Ami...",
    beginningBalance: 1801.33,
    beneficiaryAllocation: 0.0,
    distributionAmount: 0.0,
    forfeit: 0.0,
    endingBalance: 1801.33,
    vestingBalance: 1801.33,
    dateTerm: "XX/XX/X...",
    ytdHours: 1788,
    years: "XX",
    vested: "XX%",
    age: "XX"
  },
  {
    badge: 94861,
    name: "Curtis, John",
    beginningBalance: 2922.24,
    beneficiaryAllocation: 0.0,
    distributionAmount: 0.0,
    forfeit: 0.0,
    endingBalance: 2922.24,
    vestingBalance: 2922.24,
    dateTerm: "XX/XX/X...",
    ytdHours: 232.25,
    years: "XX",
    vested: "XX%",
    age: "XX"
  }
];

const ReportGrid: React.FC<ReportGridProps> = ({ params, onLoadingChange }) => {
  const getReportTitle = () => {
    const matchingPreset = presets.find((preset) => JSON.stringify(preset.params) === JSON.stringify(params));

    if (matchingPreset) {
      return matchingPreset.description.toUpperCase();
    }

    return "N/A";
  };

  // Calculate summary data from dummy data
  const summaryData = useMemo(() => {
    const totalBeginningBalance = dummyData.reduce((sum, row) => sum + row.beginningBalance, 0);
    const totalVestedAmount = dummyData.reduce((sum, row) => sum + row.vestingBalance, 0);
    const totalForfeitures = dummyData.reduce((sum, row) => sum + row.forfeit, 0);
    const totalLoans = 0; // No loan data in dummy data
    const totalBeneficiaryAllocations = dummyData.reduce((sum, row) => sum + row.beneficiaryAllocation, 0);

    return {
      amountInProfitSharing: numberToCurrency(totalBeginningBalance),
      vestedAmount: numberToCurrency(totalVestedAmount),
      totalForfeitures: numberToCurrency(totalForfeitures),
      totalLoans: numberToCurrency(totalLoans),
      totalBeneficiaryAllocations: numberToCurrency(totalBeneficiaryAllocations)
    };
  }, []);

  const columnDefs = useMemo(() => GetQPAY066AdHocGridColumns(), []);

  return (
    <Grid2
      container
      spacing={3}>
      {/* Summary Section */}
      <Grid2 size={{ xs: 12 }}>
        <div style={{ padding: "0 24px" }}>
          <Typography
            variant="h2"
            sx={{ color: "#0258A5", marginBottom: "16px" }}>
            {getReportTitle()}
          </Typography>
        </div>
        <div style={{ padding: "0 24px" }}>
          <TableContainer sx={{ mb: 4.5 }}>
            <Table size="small">
              <TableBody>
                <TableRow>
                  <TableCell sx={{ fontWeight: "bold", fontSize: "0.9rem", py: 1.5, width: "20%" }}>
                    Amount In Profit Sharing
                  </TableCell>
                  <TableCell sx={{ fontWeight: "bold", fontSize: "0.9rem", py: 1.5, width: "20%" }}>
                    Vested Amount
                  </TableCell>
                  <TableCell sx={{ fontWeight: "bold", fontSize: "0.9rem", py: 1.5, width: "20%" }}>
                    Total Forfeitures
                  </TableCell>
                  <TableCell sx={{ fontWeight: "bold", fontSize: "0.9rem", py: 1.5, width: "20%" }}>
                    Total Loans
                  </TableCell>
                  <TableCell sx={{ fontWeight: "bold", fontSize: "0.9rem", py: 1.5, width: "20%" }}>
                    Total Beneficiary Allocations
                  </TableCell>
                </TableRow>
                <TableRow>
                  <TableCell sx={{ fontSize: "0.9rem", py: 1.5 }}>{summaryData.amountInProfitSharing}</TableCell>
                  <TableCell sx={{ fontSize: "0.9rem", py: 1.5 }}>{summaryData.vestedAmount}</TableCell>
                  <TableCell sx={{ fontSize: "0.9rem", py: 1.5 }}>{summaryData.totalForfeitures}</TableCell>
                  <TableCell sx={{ fontSize: "0.9rem", py: 1.5 }}>{summaryData.totalLoans}</TableCell>
                  <TableCell sx={{ fontSize: "0.9rem", py: 1.5 }}>{summaryData.totalBeneficiaryAllocations}</TableCell>
                </TableRow>
              </TableBody>
            </Table>
          </TableContainer>
        </div>
      </Grid2>

      {/* Grid Section */}
      <Grid2 size={{ xs: 12 }}>
        <DSMGrid
          preferenceKey="QPAY066_ADHOC_REPORT"
          isLoading={false}
          providedOptions={{
            rowData: dummyData,
            columnDefs: columnDefs
          }}
        />
      </Grid2>
    </Grid2>
  );
};

export default ReportGrid;
