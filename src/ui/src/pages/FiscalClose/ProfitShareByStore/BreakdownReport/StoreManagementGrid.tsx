import FullscreenIcon from "@mui/icons-material/Fullscreen";
import FullscreenExitIcon from "@mui/icons-material/FullscreenExit";
import { Grid, Typography, IconButton } from "@mui/material";
import { useCallback, useEffect, useMemo } from "react";
import { useSelector } from "react-redux";
import { useLazyGetBreakdownByStoreQuery } from "reduxstore/api/AdhocApi";
import { RootState } from "reduxstore/store";
import { DSMPaginatedGrid } from "../../../../components/DSMPaginatedGrid/DSMPaginatedGrid";
import { GRID_KEYS } from "../../../../constants";
import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import { SortParams, useGridPagination } from "../../../../hooks/useGridPagination";
import { GetStoreManagementGridColumns } from "./StoreManagementGridColumns";

interface StoreManagementGridProps {
  store: number;
  pageNumberReset: boolean;
  setPageNumberReset: (reset: boolean) => void;
  onLoadingChange?: (isLoading: boolean) => void;
  refetchTrigger?: number;
  isGridExpanded?: boolean;
  onToggleExpand?: () => void;
}

const StoreManagementGrid: React.FC<StoreManagementGridProps> = ({
  store,
  pageNumberReset,
  setPageNumberReset,
  onLoadingChange,
  refetchTrigger,
  isGridExpanded = false,
  onToggleExpand
}) => {
  const [fetchStoreManagement, { isFetching }] = useLazyGetBreakdownByStoreQuery();
  const storeManagement = useSelector((state: RootState) => state.yearsEnd.storeManagementBreakdown);
  const queryParams = useSelector((state: RootState) => state.yearsEnd.breakdownByStoreQueryParams);
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const profitYear = useDecemberFlowProfitYear();

  // Extract param builder to reduce duplication
  const buildParams = useCallback(
    (pageNum: number, pageSz: number, sortPrms: SortParams) => ({
      profitYear: queryParams?.profitYear || profitYear,
      storeNumber: store || undefined,
      storeManagement: true,
      badgeNumber: queryParams?.badgeNumber && queryParams.badgeNumber > 0 ? queryParams.badgeNumber : undefined,
      employeeName: queryParams?.employeeName?.trim() || undefined,
      pagination: {
        skip: pageNum * pageSz,
        take: pageSz,
        sortBy: sortPrms.sortBy,
        isSortDescending: sortPrms.isSortDescending
      }
    }),
    [queryParams?.profitYear, profitYear, store, queryParams?.badgeNumber, queryParams?.employeeName]
  );

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
          await fetchStoreManagement(buildParams(pageNum, pageSz, sortPrms));
        }
      },
      [hasToken, fetchStoreManagement, buildParams]
    )
  });

  const fetchData = useCallback(() => {
    if (hasToken) {
      fetchStoreManagement(buildParams(pageNumber, pageSize, sortParams));
    }
  }, [fetchStoreManagement, hasToken, pageNumber, pageSize, sortParams, buildParams]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  // Refetch when trigger changes - directly call the API with current params
  useEffect(() => {
    if (refetchTrigger !== undefined && refetchTrigger > 0 && hasToken) {
      fetchStoreManagement(buildParams(pageNumber, pageSize, sortParams));
    }
  }, [refetchTrigger, hasToken, pageNumber, pageSize, sortParams, fetchStoreManagement, buildParams]);

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

  const columnDefs = useMemo(() => GetStoreManagementGridColumns(), []);

  return (
    <Grid
      container
      direction="column"
      width="100%">
      <Grid width="100%">
        <DSMPaginatedGrid
          preferenceKey={`${GRID_KEYS.STORE_MANAGEMENT_PREFIX}${store}`}
          data={storeManagement?.response?.results || []}
          columnDefs={columnDefs}
          totalRecords={storeManagement?.response?.total || 0}
          isLoading={isFetching}
          pagination={{
            pageNumber,
            pageSize,
            sortParams,
            handlePageNumberChange,
            handlePageSizeChange,
            handleSortChange
          }}
          showPagination={storeManagement?.response?.results && storeManagement.response.results.length > 0}
          header={
            <Typography
              variant="h6"
              sx={{ color: "#0258A5", marginBottom: "16px", paddingX: "24px" }}>
              Store Management
            </Typography>
          }
          headerActions={
            onToggleExpand && (
              <IconButton
                onClick={onToggleExpand}
                sx={{ zIndex: 1 }}>
                {isGridExpanded ? <FullscreenExitIcon /> : <FullscreenIcon />}
              </IconButton>
            )
          }
          heightConfig={{
            mode: "content-aware",
            heightPercentage: isGridExpanded ? 0.85 : 0.4,
            isExpanded: isGridExpanded
          }}
        />
      </Grid>
    </Grid>
  );
};

export default StoreManagementGrid;
