import { Typography } from "@mui/material";
import { useCallback, useMemo, useState, useEffect } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import { GetYTDWagesColumns } from "./YTDWagesGridColumn";

import { RefObject } from "react";
import { useLazyGetEmployeeWagesForYearQuery } from "reduxstore/api/YearsEndApi";

interface YTDWagesGridProps {
  innerRef: RefObject<HTMLDivElement | null>;
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const YTDWagesGrid = ({ innerRef, initialSearchLoaded, setInitialSearchLoaded }: YTDWagesGridProps) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });
  const [triggerSearch, { isFetching }] = useLazyGetEmployeeWagesForYearQuery();
  const { employeeWagesForYearQueryParams } = useSelector((state: RootState) => state.yearsEnd);

  const onSearch = useCallback(async () => {
    const request = {
      profitYear: employeeWagesForYearQueryParams?.profitYear ?? 0,
      pagination: { skip: pageNumber * pageSize, take: pageSize, sort: sortParams.sortBy, isSortDescending: sortParams.isSortDescending },
      acceptHeader: "application/json"
    };

    await triggerSearch(request, false);
  }, [pageNumber, pageSize, sortParams, triggerSearch, employeeWagesForYearQueryParams?.profitYear]);

  useEffect(() => {
    if (initialSearchLoaded) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, sortParams, onSearch]);

  const { employeeWagesForYear } = useSelector((state: RootState) => state.yearsEnd);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetYTDWagesColumns(), []);

  return (
    <>
      {employeeWagesForYear?.response && (
        <div ref={innerRef}>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`${CAPTIONS.YTD_WAGES_EXTRACT} (${employeeWagesForYear.response.total || 0} records)`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"TERM"}
            isLoading={false}
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
