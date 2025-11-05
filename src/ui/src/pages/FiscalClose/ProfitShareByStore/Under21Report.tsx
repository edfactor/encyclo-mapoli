import { Box, CircularProgress, Divider, Grid } from "@mui/material";
import { useEffect, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { Page } from "smart-ui-library";
import StatusDropdownActionNode from "../../../components/StatusDropdownActionNode";
import { CAPTIONS } from "../../../constants";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { useGridPagination } from "../../../hooks/useGridPagination";
import { useLazyGetPostFrozenUnder21Query, useLazyGetUnder21TotalsQuery } from "../../../reduxstore/api/YearsEndApi";
import { RootState } from "../../../reduxstore/store";
import PostFrozenUnder21ReportGrid from "./PostFrozenUnder21ReportGrid";
import Under21Summary from "./Under21/Under21Summary";

const Under21Report = () => {
  const [fetchUnder21Totals, { isLoading: isTotalsLoading }] = useLazyGetUnder21TotalsQuery();
  const [fetchProfitSharingUnder21Report, { isLoading: isBreakdownLoading }] = useLazyGetPostFrozenUnder21Query();
  const under21Totals = useSelector((state: RootState) => state.yearsEnd.under21Totals);
  const under21Breakdown = useSelector((state: RootState) => state.yearsEnd.profitSharingUnder21Report);
  const [initialLoad, setInitialLoad] = useState(true);
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [manualLoading, setManualLoading] = useState(false);

  const gridPagination = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "badgeNumber",
    initialSortDescending: false
  });

  const profitYear = useDecemberFlowProfitYear();
  const isLoading = isTotalsLoading || isBreakdownLoading || manualLoading;
  const hasData = !!under21Totals && !!under21Breakdown;
  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  useEffect(() => {
    const fetchData = async () => {
      setManualLoading(true);
      
      const queryParams = {
        profitYear: profitYear,
        isSortDescending: gridPagination.sortParams.isSortDescending,
        pagination: {
          take: gridPagination.pageSize,
          skip: gridPagination.pageNumber * gridPagination.pageSize,
          sortBy: gridPagination.sortParams.sortBy,
          isSortDescending: gridPagination.sortParams.isSortDescending
        }
      };

      try {
        await Promise.all([fetchUnder21Totals(queryParams), fetchProfitSharingUnder21Report(queryParams)]);
      } finally {
        setManualLoading(false);
        setInitialLoad(false);
      }
    };

    fetchData();
  }, [
    fetchUnder21Totals,
    fetchProfitSharingUnder21Report,
    gridPagination.pageNumber,
    gridPagination.pageSize,
    gridPagination.sortParams.sortBy,
    gridPagination.sortParams.isSortDescending,
    profitYear
  ]);

  useEffect(() => {
    if (initialSearchLoaded) {
      setManualLoading(true);
      
      const queryParams = {
        profitYear: profitYear,
        isSortDescending: gridPagination.sortParams.isSortDescending,
        pagination: {
          take: gridPagination.pageSize,
          skip: gridPagination.pageNumber * gridPagination.pageSize,
          sortBy: gridPagination.sortParams.sortBy,
          isSortDescending: gridPagination.sortParams.isSortDescending
        }
      };

      Promise.all([
        fetchUnder21Totals(queryParams),
        fetchProfitSharingUnder21Report(queryParams)
      ]).finally(() => {
        setManualLoading(false);
      });
    }
  }, [
    initialSearchLoaded,
    gridPagination.pageNumber,
    gridPagination.pageSize,
    gridPagination.sortParams.sortBy,
    gridPagination.sortParams.isSortDescending,
    fetchUnder21Totals,
    fetchProfitSharingUnder21Report,
    profitYear
  ]);

  // Reset page number when profit year changes (not when data updates during pagination)
  const prevProfitYear = useRef<number>(profitYear);
  useEffect(() => {
    if (profitYear !== prevProfitYear.current) {
      gridPagination.handlePaginationChange(0, gridPagination.pageSize);
      prevProfitYear.current = profitYear;
    }
  }, [profitYear, gridPagination]);

  /*
  const handleSearch = (profitYear: number, isSortDescending: boolean) => {
    const queryParams = {
      profitYear,
      isSortDescending,
      pagination: {
        take: pageSize,
        skip: pageNumber * pageSize,
        sortBy: sortParams.sortBy,
        isSortDescending
      }
    };

    fetchUnder21Totals(queryParams);
    fetchUnder21Breakdown(queryParams);
  };

  */

  return (
    <Page
      label={CAPTIONS.QPAY066_UNDER21}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>

        {initialLoad ? (
          <Grid width="100%">
            <Box
              display="flex"
              justifyContent="center"
              alignItems="center"
              minHeight="200px">
              <CircularProgress />
            </Box>
          </Grid>
        ) : (
          <>
            {hasData && !isLoading && (
              <Grid
                width="100%"
                paddingX="24px">
                <Under21Summary
                  totals={under21Totals}
                  isLoading={isTotalsLoading}
                  title="UNDER 21 REPORT (QPAY066-UNDR21)"
                />
              </Grid>
            )}

            <Grid width="100%">
              <PostFrozenUnder21ReportGrid
                isLoading={isLoading}
                setInitialSearchLoaded={setInitialSearchLoaded}
                gridPagination={gridPagination}
              />
            </Grid>
          </>
        )}
      </Grid>
    </Page>
  );
};

export default Under21Report;
