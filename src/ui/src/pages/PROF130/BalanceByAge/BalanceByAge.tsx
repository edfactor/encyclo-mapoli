import { Box, CircularProgress, Divider } from "@mui/material";
import { Grid } from "@mui/material";
import { Page } from "smart-ui-library";
import BalanceByAgeGrid from "./BalanceByAgeGrid";
import { useState, useEffect } from "react";
import { useSelector, useDispatch } from "react-redux";
import { RootState } from "reduxstore/store";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useLazyGetBalanceByAgeQuery } from "reduxstore/api/YearsEndApi";
import { setBalanceByAgeQueryParams } from "reduxstore/slices/yearsEndSlice";
import { FrozenReportsByAgeRequestType } from "reduxstore/types";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";

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
    <Page
      label="Balance By Age"
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
  );
};

export default BalanceByAge;
