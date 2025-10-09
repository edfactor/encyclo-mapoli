import { Typography } from "@mui/material";
import { useMemo, useCallback, useEffect } from "react";
import { useSelector } from "react-redux";
import { useLazyGetHistoricalFrozenStateResponseQuery } from "reduxstore/api/ItOperationsApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, Pagination } from "smart-ui-library";
import { GetFreezeColumns } from "./DemographicFreezeGridColumns";
import { useGridPagination } from "../../../hooks/useGridPagination";

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

  const { pageNumber, pageSize, sortParams, handlePaginationChange, handleSortChange, resetPagination } =
    useGridPagination({
      initialPageSize: 25,
      initialSortBy: "createdDateTime",
      initialSortDescending: true,
      onPaginationChange: useCallback(
        (pageNum: number, pageSz: number, sortPrms: any) => {
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

  return (
    <>
      {freezeResults && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`Previous Freezes`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"FREEZE"}
            isLoading={isFetching}
            handleSortChanged={handleSortChange}
            providedOptions={{
              rowData: freezeResults?.results,
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {!!freezeResults && freezeResults.results.length > 0 && (
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
          recordCount={freezeResults.total}
        />
      )}
    </>
  );
};

export default DemographicFreeze;
