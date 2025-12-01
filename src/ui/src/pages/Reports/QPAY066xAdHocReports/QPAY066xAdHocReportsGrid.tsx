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
import { DSMGrid, numberToCurrency, Pagination } from "smart-ui-library";
import { useDynamicGridHeight } from "../../../hooks/useDynamicGridHeight";
import { useGridPagination } from "../../../hooks/useGridPagination";
import { GetQPAY066xAdHocCommonGridColumns } from "./QPAY066xAdHocGridColumns";

interface QPAY066xAdHocReportsGridProps {
  reportTitle: string;
  isLoading: boolean;
  storeNumber: string;
  gridPagination: ReturnType<typeof useGridPagination>;
}

const QPAY066xAdHocReportsGrid: React.FC<QPAY066xAdHocReportsGridProps> = ({
  reportTitle,
  isLoading,
  storeNumber,
  gridPagination
}) => {
  const { pageNumber, pageSize, handlePaginationChange } = gridPagination;

  const breakdownByStoreManagement = useSelector((state: RootState) => state.yearsEnd.breakdownByStoreManagement);
  const breakdownByStore = useSelector((state: RootState) => state.yearsEnd.breakdownByStore);
  const breakdownByStoreTotals = useSelector((state: RootState) => state.yearsEnd.breakdownByStoreTotals);

  // Use dynamic grid height utility hook
  const gridMaxHeight = useDynamicGridHeight();

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
    
    
    
    return GetQPAY066xAdHocCommonGridColumns();
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
        <DSMGrid
          preferenceKey="QPAY066_ADHOC_REPORT"
          isLoading={isLoading}
          maxHeight={gridMaxHeight}
          providedOptions={{
            rowData: rowData,
            columnDefs: columnDefs
          }}
        />
      </Grid>

      {/* Pagination */}
      {rowData && rowData.length > 0 && (
        <Grid size={{ xs: 12 }}>
          <Pagination
            pageNumber={pageNumber}
            setPageNumber={(value: number) => {
              handlePaginationChange(value - 1, pageSize);
            }}
            pageSize={pageSize}
            setPageSize={(value: number) => {
              handlePaginationChange(0, value);
            }}
            recordCount={totalRecords}
          />
        </Grid>
      )}
    </Grid>
  );
};

export default QPAY066xAdHocReportsGrid;
