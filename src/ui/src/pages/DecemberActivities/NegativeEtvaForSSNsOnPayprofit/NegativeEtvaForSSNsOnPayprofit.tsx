import { Divider } from "@mui/material";
import { useState } from "react";
import { Grid } from "@mui/material";
import NegativeEtvaForSSNsOnPayprofitGrid from "./NegativeEtvaForSSNsOnPayprofitGrid";
import { Page } from "smart-ui-library";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";

const NegativeEtvaForSSNsOnPayprofit = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(true);
  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };
  return (
    <Page
      label="Negative ETVA for SSNs on Payprofit"
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>

        <Grid width="100%">
          <NegativeEtvaForSSNsOnPayprofitGrid
            initialSearchLoaded={initialSearchLoaded}
            setInitialSearchLoaded={setInitialSearchLoaded}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default NegativeEtvaForSSNsOnPayprofit;
