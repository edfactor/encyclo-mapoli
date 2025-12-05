import { Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { setYearEndProfitSharingReportQueryParams } from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { useLazyGetYearEndProfitSharingReportTotalsQuery } from "../../../reduxstore/api/YearsEndApi.ts";
import ProfitSummary from "../../FiscalClose/PAY426Reports/ProfitSummary/ProfitSummary";

const ProfitShareReport = () => {
  const { yearEndProfitSharingReportTotals } = useSelector((state: RootState) => state.yearsEnd);
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const dispatch = useDispatch();
  const [triggerSearch] = useLazyGetYearEndProfitSharingReportTotalsQuery();
  const profitYear = useDecemberFlowProfitYear();

  // Load both tables when page loads - this is consistent with other pages which only display data and do not take input.
  useEffect(() => {
    if (hasToken && profitYear) {
      const totalsRequest = {
        profitYear: profitYear,
        useFrozenData: false,
        badgeNumber: null
      };

      triggerSearch(totalsRequest, false)
        .then((result) => {
          if (result.data) {
            dispatch(setYearEndProfitSharingReportQueryParams(profitYear));
          }
        })
        .catch((_error) => {
          // Error handled silently
        });
    }
  }, [hasToken, profitYear, triggerSearch, dispatch]);

  const handleStatusChange = (_newStatus: string, statusName?: string) => {
    // Check if the status is "Complete" and trigger search with archive=true
    if (statusName === "Complete" && profitYear) {
      const totalsRequest = {
        profitYear: profitYear,
        useFrozenData: false,
        badgeNumber: null,
        archive: true
      };

      triggerSearch(totalsRequest, false)
        .then((result) => {
          if (result.data) {
            dispatch(setYearEndProfitSharingReportQueryParams(profitYear));
          }
        })
        .catch((error) => {
          console.error("Archive search failed:", error);
        });
    }
  };

  const renderActionNode = () => {
    if (!yearEndProfitSharingReportTotals) return null;

    return (
      <div className="flex h-10 items-center gap-2">
        <StatusDropdownActionNode onStatusChange={handleStatusChange} />
      </div>
    );
  };

  return (
    <Page
      label={CAPTIONS.PROFIT_SHARE_REPORT}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        {/*}
        <Grid width="100%">

          <Box sx={{ mb: 3 }}>
            <div style={{ padding: "0 24px 0 24px" }}>
              <Typography
                variant="h2"
                sx={{ color: "#0258A5" }}>
                {`Totals`}
              </Typography>
            </div>

            {isLoadingTotals ? (
              <Box sx={{ display: "flex", justifyContent: "center", p: 3 }}>
                <CircularProgress />
              </Box>
            ) : (
              <Box sx={{ px: 3, mt: 2 }}>
                <ProfitShareTotalsDisplay totalsData={yearEndProfitSharingReportTotals} />
              </Box>
            )}
          </Box>

        </Grid>
        */}
        <Grid width="100%">
          <ProfitSummary frozenData={false} profitYear={profitYear} />
        </Grid>
      </Grid>
    </Page>
  );
};

export default ProfitShareReport;
