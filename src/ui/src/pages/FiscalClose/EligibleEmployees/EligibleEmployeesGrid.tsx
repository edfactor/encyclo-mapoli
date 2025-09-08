import React, { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetEligibleEmployeesQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import ReportSummary from "../../../components/ReportSummary";
import { GetEligibleEmployeesColumns } from "./EligibleEmployeesGridColumns";

interface EligibleEmployeesGridProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const EligibleEmployeesGrid: React.FC<EligibleEmployeesGridProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded
}) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });

  const { eligibleEmployees, eligibleEmployeesQueryParams } = useSelector((state: RootState) => state.yearsEnd);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetEligibleEmployeesColumns(), []);

  const [triggerSearch, { isFetching }] = useLazyGetEligibleEmployeesQuery();

  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);

  const onSearch = useCallback(async () => {
    const request = {
      profitYear: eligibleEmployeesQueryParams?.profitYear ?? 0,
      pagination: {
        skip: pageNumber * pageSize,
        take: pageSize,
        sortBy: sortParams.sortBy,
        isSortDescending: sortParams.isSortDescending
      }
    };

    await triggerSearch(request, false);
  }, [
    eligibleEmployeesQueryParams?.profitYear,
    pageNumber,
    pageSize,
    sortParams.isSortDescending,
    sortParams.sortBy,
    triggerSearch
  ]);

  useEffect(() => {
    if (hasToken && initialSearchLoaded) {
      onSearch();
    }
  }, [hasToken, initialSearchLoaded, pageNumber, pageSize, sortParams, onSearch]);

  // Need a useEffect on a change in eligibleEmployees to reset the page number when total count changes (new search, not pagination)
  const prevEligibleEmployees = useRef<any>(null);
  useEffect(() => {
    if (
      eligibleEmployees !== prevEligibleEmployees.current &&
      eligibleEmployees?.response?.total !== undefined &&
      eligibleEmployees.response.total !== prevEligibleEmployees.current?.response?.total
    ) {
      setPageNumber(0);
    }
    prevEligibleEmployees.current = eligibleEmployees;
  }, [eligibleEmployees]);

  return (
    <>
      {eligibleEmployees?.response && (
        <>
          <ReportSummary report={eligibleEmployees} />
          <DSMGrid
            preferenceKey={"ELIGIBLE_EMPLOYEES"}
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: eligibleEmployees?.response.results,
              columnDefs: columnDefs,
              suppressMultiSort: true
            }}
          />
        </>
      )}
      {!!eligibleEmployees && eligibleEmployees.response.results.length > 0 && (
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
          recordCount={eligibleEmployees.response.total}
        />
      )}
    </>
  );
};

export default EligibleEmployeesGrid;
