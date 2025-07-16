import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { Page } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import ProfallGrid from "./ProfallGrid";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useState } from "react";

const Profall = () => {
  const [pageNumberReset, setPageNumberReset] = useState(false);

  const renderActionNode = () => {
    return (
      <StatusDropdownActionNode />
    );
  };

  return (
    <Page label={CAPTIONS.PROFALL} actionNode={renderActionNode()}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width="100%">
          <ProfallGrid 
            pageNumberReset={pageNumberReset}
            setPageNumberReset={setPageNumberReset}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default Profall;
