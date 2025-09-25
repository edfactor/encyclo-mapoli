import { Divider, Grid } from "@mui/material";
import { useState } from "react";
import { Page } from "smart-ui-library";
import DemographicFreezeGrid from "./DemographicFreezeGrid";
import DemographicFreezeManager from "./DemographicFreezeManager";

const DemographicFreeze = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [pageNumberReset, setPageNumberReset] = useState(false);

  return (
    <Page label="IT Commands - Demographic Freeze">
      <Grid
        container
        rowSpacing={3}>
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <DemographicFreezeManager
            setInitialSearchLoaded={setInitialSearchLoaded}
            setPageReset={setPageNumberReset}
          />
        </Grid>

        <Grid width="100%">
          <DemographicFreezeGrid
            setInitialSearchLoaded={setInitialSearchLoaded}
            initialSearchLoaded={initialSearchLoaded}
            pageNumberReset={pageNumberReset}
            setPageNumberReset={setPageNumberReset}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default DemographicFreeze;
