import { Button, Divider } from "@mui/material";
import { Grid } from "@mui/material";
import { DSMAccordion, Page } from "smart-ui-library";
import DistributionsAndForfeituresSearchFilter from "./DistributionAndForfeituresSearchFilter";
import DistributionsAndForfeituresGrid from "./DistributionAndForfeituresGrid";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useNavigate } from "react-router";
import { MENU_LABELS } from "../../../constants";
import { useState } from "react";

const DistributionsAndForfeitures = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);

  const navigate = useNavigate();

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
