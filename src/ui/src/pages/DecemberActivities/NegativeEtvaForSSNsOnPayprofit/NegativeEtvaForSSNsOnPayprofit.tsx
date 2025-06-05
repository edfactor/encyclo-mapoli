import { Divider } from "@mui/material";
import { useState } from "react";
import Grid2 from '@mui/material/Grid2';
import NegativeEtvaForSSNsOnPayprofitGrid from "./NegativeEtvaForSSNsOnPayprofitGrid";
import { Page } from "smart-ui-library";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";

const NegativeEtvaForSSNsOnPayprofit = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(true);
  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };
  return (
    <Page label="Negative ETVA for SSNs on Payprofit" actionNode={renderActionNode()}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>

        <Grid2 width="100%">
          <NegativeEtvaForSSNsOnPayprofitGrid
            initialSearchLoaded={initialSearchLoaded}
            setInitialSearchLoaded={setInitialSearchLoaded}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default NegativeEtvaForSSNsOnPayprofit;
