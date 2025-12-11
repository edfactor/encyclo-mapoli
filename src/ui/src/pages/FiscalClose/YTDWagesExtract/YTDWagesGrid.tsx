import { useMemo } from "react";
import { DSMGrid, numberToCurrency, Pagination } from "smart-ui-library";

import { RefObject } from "react";
import { RowClassParams } from "ag-grid-community";
import ReportSummary from "../../../components/ReportSummary";
import { useContentAwareGridHeight } from "../../../hooks/useContentAwareGridHeight";
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

  // Calculate totals for pinned top row - use API totals if available, otherwise calculate
  const totalsRow = useMemo(() => {
    if (!clonedData?.response?.results) return null;

    // Check if API provided totals
    const totalHours =
      clonedData.totalHoursCurrentYearWages ??
      clonedData.response.results.reduce((sum, row) => sum + (row.hoursCurrentYear || 0), 0);

    const totalIncome =
      clonedData.totalIncomeCurrentYearWages ??
      clonedData.response.results.reduce((sum, row) => sum + (row.incomeCurrentYear || 0), 0);

    return {
      badgeNumber: "TOTALS",
      hoursCurrentYear: totalHours,
      incomeCurrentYear: totalIncome,
      isExecutive: false,
      storeNumber: 0
    };
  }, [clonedData?.response?.results, clonedData?.totalHoursCurrentYearWages, clonedData?.totalIncomeCurrentYearWages]);

  // Use content-aware grid height utility hook
  const gridMaxHeight = useContentAwareGridHeight({
    rowCount: clonedData?.response?.results?.length ?? 0
  });

  return (
    <div className="relative">
      {showData && clonedData?.response && (
        <div ref={innerRef}>
          <div className="mb-[21px] mt-[37px] flex items-center gap-6 px-6">
            <div className="flex items-center gap-2">
              <span className="font-semibold">Total Hours:</span>
              <span>
                {clonedData.totalHoursCurrentYearWages?.toFixed(2) ?? totalsRow?.hoursCurrentYear.toFixed(2) ?? "0.00"}
              </span>
            </div>
            <div className="flex items-center gap-2">
              <span className="font-semibold">Total Income:</span>
              <span>
                {numberToCurrency(clonedData.totalIncomeCurrentYearWages ?? totalsRow?.incomeCurrentYear ?? 0)}
              </span>
            </div>
          </div>
          <ReportSummary report={clonedData} />
          <DSMGrid
            preferenceKey={"TERM"}
            isLoading={isLoading}
            maxHeight={gridMaxHeight}
            handleSortChanged={onSortChange}
            providedOptions={{
              rowData: clonedData.response.results,
              pinnedTopRowData: totalsRow ? [totalsRow] : [],
              columnDefs: columnDefs,
              getRowStyle: (params: RowClassParams) => {
                if (params.node.rowPinned) {
                  return { background: "#f0f0f0", fontWeight: "bold" };
                }
                return undefined;
              }
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
