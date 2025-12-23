import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo } from "react";
import { useSelector } from "react-redux";
import { useLazyGetProfitShareEditQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { ProfitShareUpdateRequest } from "reduxstore/types";
import { DSMPaginatedGrid } from "../../../components/DSMPaginatedGrid/DSMPaginatedGrid";
import { GRID_KEYS } from "../../../constants";
import { useContentAwareGridHeight } from "../../../hooks/useContentAwareGridHeight";
import { SortParams, useGridPagination } from "../../../hooks/useGridPagination";
import { ProfitShareEditUpdateGridColumns } from "./ProfitShareEditGridColumns";

interface ProfitShareEditGridProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  pageNumberReset: boolean;
  setPageNumberReset: (reset: boolean) => void;
}

const ProfitShareEditGrid = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
  pageNumberReset,
  setPageNumberReset
}: ProfitShareEditGridProps) => {
  const hasToken = !!useSelector((state: RootState) => state.security.token);

  const editColumnDefs = useMemo(() => ProfitShareEditUpdateGridColumns(), []);
  const { profitSharingEdit, profitSharingEditQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearchUpdate, { isFetching }] = useLazyGetProfitShareEditQuery();

  // Use content-aware grid height utility hook
  const gridMaxHeight = useContentAwareGridHeight({
    rowCount: profitSharingEdit?.response?.results?.length ?? 0
  });

  const {
    pageNumber,
    pageSize,
    sortParams,
    handlePageNumberChange,
    handlePageSizeChange,
    handleSortChange,
    resetPagination
  } = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "name",
    initialSortDescending: false,
    persistenceKey: GRID_KEYS.PROFIT_SHARE_EDIT,
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
            profitYear: profitSharingEditQueryParams?.profitYear.getFullYear() ?? 0,
            contributionPercent: profitSharingEditQueryParams?.contributionPercent ?? 0,
            earningsPercent: profitSharingEditQueryParams?.earningsPercent ?? 0,
            incomingForfeitPercent: profitSharingEditQueryParams?.incomingForfeitPercent ?? 0,
            secondaryEarningsPercent: profitSharingEditQueryParams?.secondaryEarningsPercent ?? 0,
            maxAllowedContributions: profitSharingEditQueryParams?.maxAllowedContributions ?? 0,
            badgeToAdjust: profitSharingEditQueryParams?.badgeToAdjust ?? 0,
            adjustContributionAmount: profitSharingEditQueryParams?.adjustContributionAmount ?? 0,
            adjustEarningsAmount: profitSharingEditQueryParams?.adjustEarningsAmount ?? 0,
            adjustIncomingForfeitAmount: profitSharingEditQueryParams?.adjustEarningsSecondaryAmount ?? 0,
            badgeToAdjust2: profitSharingEditQueryParams?.badgeToAdjust2 ?? 0,
            adjustEarningsSecondaryAmount: profitSharingEditQueryParams?.adjustEarningsSecondaryAmount ?? 0
          };
          await triggerSearchUpdate(request, false);
        }
      },
      [initialSearchLoaded, hasToken, profitSharingEditQueryParams, triggerSearchUpdate]
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
      profitYear: profitSharingEditQueryParams?.profitYear.getFullYear() ?? 0,
      contributionPercent: profitSharingEditQueryParams?.contributionPercent ?? 0,
      earningsPercent: profitSharingEditQueryParams?.earningsPercent ?? 0,
      incomingForfeitPercent: profitSharingEditQueryParams?.incomingForfeitPercent ?? 0,
      secondaryEarningsPercent: profitSharingEditQueryParams?.secondaryEarningsPercent ?? 0,
      maxAllowedContributions: profitSharingEditQueryParams?.maxAllowedContributions ?? 0,
      badgeToAdjust: profitSharingEditQueryParams?.badgeToAdjust ?? 0,
      adjustContributionAmount: profitSharingEditQueryParams?.adjustContributionAmount ?? 0,
      adjustEarningsAmount: profitSharingEditQueryParams?.adjustEarningsAmount ?? 0,
      adjustIncomingForfeitAmount: profitSharingEditQueryParams?.adjustEarningsSecondaryAmount ?? 0,
      badgeToAdjust2: profitSharingEditQueryParams?.badgeToAdjust2 ?? 0,
      adjustEarningsSecondaryAmount: profitSharingEditQueryParams?.adjustEarningsSecondaryAmount ?? 0
    };

    await triggerSearchUpdate(request, false);
  }, [pageNumber, pageSize, sortParams, triggerSearchUpdate, profitSharingEditQueryParams]);

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
          {`Profit Share Edit (PAY447)`}
        </Typography>
      </div>
      {!!profitSharingEdit && (
        <DSMPaginatedGrid
          preferenceKey={GRID_KEYS.PROFIT_SHARE_EDIT}
          data={"response" in profitSharingEdit ? (profitSharingEdit.response?.results ?? []) : []}
          columnDefs={editColumnDefs}
          totalRecords={profitSharingEdit?.response.total ?? 0}
          isLoading={isFetching}
          heightConfig={{ maxHeight: gridMaxHeight }}
          pagination={{
            pageNumber,
            pageSize,
            sortParams,
            handlePageNumberChange: (value: number) => {
              handlePageNumberChange(value - 1);
              setInitialSearchLoaded(true);
            },
            handlePageSizeChange: (value: number) => {
              handlePageSizeChange(value);
              setInitialSearchLoaded(true);
            },
            handleSortChange,
          }}
          showPagination
        />
      )}
    </>
  );
};

export default ProfitShareEditGrid;
