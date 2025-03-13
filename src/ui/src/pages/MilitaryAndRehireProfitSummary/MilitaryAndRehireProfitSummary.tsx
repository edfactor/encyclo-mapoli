import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import MilitaryAndRehireProfitSummarySearchFilter from "./MilitaryAndRehireProfitSummarySearchFilter";
import MilitaryAndRehireProfitSummaryGrid from "./MilitaryAndRehireProfitSummaryGrid";

const MilitaryAndRehireProfitSummary = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  return (
    <Page label="Military and Rehire Profit Summary">
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <MilitaryAndRehireProfitSummarySearchFilter setInitialSearchLoaded={setInitialSearchLoaded} />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <MilitaryAndRehireProfitSummaryGrid
            initialSearchLoaded={initialSearchLoaded}
            setInitialSearchLoaded={setInitialSearchLoaded}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default MilitaryAndRehireProfitSummary;
