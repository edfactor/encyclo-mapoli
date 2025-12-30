import { Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import PageErrorBoundary from "components/PageErrorBoundary";
import { useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { closeDrawer, openDrawer, setFullscreen } from "reduxstore/slices/generalSlice";
import { RootState } from "reduxstore/store";
import { Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import ProfitSummary from "../../FiscalClose/PAY426Reports/ProfitSummary/ProfitSummary";

const ProfitShareReport = () => {
  // Read totals from Redux - ProfitSummary handles the fetching
  const { yearEndProfitSharingReportTotalsLive: yearEndProfitSharingReportTotals } = useSelector(
    (state: RootState) => state.yearsEnd
  );
  const isDrawerOpen = useSelector((state: RootState) => state.general.isDrawerOpen);
  const dispatch = useDispatch();
  const [isGridExpanded, setIsGridExpanded] = useState(false);
  const [wasDrawerOpenBeforeExpand, setWasDrawerOpenBeforeExpand] = useState(false);
  const [triggerArchive, setTriggerArchive] = useState(false);

  const handleStatusChange = (_newStatus: string, statusName?: string) => {
    // When status changes to "Complete", trigger archive for ProfitSummary component
    if (statusName === "Complete") {
      setTriggerArchive(true);
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
    <PageErrorBoundary pageName="Profit Share Report">
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
              pageTitle=""
            />
          </Grid>
        </Grid>
      </Page>
    </PageErrorBoundary>
  );
};

export default ProfitShareReport;
