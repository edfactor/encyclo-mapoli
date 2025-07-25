import { Divider } from "@mui/material";
import { Grid } from "@mui/material";
import { Page } from "smart-ui-library";
import ContributionsByAgeGrid from "./ContributionsByAgeGrid";
import { useState, useEffect } from "react";
import { useSelector, useDispatch } from "react-redux";
import { RootState } from "reduxstore/store";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useLazyGetContributionsByAgeQuery } from "reduxstore/api/YearsEndApi";
import { setContributionsByAgeQueryParams } from "reduxstore/slices/yearsEndSlice";
import { FrozenReportsByAgeRequestType } from "reduxstore/types";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";

const ContributionsByAge = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [hasInitialSearchRun, setHasInitialSearchRun] = useState(false);
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const profitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();
  const [triggerSearch] = useLazyGetContributionsByAgeQuery();

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
            dispatch(setContributionsByAgeQueryParams(profitYear));
            setInitialSearchLoaded(true);
          }
        })
        .catch((error) => {
          console.error("Initial contributions by age search failed:", error);
        });
    }
  }, [hasToken, profitYear, hasInitialSearchRun, triggerSearch, dispatch]);

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  return (
    <Page
      label="Contributions By Age"
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>

        <Grid width="100%">
          <ContributionsByAgeGrid initialSearchLoaded={initialSearchLoaded} />
        </Grid>
      </Grid>
    </Page>
  );
};

export default ContributionsByAge;
