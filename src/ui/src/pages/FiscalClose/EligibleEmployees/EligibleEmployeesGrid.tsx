import { RefObject, useMemo } from "react";
import { DSMGrid, Pagination } from "smart-ui-library";
import ReportSummary from "../../../components/ReportSummary";
import { GRID_KEYS } from "../../../constants";
import { useContentAwareGridHeight } from "../../../hooks/useContentAwareGridHeight";
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
  // I need to clone the data object, but alter the reportName. I need to keep the year from the end
  // of the existing report name, but add "Eligible Employees " to the front.
  const clonedData = data ? ({ ...data } as EligibleEmployeeResponseDto) : null;
  const year = clonedData?.reportName?.match(/\d{4}$/)?.[0];
  if (clonedData && year) {
    clonedData.reportName = `Eligible Employees ${year}`;
  }

  const columnDefs = useMemo(() => GetEligibleEmployeesColumns(), []);
  const gridMaxHeight = useContentAwareGridHeight({
    rowCount: clonedData?.response?.results?.length ?? 0
  });

  return (
    <>
      {showData && clonedData?.response && (
        <div ref={innerRef}>
          <ReportSummary report={clonedData} />
          <DSMGrid
            preferenceKey={GRID_KEYS.ELIGIBLE_EMPLOYEES}
            isLoading={isLoading}
            handleSortChanged={onSortChange}
            maxHeight={gridMaxHeight}
            providedOptions={{
              rowData: clonedData.response.results,
              columnDefs: columnDefs,
              suppressMultiSort: true
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

export default EligibleEmployeesGrid;
