import { RefObject, useMemo } from "react";
import { DSMGrid, Pagination } from "smart-ui-library";
import ReportSummary from "../../../components/ReportSummary";
import { EligibleEmployeesResponse } from "../../../reduxstore/types";
import { GetEligibleEmployeesColumns } from "./EligibleEmployeesGridColumns";
import { GridPaginationState, GridPaginationActions } from "../../../hooks/useGridPagination";

interface EligibleEmployeesGridProps {
  innerRef: RefObject<HTMLDivElement | null>;
  data: EligibleEmployeesResponse | null;
  isLoading: boolean;
  showData: boolean;
  hasResults: boolean;
  pagination: GridPaginationState & GridPaginationActions;
  onPaginationChange: (pageNumber: number, pageSize: number) => void;
  onSortChange: (sortParams: any) => void;
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
  const columnDefs = useMemo(() => GetEligibleEmployeesColumns(), []);

  return (
    <>
      {showData && data?.response && (
        <div ref={innerRef}>
          <ReportSummary report={data} />
          <DSMGrid
            preferenceKey={"ELIGIBLE_EMPLOYEES"}
            isLoading={isLoading}
            handleSortChanged={onSortChange}
            providedOptions={{
              rowData: data.response.results,
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
