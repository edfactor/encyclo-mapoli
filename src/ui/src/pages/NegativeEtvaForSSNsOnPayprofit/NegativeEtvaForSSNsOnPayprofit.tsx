import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { Page } from "smart-ui-library";

const NegativeEtvaForSSNsOnPayprofit = () => {
  return (
    <Page label="Negative ETVA for SSNs on Payprofit">
        <Grid2
          container
          rowSpacing="24px">
          <Grid2 width={"100%"}>
            <Divider />
          </Grid2>
          <Grid2
            width={"100%"}>
             Search Filters
          </Grid2>

          <Grid2 width="100%">
            Grid
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default NegativeEtvaForSSNsOnPayprofit;
