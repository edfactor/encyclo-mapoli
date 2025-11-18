import { Divider, Grid } from "@mui/material";
import { useCallback, useEffect, useState } from "react";
import { Page } from "smart-ui-library";
import { SortParams } from "../../../hooks/useGridPagination";
import { useLazySearchAuditQuery } from "../../../reduxstore/api/ItOperationsApi";
import { useLazyGetNavigationStatusQuery } from "../../../reduxstore/api/NavigationStatusApi";
import AuditSearchGrid from "./AuditSearchGrid";
import AuditSearchManager from "./AuditSearchManager";

interface AuditSearchFilters {
  tableName: string;
  operation: string;
  userName: string;
  startDate: Date | null;
  endDate: Date | null;
}

const AuditSearch = () => {
  const [triggerSearch, { data, isFetching }] = useLazySearchAuditQuery();
  const [triggerGetNavigationStatus, { data: navigationStatusData }] = useLazyGetNavigationStatusQuery();
  const [currentFilters, setCurrentFilters] = useState<AuditSearchFilters>({
    tableName: "",
    operation: "",
    userName: "",
    startDate: null,
    endDate: null
  });

  useEffect(() => {
    triggerGetNavigationStatus({});
  }, [triggerGetNavigationStatus]);
  const [, setCurrentPageNumber] = useState(0);
  const [currentPageSize, setCurrentPageSize] = useState(25);
  const [currentSortParams, setCurrentSortParams] = useState<SortParams>({
    sortBy: "createdAt",
    isSortDescending: true
  });

  const executeSearch = useCallback(
    (filters: AuditSearchFilters, pageNumber: number, pageSize: number, sortParams: SortParams) => {
      triggerSearch({
        tableName: filters.tableName || undefined,
        operation: filters.operation || undefined,
        userName: filters.userName || undefined,
        startTime: filters.startDate ? filters.startDate.toISOString() : undefined,
        endTime: filters.endDate ? filters.endDate.toISOString() : undefined,
        skip: pageNumber * pageSize,
        take: pageSize,
        sortBy: sortParams.sortBy,
        isSortDescending: sortParams.isSortDescending
      });
    },
    [triggerSearch]
  );

  const handleSearch = useCallback(
    (filters: AuditSearchFilters) => {
      setCurrentFilters(filters);
      setCurrentPageNumber(0);
      executeSearch(filters, 0, currentPageSize, currentSortParams);
    },
    [currentPageSize, currentSortParams, executeSearch]
  );

  const handlePaginationChange = useCallback(
    (pageNumber: number, pageSize: number, sortParams: SortParams) => {
      setCurrentPageNumber(pageNumber);
      setCurrentPageSize(pageSize);
      setCurrentSortParams(sortParams);
      executeSearch(currentFilters, pageNumber, pageSize, sortParams);
    },
    [currentFilters, executeSearch]
  );

  return (
    <Page label="Audit Search">
      <Grid
        container
        rowSpacing={3}>
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <AuditSearchManager
            onSearch={handleSearch}
            isLoading={isFetching}
          />
        </Grid>
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <AuditSearchGrid
            data={data?.results || []}
            total={data?.total || 0}
            isLoading={isFetching}
            onPaginationChange={handlePaginationChange}
            navigationStatusList={navigationStatusData?.navigationStatusList || []}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default AuditSearch;
