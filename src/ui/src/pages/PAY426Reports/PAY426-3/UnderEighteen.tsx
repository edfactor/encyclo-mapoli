import { CAPTIONS } from "../../../constants";
import { Divider } from "@mui/material";
import { Grid } from "@mui/material";
import UnderEighteenGrid from "./UnderEighteenGrid";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { Page } from "smart-ui-library";
import { useState } from "react";

const renderActionNode = () => {
  return <StatusDropdownActionNode />;
};

const UnderEighteen = () => {
  const [pageNumberReset, setPageNumberReset] = useState(false);

  return (
    <Page
      label={CAPTIONS.PAY426_ACTIVE_UNDER_18}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width="100%">
          <UnderEighteenGrid
            pageNumberReset={pageNumberReset}
            setPageNumberReset={setPageNumberReset}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default UnderEighteen;
