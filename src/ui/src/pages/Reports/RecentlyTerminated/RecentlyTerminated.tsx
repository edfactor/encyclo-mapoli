import { Divider, Grid } from "@mui/material";
import MissiveAlerts from "components/MissiveAlerts/MissiveAlerts";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { DSMAccordion, Page } from "smart-ui-library";
import { MissiveAlertProvider } from "../../../components/MissiveAlerts/MissiveAlertContext";
import { CAPTIONS } from "../../../constants";
import { useMissiveAlerts } from "../../../hooks/useMissiveAlerts";
import useRecentlyTerminated from "./hooks/useRecentlyTerminated";
import RecentlyTerminatedGrid from "./RecentlyTerminatedGrid";
import RecentlyTerminatedSearchFilter from "./RecentlyTerminatedSearchFilter";

const RecentlyTerminatedContent = () => {
  const { reportData, isLoadingReport, searchCompleted, executeSearch, resetSearch, gridPagination } =
    useRecentlyTerminated();
  const { missiveAlerts } = useMissiveAlerts();

  return (
    <Grid container rowSpacing="24px">
      <Grid width="100%">
        <Divider />
      </Grid>

      {missiveAlerts.length > 0 && <MissiveAlerts />}

      <Grid width="100%">
        <DSMAccordion title="Filter">
          <RecentlyTerminatedSearchFilter onSearch={executeSearch} onReset={resetSearch} />
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
    <Page label={CAPTIONS.RECENTLY_TERMINATED} actionNode={renderActionNode()}>
      <MissiveAlertProvider>
        <RecentlyTerminatedContent />
      </MissiveAlertProvider>
    </Page>
  );
};

export default RecentlyTerminated;
