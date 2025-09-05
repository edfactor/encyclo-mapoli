import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import React, { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetRecentlyTerminatedReportQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import "./RecentlyTerminatedGrid.css";
import { GetRecentlyTerminatedColumns } from "./RecentlyTerminatedGridColumns";

interface RecentlyTerminatedGridSearchProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const RecentlyTerminatedGrid: React.FC<RecentlyTerminatedGridSearchProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded
}) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "fullName, terminationDate",
    isSortDescending: false
  });

  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const { recentlyTerminated, recentlyTerminatedQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const profitYear = useDecemberFlowProfitYear();
  const [triggerSearch, { isFetching }] = useLazyGetRecentlyTerminatedReportQuery();

  const onSearch = useCallback(async () => {
    const request: any = {
      profitYear: profitYear || 0,
      pagination: {
        skip: pageNumber * pageSize,
        take: pageSize,
        sortBy: sortParams.sortBy,
        isSortDescending: sortParams.isSortDescending
      }
    };

    if (recentlyTerminatedQueryParams?.beginningDate !== undefined) {
      request.beginningDate = recentlyTerminatedQueryParams.beginningDate;
    }
    if (recentlyTerminatedQueryParams?.endingDate !== undefined) {
      request.endingDate = recentlyTerminatedQueryParams.endingDate;
    }

    await triggerSearch(request, false);
  }, [
    recentlyTerminatedQueryParams?.endingDate,
    recentlyTerminatedQueryParams?.beginningDate,
    profitYear,
    pageNumber,
    pageSize,
    sortParams,
    triggerSearch
  ]);

  // Need a useEffect on a change in RecentlyTerminated to reset the page number
  const prevRecentlyTerminated = useRef<any>(null);
  useEffect(() => {
    if (
      recentlyTerminated !== prevRecentlyTerminated.current &&
      recentlyTerminated?.response?.results &&
      recentlyTerminated.response.results.length !== prevRecentlyTerminated.current?.response?.results?.length
    ) {
      setPageNumber(0);
    }
    prevRecentlyTerminated.current = recentlyTerminated;
  }, [recentlyTerminated]);

  useEffect(() => {
    if (hasToken && initialSearchLoaded) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, sortParams, onSearch, hasToken, setInitialSearchLoaded]);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
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
            setPageNumber(value - 1);
            setInitialSearchLoaded(true);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            setPageSize(value);
            setPageNumber(1);
            setInitialSearchLoaded(true);
          }}
          recordCount={recentlyTerminated.response.total}
        />
      )}
    </>
  );
};

export default RecentlyTerminatedGrid;
