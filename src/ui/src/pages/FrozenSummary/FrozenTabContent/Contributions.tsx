import Grid2 from '@mui/material/Grid2';
import ContributionsByAgeGrid from "pages/PROF130/ContributionsByAge/ContributionsByAgeGrid";

export const Contributions = () => {
  return (
    <Grid2
      container
      width="100%"
      rowSpacing="24px">
      <Grid2
        paddingX={"24px"}
        width="100%">
        <ContributionsByAgeGrid />
      </Grid2>
    </Grid2>
  );
};

export default Contributions;
