import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import NegativeEtvaForSSNsOnPayprofitGrid from "pages/DecemberActivities/NegativeEtvaForSSNsOnPayprofit/NegativeEtvaForSSNsOnPayprofitGrid";
import { useState } from "react";

export const NegativeEtvaTab = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  return (
    <Grid2
      container
      rowSpacing="24px">
      <Grid2 width={"100%"}>
        <Divider />
      </Grid2>

      <Grid2 width="100%">
        <NegativeEtvaForSSNsOnPayprofitGrid
          setInitialSearchLoaded={setInitialSearchLoaded}
          initialSearchLoaded={initialSearchLoaded}
        />
      </Grid2>
    </Grid2>
  );
};

export default NegativeEtvaTab;
