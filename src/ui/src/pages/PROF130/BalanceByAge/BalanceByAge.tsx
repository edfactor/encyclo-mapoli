import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { Page } from "smart-ui-library";
import BalanceByAgeGrid from "./BalanceByAgeGrid";
import { useState, useEffect } from "react";
import { useSelector, useDispatch } from "react-redux";
import { RootState } from "reduxstore/store";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useLazyGetBalanceByAgeQuery } from "reduxstore/api/YearsEndApi";
import { setBalanceByAgeQueryParams } from "reduxstore/slices/yearsEndSlice";
import { FrozenReportsByAgeRequestType } from "reduxstore/types";

const BalanceByAge = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [hasInitialSearchRun, setHasInitialSearchRun] = useState(false);
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
        .then(results => {
          if (results[0].data) {
            dispatch(setBalanceByAgeQueryParams(profitYear));
            setInitialSearchLoaded(true);
          }
        })
        .catch(error => {
          console.error("Initial balance by age search failed:", error);
        });
    }
  }, [hasToken, profitYear, hasInitialSearchRun, triggerSearch, dispatch]);

  return (
    <Page label="Balance By Age">
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>

        <Grid2 width="100%">
          <BalanceByAgeGrid initialSearchLoaded={initialSearchLoaded} />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default BalanceByAge;
