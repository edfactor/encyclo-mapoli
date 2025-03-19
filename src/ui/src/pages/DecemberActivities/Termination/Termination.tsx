import StatusDropdown, { ProcessStatus } from "components/StatusDropdown";
import { useState } from "react";
import { useNavigate } from "react-router";
import { DSMAccordion, Page } from "smart-ui-library";

import { Button, Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";

import { CAPTIONS, MENU_LABELS } from "../../../constants";
import TerminationGrid from "./TerminationGrid";
import TerminationSearchFilter from "./TerminationSearchFilter";

const Termination = () => {
  const navigate = useNavigate();

  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);

  const handleStatusChange = async (newStatus: ProcessStatus) => {
    console.info("Logging new status: ", newStatus);
  };

  const renderActionNode = () => {
    return (
      <div className="flex items-center gap-2 h-10">
        <StatusDropdown onStatusChange={handleStatusChange} />
        <Button
          onClick={() => navigate("/december-process-accordion")}
          variant="outlined"
          className="h-10 whitespace-nowrap min-w-fit">
          {MENU_LABELS.DECEMBER_ACTIVITIES}
        </Button>
      </div>
    );
  };

  return (
    <Page
      label={CAPTIONS.TERMINATIONS}
      actionNode={renderActionNode()}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <TerminationSearchFilter setInitialSearchLoaded={setInitialSearchLoaded} />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <TerminationGrid
            setInitialSearchLoaded={setInitialSearchLoaded}
            initialSearchLoaded={initialSearchLoaded}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default Termination;
