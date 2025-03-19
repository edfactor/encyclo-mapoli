import Grid2 from '@mui/material/Grid2';
import ForfeituresByAgeGrid from "pages/PROF130/ForfeituresByAge/ForfeituresByAgeGrid";

export const Forfeitures = () => {
  return (
    <Grid2
      container
      width="100%"
      rowSpacing="24px">
      <Grid2
        paddingX={"24px"}
        width="100%">
        <ForfeituresByAgeGrid />
      </Grid2>
    </Grid2>
  );
};

export default Forfeitures;
