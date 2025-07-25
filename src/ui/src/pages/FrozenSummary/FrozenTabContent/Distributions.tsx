import { Grid } from "@mui/material";
import DistributionByAgeGrid from "pages/PROF130/DistributionByAge/DistributionByAgeGrid";

export const Distributions = () => {
  return (
    <Grid
      container
      width="100%"
      rowSpacing="24px">
      <Grid
        paddingX={"24px"}
        width="100%">
        <DistributionByAgeGrid />
      </Grid>
    </Grid>
  );
};

export default Distributions;
