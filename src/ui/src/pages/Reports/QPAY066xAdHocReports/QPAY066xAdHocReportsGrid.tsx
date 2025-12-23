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
import { ColDef } from "ag-grid-community";
import React, { useMemo } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { numberToCurrency } from "smart-ui-library";
import { DSMPaginatedGrid } from "../../../components/DSMPaginatedGrid/DSMPaginatedGrid";
import { GRID_KEYS } from "../../../constants";
import { useContentAwareGridHeight } from "../../../hooks/useContentAwareGridHeight";
import { useGridPagination } from "../../../hooks/useGridPagination";
import {
    GetQPAY066xAdHocCommonGridColumns,
    GetQPAY066xAgeAtTerminationColumn,
    GetQPAY066xAgeColumn,
    GetQPAY066xBeneficiaryAllocationColumn,
    GetQPAY066xDistributionAmountColumn,
    GetQPAY066xEnrollmentCodeColumn,
    GetQPAY066xInactiveDateColumn,
    GetQPAY066xPSYearsColumn,
    GetQPAY066xTerminationCodeColumn,
    GetQPAY066xTerminationDateColumn,
    GetQPAY066xVestedPercentageColumn
} from "./QPAY066xAdHocGridColumns";

interface QPAY066xAdHocReportsGridProps {
  reportTitle: string;
  reportId: string;
  isLoading: boolean;
  storeNumber: string;
  gridPagination: ReturnType<typeof useGridPagination>;
}

const QPAY066xAdHocReportsGrid: React.FC<QPAY066xAdHocReportsGridProps> = ({
  reportTitle,
  reportId,
  isLoading,
  storeNumber,
  gridPagination
}) => {
  const { pageNumber, pageSize, handlePageNumberChange, handlePageSizeChange } = gridPagination;

  const breakdownByStoreManagement = useSelector((state: RootState) => state.yearsEnd.breakdownByStoreManagement);
  const breakdownByStore = useSelector((state: RootState) => state.yearsEnd.breakdownByStore);
  const breakdownByStoreTotals = useSelector((state: RootState) => state.yearsEnd.breakdownByStoreTotals);

  // Check if storeNumber is provided to determine if totals should be shown
  const showTotals = storeNumber && storeNumber.trim() !== "";

  // Get total count from Redux state
  const totalRecords = useMemo(() => {
    if (breakdownByStoreManagement?.response?.total) {
      return breakdownByStoreManagement.response.total;
    }
    if (breakdownByStore?.response?.total) {
      return breakdownByStore.response.total;
    }
    return 0;
  }, [breakdownByStoreManagement, breakdownByStore]);

  // Calculate summary data
  const summaryData = useMemo(() => {
    if (breakdownByStoreTotals) {
      return {
        amountInProfitSharing: numberToCurrency(breakdownByStoreTotals.totalBeginningBalances),
        vestedAmount: numberToCurrency(breakdownByStoreTotals.totalVestedBalance),
        totalForfeitures: numberToCurrency(breakdownByStoreTotals.totalForfeitures),
        totalLoans: numberToCurrency(0), // Not provided in the API response
        totalBeneficiaryAllocations: numberToCurrency(0) // Not provided in the API response
      };
    }
    return {
      amountInProfitSharing: numberToCurrency(0),
      vestedAmount: numberToCurrency(0),
      totalForfeitures: numberToCurrency(0),
      totalLoans: numberToCurrency(0),
      totalBeneficiaryAllocations: numberToCurrency(0)
    };
  }, [breakdownByStoreTotals]);

  const columnDefs = (reportType: string): ColDef[] => {
    console.log("Generating columns for report type:", reportType);

    let columns: ColDef[] = GetQPAY066xAdHocCommonGridColumns();

    switch (reportType) {
      case "QPAY066A":
      case "QPAY066AF":
        columns = [
          ...columns,
          GetQPAY066xBeneficiaryAllocationColumn(),
          GetQPAY066xDistributionAmountColumn(),
          GetQPAY066xPSYearsColumn(),
          GetQPAY066xAgeAtTerminationColumn(),
          GetQPAY066xEnrollmentCodeColumn()
        ];
        break;
      case "QPAY066A-1":
        columns = [
          ...columns,
          GetQPAY066xBeneficiaryAllocationColumn(),
          GetQPAY066xDistributionAmountColumn(),
          GetQPAY066xPSYearsColumn(),
          GetQPAY066xAgeColumn()
        ];
        break;
      case "QPAY066B":
        columns = [
          ...columns,
          GetQPAY066xBeneficiaryAllocationColumn(),
          GetQPAY066xTerminationDateColumn(),
          GetQPAY066xVestedPercentageColumn(),
          GetQPAY066xDistributionAmountColumn(),
          GetQPAY066xAgeAtTerminationColumn(),
          GetQPAY066xEnrollmentCodeColumn()
        ];
        break;
      case "QPAY066-AGE70":
      case "QPAY066F":
        columns = [
          ...columns,
          GetQPAY066xBeneficiaryAllocationColumn(),
          GetQPAY066xTerminationDateColumn(),
          GetQPAY066xVestedPercentageColumn(),
          GetQPAY066xDistributionAmountColumn(),
          GetQPAY066xAgeColumn(),
          GetQPAY066xEnrollmentCodeColumn()
        ];
        break;
      case "QPAY066D":
      case "QPAY066-I":
        columns = [
          ...columns,
          GetQPAY066xBeneficiaryAllocationColumn(),
          GetQPAY066xTerminationDateColumn(),
          GetQPAY066xVestedPercentageColumn(),
          GetQPAY066xDistributionAmountColumn(),
          GetQPAY066xAgeColumn(),
          GetQPAY066xAgeAtTerminationColumn(),
          GetQPAY066xEnrollmentCodeColumn()
        ];
        break;
      case "QPAY066-INACTIVE":
        columns = [
          ...columns,
          GetQPAY066xBeneficiaryAllocationColumn(),
          GetQPAY066xTerminationDateColumn(),
          GetQPAY066xInactiveDateColumn(),
          GetQPAY066xVestedPercentageColumn(),
          GetQPAY066xTerminationCodeColumn(),
          GetQPAY066xAgeColumn(),
          GetQPAY066xAgeAtTerminationColumn(),
          GetQPAY066xEnrollmentCodeColumn()
        ];
        break;
      case "QPAY066W":
        columns = [
          ...columns,
          GetQPAY066xBeneficiaryAllocationColumn(),
          GetQPAY066xVestedPercentageColumn(),
          GetQPAY066xDistributionAmountColumn(),
          GetQPAY066xAgeColumn(),
          GetQPAY066xAgeAtTerminationColumn(),
          GetQPAY066xEnrollmentCodeColumn()
        ];
        break;
      case "QPAY066M":
        columns = [
          ...columns,
          GetQPAY066xBeneficiaryAllocationColumn(),
          GetQPAY066xTerminationDateColumn(),
          GetQPAY066xVestedPercentageColumn(),
          GetQPAY066xDistributionAmountColumn(),
          GetQPAY066xAgeColumn(),
          GetQPAY066xEnrollmentCodeColumn()
        ];
        break;
      default:
        break;
    }
    return columns;
  };

  const rowData = useMemo(() => {
    // Check both locations - data location depends on storeManagement flag
    if (breakdownByStoreManagement?.response?.results) {
      return breakdownByStoreManagement.response.results;
    }
    if (breakdownByStore?.response?.results) {
      return breakdownByStore.response.results;
    }
    return [];
  }, [breakdownByStoreManagement, breakdownByStore]);

  // Use content-aware grid height utility hook
  const gridMaxHeight = useContentAwareGridHeight({
    rowCount: rowData?.length ?? 0
  });

  return (
    <Grid
      container
      spacing={3}>
      {/* Summary Section - Only show if storeNumber is provided */}
      <Grid size={{ xs: 12 }}>
        <div style={{ padding: "0 24px" }}>
          <Typography
            variant="h2"
            sx={{ color: "#0258A5", marginBottom: "16px" }}>
            {reportTitle}
          </Typography>
        </div>
        {showTotals && (
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
                    {isLoading ? (
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
        )}
      </Grid>

      {/* Grid Section */}
      <Grid size={{ xs: 12 }}>
        <DSMPaginatedGrid
          preferenceKey={GRID_KEYS.QPAY066_ADHOC_REPORT}
          data={rowData ?? []}
          columnDefs={columnDefs(reportId)}
          totalRecords={totalRecords}
          isLoading={isLoading}
          heightConfig={{ maxHeight: gridMaxHeight }}
          pagination={{
            pageNumber,
            pageSize,
            sortParams: { sortBy: "", isSortDescending: false },
            handlePageNumberChange,
            handlePageSizeChange,
            handleSortChange: () => {}
          }}
          gridOptions={{
            maintainColumnOrder: true
          }}
          showPagination={rowData && rowData.length > 0}
        />
      </Grid>
    </Grid>
  );
};

export default QPAY066xAdHocReportsGrid;
