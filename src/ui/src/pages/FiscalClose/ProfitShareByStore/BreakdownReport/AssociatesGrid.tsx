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
import { GetAssociatesColumns } from "./AssociatesGridColumns";

interface AssociatesGridProps {
  store: number;
  pageNumberReset: boolean;
  setPageNumberReset: (reset: boolean) => void;
  onLoadingChange?: (isLoading: boolean) => void;
  refetchTrigger?: number;
  isGridExpanded?: boolean;
  onToggleExpand?: () => void;
}

const AssociatesGrid: React.FC<AssociatesGridProps> = ({
  store,
  pageNumberReset,
  setPageNumberReset,
  onLoadingChange,
  refetchTrigger,
  isGridExpanded = false,
  onToggleExpand
}) => {
  const [fetchBreakdownByStore, { isFetching }] = useLazyGetBreakdownByStoreQuery();
  const breakdownByStore = useSelector((state: RootState) => state.yearsEnd.breakdownByStore);
  const queryParams = useSelector((state: RootState) => state.yearsEnd.breakdownByStoreQueryParams);
  const profitYear = useDecemberFlowProfitYear();
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);

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
    persistenceKey: `${GRID_KEYS.BREAKDOWN_REPORT_ASSOCIATES_PREFIX}${store}`,
    onPaginationChange: useCallback(
      async (pageNum: number, pageSz: number, sortPrms: SortParams) => {
        if (hasToken) {
          const params = {
            profitYear: queryParams?.profitYear || profitYear,
            storeNumber: store || undefined,
            storeManagement: false,
            badgeNumber: queryParams?.badgeNumber && queryParams.badgeNumber > 0 ? queryParams.badgeNumber : undefined,
            employeeName: queryParams?.employeeName && queryParams.employeeName.trim() !== "" ? queryParams.employeeName : undefined,
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

  const fetchData = useCallback(() => {
    const params = {
      profitYear: queryParams?.profitYear || profitYear,
      storeNumber: store || undefined,
      storeManagement: false,
      badgeNumber: queryParams?.badgeNumber && queryParams.badgeNumber > 0 ? queryParams.badgeNumber : undefined,
      employeeName: queryParams?.employeeName && queryParams.employeeName.trim() !== "" ? queryParams.employeeName : undefined,
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

  // Refetch when trigger changes - directly call the API with current params
  useEffect(() => {
    if (refetchTrigger !== undefined && refetchTrigger > 0) {
      if (hasToken) {
        const params = {
          profitYear: queryParams?.profitYear || profitYear,
          storeNumber: store || undefined,
          storeManagement: false,
          badgeNumber: queryParams?.badgeNumber && queryParams.badgeNumber > 0 ? queryParams.badgeNumber : undefined,
          employeeName: queryParams?.employeeName && queryParams.employeeName.trim() !== "" ? queryParams.employeeName : undefined,
          pagination: {
            skip: pageNumber * pageSize,
            take: pageSize,
            sortBy: sortParams.sortBy,
            isSortDescending: sortParams.isSortDescending
          }
        };
        fetchBreakdownByStore(params);
      }
    }
  }, [refetchTrigger, hasToken, queryParams, profitYear, store, pageNumber, pageSize, sortParams, fetchBreakdownByStore]);

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

  const columnDefs = useMemo(() => GetAssociatesColumns(), []);

  return (
    <Grid
      container
      direction="column"
      width="100%">
      <Grid width="100%">
        <DSMPaginatedGrid
          preferenceKey={`${GRID_KEYS.BREAKDOWN_REPORT_ASSOCIATES_PREFIX}${store}`}
          data={breakdownByStore?.response?.results || []}
          columnDefs={columnDefs}
          totalRecords={breakdownByStore?.response?.total || 0}
          isLoading={isFetching}
          pagination={{
            pageNumber,
            pageSize,
            sortParams,
            handlePageNumberChange,
            handlePageSizeChange,
            handleSortChange
          }}
          showPagination={breakdownByStore?.response?.results && breakdownByStore.response.results.length > 0}
          header={
            <Typography
              variant="h6"
              sx={{ color: "#0258A5", marginBottom: "16px", paddingX: "24px" }}>
              Associates
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

export default AssociatesGrid;
