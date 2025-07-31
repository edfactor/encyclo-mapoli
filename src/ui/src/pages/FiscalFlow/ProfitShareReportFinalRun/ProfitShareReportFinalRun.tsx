import { Divider } from "@mui/material";
import { Grid } from "@mui/material";
import { DSMAccordion, Page } from "smart-ui-library";
import ProfitShareReportFinalRunParameters from "./ProfitShareReportFinalRunParameters";
import { CAPTIONS } from "../../../constants";
import ProfitShareReportFinalRunResults from "./ProfitShareReportFinalRunResults";

const ProfitShareReportFinalRun = () => {
  return (
    <Page label={CAPTIONS.PROFIT_SHARE_REPORT_FINAL_RUN}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <DSMAccordion title="Filter">
            <ProfitShareReportFinalRunParameters />
          </DSMAccordion>
        </Grid>

        <Grid width="100%">
          <ProfitShareReportFinalRunResults />
        </Grid>
      </Grid>
    </Page>
  );
};

export default ProfitShareReportFinalRun;
