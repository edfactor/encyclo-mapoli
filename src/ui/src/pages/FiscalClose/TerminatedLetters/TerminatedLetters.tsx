import { Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import TerminatedLettersGrid from "./TerminatedLettersGrid";
import TerminatedLettersSearchFilter from "./TerminatedLettersSearchFilter";

const TerminatedLetters = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  return (
    <Page
      label={CAPTIONS.TERMINATED_LETTERS}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <DSMAccordion title="Filter">
            <TerminatedLettersSearchFilter setInitialSearchLoaded={setInitialSearchLoaded} />
          </DSMAccordion>
        </Grid>

        <Grid width="100%">
          <TerminatedLettersGrid
            setInitialSearchLoaded={setInitialSearchLoaded}
            initialSearchLoaded={initialSearchLoaded}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default TerminatedLetters;