import { Typography } from "@mui/material";
import { Grid } from "@mui/material";
import { useCallback, useEffect, useMemo } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { useLazyGetBreakdownByStoreQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, Pagination } from "smart-ui-library";
import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import { SortParams, useGridPagination } from "../../../../hooks/useGridPagination";
import { GetAssociatesColumns } from "./AssociatesGridColumns";

interface AssociatesGridProps {
  store: number;
  pageNumberReset: boolean;
  setPageNumberReset: (reset: boolean) => void;
  onLoadingChange?: (isLoading: boolean) => void;
}

const AssociatesGrid: React.FC<AssociatesGridProps> = ({
  store,
  pageNumberReset,
  setPageNumberReset,
  onLoadingChange
}) => {
  const [fetchBreakdownByStore, { isFetching }] = useLazyGetBreakdownByStoreQuery();
  const breakdownByStore = useSelector((state: RootState) => state.yearsEnd.breakdownByStore);
  const queryParams = useSelector((state: RootState) => state.yearsEnd.breakdownByStoreQueryParams);
  const navigate = useNavigate();
  const profitYear = useDecemberFlowProfitYear();
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);

  const { pageNumber, pageSize, sortParams, handlePaginationChange, handleSortChange, resetPagination } =
    useGridPagination({
      initialPageSize: 10,
      initialSortBy: "badgeNumber",
      initialSortDescending: false,
      onPaginationChange: useCallback(
        async (pageNum: number, pageSz: number, sortPrms: SortParams) => {
          if (hasToken) {
            const params = {
              profitYear: queryParams?.profitYear || profitYear,
              storeNumber: store,
              storeManagement: false,
              badgeNumber: queryParams?.badgeNumber,
              employeeName: queryParams?.employeeName,
              pagination: {
                skip: pageNum * pageSz,
                take: pageSz,
                sortBy: sortPrms.sortBy,
                isSortDescending: sortPrms.isSortDescending
              }
            };
            await fetchBreakdownByStore(params);
          }
        },
        [
          hasToken,
          queryParams?.profitYear,
          profitYear,
          store,
          queryParams?.badgeNumber,
          queryParams?.employeeName,
          fetchBreakdownByStore
        ]
      )
    });

  const handleNavigation = useCallback(
    (path: string) => {
      navigate(path);
    },
    [navigate]
  );

  const fetchData = useCallback(() => {
    const params = {
      profitYear: queryParams?.profitYear || profitYear,
      storeNumber: store,
      storeManagement: false,
      badgeNumber: queryParams?.badgeNumber,
      employeeName: queryParams?.employeeName,
      pagination: {
        skip: pageNumber * pageSize,
        take: pageSize,
        sortBy: sortParams.sortBy,
        isSortDescending: sortParams.isSortDescending
      }
    };
    if (hasToken) {
      fetchBreakdownByStore(params);
    }
  }, [
    fetchBreakdownByStore,
    hasToken,
    pageNumber,
    pageSize,
    profitYear,
    queryParams?.profitYear,
    queryParams?.badgeNumber,
    queryParams?.employeeName,
    sortParams.isSortDescending,
    sortParams.sortBy,
    store
  ]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  useEffect(() => {
    if (pageNumberReset) {
      resetPagination();
      setPageNumberReset(false);
    }
  }, [pageNumberReset, setPageNumberReset, resetPagination]);

  // Notify parent component about loading state changes
  useEffect(() => {
    onLoadingChange?.(isFetching);
  }, [isFetching, onLoadingChange]);

  const columnDefs = useMemo(() => GetAssociatesColumns(handleNavigation), [handleNavigation]);

  return (
    <Grid
      container
      direction="column"
      width="100%">
      <Grid paddingX="24px">
        <Typography
          variant="h6"
          sx={{ color: "#0258A5", marginBottom: "16px" }}>
          Associates
        </Typography>
      </Grid>
      <Grid width="100%">
        <DSMGrid
          preferenceKey={`BREAKDOWN_REPORT_ASSOCIATES_STORE_${store}`}
          isLoading={isFetching}
          handleSortChanged={handleSortChange}
          providedOptions={{
            rowData: breakdownByStore?.response?.results || [],
            columnDefs: columnDefs
          }}
        />
        {breakdownByStore?.response?.results && breakdownByStore.response.results.length > 0 && (
          <Pagination
            pageNumber={pageNumber}
            setPageNumber={(value: number) => handlePaginationChange(value - 1, pageSize)}
            pageSize={pageSize}
            setPageSize={(value: number) => handlePaginationChange(0, value)}
            recordCount={breakdownByStore.response.total || 0}
          />
        )}
      </Grid>
    </Grid>
  );
};

export default AssociatesGrid;
