import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import ManageExecutiveHoursAndDollarsSearchFilter from "./ManageExecutiveHoursAndDollarsSearchFilter";
import ManageExecutiveHoursAndDollarsGrid from "./ManageExecutiveHoursAndDollarsGrid";

const ManageExecutiveHoursAndDollars = () => {
  return (
    <Page label="Manage Edxecutive Hours And Dollars">
        <Grid2
          container
          rowSpacing="24px">
          <Grid2 width={"100%"}>
            <Divider />
          </Grid2>
          <Grid2
            width={"100%"}>
              <DSMAccordion title="Filter">
                <ManageExecutiveHoursAndDollarsSearchFilter />
              </DSMAccordion>
             
          </Grid2>

          <Grid2 width="100%">
            <ManageExecutiveHoursAndDollarsGrid />
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default ManageExecutiveHoursAndDollars;
