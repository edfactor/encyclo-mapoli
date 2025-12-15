import { Box, CircularProgress } from "@mui/material";
import { useCallback, useEffect, useMemo } from "react";
import { useLazyGetYearEndProfitSharingReportLiveQuery } from "reduxstore/api/YearsEndApi";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { useDynamicGridHeight } from "../../../hooks/useDynamicGridHeight";
import { GRID_KEYS } from "../../../constants";
import { SortParams, useGridPagination } from "../../../hooks/useGridPagination";
import { GetProfitShareReportColumns } from "./ProfitShareReportGridColumns";

interface ProfitShareReportSearchParams {
  reportId: number;
  badgeNumber?: number | null;
  [key: string]: unknown;
}

interface ProfitShareReportGridProps {
  searchParams: ProfitShareReportSearchParams | null;
  isInitialSearchLoaded: boolean;
  profitYear: number;
}

const ProfitShareReportGrid: React.FC<ProfitShareReportGridProps> = ({
  searchParams,
  isInitialSearchLoaded,
  profitYear
}) => {
  const [triggerSearch, { data: searchResults, isFetching }] = useLazyGetYearEndProfitSharingReportLiveQuery();

  const data = searchResults?.response?.results || [];
  const recordCount = searchResults?.response?.total || 0;
  const columnDefs = useMemo(() => GetProfitShareReportColumns(), []);

  // Use dynamic grid height utility hook
  const gridMaxHeight = useDynamicGridHeight();

  const createRequest = useCallback(
    (skip: number, sortBy: string, isSortDescending: boolean, profitYear: number, pageSz: number) => {
      if (!searchParams) return null;

      return {
        ...searchParams,
        profitYear,
        pagination: { skip, take: pageSz, sortBy, isSortDescending }
      };
    },
    [searchParams]
  );

  const { pageNumber, pageSize, sortParams, handlePaginationChange, handleSortChange } = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "badgeNumber",
    initialSortDescending: false,
    persistenceKey: GRID_KEYS.PROFIT_SHARE_REPORT,
    onPaginationChange: useCallback(
      async (pageNum: number, pageSz: number, sortPrms: SortParams) => {
        if (isInitialSearchLoaded && searchParams) {
          const params = createRequest(
            pageNum * pageSz,
            sortPrms.sortBy,
            sortPrms.isSortDescending,
            profitYear,
            pageSz
          );
          if (params) {
            triggerSearch(params, false);
          }
        }
      },
      [isInitialSearchLoaded, searchParams, createRequest, profitYear, triggerSearch]
    )
  });

  // Initial search effect - trigger search when component first loads with search params
  useEffect(() => {
    // Don't load data until search button is clicked
    if (!isInitialSearchLoaded || !searchParams) return;

    const executeInitialSearch = async () => {
      const params = createRequest(
        pageNumber * pageSize,
        sortParams.sortBy,
        sortParams.isSortDescending,
        profitYear,
        pageSize
      );
      if (params) {
        await triggerSearch(params, false);
      }
    };

    executeInitialSearch();
  }, [isInitialSearchLoaded, searchParams, createRequest, profitYear, pageNumber, pageSize, sortParams, triggerSearch]);

  const handleSortChanged = useCallback(
    (update: ISortParams) => {
      // Handle empty sortBy case - set default to badgeNumber
      if (update.sortBy === "") {
        update.sortBy = "badgeNumber";
        update.isSortDescending = false;
      }

      handleSortChange(update);
    },
    [handleSortChange]
  );

  // Show loading spinner when fetching data
  if (isFetching && data.length === 0) {
    return (
      <Box sx={{ display: "flex", justifyContent: "center", p: 3 }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <div className="relative">
      <DSMGrid
        preferenceKey={"ProfitShareReportGrid"}
        isLoading={isFetching}
        maxHeight={gridMaxHeight}
        handleSortChanged={handleSortChanged}
        providedOptions={{
          rowData: data,
          columnDefs: columnDefs
        }}
      />
      {data.length > 0 && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => {
            handlePaginationChange(value - 1, pageSize);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            handlePaginationChange(0, value);
          }}
          recordCount={recordCount}
          rowsPerPageOptions={[5, 10, 25, 50, 100]}
        />
      )}
    </div>
  );
};

export default ProfitShareReportGrid;
