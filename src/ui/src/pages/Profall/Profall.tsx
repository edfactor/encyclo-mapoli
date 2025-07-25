import { Divider } from "@mui/material";
import { Grid } from "@mui/material";
import { Page } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import ProfallGrid from "./ProfallGrid";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useState } from "react";

const Profall = () => {
  const [pageNumberReset, setPageNumberReset] = useState(false);

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  return (
    <Page
      label={CAPTIONS.PROFALL}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width="100%">
          <ProfallGrid
            pageNumberReset={pageNumberReset}
            setPageNumberReset={setPageNumberReset}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default Profall;
