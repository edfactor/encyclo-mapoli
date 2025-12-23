import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo } from "react";
import { useSelector } from "react-redux";
import { useLazyGetHistoricalFrozenStateResponseQuery } from "reduxstore/api/ItOperationsApi";
import { RootState } from "reduxstore/store";
import { DSMPaginatedGrid } from "../../../components/DSMPaginatedGrid";
import { GRID_KEYS } from "../../../constants";
import { SortParams, useGridPagination } from "../../../hooks/useGridPagination";
import { FrozenStateResponse } from "../../../types";
import { GetFreezeColumns } from "./DemographicFreezeGridColumns";

interface DemoFreezeSearchProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  pageNumberReset: boolean;
  setPageNumberReset: (reset: boolean) => void;
}

const DemographicFreeze: React.FC<DemoFreezeSearchProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
  pageNumberReset,
  setPageNumberReset
}) => {
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const freezeResults = useSelector((state: RootState) => state.frozen.frozenStateCollectionData);
  const [triggerSearch, { isFetching }] = useLazyGetHistoricalFrozenStateResponseQuery();

  const pagination = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "createdDateTime",
    initialSortDescending: true,
    persistenceKey: GRID_KEYS.DEMOGRAPHIC_FREEZE,
    onPaginationChange: useCallback(
      (pageNum: number, pageSz: number, sortPrms: SortParams) => {
        if (initialSearchLoaded) {
          // Trigger search when pagination or sorting changes
          const request = {
            skip: pageNum * pageSz,
            take: pageSz,
            sortBy: sortPrms.sortBy,
            isSortDescending: sortPrms.isSortDescending
          };
          triggerSearch(request, false);
        }
      },
      [initialSearchLoaded, triggerSearch]
    )
  });

  const { pageNumber, pageSize, sortParams, handlePageNumberChange, handlePageSizeChange, resetPagination } = pagination;

  const onSearch = useCallback(async () => {
    const request = {
      skip: pageNumber * pageSize,
      take: pageSize,
      sortBy: sortParams.sortBy,
      isSortDescending: sortParams.isSortDescending
    };

    await triggerSearch(request, false);
  }, [pageNumber, pageSize, sortParams, triggerSearch]);

  // First useEffect to trigger the search on initial render
  useEffect(() => {
    if (!hasToken) return;
    onSearch();
  }, [onSearch, hasToken]);

  const columnDefs = useMemo(() => GetFreezeColumns(), []);

  useEffect(() => {
    if (pageNumberReset) {
      resetPagination();
      setPageNumberReset(false);
    }
  }, [pageNumberReset, setPageNumberReset, resetPagination]);

  // Wrap handlers to include side effects
  const handlePageNumberChangeWithCallback = useCallback(
    (value: number) => {
      handlePageNumberChange(value);
      setInitialSearchLoaded(true);
    },
    [handlePageNumberChange, setInitialSearchLoaded]
  );

  const handlePageSizeChangeWithCallback = useCallback(
    (value: number) => {
      handlePageSizeChange(value);
      setInitialSearchLoaded(true);
    },
    [handlePageSizeChange, setInitialSearchLoaded]
  );

  if (!freezeResults) {
    return null;
  }

  return (
    <DSMPaginatedGrid<FrozenStateResponse>
      preferenceKey={GRID_KEYS.DEMOGRAPHIC_FREEZE}
      data={freezeResults.results}
      columnDefs={columnDefs}
      totalRecords={freezeResults.total}
      isLoading={isFetching}
      pagination={{
        ...pagination,
        handlePageNumberChange: handlePageNumberChangeWithCallback,
        handlePageSizeChange: handlePageSizeChangeWithCallback
      }}
      onSortChange={pagination.handleSortChange}
      showPagination={freezeResults.results.length > 0}
      header={
        <Typography variant="h2" sx={{ color: "#0258A5" }}>
          Previous Freezes
        </Typography>
      }
      slotClassNames={{ headerClassName: "px-6" }}
    />
  );
};

export default DemographicFreeze;
