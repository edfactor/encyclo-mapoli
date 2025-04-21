import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import ForfeituresAdjustmentGrid from "./ForfeituresAdjustmentGrid";
import ForfeituresAdjustmentSearchParameters from "./ForfeituresAdjustmentSearchParameters";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";

const ForfeituresAdjustment = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);

  const renderActionNode = () => {
    return (
      <StatusDropdownActionNode />
    );
  };

  return (
    <Page label={CAPTIONS.FORFEITURES_ADJUSTMENT} actionNode={renderActionNode()}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <ForfeituresAdjustmentSearchParameters setInitialSearchLoaded={setInitialSearchLoaded} />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <ForfeituresAdjustmentGrid
            initialSearchLoaded={initialSearchLoaded}
            setInitialSearchLoaded={setInitialSearchLoaded}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default ForfeituresAdjustment;
