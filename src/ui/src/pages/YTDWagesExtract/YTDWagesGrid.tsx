import { useMemo } from "react";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";

import { RefObject } from "react";
import ReportSummary from "../../components/ReportSummary";
import { EmployeeWagesForYearResponse } from "../../reduxstore/types";
import { GetYTDWagesColumns } from "./YTDWagesGridColumns";
import { PaginationState } from "./hooks/useYTDWagesReducer";

interface YTDWagesGridProps {
  innerRef: RefObject<HTMLDivElement | null>;
  data: EmployeeWagesForYearResponse | null;
  isLoading: boolean;
  showData: boolean;
  hasResults: boolean;
  pagination: PaginationState;
  onPaginationChange: (pageNumber: number, pageSize: number, sortParams: ISortParams) => void;
  onSortChange: (sortParams: ISortParams) => void;
}

const YTDWagesGrid = ({
  innerRef,
  data,
  isLoading,
  showData,
  hasResults,
  pagination,
  onPaginationChange,
  onSortChange
}: YTDWagesGridProps) => {
  const columnDefs = useMemo(() => GetYTDWagesColumns(), []);

  const handlePaginationChange = (pageNumber: number) => {
    // Pagination component sends 1-based page number, convert to 0-based
    onPaginationChange(pageNumber - 1, pagination.pageSize, pagination.sortParams);
  };

  const handlePageSizeChange = (pageSize: number) => {
    // When page size changes, reset to page 0 with new page size
    onPaginationChange(0, pageSize, pagination.sortParams);
  };

  return (
    <>
      {showData && data?.response && (
        <div ref={innerRef}>
          <ReportSummary report={data} />
          <DSMGrid
            preferenceKey={"TERM"}
            isLoading={isLoading}
            handleSortChanged={onSortChange}
            providedOptions={{
              rowData: data.response.results,
              columnDefs: columnDefs
            }}
          />
        </div>
      )}
      {hasResults && data?.response && (
        <>
          <Pagination
            rowsPerPageOptions={[5, 10, 25, 50, 100]}
            pageNumber={pagination.pageNumber}
            setPageNumber={handlePaginationChange}
            pageSize={pagination.pageSize}
            setPageSize={handlePageSizeChange}
            recordCount={data.response.total || 0}
          />
        </>
      )}
    </>
  );
};

export default YTDWagesGrid;
