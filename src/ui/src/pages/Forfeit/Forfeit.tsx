import { Divider, Grid } from "@mui/material";
import { useEffect, useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import StatusDropdownActionNode from "../../components/StatusDropdownActionNode";
import { CAPTIONS } from "../../constants";
import ForfeitGrid from "./ForfeitGrid";
import ForfeitSearchFilter from "./ForfeitSearchFilter";

const Forfeit = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [pageNumberReset, setPageNumberReset] = useState(false);
  const [shouldArchive, setShouldArchive] = useState(false);
  const [searchClickedTrigger, setSearchClickedTrigger] = useState(0);

  const handleStatusChange = (_newStatus: string, statusName?: string) => {
    console.log("=== STATUS CHANGE ===");
    console.log("newStatus:", _newStatus);
    console.log("statusName:", statusName);

    // When status is set to "Complete", trigger archiving
    if (statusName === "Complete") {
      console.log("=== SETTING SHOULD ARCHIVE TO TRUE ===");
      setShouldArchive(true);
    }
  };

  const handleSearchClicked = () => {
    console.log("=== SEARCH CLICKED ===");
    // Increment trigger to notify StatusDropdownActionNode
    setSearchClickedTrigger((prev) => prev + 1);
  };

  // Reset shouldArchive after the archive request is triggered
  useEffect(() => {
    console.log("=== FORFEIT USEEFFECT - shouldArchive:", shouldArchive);
    if (shouldArchive) {
      // The ForfeitGrid will handle the archive request, then we reset the flag
      const timer = setTimeout(() => {
        console.log("=== RESETTING SHOULD ARCHIVE TO FALSE ===");
        setShouldArchive(false);
      }, 100);
      return () => clearTimeout(timer);
    }
  }, [shouldArchive]);

  const renderActionNode = () => {
    return (
      <StatusDropdownActionNode
        onStatusChange={handleStatusChange}
        onSearchClicked={searchClickedTrigger > 0 ? () => {} : undefined}
      />
    );
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
              onSearchClicked={handleSearchClicked}
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
