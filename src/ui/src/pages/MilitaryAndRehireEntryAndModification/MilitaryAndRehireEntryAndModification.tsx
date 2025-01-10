import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import MilitaryAndRehireSearchFilter from "./MilitaryAndRehireEntryAndModificationSearchFilter";
import MilitaryAndRehireEntryAndModificationGrid from "./MilitaryAndRehireEntryAndModificationGrid";

const MilitaryAndRehireEntryAndModification = () => {
  return (
    <Page label="Military and Rehire - ENTRY AND MOD">
        <Grid2
          container
          rowSpacing="24px">
          <Grid2 width={"100%"}>
            <Divider />
          </Grid2>
          <Grid2
            width={"100%"}>
              <DSMAccordion title="Filter">
                <MilitaryAndRehireSearchFilter />
              </DSMAccordion>
             
          </Grid2>

          <Grid2 width="100%">
            <MilitaryAndRehireEntryAndModificationGrid />
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default MilitaryAndRehireEntryAndModification;
