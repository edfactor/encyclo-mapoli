import { Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useRef } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import useYTDWages from "./hooks/useYTDWages";
import YTDWagesGrid from "./YTDWagesGrid";
import YTDWagesSearchFilter from "./YTDWagesSearchFilter";

const YTDWages: React.FC = () => {
  const componentRef = useRef<HTMLDivElement>(null);
  const { searchResults, isSearching, pagination, showData, hasResults, executeSearch } = useYTDWages();

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  //const recordCount = searchResults?.response?.total || 0;

  return (
    <Page
      label={`${CAPTIONS.YTD_WAGES_EXTRACT}`}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid
          width={"100%"}
          hidden={true}>
          <DSMAccordion title="Filter">
            <YTDWagesSearchFilter
              onSearch={executeSearch}
              isSearching={isSearching}
            />
          </DSMAccordion>
        </Grid>

        <Grid width="100%">
          <YTDWagesGrid
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

export default YTDWages;
