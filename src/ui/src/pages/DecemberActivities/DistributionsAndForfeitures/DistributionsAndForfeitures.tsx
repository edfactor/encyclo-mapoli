import { Divider, Grid } from "@mui/material";
import { useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import PageErrorBoundary from "../../../components/PageErrorBoundary";
import StatusDropdownActionNode from "../../../components/StatusDropdownActionNode";
import { CAPTIONS } from "../../../constants";
import { useGridExpansion } from "../../../hooks/useGridExpansion";
import DistributionsAndForfeituresGrid from "./DistributionsAndForfeituresGrid";
import DistributionsAndForfeituresSearchFilter from "./DistributionsAndForfeituresSearchFilter";
import { useDistributionsAndForfeituresState } from "./useDistributionsAndForfeituresState";

const DistributionsAndForfeitures = () => {
  const { state, actions } = useDistributionsAndForfeituresState();
  const [isFetching, setIsFetching] = useState(false);
  const { isGridExpanded, handleToggleGridExpand } = useGridExpansion();

  const renderActionNode = () => {
    return <StatusDropdownActionNode onStatusChange={actions.handleStatusChange} />;
  };

  return (
    <PageErrorBoundary pageName="Distributions and Forfeitures">
      <Page
        label={isGridExpanded ? "" : `${CAPTIONS.DISTRIBUTIONS_AND_FORFEITURES}`}
        actionNode={isGridExpanded ? undefined : renderActionNode()}>
        <Grid
          container
          rowSpacing="24px">
          {!isGridExpanded && (
            <Grid width={"100%"}>
              <Divider />
            </Grid>
          )}
          {!isGridExpanded && (
            <Grid width={"100%"}>
              <DSMAccordion title="Filter">
                <DistributionsAndForfeituresSearchFilter
                  setInitialSearchLoaded={actions.setInitialSearchLoaded}
                  isFetching={isFetching}
                />
              </DSMAccordion>
            </Grid>
          )}

          <Grid width="100%">
            <DistributionsAndForfeituresGrid
              setInitialSearchLoaded={actions.setInitialSearchLoaded}
              initialSearchLoaded={state.initialSearchLoaded}
              shouldArchive={state.shouldArchive}
              onArchiveHandled={actions.handleArchiveHandled}
              onLoadingChange={setIsFetching}
              isGridExpanded={isGridExpanded}
              onToggleExpand={handleToggleGridExpand}
            />
          </Grid>
        </Grid>
      </Page>
    </PageErrorBoundary>
  );
};

export default DistributionsAndForfeitures;
