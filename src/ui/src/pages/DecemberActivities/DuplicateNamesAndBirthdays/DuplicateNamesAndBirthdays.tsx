import { Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useRef } from "react";
import { Page } from "smart-ui-library";
import DuplicateNamesAndBirthdaysGrid from "./DuplicateNamesAndBirthdaysGrid";
import useDuplicateNamesAndBirthdays from "./hooks/useDuplicateNamesAndBirthdays";

const DuplicateNamesAndBirthdays = () => {
  const componentRef = useRef<HTMLDivElement>(null);
  const { searchResults, isSearching, pagination, showData, hasResults } =
    useDuplicateNamesAndBirthdays();

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  const recordCount = searchResults?.response?.total || 0;

  return (
    <Page
      label={`Duplicate Names and Birthdays (${recordCount} records)`}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>

        <Grid width="100%">
          <DuplicateNamesAndBirthdaysGrid
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

export default DuplicateNamesAndBirthdays;
