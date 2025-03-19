import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { DSMAccordion, Page } from "smart-ui-library";
import DemographicFreezeManager from "./DemographicFreezeManager";
import DemographicFreezeGrid from "./DemographicFreezeGrid";
import { useState } from "react";

const DemographicFreeze = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  return (
    <Page label="Demographic Freeze">
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <DemographicFreezeManager setInitialSearchLoaded={setInitialSearchLoaded} />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DemographicFreezeGrid
            setInitialSearchLoaded={setInitialSearchLoaded}
            initialSearchLoaded={initialSearchLoaded}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default DemographicFreeze;
