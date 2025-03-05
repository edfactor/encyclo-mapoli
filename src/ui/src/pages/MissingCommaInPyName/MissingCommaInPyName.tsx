import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import MissingCommaInPyNameSearchFilter from "./MissingCommaInPyNameSearchFilter";
import MissingCommaInPyNameGrid from "./MissingCommaInPyNameGrid";
import { useState } from "react";

const MissingCommaInPyName = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  return (
    <Page label="Missing Comma in Full Name">
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <MissingCommaInPyNameSearchFilter setInitialSearchLoaded={setInitialSearchLoaded} />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <MissingCommaInPyNameGrid
            initialSearchLoaded={initialSearchLoaded}
            setInitialSearchLoaded={setInitialSearchLoaded}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default MissingCommaInPyName;
