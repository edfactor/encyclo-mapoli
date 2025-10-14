import { Box, CircularProgress, Typography } from "@mui/material";
import React, { useCallback, useEffect, useMemo } from "react";
import { useSelector } from "react-redux";
import { Path, useNavigate } from "react-router-dom";
import { DSMGrid, numberToCurrency, Pagination, TotalsGrid } from "smart-ui-library";
import { useDynamicGridHeight } from "../../hooks/useDynamicGridHeight";
import { useGridPagination } from "../../hooks/useGridPagination";
import { useLazyGetQPAY066BTerminatedWithVestedBalanceQuery } from "../../reduxstore/api/YearsEndApi";
import { RootState } from "../../reduxstore/store";
import { QPAY066BFilterParams } from "./QPAY066BFilterSection";
import { GetQPAY066BGridColumns } from "./QPAY066BGridColumns";

interface QPAY066BGridProps {
  filterParams: QPAY066BFilterParams;
  onLoadingChange?: (isLoading: boolean) => void;
}

const QPAY066BGrid: React.FC<QPAY066BGridProps> = ({ _filterParams, onLoadingChange }) => {
  const navigate = useNavigate();
  const hasToken = useSelector((state: RootState) => !!state.security.token);
  const [getQPAY066BData, { data: qpay066bData, isFetching }] = useLazyGetQPAY066BTerminatedWithVestedBalanceQuery();

  // Use dynamic grid height utility hook
  const gridMaxHeight = useDynamicGridHeight();

  const { pageNumber, pageSize, handlePaginationChange, handleSortChange } = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "badgeNumber",
    initialSortDescending: false,
    onPaginationChange: useCallback(
      (pageNum: number, pageSz: number, sortPrms: any) => {
        if (hasToken) {
          getQPAY066BData({
            profitYear: 2024,
            pagination: {
              take: pageSz,
              skip: pageNum * pageSz,
              sortBy: sortPrms.sortBy,
              isSortDescending: sortPrms.isSortDescending
            }
          });
        }
      },
      [hasToken, getQPAY066BData]
    )
  });

  useEffect(() => {
    onLoadingChange?.(isFetching);
  }, [isFetching, onLoadingChange]);

  useEffect(() => {
    if (hasToken) {
      getQPAY066BData({
        profitYear: 2024,
        pagination: {
          take: pageSize,
          skip: pageNumber * pageSize,
          sortBy: "badgeNumber",
          isSortDescending: false
        }
      });
    }
  }, [hasToken, getQPAY066BData]);

  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  const sortEventHandler = (update: any) => {
    handleSortChange(update);
  };

  const columnDefs = useMemo(() => GetQPAY066BGridColumns(handleNavigationForButton), [handleNavigationForButton]);

  const showTotals = !!qpay066bData?.response?.results;

  return (
    <div className="relative">
      {showTotals && (
        <div className="sticky top-0 z-10 flex bg-white">
          <TotalsGrid
            displayData={[[numberToCurrency(0)]]}
            leftColumnHeaders={["Amount in Profit Sharing"]}
            topRowHeaders={[]}
            breakpoints={{ md: 3, lg: 3 }}
          />
          <TotalsGrid
            displayData={[[numberToCurrency(0)]]}
            leftColumnHeaders={["Vested Amount"]}
            topRowHeaders={[]}
            breakpoints={{ md: 3, lg: 3 }}
          />
          <TotalsGrid
            displayData={[[numberToCurrency(0)]]}
            leftColumnHeaders={["Total Forfeitures"]}
            topRowHeaders={[]}
            breakpoints={{ md: 3, lg: 3 }}
          />
          <TotalsGrid
            displayData={[[numberToCurrency(0)]]}
            leftColumnHeaders={["Total Beneficiary Allocations"]}
            topRowHeaders={[]}
            breakpoints={{ md: 3, lg: 3 }}
          />
        </div>
      )}

      <div style={{ padding: "0 24px" }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          {qpay066bData?.reportName || "QPAY066B"}
        </Typography>
      </div>

      {isFetching ? (
        <Box
          display="flex"
          justifyContent="center"
          alignItems="center"
          py={4}>
          <CircularProgress />
        </Box>
      ) : (
        <>
          <DSMGrid
            preferenceKey="QPAY066B_GRID"
            isLoading={isFetching}
            maxHeight={gridMaxHeight}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: qpay066bData?.response?.results || [],
              columnDefs: columnDefs
            }}
          />
          {!!qpay066bData?.response?.results?.length && (
            <Pagination
              pageNumber={pageNumber}
              setPageNumber={(value: number) => handlePaginationChange(value - 1, pageSize)}
              pageSize={pageSize}
              setPageSize={(value: number) => handlePaginationChange(0, value)}
              recordCount={qpay066bData.response.total}
            />
          )}
        </>
      )}
    </div>
  );
};

export default QPAY066BGrid;
