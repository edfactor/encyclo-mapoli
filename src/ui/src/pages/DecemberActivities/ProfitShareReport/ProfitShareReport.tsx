import { Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { closeDrawer, openDrawer, setFullscreen } from "reduxstore/slices/generalSlice";
import { setYearEndProfitSharingReportQueryParams } from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { useLazyGetYearEndProfitSharingReportTotalsQuery } from "../../../reduxstore/api/YearsEndApi.ts";
import ProfitSummary from "../../FiscalClose/PAY426Reports/ProfitSummary/ProfitSummary";

const ProfitShareReport = () => {
  const { yearEndProfitSharingReportTotals } = useSelector((state: RootState) => state.yearsEnd);
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const isDrawerOpen = useSelector((state: RootState) => state.general.isDrawerOpen);
  const dispatch = useDispatch();
  const [triggerSearch] = useLazyGetYearEndProfitSharingReportTotalsQuery();
  const [isGridExpanded, setIsGridExpanded] = useState(false);
  const [wasDrawerOpenBeforeExpand, setWasDrawerOpenBeforeExpand] = useState(false);
  const [triggerArchive, setTriggerArchive] = useState(false);
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
      // Trigger archive for ProfitSummary component
      setTriggerArchive(true);

      // Also trigger archive for the totals query
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

  const handleArchiveComplete = () => {
    setTriggerArchive(false);
  };

  const handleToggleGridExpand = () => {
    if (!isGridExpanded) {
      // Expanding: remember current drawer state and close it
      setWasDrawerOpenBeforeExpand(isDrawerOpen || false);
      dispatch(closeDrawer());
      dispatch(setFullscreen(true));
      setIsGridExpanded(true);
    } else {
      // Collapsing: restore previous state
      dispatch(setFullscreen(false));
      setIsGridExpanded(false);
      if (wasDrawerOpenBeforeExpand) {
        dispatch(openDrawer());
      }
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
      label={isGridExpanded ? "" : CAPTIONS.PROFIT_SHARE_REPORT}
      actionNode={isGridExpanded ? undefined : renderActionNode()}>
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
          <ProfitSummary
            frozenData={false}
            externalIsGridExpanded={isGridExpanded}
            externalOnToggleExpand={handleToggleGridExpand}
            triggerArchive={triggerArchive}
            onArchiveComplete={handleArchiveComplete}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default ProfitShareReport;
