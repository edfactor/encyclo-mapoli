import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import MilitaryAndRehireProfitSummarySearchFilter from "./MilitaryAndRehireProfitSummarySearchFilter";
import MilitaryAndRehireProfitSummaryGrid from "./MilitaryAndRehireProfitSummaryGrid";

const MilitaryAndRehireProfitSummary = () => {
  return (
    <Page label="Military and Rehire Profit Summary">
        <Grid2
          container
          rowSpacing="24px">
          <Grid2 width={"100%"}>
            <Divider />
          </Grid2>
          <Grid2
            width={"100%"}>
              <DSMAccordion title="Filter">
                <MilitaryAndRehireProfitSummarySearchFilter />
              </DSMAccordion>
             
          </Grid2>

          <Grid2 width="100%">
            <MilitaryAndRehireProfitSummaryGrid />
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default MilitaryAndRehireProfitSummary;
