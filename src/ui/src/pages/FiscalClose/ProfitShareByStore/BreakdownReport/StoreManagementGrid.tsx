import { Typography } from "@mui/material";
import { Grid } from "@mui/material";
import { useCallback, useEffect } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { useLazyGetBreakdownByStoreQuery } from "reduxstore/api/AdhocApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, Pagination } from "smart-ui-library";
import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import { GRID_KEYS } from "../../../../constants";
import { useGridPagination, SortParams } from "../../../../hooks/useGridPagination";
import { GetStoreManagementGridColumns } from "./StoreManagementGridColumns";

interface StoreManagementGridProps {
  store: number;
  pageNumberReset: boolean;
  setPageNumberReset: (reset: boolean) => void;
  onLoadingChange?: (isLoading: boolean) => void;
}

const StoreManagementGrid: React.FC<StoreManagementGridProps> = ({
  store,
  pageNumberReset,
  setPageNumberReset,
  onLoadingChange
}) => {
  const [fetchStoreManagement, { isFetching }] = useLazyGetBreakdownByStoreQuery();
  const storeManagement = useSelector((state: RootState) => state.yearsEnd.storeManagementBreakdown);
  const queryParams = useSelector((state: RootState) => state.yearsEnd.breakdownByStoreQueryParams);
  const navigate = useNavigate();
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const profitYear = useDecemberFlowProfitYear();

  const {
    pageNumber,
    pageSize,
    sortParams,
    handlePageNumberChange,
    handlePageSizeChange,
    handleSortChange,
    resetPagination
  } = useGridPagination({
    initialPageSize: 10,
    initialSortBy: "badgeNumber",
    initialSortDescending: false,
    persistenceKey: `${GRID_KEYS.STORE_MANAGEMENT_PREFIX}${store}`,
    onPaginationChange: useCallback(
      async (pageNum: number, pageSz: number, sortPrms: SortParams) => {
        if (hasToken) {
          const params = {
            profitYear: queryParams?.profitYear || profitYear,
            storeNumber: store,
            storeManagement: true,
            badgeNumber: queryParams?.badgeNumber,
            employeeName: queryParams?.employeeName,
            pagination: {
              skip: pageNum * pageSz,
              take: pageSz,
              sortBy: sortPrms.sortBy,
              isSortDescending: sortPrms.isSortDescending
            }
          };
          await fetchStoreManagement(params);
        }
      },
      [
        hasToken,
        queryParams?.profitYear,
        profitYear,
        store,
        queryParams?.badgeNumber,
        queryParams?.employeeName,
        fetchStoreManagement
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
      storeManagement: true,
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
      fetchStoreManagement(params);
    }
  }, [
    fetchStoreManagement,
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

  const columnDefs = useCallback(() => GetStoreManagementGridColumns(handleNavigation), [handleNavigation])();

  return (
    <Grid
      container
      direction="column"
      width="100%">
      <Grid paddingX="24px">
        <Typography
          variant="h6"
          sx={{ color: "#0258A5", marginBottom: "16px" }}>
          Store Management
        </Typography>
      </Grid>
      <Grid width="100%">
        <DSMGrid
          preferenceKey={`${GRID_KEYS.STORE_MANAGEMENT_PREFIX}${store}`}
          isLoading={isFetching}
          handleSortChanged={handleSortChange}
          providedOptions={{
            rowData: storeManagement?.response?.results || [],
            columnDefs: columnDefs
          }}
        />
        {storeManagement?.response?.results && storeManagement.response.results.length > 0 && (
          <Pagination
            pageNumber={pageNumber}
            setPageNumber={(value: number) => handlePageNumberChange(value - 1)}
            pageSize={pageSize}
            setPageSize={handlePageSizeChange}
            recordCount={storeManagement.response.total || 0}
          />
        )}
      </Grid>
    </Grid>
  );
};

export default StoreManagementGrid;
