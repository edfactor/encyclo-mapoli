import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import VestedAmountsByAgeSearchFilter from "./VestedAmountsByAgeSearchFilter";
import VestedAmountsByAgeGrid from "./VestedAmountsByAgeGrid";

const VestedAmountsByAge = () => {
  return (
    <Page label="Vested Amounts by Age">
        <Grid2
          container
          rowSpacing="24px">
          <Grid2 width={"100%"}>
            <Divider />
          </Grid2>
          <Grid2
            width={"100%"}>
             <DSMAccordion title="Filter">
              <VestedAmountsByAgeSearchFilter />
             </DSMAccordion>
          </Grid2>

          <Grid2 width="100%">
            <VestedAmountsByAgeGrid />
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default VestedAmountsByAge;
