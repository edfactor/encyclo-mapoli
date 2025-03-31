import { useState, useEffect } from "react";
import { Button, Divider, Typography, Box, CircularProgress } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { DSMAccordion, Page } from "smart-ui-library";
import ProfitShareTotals426SearchFilter from "./ProfitShareTotals426SearchFilter";
import StatusDropdown, { ProcessStatus } from "components/StatusDropdown";
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
  const navigate = useNavigate();
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const profitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();
  const [triggerSearch, { isLoading }] = useLazyGetYearEndProfitSharingReportQuery();
  const { yearEndProfitSharingReport } = useSelector((state: RootState) => state.yearsEnd);

  useEffect(() => {
    if (hasToken && profitYear) {
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
        pagination: { skip: 0, take: 5 }
      };
      triggerSearch(request, false);
      dispatch(setYearEndProfitSharingReportQueryParams(profitYear));
    }
  }, [hasToken, profitYear, triggerSearch, dispatch]);

  const handleStatusChange = async (newStatus: ProcessStatus) => {
    console.info("Logging new status: ", newStatus);
  };

  const renderActionNode = () => {
    return (
      <div className="flex items-center gap-2 h-10">
        <StatusDropdown onStatusChange={handleStatusChange} />
        <Button
          onClick={() => navigate("/profit-share-report")}
          variant="outlined"
          className="h-10 whitespace-nowrap min-w-fit">
          {MENU_LABELS.GO_TO_PROFIT_SHARE_REPORT}
        </Button>
      </div>
    );
  };

  return (
    <Page
      label={CAPTIONS.PROFIT_SHARE_TOTALS}
      actionNode={renderActionNode()}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <ProfitShareTotals426SearchFilter setInitialSearchLoaded={setInitialSearchLoaded} />
          </DSMAccordion>
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

            {isLoading ? (
              <Box display="flex" justifyContent="center" alignItems="center" minHeight="200px">
                <CircularProgress />
              </Box>
            ) : (
              <Box sx={{ px: 3, mt: 2 }}>
                <ProfitShareTotalsDisplay data={yearEndProfitSharingReport} />
              </Box>
            )}
          </Box>
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default ProfitShareTotals426; 