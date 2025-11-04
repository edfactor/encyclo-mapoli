import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo } from "react";
import { Path } from "react-router";
import { UpdateSummaryResponse } from "reduxstore/types";
import { Pagination } from "smart-ui-library";
import { DSMGrid } from "../../../components/DSMGrid/DSMGrid";
import { useGridPagination } from "../../../hooks/useGridPagination";
import { GetPayMasterUpdateGridColumns } from "./PayMasterUpdateGridColumns";

interface PayMasterUpdateGridProps {
  summaryData: UpdateSummaryResponse | null;
  gridPagination: ReturnType<typeof useGridPagination>;
  pageNumberReset: boolean;
  setPageNumberReset: (reset: boolean) => void;
  isLoading?: boolean;
}

const PayMasterUpdateGrid: React.FC<PayMasterUpdateGridProps> = ({
  summaryData,
  gridPagination,
  pageNumberReset,
  setPageNumberReset,
  isLoading = false
}) => {
  const { pageNumber, pageSize, handlePaginationChange, handleSortChange, resetPagination } = gridPagination;

  useEffect(() => {
    if (pageNumberReset) {
      resetPagination();
      setPageNumberReset(false);
    }
  }, [pageNumberReset, setPageNumberReset, resetPagination]);

  // Mock function to handle navigation (needed for GetPayMasterUpdateGridColumns)
  const handleNavigationForButton = useCallback((destination: string | Partial<Path>) => {
    console.log("Navigation to", destination);
  }, []);

  const columnDefs = useMemo(
    () => GetPayMasterUpdateGridColumns(handleNavigationForButton),
    [handleNavigationForButton]
  );

  const getSummaryRow = useCallback(() => {
    if (!summaryData) return [];

    return [
      {
        psAmountOriginal: summaryData.totalBeforeProfitSharingAmount,
        psVestedOriginal: summaryData.totalBeforeVestedAmount,
        psAmountUpdated: summaryData.totalAfterProfitSharingAmount,
        psVestedUpdated: summaryData.totalAfterVestedAmount
      }
    ];
  }, [summaryData]);

  const gridData = useMemo(() => {
    if (!summaryData?.response?.results) return [];

    return summaryData.response.results.map((employee) => ({
      badgeNumber: employee.badgeNumber,
      employeeName: employee.name,
      storeNumber: employee.storeNumber === 0 ? "-" : employee.storeNumber,
      psAmountOriginal: employee.before.profitSharingAmount,
      psVestedOriginal: employee.before.vestedProfitSharingAmount,
      yearsOriginal: employee.before.yearsInPlan,
      enrollOriginal: employee.before.enrollmentId,
      psAmountUpdated: employee.after.profitSharingAmount,
      psVestedUpdated: employee.after.vestedProfitSharingAmount,
      yearsUpdated: employee.after.yearsInPlan,
      enrollUpdated: employee.after.enrollmentId
    }));
  }, [summaryData]);

  return (
    <>
      {summaryData?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`UPDATE SUMMARY FOR PROFIT SHARING (${summaryData.response.total || 0} records)`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"ELIGIBLE_EMPLOYEES"}
            handleSortChanged={handleSortChange}
            isLoading={isLoading}
            providedOptions={{
              rowData: gridData,
              pinnedTopRowData: getSummaryRow(),
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {summaryData?.response && summaryData.response.results.length > 0 && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => {
            handlePaginationChange(value - 1, pageSize);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            handlePaginationChange(0, value);
          }}
          recordCount={summaryData.response.total}
        />
      )}
    </>
  );
};

export default PayMasterUpdateGrid;
