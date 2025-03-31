import Grid2 from '@mui/material/Grid2';
import { Page } from "smart-ui-library";
import DemographicBadgesNotInPayprofitGrid from "./DemographicBadgesNotInPayprofitGrid";
import { CAPTIONS } from "../../../constants";

const DemographicBadgesNotInPayprofit = () => {
  return (
    <Page label={CAPTIONS.DEMOGRAPHIC_BADGES}>
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
