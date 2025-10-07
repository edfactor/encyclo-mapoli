import { Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useRef } from "react";
import { Page } from "smart-ui-library";
import DuplicateSSNsOnDemographicsGrid from "./DuplicateSSNsOnDemographicsGrid";
import useDuplicateSSNsOnDemographics from "./hooks/useDuplicateSSNsOnDemographics";

const DuplicateSSNsOnDemographics = () => {
  const componentRef = useRef<HTMLDivElement>(null);
  const { searchResults, isSearching, pagination, showData, hasResults } = useDuplicateSSNsOnDemographics();

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  const recordCount = searchResults?.response?.total || 0;

  return (
    <Page
      label={`Duplicate SSNs on Demographics (${recordCount} records)`}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>

        <Grid width="100%">
          <DuplicateSSNsOnDemographicsGrid
            innerRef={componentRef}
            data={searchResults}
            isLoading={isSearching}
            showData={showData}
            hasResults={hasResults ?? false}
            pagination={pagination}
            onPaginationChange={pagination.handlePaginationChange}
            onSortChange={pagination.handleSortChange}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default DuplicateSSNsOnDemographics;
