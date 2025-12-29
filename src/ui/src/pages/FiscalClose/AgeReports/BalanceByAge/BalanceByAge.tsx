import PageErrorBoundary from "@/components/PageErrorBoundary";
import { Box, CircularProgress, Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetBalanceByAgeQuery } from "reduxstore/api/YearsEndApi";
import { setBalanceByAgeQueryParams } from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { FrozenReportsByAgeRequestType } from "reduxstore/types";
import { Page } from "smart-ui-library";
import { CAPTIONS } from "../../../../constants";
import BalanceByAgeGrid from "./BalanceByAgeGrid";

const BalanceByAge = () => {
  const [hasInitialSearchRun, setHasInitialSearchRun] = useState(false);
  const [initialLoad, setInitialLoad] = useState(true);
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const profitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();
  const [triggerSearch] = useLazyGetBalanceByAgeQuery();

  useEffect(() => {
    if (hasToken && profitYear && !hasInitialSearchRun) {
      setHasInitialSearchRun(true);

      const fetchReport = (reportType: FrozenReportsByAgeRequestType) => {
        return triggerSearch(
          {
            profitYear: profitYear,
            reportType: reportType,
            pagination: { skip: 0, take: 255 }
          },
          false
        );
      };

      Promise.all([
        fetchReport(FrozenReportsByAgeRequestType.Total),
        fetchReport(FrozenReportsByAgeRequestType.FullTime),
        fetchReport(FrozenReportsByAgeRequestType.PartTime)
      ])
        .then((results) => {
          if (results[0].data) {
            dispatch(setBalanceByAgeQueryParams(profitYear));
          }
        })
        .catch((error) => {
          console.error("Initial balance by age search failed:", error);
        })
        .finally(() => {
          setInitialLoad(false);
        });
    }
  }, [hasToken, profitYear, hasInitialSearchRun, triggerSearch, dispatch]);

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  return (
    <PageErrorBoundary pageName="Balance By Age">
      <Page
        label={CAPTIONS.BALANCE_BY_AGE}
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
            <Grid width="100%">
              <BalanceByAgeGrid />
            </Grid>
          )}
        </Grid>
      </Page>
    </PageErrorBoundary>
  );
};

export default BalanceByAge;
