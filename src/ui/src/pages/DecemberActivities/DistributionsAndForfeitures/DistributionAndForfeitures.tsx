import { Button, Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
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
    return (
      <StatusDropdownActionNode />
    );
  };

  return (
    <Page 
      label="Distributions And Forfeitures (QPAY129)"
      actionNode={renderActionNode()}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <DistributionsAndForfeituresSearchFilter setInitialSearchLoaded={setInitialSearchLoaded} />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DistributionsAndForfeituresGrid
            setInitialSearchLoaded={setInitialSearchLoaded}
            initialSearchLoaded={initialSearchLoaded}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default DistributionsAndForfeitures;
