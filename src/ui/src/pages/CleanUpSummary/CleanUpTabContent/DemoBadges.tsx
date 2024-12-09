import DemographicBadgesNotInPayprofitGrid from "pages/DemographicBadgesNotInPayprofit/DemographicBadgesNotInPayprofitGrid";

import Grid2 from "@mui/material/Unstable_Grid2";

export const DemoBadges = () => {
  return (
    <Grid2
      container
      width="100%"
      rowSpacing="24px">
      <Grid2
        paddingX={"24px"}
        width="100%">
        <DemographicBadgesNotInPayprofitGrid />
      </Grid2>
    </Grid2>
  );
};

export default DemoBadges;
