import { Grid } from "@mui/material";
import ForfeituresByAgeGrid from "pages/PROF130/ForfeituresByAge/ForfeituresByAgeGrid";

export const Forfeitures = () => {
  return (
    <Grid
      container
      width="100%"
      rowSpacing="24px">
      <Grid
        paddingX={"24px"}
        width="100%">
        <ForfeituresByAgeGrid />
      </Grid>
    </Grid>
  );
};

export default Forfeitures;
