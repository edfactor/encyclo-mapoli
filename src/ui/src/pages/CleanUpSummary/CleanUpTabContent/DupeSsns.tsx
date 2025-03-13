import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import DuplicateSSNsOnDemographicsGrid from "pages/DuplicateSSNsOnDemographics/DuplicateSSNsOnDemographicsGrid";

export const DupeSsns = () => {
  return (
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
  );
};

export default DupeSsns;
