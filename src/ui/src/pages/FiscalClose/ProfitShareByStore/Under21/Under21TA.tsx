import PageErrorBoundary from "@/components/PageErrorBoundary";
import { Box, CircularProgress, Divider, Grid } from "@mui/material";
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { Page } from "smart-ui-library";
import StatusDropdownActionNode from "../../../../components/StatusDropdownActionNode";
import { CAPTIONS } from "../../../../constants";
import { useInitialLoad } from "../../../../hooks/useInitialLoad";
import useNavigationYear from "../../../../hooks/useNavigationYear";
import {
    useLazyGetUnder21BreakdownByStoreQuery,
    useLazyGetUnder21TotalsQuery
} from "../../../../reduxstore/api/YearsEndApi";
import { RootState } from "../../../../reduxstore/store";
import Under21BreakdownGrid from "./Under21BreakdownGrid";
import Under21Summary from "./Under21Summary";

const Under21TA = () => {
  const [fetchUnder21Totals, { isLoading: isTotalsLoading }] = useLazyGetUnder21TotalsQuery();
  const [fetchUnder21BreakdownByStore, { isLoading: isInactiveLoading }] = useLazyGetUnder21BreakdownByStoreQuery();
  const under21Totals = useSelector((state: RootState) => state.yearsEnd.under21Totals);
  const under21BreakdownByStore = useSelector((state: RootState) => state.yearsEnd.under21BreakdownByStore);
  const [initialLoad, setInitialLoad] = useState(true);
  const { isLoaded: initialSearchLoaded, setLoaded: setInitialSearchLoaded } = useInitialLoad();
  const [pageNumber, setPageNumber] = useState<number>(0);
  const [pageSize] = useState<number>(25);
  const [sortParams] = useState({
    sortBy: "badgeNumber",
    isSortDescending: false
  });
  const profitYear = useNavigationYear();

  const isLoading = isTotalsLoading || isInactiveLoading;

  const hasData = !!under21Totals && !!under21BreakdownByStore;
  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  useEffect(() => {
    const fetchData = async () => {
      const queryParams = {
        profitYear: profitYear ?? new Date().getFullYear(),
        isSortDescending: sortParams.isSortDescending,
        pagination: {
          take: pageSize,
          skip: pageNumber * pageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        }
      };

      await Promise.all([fetchUnder21Totals(queryParams), fetchUnder21BreakdownByStore(queryParams)]);

      setInitialLoad(false);
    };

    fetchData();
  }, [fetchUnder21Totals, fetchUnder21BreakdownByStore, pageNumber, pageSize, sortParams, profitYear]);

  useEffect(() => {
    if (initialSearchLoaded) {
      const queryParams = {
        profitYear: profitYear ?? new Date().getFullYear(),
        isSortDescending: sortParams.isSortDescending,
        pagination: {
          take: pageSize,
          skip: pageNumber * pageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        }
      };

      fetchUnder21Totals(queryParams);
      fetchUnder21BreakdownByStore(queryParams);
    }
  }, [
    initialSearchLoaded,
    pageNumber,
    pageSize,
    sortParams,
    fetchUnder21Totals,
    fetchUnder21BreakdownByStore,
    profitYear
  ]);

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
    fetchUnder21Inactive(queryParams);
  };
*/
  return (
    <PageErrorBoundary pageName="Under 21 TA">
      <Page
        label={CAPTIONS.QPAY066TA_UNDER21}
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
                    title="UNDER 21 (QPAY066TA-UNDR21)"
                  />
                </Grid>
              )}

              <Grid width="100%">
                <Under21BreakdownGrid
                  isLoading={isInactiveLoading}
                  setInitialSearchLoaded={setInitialSearchLoaded}
                  pageNumber={pageNumber}
                  pageSize={pageSize}
                  onPageChange={setPageNumber}
                />
              </Grid>
            </>
          )}
        </Grid>
      </Page>
    </PageErrorBoundary>
  );
};

export default Under21TA;
