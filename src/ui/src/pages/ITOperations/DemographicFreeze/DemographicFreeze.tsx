import { Divider, Grid } from "@mui/material";
import { useState } from "react";
import { Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { useInitialLoad } from "../../../hooks/useInitialLoad";
import DemographicFreezeGrid from "./DemographicFreezeGrid";
import DemographicFreezeManager from "./DemographicFreezeManager";

const DemographicFreeze = () => {
  const { isLoaded: initialSearchLoaded, setLoaded: setInitialSearchLoaded } = useInitialLoad();
  const [pageNumberReset, setPageNumberReset] = useState(false);

  return (
    <Page label={`${CAPTIONS.DEMOGRAPHIC_FREEZE}`}>
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
