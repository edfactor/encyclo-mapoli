import { useMemo } from "react";
import { DSMGrid, Pagination } from "smart-ui-library";

import { RefObject } from "react";
import ReportSummary from "../../components/ReportSummary";
import { EmployeeWagesForYearResponse } from "../../reduxstore/types";
import { GetYTDWagesColumns } from "./YTDWagesGridColumns";
import { GridPaginationState, GridPaginationActions } from "../../hooks/useGridPagination";

interface YTDWagesGridProps {
  innerRef: RefObject<HTMLDivElement | null>;
  data: EmployeeWagesForYearResponse | null;
  isLoading: boolean;
  showData: boolean;
  hasResults: boolean;
  pagination: GridPaginationState & GridPaginationActions;
  onPaginationChange: (pageNumber: number, pageSize: number) => void;
  onSortChange: (sortParams: any) => void;
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
        <Pagination
          pageNumber={pagination.pageNumber}
          setPageNumber={(value: number) => {
            onPaginationChange(value - 1, pagination.pageSize);
          }}
          pageSize={pagination.pageSize}
          setPageSize={(value: number) => {
            onPaginationChange(0, value);
          }}
          recordCount={data.response.total || 0}
        />
      )}
    </>
  );
};

export default YTDWagesGrid;
