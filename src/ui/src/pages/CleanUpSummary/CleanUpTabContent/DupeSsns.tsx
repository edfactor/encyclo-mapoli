import Grid2 from "@mui/material/Unstable_Grid2";
import DuplicateSSNsOnDemographicsGrid from "pages/DuplicateSSNsOnDemographics/DuplicateSSNsOnDemographicsGrid";

export const DupeSsns = () => {
  return (
    <Grid2
      container
      width="100%"
      rowSpacing="24px">
      <Grid2
        paddingX={"24px"}
        width="100%">
        <DuplicateSSNsOnDemographicsGrid />
      </Grid2>
    </Grid2>
  );
};

export default DupeSsns;
