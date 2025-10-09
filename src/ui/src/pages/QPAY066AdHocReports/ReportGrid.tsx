import {
  Box,
  CircularProgress,
  Grid,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableRow,
  Typography
} from "@mui/material";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import React, { useEffect, useMemo } from "react";
import { useSelector } from "react-redux";
import { useLazyGetBreakdownByStoreQuery, useLazyGetBreakdownByStoreTotalsQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { FilterParams } from "reduxstore/types";
import { DSMGrid, numberToCurrency } from "smart-ui-library";
import presets from "./presets";
import { GetQPAY066AdHocGridColumns } from "./QPAY066AdHocGridColumns";

interface ReportGridProps {
  params: FilterParams;
  storeNumber: number;
  onLoadingChange?: (isLoading: boolean) => void;
}

// Dummy data for demonstration
const dummyData = [
  {
    badgeNumber: 47425,
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
    badgeNumber: 82424,
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
    badgeNumber: 85744,
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
    badgeNumber: 94861,
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

const ReportGrid: React.FC<ReportGridProps> = ({ params, storeNumber, onLoadingChange }) => {
  const [fetchBreakdownByStore, { isFetching: isBreakdownFetching }] = useLazyGetBreakdownByStoreQuery();
  const [fetchBreakdownByStoreTotals, { isFetching: isTotalsFetching }] = useLazyGetBreakdownByStoreTotalsQuery();

  const breakdownByStoreManagement = useSelector((state: RootState) => state.yearsEnd.breakdownByStoreMangement);
  const breakdownByStoreTotals = useSelector((state: RootState) => state.yearsEnd.breakdownByStoreTotals);
  const hasToken = useSelector((state: RootState) => state.security.token);

  const profitYear = useDecemberFlowProfitYear();

  const isQPAY066MReport = params.reportId === 8;

  const isLoadingQPAY066MData = isQPAY066MReport && (isBreakdownFetching || isTotalsFetching);

  useEffect(() => {
    if (isQPAY066MReport && hasToken && profitYear && storeNumber) {
      fetchBreakdownByStore({
        profitYear: profitYear,
        storeNumber: storeNumber,
        storeManagement: true,
        pagination: {
          skip: 0,
          take: 255,
          sortBy: "badgeNumber",
          isSortDescending: false
        }
      });

      // Fetch totals data
      fetchBreakdownByStoreTotals({
        profitYear: profitYear,
        storeNumber: storeNumber,
        storeManagement: true,
        pagination: {
          skip: 0,
          take: 255,
          sortBy: "",
          isSortDescending: false
        }
      });
    }
  }, [isQPAY066MReport, hasToken, profitYear, storeNumber, fetchBreakdownByStore, fetchBreakdownByStoreTotals]);

  useEffect(() => {
    if (onLoadingChange) {
      onLoadingChange(isBreakdownFetching || isTotalsFetching);
    }
  }, [isBreakdownFetching, isTotalsFetching, onLoadingChange]);

  const getReportTitle = () => {
    const matchingPreset = presets.find((preset) => preset.params.reportId === params.reportId);

    if (matchingPreset) {
      return matchingPreset.description.toUpperCase();
    }

    return "N/A";
  };

  // Calculate summary data
  const summaryData = useMemo(() => {
    if (isQPAY066MReport && breakdownByStoreTotals) {
      return {
        amountInProfitSharing: numberToCurrency(breakdownByStoreTotals.totalBeginningBalances),
        vestedAmount: numberToCurrency(breakdownByStoreTotals.totalVestedBalance),
        totalForfeitures: numberToCurrency(breakdownByStoreTotals.totalForfeitures),
        totalLoans: numberToCurrency(0), // Not provided in the API response
        totalBeneficiaryAllocations: numberToCurrency(0) // Not provided in the API response
      };
    } else {
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
    }
  }, [isQPAY066MReport, breakdownByStoreTotals]);

  const columnDefs = useMemo(() => {
    return GetQPAY066AdHocGridColumns();
  }, []);

  const rowData = useMemo(() => {
    if (isQPAY066MReport && breakdownByStoreManagement?.response?.results) {
      return breakdownByStoreManagement.response.results;
    }
    return dummyData;
  }, [isQPAY066MReport, breakdownByStoreManagement]);

  /*
  const recordCount = useMemo(() => {
    if (isQPAY066MReport && breakdownByStoreManagement?.response?.total) {
      return breakdownByStoreManagement.response.total;
    }
    return dummyData.length;
  }, [isQPAY066MReport, breakdownByStoreManagement]);
*/
  return (
    <Grid
      container
      spacing={3}>
      {/* Summary Section */}
      <Grid size={{ xs: 12 }}>
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
                  {isLoadingQPAY066MData ? (
                    <TableCell
                      colSpan={5}
                      sx={{ textAlign: "center", py: 3 }}>
                      <Box
                        display="flex"
                        justifyContent="center"
                        alignItems="center"
                        gap={2}>
                        <CircularProgress size={20} />
                        <Typography
                          variant="body2"
                          color="text.secondary"></Typography>
                      </Box>
                    </TableCell>
                  ) : (
                    <>
                      <TableCell sx={{ fontSize: "0.9rem", py: 1.5 }}>{summaryData.amountInProfitSharing}</TableCell>
                      <TableCell sx={{ fontSize: "0.9rem", py: 1.5 }}>{summaryData.vestedAmount}</TableCell>
                      <TableCell sx={{ fontSize: "0.9rem", py: 1.5 }}>{summaryData.totalForfeitures}</TableCell>
                      <TableCell sx={{ fontSize: "0.9rem", py: 1.5 }}>{summaryData.totalLoans}</TableCell>
                      <TableCell sx={{ fontSize: "0.9rem", py: 1.5 }}>
                        {summaryData.totalBeneficiaryAllocations}
                      </TableCell>
                    </>
                  )}
                </TableRow>
              </TableBody>
            </Table>
          </TableContainer>
        </div>
      </Grid>

      {/* Grid Section */}
      <Grid size={{ xs: 12 }}>
        <DSMGrid
          preferenceKey="QPAY066_ADHOC_REPORT"
          isLoading={isBreakdownFetching || isTotalsFetching}
          providedOptions={{
            rowData: rowData,
            columnDefs: columnDefs
          }}
        />
      </Grid>
    </Grid>
  );
};

export default ReportGrid;
