import { Divider, Grid } from "@mui/material";
import PageErrorBoundary from "components/PageErrorBoundary";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useRef } from "react";
import { Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
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
    <PageErrorBoundary pageName="Duplicate SSNs on Demographics">
      <Page
        label={`${CAPTIONS.DUPLICATE_SSNS} (${recordCount} records)`}
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
              onSortChange={pagination.handleSortChange}
            />
          </Grid>
        </Grid>
      </Page>
    </PageErrorBoundary>
  );
};

export default DuplicateSSNsOnDemographics;
