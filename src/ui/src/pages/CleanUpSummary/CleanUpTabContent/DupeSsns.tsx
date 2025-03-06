import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import DuplicateSSNsOnDemographicsGrid from "pages/DuplicateSSNsOnDemographics/DuplicateSSNsOnDemographicsGrid";
import DuplicateSSNsOnDemographicsSearchFilter from "pages/DuplicateSSNsOnDemographics/DuplicateSSNsOnDemographicsSearchFilter";
import { useState } from "react";
import { DSMAccordion } from "smart-ui-library";

export const DupeSsns = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  return (
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
  );
};

export default DupeSsns;
