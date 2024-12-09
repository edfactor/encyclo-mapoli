import Grid2 from "@mui/material/Unstable_Grid2";
import DuplicateNamesAndBirthdaysGrid from "pages/DuplicateNamesAndBirthdays/DuplicateNamesAndBirthdaysGrid";

export const DupeNames = () => {
  return (
    <Grid2
      container
      width="100%"
      rowSpacing="24px">
      <Grid2
        paddingX={"24px"}
        width="100%">
        <DuplicateNamesAndBirthdaysGrid />
      </Grid2>
    </Grid2>
  );
};

export default DupeNames;
