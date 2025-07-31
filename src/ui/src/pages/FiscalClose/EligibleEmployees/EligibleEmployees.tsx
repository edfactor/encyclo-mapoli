import { Divider } from "@mui/material";
import { Grid } from "@mui/material";
import { Page } from "smart-ui-library";
import { useState, useEffect } from "react";
import EligibleEmployeesGrid from "./EligibleEmployeesGrid";
import { useSelector, useDispatch } from "react-redux";
import { RootState } from "reduxstore/store";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useLazyGetEligibleEmployeesQuery } from "reduxstore/api/YearsEndApi";
import { setEligibleEmployeesQueryParams } from "reduxstore/slices/yearsEndSlice";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";

const EligibleEmployees = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [hasInitialSearchRun, setHasInitialSearchRun] = useState(false);
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const profitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();
  const [triggerSearch] = useLazyGetEligibleEmployeesQuery();

  useEffect(() => {
    if (hasToken && profitYear && !hasInitialSearchRun) {
      setHasInitialSearchRun(true);

      const request = {
        profitYear: profitYear,
        pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: false }
      };

      triggerSearch(request, false)
        .then((result) => {
          if (result.data) {
            dispatch(setEligibleEmployeesQueryParams(profitYear));
            setInitialSearchLoaded(true);
          }
        })
        .catch((error) => {
          console.error("Initial eligible employees search failed:", error);
        });
    }
  }, [hasToken, profitYear, hasInitialSearchRun, triggerSearch, dispatch]);

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  return (
    <Page
      label="Get Eligible Employees"
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>

        <Grid width="100%">
          <EligibleEmployeesGrid
            initialSearchLoaded={initialSearchLoaded}
            setInitialSearchLoaded={setInitialSearchLoaded}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default EligibleEmployees;
