import { Divider, Grid } from "@mui/material";
import { useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import StatusDropdownActionNode from "../../../components/StatusDropdownActionNode";
import DistributionsAndForfeituresGrid from "./DistributionAndForfeituresGrid";
import DistributionsAndForfeituresSearchFilter from "./DistributionAndForfeituresSearchFilter";

const DistributionsAndForfeitures = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  return (
    <Page
      label="Distributions And Forfeitures (QPAY129)"
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <DSMAccordion title="Filter">
            <DistributionsAndForfeituresSearchFilter setInitialSearchLoaded={setInitialSearchLoaded} />
          </DSMAccordion>
        </Grid>

        <Grid width="100%">
          <DistributionsAndForfeituresGrid
            setInitialSearchLoaded={setInitialSearchLoaded}
            initialSearchLoaded={initialSearchLoaded}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default DistributionsAndForfeitures;
