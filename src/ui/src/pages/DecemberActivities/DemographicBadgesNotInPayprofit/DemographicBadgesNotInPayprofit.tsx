import { Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useRef } from "react";
import { Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import DemographicBadgesNotInPayprofitGrid from "./DemographicBadgesNotInPayprofitGrid";
import useDemographicBadgesNotInPayprofit from "./hooks/useDemographicBadgesNotInPayprofit";

const DemographicBadgesNotInPayprofit = () => {
  const componentRef = useRef<HTMLDivElement>(null);
  const { searchResults, isSearching, pagination, showData, hasResults } = useDemographicBadgesNotInPayprofit();

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  const recordCount = searchResults?.response?.total || 0;

  return (
    <Page
      label={`${CAPTIONS.DEMOGRAPHIC_BADGES} (${recordCount} records)`}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width="100%">
          <DemographicBadgesNotInPayprofitGrid
            innerRef={componentRef}
            data={searchResults}
            isLoading={isSearching}
            showData={showData}
            hasResults={hasResults ?? false}
            pagination={pagination}
            onSortChange={pagination.handleSortChange}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default DemographicBadgesNotInPayprofit;
