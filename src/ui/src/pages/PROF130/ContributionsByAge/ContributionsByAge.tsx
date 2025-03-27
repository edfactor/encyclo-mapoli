import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import ContributionsByAgeSearchFilter from "./ContributionsByAgeSearchFilter";
import ContributionsByAgeGrid from "./ContributionsByAgeGrid";
import { useState } from "react";

const ContributionsByAge = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  return (
    <Page label="Contributions By Age">
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <ContributionsByAgeSearchFilter setInitialSearchLoaded={setInitialSearchLoaded} />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <ContributionsByAgeGrid initialSearchLoaded={initialSearchLoaded} />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default ContributionsByAge;
