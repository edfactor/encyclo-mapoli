import { Box, CircularProgress, Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetForfeituresByAgeQuery } from "reduxstore/api/YearsEndApi";
import { setForfeituresByAgeQueryParams } from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { FrozenReportsByAgeRequestType } from "reduxstore/types";
import { Page } from "smart-ui-library";
import { CAPTIONS } from "../../../../constants";
import ForfeituresByAgeGrid from "./ForfeituresByAgeGrid";
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
      label={CAPTIONS.FORFEITURES_BY_AGE}
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
