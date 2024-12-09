import Grid2 from "@mui/material/Unstable_Grid2";
import NegativeEtvaForSSNsOnPayprofitGrid from "pages/NegativeEtvaForSSNsOnPayprofit/NegativeEtvaForSSNsOnPayprofitGrid";

export const NegativeEtvaTab = () => {
  return (
    <Grid2
      container
      width="100%"
      rowSpacing="24px">
      <Grid2
        paddingX={"24px"}
        width="100%">
        <NegativeEtvaForSSNsOnPayprofitGrid />
      </Grid2>
    </Grid2>
  );
};

export default NegativeEtvaTab;
