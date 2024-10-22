import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import MilitaryAndRehireForfeituresSearchFilter from "./MilitaryAndRehireForfeituresSearchFilter";
import MilitaryAndRehireForfeituresGrid from "./MilitaryAndRehireForfeituresGrid";

const MilitaryAndRehireForfeitures = () => {
  return (
    <Page label="Military and Rehire Forfeitures">
        <Grid2
          container
          rowSpacing="24px">
          <Grid2 width={"100%"}>
            <Divider />
          </Grid2>
          <Grid2
            width={"100%"}>
              <DSMAccordion title="Filter">
                <MilitaryAndRehireForfeituresSearchFilter />
              </DSMAccordion>
             
          </Grid2>

          <Grid2 width="100%">
            <MilitaryAndRehireForfeituresGrid />
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default MilitaryAndRehireForfeitures;
