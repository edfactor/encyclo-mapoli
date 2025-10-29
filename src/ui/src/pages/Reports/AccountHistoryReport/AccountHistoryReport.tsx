import { Divider, Grid } from "@mui/material";
import React, { useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { useGetAccountHistoryReportQuery } from "../../../reduxstore/api/AccountHistoryReportApi";
import AccountHistoryReportFilterSection, {
  AccountHistoryReportFilterParams
} from "./AccountHistoryReportFilterSection";
import AccountHistoryReportTable from "./AccountHistoryReportTable";

const AccountHistoryReport: React.FC = () => {
  const [filterParams, setFilterParams] = useState<AccountHistoryReportFilterParams | null>(null);

  // Construct the query parameters
  const queryParams = filterParams
    ? {
        badgeNumber: parseInt(filterParams.badgeNumber, 10),
        startDate: filterParams.startDate ? filterParams.startDate.toISOString().split("T")[0] : undefined,
        endDate: filterParams.endDate ? filterParams.endDate.toISOString().split("T")[0] : undefined
      }
    : null;

  // Execute the query only when we have valid filter params
  const {
    data,
    isLoading: isQueryLoading,
    error
  } = useGetAccountHistoryReportQuery(queryParams || { badgeNumber: 0, startDate: undefined, endDate: undefined }, {
    skip: !queryParams
  });

  const handleFilterChange = (params: AccountHistoryReportFilterParams) => {
    setFilterParams(params);
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
              isLoading={isQueryLoading}
            />
          </DSMAccordion>
        </Grid>

        {filterParams && (
          <Grid width="100%">
            <AccountHistoryReportTable
              data={data}
              isLoading={isQueryLoading}
              error={error}
              showData={!!filterParams}
            />
          </Grid>
        )}
      </Grid>
    </Page>
  );
};

export default AccountHistoryReport;
