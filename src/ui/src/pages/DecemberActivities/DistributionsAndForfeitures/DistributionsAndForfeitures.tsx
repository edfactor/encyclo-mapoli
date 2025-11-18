import { Divider, Grid } from "@mui/material";
import { useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import StatusDropdownActionNode from "../../../components/StatusDropdownActionNode";
import { CAPTIONS } from "../../../constants";
import DistributionsAndForfeituresGrid from "./DistributionsAndForfeituresGrid";
import DistributionsAndForfeituresSearchFilter from "./DistributionsAndForfeituresSearchFilter";
import { useDistributionsAndForfeituresState } from "./useDistributionsAndForfeituresState";

const DistributionsAndForfeitures = () => {
  const { state, actions } = useDistributionsAndForfeituresState();
  const [isFetching, setIsFetching] = useState(false);

  const renderActionNode = () => {
    return <StatusDropdownActionNode onStatusChange={actions.handleStatusChange} />;
  };

  return (
    <Page
      label={`${CAPTIONS.DISTRIBUTIONS_AND_FORFEITURES}`}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <DSMAccordion title="Filter">
            <DistributionsAndForfeituresSearchFilter
              setInitialSearchLoaded={actions.setInitialSearchLoaded}
              isFetching={isFetching}
            />
          </DSMAccordion>
        </Grid>

        <Grid width="100%">
          <DistributionsAndForfeituresGrid
            setInitialSearchLoaded={actions.setInitialSearchLoaded}
            initialSearchLoaded={state.initialSearchLoaded}
            shouldArchive={state.shouldArchive}
            onArchiveHandled={actions.handleArchiveHandled}
            onLoadingChange={setIsFetching}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default DistributionsAndForfeitures;
