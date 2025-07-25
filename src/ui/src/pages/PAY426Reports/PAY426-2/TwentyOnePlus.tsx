import { Divider } from "@mui/material";
import { Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import TwentyOnePlusGrid from "./TwentyOnePlusGrid";
import { useState } from "react";

const renderActionNode = () => {
  return <StatusDropdownActionNode />;
};

const TwentyOnePlus = () => {
  const [pageNumberReset, setPageNumberReset] = useState(false);

  return (
    <Page
      label={CAPTIONS.PAY426_ACTIVE_21_PLUS}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width="100%">
          <TwentyOnePlusGrid
            pageNumberReset={pageNumberReset}
            setPageNumberReset={setPageNumberReset}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default TwentyOnePlus;
