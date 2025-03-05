import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import DuplicateSSNsOnDemographicsSearchFilter from "./DuplicateSSNsOnDemographicsSearchFilter";
import DuplicateSSNsOnDemographicsGrid from "./DuplicateSSNsOnDemographicsGrid";

const DuplicateSSNsOnDemographics = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);

  return (
    <Page label="Duplicate SSNs on Demographics">
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <DuplicateSSNsOnDemographicsSearchFilter setInitialSearchLoaded={setInitialSearchLoaded} />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DuplicateSSNsOnDemographicsGrid
            initialSearchLoaded={initialSearchLoaded}
            setInitialSearchLoaded={setInitialSearchLoaded}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default DuplicateSSNsOnDemographics;
