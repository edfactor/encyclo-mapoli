import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { CAPTIONS } from "../../../constants";
import { DSMAccordion, Page } from "smart-ui-library";
import ProfitShareByStoreParameters from "./ProfitShareByStoreParameters";
import ProfitShareByStoreResults from "./ProfitShareByStoreResults";

const ProfitShareByStore = () => {
  return (
    <Page label={CAPTIONS.PROFIT_SHARE_BY_STORE}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <ProfitShareByStoreParameters />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <ProfitShareByStoreResults />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default ProfitShareByStore;
