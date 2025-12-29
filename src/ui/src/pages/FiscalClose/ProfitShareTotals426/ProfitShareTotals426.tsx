import PageErrorBoundary from "@/components/PageErrorBoundary";
import ProfitShareTotalsDisplay from "@/components/ProfitShareTotalsDisplay";
import { Box, Button, CircularProgress, Divider, Grid, Typography } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate } from "react-router";
import { useLazyGetYearEndProfitSharingReportLiveQuery } from "reduxstore/api/YearsEndApi";
import { setYearEndProfitSharingReportQueryParams } from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { Page } from "smart-ui-library";
import { CAPTIONS, MENU_LABELS } from "../../../constants";
import { useInitialLoad } from "../../../hooks/useInitialLoad";

const ProfitShareTotals426 = () => {
  const { setLoaded: setInitialSearchLoaded } = useInitialLoad();
  const [hasInitialSearchRun, setHasInitialSearchRun] = useState(false);
  const navigate = useNavigate();
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const profitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();
  const [triggerSearch, { isLoading }] = useLazyGetYearEndProfitSharingReportLiveQuery();
  const { yearEndProfitSharingReportTotalsFrozen: yearEndProfitSharingReportTotals } = useSelector(
    (state: RootState) => state.yearsEnd
  );

  useEffect(() => {
    if (hasToken && profitYear && !hasInitialSearchRun) {
      setHasInitialSearchRun(true);

      const request = {
        reportId: 0,
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
  }, [hasToken, profitYear, hasInitialSearchRun, triggerSearch, dispatch, setInitialSearchLoaded]);

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
    <PageErrorBoundary pageName="Profit Share Totals">
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
                  <ProfitShareTotalsDisplay totalsData={yearEndProfitSharingReportTotals} />
                </Box>
              )}
            </Box>
          </Grid>
        </Grid>
      </Page>
    </PageErrorBoundary>
  );
};

export default ProfitShareTotals426;
