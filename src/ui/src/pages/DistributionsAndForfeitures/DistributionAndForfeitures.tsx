import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import DistributionsAndForfeituresSearchFilter from "./DistributionAndForfeituresSearchFilter";
import DistributionsAndForfeituresGrid from "./DistributionAndForfeituresGrid";

const DistributionsAndForfeitures = () => {
  return (
    <Page label="Distributions And Forfeitures (QPAY129)">
        <Grid2
          container
          rowSpacing="24px">
          <Grid2 width={"100%"}>
            <Divider />
          </Grid2>
          <Grid2
            width={"100%"}>
             <DSMAccordion title="Filter">
              <DistributionsAndForfeituresSearchFilter />
             </DSMAccordion>
          </Grid2>

          <Grid2 width="100%">
            <DistributionsAndForfeituresGrid />
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default DistributionsAndForfeitures;
