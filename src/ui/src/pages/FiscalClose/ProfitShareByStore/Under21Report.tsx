import PageErrorBoundary from "@/components/PageErrorBoundary";
import { Box, CircularProgress, Divider, Grid } from "@mui/material";
import { useEffect, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { Page } from "smart-ui-library";
import StatusDropdownActionNode from "../../../components/StatusDropdownActionNode";
import { CAPTIONS, GRID_KEYS } from "../../../constants";
import { useCachedPrevious } from "../../../hooks/useCachedPrevious";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { useGridPagination } from "../../../hooks/useGridPagination";
import { useInitialLoad } from "../../../hooks/useInitialLoad";
import { useLazyGetPostFrozenUnder21Query } from "../../../reduxstore/api/YearsEndApi";
import { RootState } from "../../../reduxstore/store";
import PostFrozenUnder21ReportGrid from "./PostFrozenUnder21ReportGrid";
import Under21ReportTotals from "./Under21ReportTotals";

const Under21Report = () => {
  const [fetchProfitSharingUnder21Report, { isLoading: isBreakdownLoading }] = useLazyGetPostFrozenUnder21Query();
  const profitSharingUnder21Data = useSelector((state: RootState) => state.yearsEnd.profitSharingUnder21Report);
  const [initialLoad, setInitialLoad] = useState(true);
  const { isLoaded: initialSearchLoaded, setLoaded: setInitialSearchLoaded } = useInitialLoad();
  const [manualLoading, setManualLoading] = useState(false);

  const gridPagination = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "badgeNumber",
    initialSortDescending: false,
    persistenceKey: GRID_KEYS.UNDER_21_REPORT
  });

  const profitYear = useDecemberFlowProfitYear();
  const isLoading = isBreakdownLoading || manualLoading;
  const displayTotals = useCachedPrevious(profitSharingUnder21Data ?? null);
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
        await fetchProfitSharingUnder21Report(queryParams);
      } finally {
        setManualLoading(false);
        setInitialLoad(false);
      }
    };

    fetchData();
  }, [
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

      fetchProfitSharingUnder21Report(queryParams).finally(() => {
        setManualLoading(false);
      });
    }
  }, [
    initialSearchLoaded,
    gridPagination.pageNumber,
    gridPagination.pageSize,
    gridPagination.sortParams.sortBy,
    gridPagination.sortParams.isSortDescending,
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

  return (
    <PageErrorBoundary pageName="Under 21 Report">
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
              {displayTotals && (
                <Grid
                  width="100%"
                  paddingX="24px">
                  <Under21ReportTotals
                    totals={displayTotals}
                    isLoading={isLoading}
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
    </PageErrorBoundary>
  );
};

export default Under21Report;
