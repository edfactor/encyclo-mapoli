import { Box, CircularProgress, Divider, Grid } from "@mui/material";
import { useEffect, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { Page } from "smart-ui-library";
import StatusDropdownActionNode from "../../../components/StatusDropdownActionNode";
import { CAPTIONS } from "../../../constants";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { useLazyGetPostFrozenUnder21Query, useLazyGetUnder21TotalsQuery } from "../../../reduxstore/api/YearsEndApi";
import { RootState } from "../../../reduxstore/store";
import Under21BreakdownGrid from "./Under21/Under21BreakdownGrid";
import Under21Summary from "./Under21/Under21Summary";

const Under21Report = () => {
  const [fetchUnder21Totals, { isLoading: isTotalsLoading }] = useLazyGetUnder21TotalsQuery();
  const [fetchUnder21Breakdown, { isLoading: isBreakdownLoading }] = useLazyGetPostFrozenUnder21Query();
  const under21Totals = useSelector((state: RootState) => state.yearsEnd.under21Totals);
  const under21Breakdown = useSelector((state: RootState) => state.yearsEnd.under21BreakdownByStore);
  const [initialLoad, setInitialLoad] = useState(true);
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [pageNumber, setPageNumber] = useState<number>(0);
  const [pageSize, setPageSize] = useState<number>(25);
  const [sortParams, setSortParams] = useState({
    sortBy: "badgeNumber",
    isSortDescending: false
  });

  const profitYear = useDecemberFlowProfitYear();
  const isLoading = isTotalsLoading || isBreakdownLoading;
  const hasData = !!under21Totals && !!under21Breakdown;
  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  useEffect(() => {
    const fetchData = async () => {
      const queryParams = {
        profitYear: profitYear,
        isSortDescending: sortParams.isSortDescending,
        pagination: {
          take: pageSize,
          skip: pageNumber * pageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        }
      };

      await Promise.all([fetchUnder21Totals(queryParams), fetchUnder21Breakdown(queryParams)]);

      setInitialLoad(false);
    };

    fetchData();
  }, [fetchUnder21Totals, fetchUnder21Breakdown, pageNumber, pageSize, sortParams, profitYear]);

  useEffect(() => {
    if (initialSearchLoaded) {
      const queryParams = {
        profitYear: profitYear,
        isSortDescending: sortParams.isSortDescending,
        pagination: {
          take: pageSize,
          skip: pageNumber * pageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        }
      };

      fetchUnder21Totals(queryParams);
      fetchUnder21Breakdown(queryParams);
    }
  }, [initialSearchLoaded, pageNumber, pageSize, sortParams, fetchUnder21Totals, fetchUnder21Breakdown, profitYear]);

  // Need a useEffect to reset the page number when under21Totals changes
  const prevUnder21Totals = useRef<typeof under21Totals>(null);
  useEffect(() => {
    if (under21Totals?.numberOfEmployees !== prevUnder21Totals.current?.numberOfEmployees) {
      setPageNumber(0);
    }
    prevUnder21Totals.current = under21Totals;
  }, [under21Totals]);

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
              <Under21BreakdownGrid
                isLoading={isBreakdownLoading}
                setInitialSearchLoaded={setInitialSearchLoaded}
                pageNumber={pageNumber}
                setPageNumber={setPageNumber}
                pageSize={pageSize}
                setPageSize={setPageSize}
                sortParams={sortParams}
                setSortParams={setSortParams}
              />
            </Grid>
          </>
        )}
      </Grid>
    </Page>
  );
};

export default Under21Report;
