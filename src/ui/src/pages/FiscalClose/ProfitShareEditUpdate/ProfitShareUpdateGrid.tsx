import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo } from "react";
import { useSelector } from "react-redux";
import { useLazyGetProfitShareUpdateQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { ProfitShareUpdateRequest } from "reduxstore/types";
import { DSMGrid, Pagination } from "smart-ui-library";
import { useContentAwareGridHeight } from "../../../hooks/useContentAwareGridHeight";
import { GRID_KEYS } from "../../../constants";
import { useGridPagination, SortParams } from "../../../hooks/useGridPagination";
import { ProfitShareUpdateGridColumns } from "./ProfitShareUpdateGridColumns";

interface ProfitShareEditUpdateGridProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  pageNumberReset: boolean;
  setPageNumberReset: (reset: boolean) => void;
}

const ProfitShareEditUpdateGrid = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
  pageNumberReset,
  setPageNumberReset
}: ProfitShareEditUpdateGridProps) => {
  const hasToken = !!useSelector((state: RootState) => state.security.token);

  const columnDefs = useMemo(() => ProfitShareUpdateGridColumns(), []);
  const { profitSharingUpdate, profitSharingUpdateQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearchUpdate, { isFetching }] = useLazyGetProfitShareUpdateQuery();

  // Use content-aware grid height utility hook
  const gridMaxHeight = useContentAwareGridHeight({
    rowCount: profitSharingUpdate?.response?.results?.length ?? 0
  });

  const { pageNumber, pageSize, sortParams, handlePaginationChange, handleSortChange, resetPagination } =
    useGridPagination({
      initialPageSize: 25,
      initialSortBy: "Name",
      initialSortDescending: false,
      persistenceKey: GRID_KEYS.PROFIT_SHARE_UPDATE,
      onPaginationChange: useCallback(
        async (pageNum: number, pageSz: number, sortPrms: SortParams) => {
          if (initialSearchLoaded && hasToken) {
            const request: ProfitShareUpdateRequest = {
              pagination: {
                sortBy: sortPrms.sortBy,
                isSortDescending: sortPrms.isSortDescending,
                skip: pageNum * pageSz,
                take: pageSz
              },
              profitYear: profitSharingUpdateQueryParams?.profitYear.getFullYear() ?? 0,
              contributionPercent: profitSharingUpdateQueryParams?.contributionPercent ?? 0,
              earningsPercent: profitSharingUpdateQueryParams?.earningsPercent ?? 0,
              incomingForfeitPercent: profitSharingUpdateQueryParams?.incomingForfeitPercent ?? 0,
              secondaryEarningsPercent: profitSharingUpdateQueryParams?.secondaryEarningsPercent ?? 0,
              maxAllowedContributions: profitSharingUpdateQueryParams?.maxAllowedContributions ?? 0,
              badgeToAdjust: profitSharingUpdateQueryParams?.badgeToAdjust ?? 0,
              adjustContributionAmount: profitSharingUpdateQueryParams?.adjustContributionAmount ?? 0,
              adjustEarningsAmount: profitSharingUpdateQueryParams?.adjustEarningsAmount ?? 0,
              adjustIncomingForfeitAmount: profitSharingUpdateQueryParams?.adjustIncomingForfeitAmount ?? 0,
              badgeToAdjust2: profitSharingUpdateQueryParams?.badgeToAdjust2 ?? 0,
              adjustEarningsSecondaryAmount: profitSharingUpdateQueryParams?.adjustEarningsSecondaryAmount ?? 0
            };
            await triggerSearchUpdate(request, false);
          }
        },
        [initialSearchLoaded, hasToken, profitSharingUpdateQueryParams, triggerSearchUpdate]
      )
    });
  const onSearch = useCallback(async () => {
    const request: ProfitShareUpdateRequest = {
      pagination: {
        sortBy: sortParams.sortBy,
        isSortDescending: sortParams.isSortDescending,
        skip: pageNumber * pageSize,
        take: pageSize
      },
      profitYear: profitSharingUpdateQueryParams?.profitYear.getFullYear() ?? 0,
      contributionPercent: profitSharingUpdateQueryParams?.contributionPercent ?? 0,
      earningsPercent: profitSharingUpdateQueryParams?.earningsPercent ?? 0,
      incomingForfeitPercent: profitSharingUpdateQueryParams?.incomingForfeitPercent ?? 0,
      secondaryEarningsPercent: profitSharingUpdateQueryParams?.secondaryEarningsPercent ?? 0,
      maxAllowedContributions: profitSharingUpdateQueryParams?.maxAllowedContributions ?? 0,
      badgeToAdjust: profitSharingUpdateQueryParams?.badgeToAdjust ?? 0,
      adjustContributionAmount: profitSharingUpdateQueryParams?.adjustContributionAmount ?? 0,
      adjustEarningsAmount: profitSharingUpdateQueryParams?.adjustEarningsAmount ?? 0,
      adjustIncomingForfeitAmount: profitSharingUpdateQueryParams?.adjustIncomingForfeitAmount ?? 0,
      badgeToAdjust2: profitSharingUpdateQueryParams?.badgeToAdjust2 ?? 0,
      adjustEarningsSecondaryAmount: profitSharingUpdateQueryParams?.adjustEarningsSecondaryAmount ?? 0
    };

    await triggerSearchUpdate(request, false);
  }, [pageNumber, pageSize, sortParams, triggerSearchUpdate, profitSharingUpdateQueryParams]);

  useEffect(() => {
    if (initialSearchLoaded && hasToken) {
      onSearch();
    }
  }, [initialSearchLoaded, onSearch, hasToken]);

  useEffect(() => {
    if (pageNumberReset) {
      resetPagination();
      setPageNumberReset(false);
    }
  }, [pageNumberReset, setPageNumberReset, resetPagination]);

  return (
    <>
      <div className="px-[24px]">
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          {`Profit Share Update (PAY444)`}
        </Typography>
      </div>
      {!!profitSharingUpdate && (
        <>
          <DSMGrid
            preferenceKey={GRID_KEYS.PROFIT_SHARE_UPDATE}
            isLoading={isFetching}
            handleSortChanged={handleSortChange}
            maxHeight={gridMaxHeight}
            providedOptions={{
              rowData: "response" in profitSharingUpdate ? profitSharingUpdate.response?.results : [],
              columnDefs: columnDefs
            }}
          />
          <Pagination
            pageNumber={pageNumber}
            setPageNumber={(value: number) => {
              handlePaginationChange(value - 1, pageSize);
              setInitialSearchLoaded(true);
            }}
            pageSize={pageSize}
            setPageSize={(value: number) => {
              handlePaginationChange(0, value);
              setInitialSearchLoaded(true);
            }}
            recordCount={profitSharingUpdate?.response.total ?? 0}
          />
        </>
      )}
    </>
  );
};

export default ProfitShareEditUpdateGrid;
