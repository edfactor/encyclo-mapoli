import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import React, { useCallback, useEffect, useMemo, useRef } from "react";
import { useSelector } from "react-redux";
import { useLazyGetRecentlyTerminatedReportQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { SortParams, useGridPagination } from "../../../hooks/useGridPagination";
import { GetRecentlyTerminatedColumns } from "./RecentlyTerminatedGridColumns";

interface RecentlyTerminatedRequest {
  profitYear: number;
  pagination: {
    skip: number;
    take: number;
    sortBy: string;
    isSortDescending: boolean;
  };
  beginningDate?: string;
  endingDate?: string;
}

interface RecentlyTerminatedGridSearchProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const RecentlyTerminatedGrid: React.FC<RecentlyTerminatedGridSearchProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded
}) => {
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const { recentlyTerminated, recentlyTerminatedQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const profitYear = useDecemberFlowProfitYear();
  const [triggerSearch, { isFetching }] = useLazyGetRecentlyTerminatedReportQuery();

  const { pageNumber, pageSize, handlePaginationChange, handleSortChange, resetPagination } = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "fullName, terminationDate",
    initialSortDescending: false,
    onPaginationChange: useCallback(
      async (pageNum: number, pageSz: number, sortPrms: SortParams) => {
        if (hasToken && initialSearchLoaded && recentlyTerminatedQueryParams?.beginningDate && recentlyTerminatedQueryParams?.endingDate) {
          const request: RecentlyTerminatedRequest = {
            profitYear: profitYear || 0,
            beginningDate: recentlyTerminatedQueryParams.beginningDate,
            endingDate: recentlyTerminatedQueryParams.endingDate,
            pagination: {
              skip: pageNum * pageSz,
              take: pageSz,
              sortBy: sortPrms.sortBy,
              isSortDescending: sortPrms.isSortDescending
            }
          };

          await triggerSearch(request, false);
        }
      },
      [
        hasToken,
        initialSearchLoaded,
        profitYear,
        recentlyTerminatedQueryParams?.beginningDate,
        recentlyTerminatedQueryParams?.endingDate,
        triggerSearch
      ]
    )
  });

  // Need a useEffect on a change in RecentlyTerminated to reset the page number
  const prevRecentlyTerminated = useRef<typeof recentlyTerminated | null>(null);
  useEffect(() => {
    if (
      recentlyTerminated !== prevRecentlyTerminated.current &&
      recentlyTerminated?.response?.results &&
      recentlyTerminated.response.results.length !== prevRecentlyTerminated.current?.response?.results?.length
    ) {
      resetPagination();
    }
    prevRecentlyTerminated.current = recentlyTerminated;
  }, [recentlyTerminated, resetPagination]);

  const sortEventHandler = (update: ISortParams) => handleSortChange(update);
  const columnDefs = useMemo(() => GetRecentlyTerminatedColumns(), []);

  return (
    <>
      {recentlyTerminated?.response && (
        <>
          <DSMGrid
            preferenceKey={CAPTIONS.DISTRIBUTIONS_AND_FORFEITURES}
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: recentlyTerminated?.response.results,
              columnDefs: columnDefs,
              suppressMultiSort: true
            }}
          />
        </>
      )}
      {!!recentlyTerminated && recentlyTerminated.response.results.length > 0 && (
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
          recordCount={recentlyTerminated.response.total}
        />
      )}
    </>
  );
};

export default RecentlyTerminatedGrid;
