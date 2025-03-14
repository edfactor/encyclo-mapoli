import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import MilitaryAndRehireForfeituresSearchFilter from "./MilitaryAndRehireForfeituresSearchFilter";
import MilitaryAndRehireForfeituresGrid from "./MilitaryAndRehireForfeituresGrid";
import { useState } from "react";
import { CAPTIONS } from "../../constants";

const MilitaryAndRehireForfeitures = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);

  return (
    <Page label={`${CAPTIONS.MILITARY_FORFEITURES}`}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <MilitaryAndRehireForfeituresSearchFilter setInitialSearchLoaded={setInitialSearchLoaded} />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <MilitaryAndRehireForfeituresGrid
            initialSearchLoaded={initialSearchLoaded}
            setInitialSearchLoaded={setInitialSearchLoaded}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default MilitaryAndRehireForfeitures;
