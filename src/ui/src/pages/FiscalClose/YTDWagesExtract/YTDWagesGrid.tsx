import { useMemo } from "react";
import { DSMGrid, Pagination } from "smart-ui-library";

import { RefObject } from "react";
import ReportSummary from "../../../components/ReportSummary";
import { useDynamicGridHeight } from "../../../hooks/useDynamicGridHeight";
import { GridPaginationActions, GridPaginationState, SortParams } from "../../../hooks/useGridPagination";
import { EmployeeWagesForYearResponse } from "../../../reduxstore/types";
import { GetYTDWagesColumns } from "./YTDWagesGridColumns";

interface YTDWagesGridProps {
  innerRef: RefObject<HTMLDivElement | null>;
  data: EmployeeWagesForYearResponse | null;
  isLoading: boolean;
  showData: boolean;
  hasResults: boolean;
  pagination: GridPaginationState & GridPaginationActions;
  onPaginationChange: (pageNumber: number, pageSize: number) => void;
  onSortChange: (sortParams: SortParams) => void;
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
  // I need to clone the data object, but alter the reportName. I need to keep the year from the end
  // of the existing report name, but add "YTD Wages Extract " to the front.
  const clonedData = data ? ({ ...data } as EmployeeWagesForYearResponse) : null;
  const year = clonedData?.reportName?.match(/\d{4}$/)?.[0];
  if (clonedData && year) {
    clonedData.reportName = `YTD Wages Extract ${year}`;
  }

  const columnDefs = useMemo(() => GetYTDWagesColumns(), []);

  // Use dynamic grid height utility hook
  const gridMaxHeight = useDynamicGridHeight();

  return (
    <div className="relative">
      {showData && clonedData?.response && (
        <div ref={innerRef}>
          <ReportSummary report={clonedData} />
          <DSMGrid
            preferenceKey={"TERM"}
            isLoading={isLoading}
            maxHeight={gridMaxHeight}
            handleSortChanged={onSortChange}
            providedOptions={{
              rowData: clonedData.response.results,
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
    </div>
  );
};

export default YTDWagesGrid;
