import React, { useCallback, useEffect, useMemo, useState } from "react";
import { Typography, Box, CircularProgress } from "@mui/material";
import { DSMGrid, ISortParams, Pagination, numberToCurrency } from "smart-ui-library";
import { useNavigate, Path } from "react-router-dom";
import { useSelector } from "react-redux";
import { RootState } from "../../reduxstore/store";
import { QPAY066BTerminatedEmployee } from "../../reduxstore/types";
import { QPAY066BFilterParams } from "./QPAY066BFilterSection";
import { GetQPAY066BGridColumns } from "./QPAY066BGridColumns";
import { useLazyGetQPAY066BTerminatedWithVestedBalanceQuery } from "../../reduxstore/api/YearsEndApi";
import { TotalsGrid } from "../../components/TotalsGrid/TotalsGrid";

interface QPAY066BGridProps {
  filterParams: QPAY066BFilterParams;
  onLoadingChange?: (isLoading: boolean) => void;
}

const QPAY066BGrid: React.FC<QPAY066BGridProps> = ({ filterParams, onLoadingChange }) => {
  const navigate = useNavigate();

  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);

  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });

  const hasToken = useSelector((state: RootState) => !!state.security.token);

  const [getQPAY066BData, { data: qpay066bData, isFetching }] = 
    useLazyGetQPAY066BTerminatedWithVestedBalanceQuery();

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
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        }
      });
    }
  }, [hasToken, pageNumber, pageSize, sortParams, getQPAY066BData]);

  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  const sortEventHandler = (update: ISortParams) => {
    setSortParams(update);
  };

  const columnDefs = useMemo(
    () => GetQPAY066BGridColumns(handleNavigationForButton),
    [handleNavigationForButton]
  );

  const showTotals = !!qpay066bData?.response?.results;

  return (
    <>
      {showTotals && (
        <div className="flex sticky top-0 z-10 bg-white">
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
        <Typography variant="h2" sx={{ color: "#0258A5" }}>
          {qpay066bData?.reportName || "QPAY066B"}
        </Typography>
      </div>

      {isFetching ? (
        <Box display="flex" justifyContent="center" alignItems="center" py={4}>
          <CircularProgress />
        </Box>
      ) : (
        <>
          <DSMGrid
            preferenceKey="QPAY066B_GRID"
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: qpay066bData?.response?.results || [],
              columnDefs: columnDefs
            }}
          />
          {!!qpay066bData?.response?.results?.length && (
            <Pagination
              pageNumber={pageNumber + 1}
              setPageNumber={(value: number) => setPageNumber(value - 1)}
              pageSize={pageSize}
              setPageSize={setPageSize}
              recordCount={qpay066bData.response.total}
            />
          )}
        </>
      )}
    </>
  );
};

export default QPAY066BGrid;