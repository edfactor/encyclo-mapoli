import Grid2 from '@mui/material/Grid2';
import { Page } from "smart-ui-library";
import DemographicBadgesNotInPayprofitGrid from "./DemographicBadgesNotInPayprofitGrid";
import { CAPTIONS } from "../../../constants";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";

const DemographicBadgesNotInPayprofit = () => {

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };
  return (
    <Page label={CAPTIONS.DEMOGRAPHIC_BADGES} actionNode={renderActionNode()}>
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
