import { Divider } from "@mui/material";
import { Grid } from "@mui/material";
import { CAPTIONS } from "../../constants";
import { DSMAccordion, Page } from "smart-ui-library";
import PaymasterUpdateParameters from "./PaymasterUpdateParameters";
import PaymasterUpdateResults from "./PaymasterUpdateResults";

const PaymasterUpdate = () => {
  return (
    <Page label={CAPTIONS.PAYMASTER_UPDATE}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <DSMAccordion title="Filter">
            <PaymasterUpdateParameters />
          </DSMAccordion>
        </Grid>

        <Grid width="100%">
          <PaymasterUpdateResults />
        </Grid>
      </Grid>
    </Page>
  );
};

export default PaymasterUpdate;
