import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import TerminationSearchFilter from "./TerminationSearchFilter";
import TerminationGrid from "./TerminationGrid";

const Termination = () => {
  return (
    <Page label="PROFTERM">
        <Grid2
          container
          rowSpacing="24px">
          <Grid2 width={"100%"}>
            <Divider />
          </Grid2>
          <Grid2
            width={"100%"}>
              <DSMAccordion title="Filter">
                <TerminationSearchFilter />
              </DSMAccordion>
             
          </Grid2>

          <Grid2 width="100%">
            <TerminationGrid />
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default Termination;
