import DemographicBadgesNotInPayprofitGrid from "pages/DemographicBadgesNotInPayprofit/DemographicBadgesNotInPayprofitGrid";

import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion } from "smart-ui-library";
import DemographicBadgesNotInPayprofitSearchFilter from "pages/DemographicBadgesNotInPayprofit/DemographicBadgesNotInPayprofitSearchFilter";
import { Divider } from "@mui/material";

export const DemoBadges = () => {
  return (
    <Grid2
      container
      rowSpacing="24px">
      <Grid2 width={"100%"}>
        <Divider />
      </Grid2>
      <Grid2
        width={"100%"}>
        <DSMAccordion title="Filter">
          <DemographicBadgesNotInPayprofitSearchFilter />
        </DSMAccordion>

      </Grid2>

      <Grid2 width="100%">
        <DemographicBadgesNotInPayprofitGrid />
      </Grid2>
    </Grid2>
  );
};

export default DemoBadges;
