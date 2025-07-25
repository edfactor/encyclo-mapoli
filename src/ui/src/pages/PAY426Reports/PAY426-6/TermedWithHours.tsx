import { CAPTIONS } from "../../../constants";
import { Divider } from "@mui/material";
import { Grid } from "@mui/material";
import TermedWithHoursGrid from "./TermedWithHoursGrid";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { Page } from "smart-ui-library";
import { useState } from "react";

const renderActionNode = () => {
  return <StatusDropdownActionNode />;
};

const TermedWithHours = () => {
  const [pageNumberReset, setPageNumberReset] = useState(false);

  return (
    <Page
      label={CAPTIONS.PAY426_TERMINATED_1000_PLUS}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width="100%">
          <TermedWithHoursGrid
            pageNumberReset={pageNumberReset}
            setPageNumberReset={setPageNumberReset}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default TermedWithHours;
