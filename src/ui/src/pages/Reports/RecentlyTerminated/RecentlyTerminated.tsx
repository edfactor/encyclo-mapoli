import { Divider, Grid } from "@mui/material";
import MissiveAlerts from "components/MissiveAlerts/MissiveAlerts";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { DSMAccordion, Page } from "smart-ui-library";
import { MissiveAlertProvider } from "../../../components/MissiveAlerts/MissiveAlertContext";
import PageErrorBoundary from "../../../components/PageErrorBoundary/PageErrorBoundary";
import { CAPTIONS } from "../../../constants";
import useRecentlyTerminated from "./hooks/useRecentlyTerminated";
import RecentlyTerminatedGrid from "./RecentlyTerminatedGrid";
import RecentlyTerminatedSearchFilter from "./RecentlyTerminatedSearchFilter";

const RecentlyTerminatedContent = () => {
  const { reportData, isLoadingReport, searchCompleted, executeSearch, resetSearch, gridPagination } =
    useRecentlyTerminated();

  return (
    <Grid
      container
      rowSpacing="24px">
      <Grid width="100%">
        <Divider />
      </Grid>

      <MissiveAlerts />

      <Grid width="100%">
        <DSMAccordion title="Filter">
          <RecentlyTerminatedSearchFilter
            onSearch={executeSearch}
            onReset={resetSearch}
          />
        </DSMAccordion>
      </Grid>

      <Grid width="100%">
        {searchCompleted && (
          <RecentlyTerminatedGrid
            reportData={reportData}
            isLoading={isLoadingReport}
            gridPagination={gridPagination}
          />
        )}
      </Grid>
    </Grid>
  );
};

const RecentlyTerminated = () => {
  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  return (
    <PageErrorBoundary pageName="Recently Terminated">
      <Page
        label={CAPTIONS.RECENTLY_TERMINATED}
        actionNode={renderActionNode()}>
        <MissiveAlertProvider>
          <RecentlyTerminatedContent />
        </MissiveAlertProvider>
      </Page>
    </PageErrorBoundary>
  );
};

export default RecentlyTerminated;
