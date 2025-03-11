import DemographicBadgesNotInPayprofitGrid from "pages/DemographicBadgesNotInPayprofit/DemographicBadgesNotInPayprofitGrid";

import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';

export const DemoBadges = () => {
  return (
    <Grid2
      container
      rowSpacing="24px">
      <Grid2 width={"100%"}>
        <Divider />
      </Grid2>

      <Grid2 width="100%">
        <DemographicBadgesNotInPayprofitGrid />
      </Grid2>
    </Grid2>
  );
};

export default DemoBadges;
