import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { Page } from "smart-ui-library";
import DuplicateSSNsOnDemographicsGrid from "./DuplicateSSNsOnDemographicsGrid";

const DuplicateSSNsOnDemographics = () => {
  return (
    <Page label="Duplicate SSNs on Demographics">
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>

        <Grid2 width="100%">
          <DuplicateSSNsOnDemographicsGrid />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default DuplicateSSNsOnDemographics;
