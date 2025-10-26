import { Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import RecentlyTerminatedGrid from "./RecentlyTerminatedGrid";
import RecentlyTerminatedSearchFilter from "./RecentlyTerminatedSearchFilter";

const RecentlyTerminated = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  return (
    <Page
      label={CAPTIONS.RECENTLY_TERMINATED}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <DSMAccordion title="Filter">
            <RecentlyTerminatedSearchFilter setInitialSearchLoaded={setInitialSearchLoaded} />
          </DSMAccordion>
        </Grid>

        <Grid width="100%">
          <RecentlyTerminatedGrid
            setInitialSearchLoaded={setInitialSearchLoaded}
            initialSearchLoaded={initialSearchLoaded}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default RecentlyTerminated;
