import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { useState } from "react";
import { Page } from "smart-ui-library";
import DemographicFreezeGrid from "./DemographicFreezeGrid";
import DemographicFreezeManager from "./DemographicFreezeManager";

const DemographicFreeze = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [pageNumberReset, setPageNumberReset] = useState(false);
  return (
    <Page label="IT Commands - Demographic Freeze">
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DemographicFreezeManager setInitialSearchLoaded={setInitialSearchLoaded} setPageReset={setPageNumberReset} />
        </Grid2>

        <Grid2 width="100%">
          <DemographicFreezeGrid
            setInitialSearchLoaded={setInitialSearchLoaded}
            initialSearchLoaded={initialSearchLoaded}
            pageNumberReset={pageNumberReset}
            setPageNumberReset={setPageNumberReset}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default DemographicFreeze;
