import { Grid } from "@mui/material";
import { Page } from "smart-ui-library";
import DemographicBadgesNotInPayprofitGrid from "./DemographicBadgesNotInPayprofitGrid";
import { CAPTIONS } from "../../../constants";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";

const DemographicBadgesNotInPayprofit = () => {
  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };
  return (
    <Page
      label={CAPTIONS.DEMOGRAPHIC_BADGES}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width="100%">
          <DemographicBadgesNotInPayprofitGrid />
        </Grid>
      </Grid>
    </Page>
  );
};

export default DemographicBadgesNotInPayprofit;
