import { Divider, Grid } from "@mui/material";
import { useCallback, useEffect, useState } from "react";
import { Page } from "smart-ui-library";
import PageErrorBoundary from "../../../components/PageErrorBoundary/PageErrorBoundary";
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
  startTime: string;
  endDate: Date | null;
  endTime: string;
}

const AuditSearch = () => {
  const [triggerSearch, { data, isFetching, reset: resetSearch }] = useLazySearchAuditQuery();
  const [triggerGetNavigationStatus, { data: navigationStatusData }] = useLazyGetNavigationStatusQuery();
  const [currentFilters, setCurrentFilters] = useState<AuditSearchFilters>({
    tableName: "",
    operation: "",
    userName: "",
    startDate: null,
    startTime: "00:00",
    endDate: null,
    endTime: "23:59"
  });

  useEffect(() => {
    triggerGetNavigationStatus({});
  }, [triggerGetNavigationStatus]);

  // Helper function to combine date and time
  const combineDateAndTime = (date: Date | null, time: string): Date | null => {
    if (!date) return null;
    const [hours, minutes] = time.split(":").map(Number);
    const combined = new Date(date);
    combined.setHours(hours, minutes, 0, 0);
    return combined;
  };

  const [, setCurrentPageNumber] = useState(0);
  const [currentPageSize, setCurrentPageSize] = useState(25);
  const [currentSortParams, setCurrentSortParams] = useState<SortParams>({
    sortBy: "createdAt",
    isSortDescending: true
  });

  const executeSearch = useCallback(
    (filters: AuditSearchFilters, pageNumber: number, pageSize: number, sortParams: SortParams) => {
      const startDateTime = combineDateAndTime(filters.startDate, filters.startTime);
      const endDateTime = combineDateAndTime(filters.endDate, filters.endTime);

      triggerSearch({
        tableName: filters.tableName || undefined,
        operation: filters.operation || undefined,
        userName: filters.userName || undefined,
        startTime: startDateTime ? startDateTime.toISOString() : undefined,
        endTime: endDateTime ? endDateTime.toISOString() : undefined,
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

  const handleReset = useCallback(() => {
    setCurrentFilters({
      tableName: "",
      operation: "",
      userName: "",
      startDate: null,
      startTime: "00:00",
      endDate: null,
      endTime: "23:59"
    });
    setCurrentPageNumber(0);
    resetSearch();
  }, [resetSearch]);

  return (
    <PageErrorBoundary pageName="Audit Search">
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
              onReset={handleReset}
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
    </PageErrorBoundary>
  );
};

export default AuditSearch;
