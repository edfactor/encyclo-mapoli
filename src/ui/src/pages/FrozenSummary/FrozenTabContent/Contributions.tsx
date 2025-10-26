import { Grid } from "@mui/material";
import ContributionsByAgeGrid from "../../../pages/FiscalClose/AgeReports/ContributionsByAge/ContributionsByAgeGrid";

export const Contributions = () => {
  return (
    <Grid
      container
      width="100%"
      rowSpacing="24px">
      <Grid
        paddingX={"24px"}
        width="100%">
        <ContributionsByAgeGrid />
      </Grid>
    </Grid>
  );
};

export default Contributions;
