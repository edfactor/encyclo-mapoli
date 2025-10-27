import { Grid } from "@mui/material";
import BalanceByAgeGrid from "../../../pages/FiscalClose/AgeReports/BalanceByAge/BalanceByAgeGrid";

export const Balance = () => {
  return (
    <Grid
      container
      width="100%"
      rowSpacing="24px">
      <Grid
        paddingX={"24px"}
        width="100%">
        <BalanceByAgeGrid />
      </Grid>
    </Grid>
  );
};

export default Balance;
