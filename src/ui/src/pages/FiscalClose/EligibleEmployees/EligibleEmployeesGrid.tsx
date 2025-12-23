import { RefObject, useMemo } from "react";
import { DSMPaginatedGrid } from "../../../components/DSMPaginatedGrid";
import ReportSummary from "../../../components/ReportSummary";
import { GRID_KEYS } from "../../../constants";
import { GridPaginationActions, GridPaginationState, SortParams } from "../../../hooks/useGridPagination";
import { EligibleEmployeeResponseDto } from "../../../reduxstore/types";
import { GetEligibleEmployeesColumns } from "./EligibleEmployeesGridColumns";

interface EligibleEmployeesGridProps {
  innerRef: RefObject<HTMLDivElement | null>;
  data: EligibleEmployeeResponseDto | null;
  isLoading: boolean;
  showData: boolean;
  hasResults: boolean;
  pagination: GridPaginationState & GridPaginationActions;
  onPaginationChange: (pageNumber: number, pageSize: number) => void;
  onSortChange: (sortParams: SortParams) => void;
}

const EligibleEmployeesGrid = ({
  innerRef,
  data,
  isLoading,
  showData,
  hasResults,
  pagination,
  onPaginationChange,
  onSortChange
}: EligibleEmployeesGridProps) => {
  // Clone the data object and modify the reportName to add "Eligible Employees " prefix
  const clonedData = data ? ({ ...data } as EligibleEmployeeResponseDto) : null;
  const year = clonedData?.reportName?.match(/\d{4}$/)?.[0];
  if (clonedData && year) {
    clonedData.reportName = `Eligible Employees ${year}`;
  }

  const columnDefs = useMemo(() => GetEligibleEmployeesColumns(), []);

  // Create a wrapper pagination object that maps the onPaginationChange callback
  const wrappedPagination = useMemo(
    () => ({
      ...pagination,
      handlePageNumberChange: (pageNumber: number) => onPaginationChange(pageNumber, pagination.pageSize),
      handlePageSizeChange: (pageSize: number) => onPaginationChange(0, pageSize)
    }),
    [pagination, onPaginationChange]
  );

  // Don't render if we shouldn't show data
  if (!showData || !clonedData?.response) {
    return null;
  }

  return (
    <DSMPaginatedGrid
      innerRef={innerRef}
      preferenceKey={GRID_KEYS.ELIGIBLE_EMPLOYEES}
      data={clonedData.response.results}
      columnDefs={columnDefs}
      totalRecords={clonedData.response.total || 0}
      isLoading={isLoading}
      pagination={wrappedPagination}
      onSortChange={onSortChange}
      showPagination={hasResults}
      beforeGrid={<ReportSummary report={clonedData} />}
    />
  );
};

export default EligibleEmployeesGrid;
