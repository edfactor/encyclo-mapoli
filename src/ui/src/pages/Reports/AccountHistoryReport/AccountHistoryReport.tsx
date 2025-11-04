import { Divider, Grid } from "@mui/material";
import React, { useCallback, useRef, useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { SortParams, useGridPagination } from "../../../hooks/useGridPagination";
import { AccountHistoryReportApi } from "../../../reduxstore/api/AccountHistoryReportApi";
import { AccountHistoryReportRequest } from "../../../types/reports/AccountHistoryReportTypes";
import AccountHistoryReportFilterSection, {
  AccountHistoryReportFilterParams
} from "./AccountHistoryReportFilterSection";
import AccountHistoryReportTable from "./AccountHistoryReportTable";

const AccountHistoryReport: React.FC = () => {
  const [filterParams, setFilterParams] = useState<AccountHistoryReportFilterParams | null>(null);
  const [triggerSearch, { data, isFetching }] = AccountHistoryReportApi.useLazyGetAccountHistoryReportQuery();

  // Pagination state
  const filterParamsRef = useRef(filterParams);
  filterParamsRef.current = filterParams;

  const handleReportPaginationChange = useCallback(
    async (pageNumber: number, pageSize: number, sortParams: SortParams) => {
      const currentFilterParams = filterParamsRef.current;

      if (!currentFilterParams) return;

      // Trigger the query with pagination parameters
      const queryParams: AccountHistoryReportRequest = {
        badgeNumber: parseInt(currentFilterParams.badgeNumber, 10),
        startDate: currentFilterParams.startDate
          ? currentFilterParams.startDate.toISOString().split("T")[0]
          : undefined,
        endDate: currentFilterParams.endDate ? currentFilterParams.endDate.toISOString().split("T")[0] : undefined,
        pagination: {
          skip: pageNumber * pageSize,
          take: pageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        }
      };

      await triggerSearch(queryParams);
    },
    [triggerSearch]
  );

  const gridPagination = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "profitYear",
    initialSortDescending: true,
    onPaginationChange: handleReportPaginationChange
  });

  const paginationRef = useRef(gridPagination);
  paginationRef.current = gridPagination;

  const handleFilterChange = (params: AccountHistoryReportFilterParams) => {
    setFilterParams(params);

    // Trigger the query immediately with the search params
    const queryParams: AccountHistoryReportRequest = {
      badgeNumber: parseInt(params.badgeNumber, 10),
      startDate: params.startDate ? params.startDate.toISOString().split("T")[0] : undefined,
      endDate: params.endDate ? params.endDate.toISOString().split("T")[0] : undefined,
      pagination: {
        skip: 0,
        take: 25,
        sortBy: "profitYear",
        isSortDescending: true
      }
    };

    triggerSearch(queryParams);

    // Reset pagination to first page on new search
    paginationRef.current.resetPagination();
  };

  const handleReset = () => {
    setFilterParams(null);
    paginationRef.current.resetPagination();
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
              gridPagination={gridPagination}
            />
          </Grid>
        )}
      </Grid>
    </Page>
  );
};

export default AccountHistoryReport;
