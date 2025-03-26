import Grid2 from '@mui/material/Grid2';
import BalanceByAgeGrid from "pages/PROF130/BalanceByAge/BalanceByAgeGrid";

export const Balance = () => {
  return (
    <Grid2
      container
      width="100%"
      rowSpacing="24px">
      <Grid2
        paddingX={"24px"}
        width="100%">
        <BalanceByAgeGrid />
      </Grid2>
    </Grid2>
  );
};

export default Balance;
