import PageErrorBoundary from "@/components/PageErrorBoundary";
import { Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useRef } from "react";
import { Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import EligibleEmployeesGrid from "./EligibleEmployeesGrid";
import useEligibleEmployees from "./hooks/useEligibleEmployees";

const EligibleEmployees = () => {
  const componentRef = useRef<HTMLDivElement>(null);
  const { searchResults, isSearching, pagination, showData, hasResults, handleStatusChange } = useEligibleEmployees();

  const renderActionNode = () => {
    return <StatusDropdownActionNode onStatusChange={handleStatusChange} />;
  };

  return (
    <PageErrorBoundary pageName="Eligible Employees">
      <Page
        label={`${CAPTIONS.ELIGIBLE_EMPLOYEES}`}
        actionNode={renderActionNode()}>
        <Grid
          container
          rowSpacing="24px">
          <Grid width={"100%"}>
            <Divider />
          </Grid>

          <Grid width="100%">
            <EligibleEmployeesGrid
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

export default EligibleEmployees;
