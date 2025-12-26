import PageErrorBoundary from "@/components/PageErrorBoundary";
import { Divider } from "@mui/material";
import { Grid } from "@mui/material";
import { CAPTIONS } from "../../../constants";
import { DSMAccordion, Page } from "smart-ui-library";
import ProfitShareByStoreParameters from "./ProfitShareByStoreParameters";
import ProfitShareByStoreResults from "./ProfitShareByStoreResults";

const ProfitShareByStore = () => {
  return (
    <PageErrorBoundary pageName="Profit Share By Store">
      <Page label={CAPTIONS.PROFIT_SHARE_BY_STORE}>
        <Grid
          container
          rowSpacing="24px">
          <Grid width={"100%"}>
            <Divider />
          </Grid>
          <Grid width={"100%"}>
            <DSMAccordion title="Filter">
              <ProfitShareByStoreParameters />
            </DSMAccordion>
          </Grid>

          <Grid width="100%">
            <ProfitShareByStoreResults />
          </Grid>
        </Grid>
      </Page>
    </PageErrorBoundary>
  );
};

export default ProfitShareByStore;
