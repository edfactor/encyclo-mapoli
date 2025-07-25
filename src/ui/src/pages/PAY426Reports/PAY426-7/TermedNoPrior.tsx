import { CAPTIONS } from "../../../constants";
import { Divider } from "@mui/material";
import { Grid } from "@mui/material";
import TermedNoPriorGrid from "./TermedNoPriorGrid";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { Page } from "smart-ui-library";
import { useState } from "react";

const renderActionNode = () => {
  return <StatusDropdownActionNode />;
};

const TermedNoPrior = () => {
  const [pageNumberReset, setPageNumberReset] = useState(false);

  return (
    <Page
      label={CAPTIONS.PAY426_TERMINATED_NO_PRIOR}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width="100%">
          <TermedNoPriorGrid
            pageNumberReset={pageNumberReset}
            setPageNumberReset={setPageNumberReset}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default TermedNoPrior;
