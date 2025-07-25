import { Divider } from "@mui/material";
import { Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import EighteenToTwentyGrid from "./EighteenToTwentyGrid";
import { useState } from "react";

const renderActionNode = () => {
  return <StatusDropdownActionNode />;
};

const EighteenToTwenty = () => {
  const [pageNumberReset, setPageNumberReset] = useState(false);

  return (
    <Page
      label={CAPTIONS.PAY426_ACTIVE_18_20}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width="100%">
          <EighteenToTwentyGrid
            pageNumberReset={pageNumberReset}
            setPageNumberReset={setPageNumberReset}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default EighteenToTwenty;
