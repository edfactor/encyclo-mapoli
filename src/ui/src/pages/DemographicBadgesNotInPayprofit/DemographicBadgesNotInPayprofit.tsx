import Grid2 from "@mui/material/Unstable_Grid2";
import { Page } from "smart-ui-library";
import DemographicBadgesNotInPayprofitGrid from "./DemographicBadgesNotInPayprofitGrid";

const DemographicBadgesNotInPayprofit = () => {
  return (
    <Page label="Demographic Badges Not In Payprofit">
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width="100%">
          <DemographicBadgesNotInPayprofitGrid />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default DemographicBadgesNotInPayprofit;
