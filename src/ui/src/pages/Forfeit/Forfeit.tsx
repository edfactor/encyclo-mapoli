import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import ForfeitGrid from "./ForfeitGrid";
import ForfeitSearchFilter from "./ForfeitSearchFilter";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";

const Forfeit = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [pageNumberReset, setPageNumberReset] = useState(false);

  const renderActionNode = () => {
    return (
      <StatusDropdownActionNode />
    );
  };

  return (
    <Page label={CAPTIONS.FORFEIT} actionNode={renderActionNode()}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <ForfeitSearchFilter setInitialSearchLoaded={setInitialSearchLoaded} setPageReset={setPageNumberReset} />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <ForfeitGrid
            initialSearchLoaded={initialSearchLoaded}
            setInitialSearchLoaded={setInitialSearchLoaded}
            pageNumberReset={pageNumberReset}
            setPageNumberReset={setPageNumberReset}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default Forfeit;
