import { useState, useEffect } from "react";
import { Button, Divider, Typography, Box, CircularProgress } from "@mui/material";
import { Grid } from "@mui/material";
import { Page } from "smart-ui-library";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useNavigate } from "react-router";
import { MENU_LABELS, CAPTIONS } from "../../constants";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";
import { YearEndProfitSharingReportRequest } from "reduxstore/types";
import { useDispatch } from "react-redux";
import { setYearEndProfitSharingReportQueryParams } from "reduxstore/slices/yearsEndSlice";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import ProfitShareTotalsDisplay from "components/ProfitShareTotalsDisplay";

const ProfitShareTotals426 = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [hasInitialSearchRun, setHasInitialSearchRun] = useState(false);
  const navigate = useNavigate();
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const profitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();
  const [triggerSearch, { isLoading }] = useLazyGetYearEndProfitSharingReportQuery();
  const { yearEndProfitSharingReport } = useSelector((state: RootState) => state.yearsEnd);

  useEffect(() => {
    if (hasToken && profitYear && !hasInitialSearchRun) {
      setHasInitialSearchRun(true);

      const request: YearEndProfitSharingReportRequest = {
        isYearEnd: false,
        minimumAgeInclusive: 18,
        maximumAgeInclusive: 98,
        minimumHoursInclusive: 1000,
        maximumHoursInclusive: 2000,
        includeActiveEmployees: true,
        includeInactiveEmployees: true,
        includeEmployeesTerminatedThisYear: false,
        includeTerminatedEmployees: true,
        includeBeneficiaries: false,
        includeEmployeesWithPriorProfitSharingAmounts: true,
        includeEmployeesWithNoPriorProfitSharingAmounts: true,
        profitYear: profitYear,
        pagination: { skip: 0, take: 5, sortBy: "badgeNumber", isSortDescending: true }
      };

      triggerSearch(request, false)
        .then((result) => {
          if (result.data) {
            dispatch(setYearEndProfitSharingReportQueryParams(profitYear));
            setInitialSearchLoaded(true);
          }
        })
        .catch((error) => {
          console.error("Initial search failed:", error);
        });
    }
  }, [hasToken, profitYear, hasInitialSearchRun, triggerSearch, dispatch]);

  const renderActionNode = () => {
    return (
      <div className="flex h-10 items-center gap-2">
        <StatusDropdownActionNode />
        <Button
          onClick={() => navigate("/profit-share-report")}
          variant="outlined"
          className="h-10 min-w-fit whitespace-nowrap">
          {MENU_LABELS.GO_TO_PROFIT_SHARE_REPORT}
        </Button>
      </div>
    );
  };

  return (
    <Page
      label={CAPTIONS.PROFIT_SHARE_TOTALS}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>

        <Grid width="100%">
          <Box sx={{ mb: 3 }}>
            <div style={{ padding: "0 24px 0 24px" }}>
              <Typography
                variant="h2"
                sx={{ color: "#0258A5" }}>
                {`${CAPTIONS.PROFIT_SHARE_TOTALS}`}
              </Typography>
            </div>

            {isLoading ? (
              <Box
                display="flex"
                justifyContent="center"
                alignItems="center"
                minHeight="200px">
                <CircularProgress />
              </Box>
            ) : (
              <Box sx={{ px: 3, mt: 2 }}>
                <ProfitShareTotalsDisplay data={yearEndProfitSharingReport} />
              </Box>
            )}
          </Box>
        </Grid>
      </Grid>
    </Page>
  );
};

export default ProfitShareTotals426;
