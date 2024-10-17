import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import NegativeEtvaForSSNsOnPayprofitSearchFilter from "./NegativeEtvaForSSNsOnPayprofitSearchFilter";
import NegativeEtvaForSSNsOnPayprofitGrid from "./NegativeEtvaForSSNsOnPayprofitGrid";

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
              <DSMAccordion title="Filter">
                <NegativeEtvaForSSNsOnPayprofitSearchFilter />
              </DSMAccordion>
             
          </Grid2>

          <Grid2 width="100%">
            <NegativeEtvaForSSNsOnPayprofitGrid />
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default NegativeEtvaForSSNsOnPayprofit;
