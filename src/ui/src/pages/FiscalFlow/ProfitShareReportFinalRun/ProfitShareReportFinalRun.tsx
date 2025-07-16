import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import ProfitShareReportFinalRunParameters from "./ProfitShareReportFinalRunParameters";
import { CAPTIONS } from "../../../constants";
import ProfitShareReportFinalRunResults from "./ProfitShareReportFinalRunResults";

const ProfitShareReportFinalRun = () => {
  return (
    <Page label={CAPTIONS.PROFIT_SHARE_REPORT_FINAL_RUN}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <ProfitShareReportFinalRunParameters />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <ProfitShareReportFinalRunResults />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default ProfitShareReportFinalRun;
