import { Divider } from "@mui/material";
import { Grid } from "@mui/material";
import { Page } from "smart-ui-library";
import DuplicateSSNsOnDemographicsGrid from "./DuplicateSSNsOnDemographicsGrid";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";

const DuplicateSSNsOnDemographics = () => {
  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };
  return (
    <Page
      label="Duplicate SSNs on Demographics"
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>

        <Grid width="100%">
          <DuplicateSSNsOnDemographicsGrid />
        </Grid>
      </Grid>
    </Page>
  );
};

export default DuplicateSSNsOnDemographics;
