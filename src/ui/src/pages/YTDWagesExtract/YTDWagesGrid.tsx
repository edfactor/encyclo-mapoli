import { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";

import { RefObject } from "react";
import { useLazyGetEmployeeWagesForYearQuery } from "reduxstore/api/YearsEndApi";
import ReportSummary from "../../components/ReportSummary";
import useFiscalCloseProfitYear from "../../hooks/useFiscalCloseProfitYear";

interface YTDWagesGridProps {
  innerRef: RefObject<HTMLDivElement | null>;
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const YTDWagesGrid = ({ innerRef, initialSearchLoaded, setInitialSearchLoaded }: YTDWagesGridProps) => {
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "storeNumber",
    isSortDescending: false
  });
  const fiscalCloseProfitYear = useFiscalCloseProfitYear();
  const [triggerSearch, { isFetching }] = useLazyGetEmployeeWagesForYearQuery();
  const { employeeWagesForYearQueryParams } = useSelector((state: RootState) => state.yearsEnd);

  const onSearch = useCallback(async () => {
    const request = {
      profitYear: employeeWagesForYearQueryParams?.profitYear ?? fiscalCloseProfitYear,
      pagination: {
        skip: pageNumber * pageSize,
        take: pageSize,
        sortBy: sortParams.sortBy,
        isSortDescending: sortParams.isSortDescending
      },
      acceptHeader: "application/json"
    };

    await triggerSearch(request, false);
  }, [employeeWagesForYearQueryParams?.profitYear, fiscalCloseProfitYear, pageNumber, pageSize, sortParams.sortBy, sortParams.isSortDescending, triggerSearch]);

  useEffect(() => {
    if (initialSearchLoaded && hasToken) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, sortParams, onSearch, hasToken]);

  const { employeeWagesForYear } = useSelector((state: RootState) => state.yearsEnd);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetYTDWagesColumns(), []);

  return (
    <>
      {employeeWagesForYear?.response && (
        <div ref={innerRef}>
          <ReportSummary report={employeeWagesForYear} />
          <DSMGrid
            preferenceKey={"TERM"}
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: employeeWagesForYear?.response.results,
              columnDefs: columnDefs
            }}
          />
        </div>
      )}
      {/* We need to check the response also because if the user asked for a CSV, this variable will exist, but have a blob in it instead of a response */}
      {!!employeeWagesForYear && employeeWagesForYear.response && employeeWagesForYear.response.results.length > 0 && (
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
          recordCount={employeeWagesForYear.response.total}
        />
      )}
    </>
  );
};

export default YTDWagesGrid;
