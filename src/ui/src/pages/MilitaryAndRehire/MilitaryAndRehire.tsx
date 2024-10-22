import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import MilitaryAndRehireSearchFilter from "./MilitaryAndRehireSearchFilter";
import MilitaryAndRehireGrid from "./MilitaryAndRehireGrid";

const MilitaryAndRehire = () => {
  return (
    <Page label="Military and Rehire">
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
            <MilitaryAndRehireGrid />
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default MilitaryAndRehire;
