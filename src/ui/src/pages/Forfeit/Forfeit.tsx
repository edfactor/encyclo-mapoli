import { Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import ForfeitGrid from "./ForfeitGrid";
import ForfeitSearchFilter from "./ForfeitSearchFilter";

const Forfeit = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [pageNumberReset, setPageNumberReset] = useState(false);
  const [shouldArchive, setShouldArchive] = useState(false);

  const handleStatusChange = (_newStatus: string, statusName?: string) => {
    // When status is set to "Complete" (statusId = 2), enable archiving
    setShouldArchive(statusName === "Complete");
  };

  const renderActionNode = () => {
    return <StatusDropdownActionNode onStatusChange={handleStatusChange} />;
  };

  return (
    <Page
      label={CAPTIONS.FORFEIT}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <DSMAccordion title="Filter">
            <ForfeitSearchFilter
              setInitialSearchLoaded={setInitialSearchLoaded}
              setPageReset={setPageNumberReset}
            />
          </DSMAccordion>
        </Grid>

        <Grid width="100%">
          <ForfeitGrid
            initialSearchLoaded={initialSearchLoaded}
            setInitialSearchLoaded={setInitialSearchLoaded}
            pageNumberReset={pageNumberReset}
            setPageNumberReset={setPageNumberReset}
            shouldArchive={shouldArchive}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default Forfeit;
