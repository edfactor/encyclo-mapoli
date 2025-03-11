import Grid2 from '@mui/material/Grid2';
import DistributionByAgeGrid from "pages/DistributionByAge/DistributionByAgeGrid";
import DuplicateNamesAndBirthdaysGrid from "pages/DuplicateNamesAndBirthdays/DuplicateNamesAndBirthdaysGrid";

export const Distributions = () => {
  return (
    <Grid2
      container
      width="100%"
      rowSpacing="24px">
      <Grid2
        paddingX={"24px"}
        width="100%">
        <DistributionByAgeGrid />
      </Grid2>
    </Grid2>
  );
};

export default Distributions;
