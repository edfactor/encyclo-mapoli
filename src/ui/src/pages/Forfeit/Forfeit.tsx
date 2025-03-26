import { Button, Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import ForfeitSearchFilter from "./ForfeitSearchFilter";
import ForfeitGrid from "./ForfeitGrid";
import { useState } from "react";

const Forfeit = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);

  return (
    <Page
      label={CAPTIONS.FORFEIT}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <ForfeitSearchFilter setInitialSearchLoaded={setInitialSearchLoaded} />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <ForfeitGrid 
            initialSearchLoaded={initialSearchLoaded}
            setInitialSearchLoaded={setInitialSearchLoaded}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default Forfeit;
