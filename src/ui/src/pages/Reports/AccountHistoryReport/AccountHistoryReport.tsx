import { Divider, Grid } from "@mui/material";
import React, { useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { AccountHistoryReportApi } from "../../../reduxstore/api/AccountHistoryReportApi";
import AccountHistoryReportFilterSection, {
  AccountHistoryReportFilterParams
} from "./AccountHistoryReportFilterSection";
import AccountHistoryReportTable from "./AccountHistoryReportTable";

const AccountHistoryReport: React.FC = () => {
  const [filterParams, setFilterParams] = useState<AccountHistoryReportFilterParams | null>(null);
  const [triggerSearch, { data, isFetching }] = AccountHistoryReportApi.useLazyGetAccountHistoryReportQuery();

  const handleFilterChange = (params: AccountHistoryReportFilterParams) => {
    setFilterParams(params);
    
    // Trigger the query immediately with the search params
    const queryParams = {
      badgeNumber: parseInt(params.badgeNumber, 10),
      startDate: params.startDate ? params.startDate.toISOString().split("T")[0] : undefined,
      endDate: params.endDate ? params.endDate.toISOString().split("T")[0] : undefined
    };
    
    triggerSearch(queryParams);
  };

  const handleReset = () => {
    setFilterParams(null);
  };

  return (
    <Page label={CAPTIONS.DIVORCE_REPORT}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <DSMAccordion title="Filter">
            <AccountHistoryReportFilterSection
              onFilterChange={handleFilterChange}
              onReset={handleReset}
              isLoading={isFetching}
            />
          </DSMAccordion>
        </Grid>

        {filterParams && (
          <Grid width="100%">
            <AccountHistoryReportTable
              data={data}
              isLoading={isFetching}
              error={undefined}
              showData={!!filterParams}
            />
          </Grid>
        )}
      </Grid>
    </Page>
  );
};

export default AccountHistoryReport;
