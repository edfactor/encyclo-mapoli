import { Box, Button, Divider, Typography } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import DSMCollapsedAccordion from "components/DSMCollapsedAccordion";
import ProfitShareTotalsDisplay from "components/ProfitShareTotalsDisplay";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate } from "react-router";
import { useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";
import { setYearEndProfitSharingReportQueryParams } from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { YearEndProfitSharingReportRequest } from "reduxstore/types";
import { Page } from "smart-ui-library";
import { CAPTIONS, MENU_LABELS, ROUTES } from "../../constants";
import ProfitShareReportGrid from "./ProfitShareReportGrid";

const ProfitShareReport = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [hasInitialSearchRun, setHasInitialSearchRun] = useState(false);
  const navigate = useNavigate();
  const { yearEndProfitSharingReport } = useSelector((state: RootState) => state.yearsEnd);
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const profitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();
  const [triggerSearch, { isLoading }] = useLazyGetYearEndProfitSharingReportQuery();

  useEffect(() => {
    if (hasToken && profitYear && !initialSearchLoaded && !hasInitialSearchRun) {
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
        pagination: { skip: 0, take: 10, sortBy: "badgeNumber", isSortDescending: true }
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
  }, [hasToken, profitYear, initialSearchLoaded, hasInitialSearchRun, triggerSearch, dispatch]);

  const renderActionNode = () => {
    return (
      <div className="flex items-center gap-2 h-10">
        <StatusDropdownActionNode />
        <Button
          onClick={() => navigate(`/${ROUTES.PROFIT_SHARE_REPORT_EDIT_RUN}`)}
          variant="outlined"
          className="h-10 whitespace-nowrap min-w-fit">
          {MENU_LABELS.GO_TO_PROFIT_SHARE_EDIT_RUN}
        </Button>
      </div>
    );
  };

  return (
    <Page
      label={CAPTIONS.PROFIT_SHARE_REPORT}
      actionNode={renderActionNode()}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width="100%">
          <Box sx={{ mb: 3 }}>
            <div style={{ padding: "0 24px 0 24px" }}>
              <Typography
                variant="h2"
                sx={{ color: "#0258A5" }}>
                {`${CAPTIONS.PROFIT_SHARE_TOTALS}`}
              </Typography>
            </div>

            {yearEndProfitSharingReport && (
              <Box sx={{ px: 3, mt: 2 }}>
                <ProfitShareTotalsDisplay data={yearEndProfitSharingReport} />
              </Box>
            )}
          </Box>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            isCollapsedOnRender={true}
            expandable={true}
            title={`${CAPTIONS.PROFIT_SHARE_REPORT} (${yearEndProfitSharingReport?.response.total || 0} records)`}>
            <ProfitShareReportGrid
              initialSearchLoaded={initialSearchLoaded}
              setInitialSearchLoaded={setInitialSearchLoaded}
            />
          </DSMCollapsedAccordion>
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default ProfitShareReport;
