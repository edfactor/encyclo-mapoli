import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import MilitaryAndRehireSearchFilter from "./ManageExecutiveHoursAndDollarsSearchFilter";
import MilitaryAndRehireGrid from "./ManageExecutiveHoursAndDollarsGrid";

const ManageExecutiveHoursAndDollars = () => {
  return (
    <Page label="Manage Executive Hours And Dollars">
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

export default ManageExecutiveHoursAndDollars;
