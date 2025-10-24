import { Box, CircularProgress, Divider } from "@mui/material";
import { Grid } from "@mui/material";
import { Page } from "smart-ui-library";
import ForfeituresByAgeGrid from "./ForfeituresByAgeGrid";
import { useState, useEffect } from "react";
import { useSelector, useDispatch } from "react-redux";
import { RootState } from "reduxstore/store";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useLazyGetForfeituresByAgeQuery } from "reduxstore/api/YearsEndApi";
import { setForfeituresByAgeQueryParams } from "reduxstore/slices/yearsEndSlice";
import { FrozenReportsByAgeRequestType } from "reduxstore/types";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
const ForfeituresByAge = () => {
  const [hasInitialSearchRun, setHasInitialSearchRun] = useState(false);
  const [initialLoad, setInitialLoad] = useState(true);
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const profitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();
  const [triggerSearch] = useLazyGetForfeituresByAgeQuery();

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
            dispatch(setForfeituresByAgeQueryParams(profitYear));
          }
        })
        .catch((error) => {
          console.error("Initial forfeitures by age search failed:", error);
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
      label="Forfeitures By Age"
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
            <ForfeituresByAgeGrid />
          </Grid>
        )}
      </Grid>
    </Page>
  );
};

export default ForfeituresByAge;
