import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import ProfitShareReportSearchFilter from "./ProfitShareReportSearchFilter";
import ProfitShareReportGrid from "./ProfitShareReportGrid";

const ProfitShareReport = () => {
  return (
    <Page label="Profit Share Report">
        <Grid2
          container
          rowSpacing="24px">
          <Grid2 width={"100%"}>
            <Divider />
          </Grid2>
          <Grid2
            width={"100%"}>
              <DSMAccordion title="Filter">
                <ProfitShareReportSearchFilter />
              </DSMAccordion>
             
          </Grid2>

          <Grid2 width="100%">
            <ProfitShareReportGrid />
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default ProfitShareReport;
