import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import DistributionByAgeSearchFilter from "./DistributionByAgeSearchFilter";
import DistributionByAgeGrid from "./DistributionByAgeGrid";

const DistributionByAge = () => {
  return (
    <Page label="Distributions By Age">
        <Grid2
          container
          rowSpacing="24px">
          <Grid2 width={"100%"}>
            <Divider />
          </Grid2>
          <Grid2
            width={"100%"}>
             <DSMAccordion title="Filter">
              <DistributionByAgeSearchFilter />
             </DSMAccordion>
          </Grid2>

          <Grid2 width="100%">
            <DistributionByAgeGrid />
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default DistributionByAge;
