import DistributionByAgeGrid from "@/pages/PROF130/DistributionsByAge/DistributionsByAgeGrid";
import { Grid } from "@mui/material";

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
