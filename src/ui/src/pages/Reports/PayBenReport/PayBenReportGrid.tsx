import { RefObject, useCallback, useMemo } from "react";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GridPaginationActions, GridPaginationState } from "../../../hooks/useGridPagination";
import { PayBenReportResponse } from "../../../types";
import { CAPTIONS } from "../../../constants";
import { PayBenReportGridColumn } from "./PayBenReportGridColumns";

interface PayBenReportGridProps {
  innerRef: RefObject<HTMLDivElement | null>;
  data: PayBenReportResponse | null;
  isLoading: boolean;
  showData: boolean;
  hasResults: boolean;
  pagination: GridPaginationState & GridPaginationActions;
  onPaginationChange: (pageNumber: number, pageSize: number) => void;
  onSortChange: (sortParams: any) => void;
}

const PayBenReportGrid = ({
  innerRef,
  data,
  isLoading,
  showData,
  hasResults,
  pagination,
  onPaginationChange,
  onSortChange
}: PayBenReportGridProps) => {
  const columnDefs = useMemo(() => PayBenReportGridColumn(), []);

  const handleSortChanged = useCallback(
    (update: ISortParams) => {
      // Handle empty sortBy case - set default (preserving original logic)
      if (update.sortBy === "") {
        update.sortBy = "ssn";
        update.isSortDescending = true;
      }

      // Reset to page 0 when sorting changes (preserving original logic)
      onPaginationChange(0, pagination.pageSize);
      onSortChange(update);
    },
    [onPaginationChange, onSortChange, pagination.pageSize]
  );

  return (
    <>
      {showData && data?.results && (
        <div ref={innerRef}>
          <DSMGrid
            preferenceKey={CAPTIONS.PAYBEN_REPORT}
            isLoading={isLoading}
            handleSortChanged={handleSortChanged}
            providedOptions={{
              rowData: data.results,
              columnDefs: columnDefs,
              suppressMultiSort: true,
              masterDetail: true
            }}
          />
        </div>
      )}
      {hasResults && data && (
        <Pagination
          pageNumber={pagination.pageNumber}
          setPageNumber={(value: number) => {
            onPaginationChange(value - 1, pagination.pageSize);
          }}
          pageSize={pagination.pageSize}
          setPageSize={(value: number) => {
            onPaginationChange(0, value);
          }}
          recordCount={data.total || 0}
        />
      )}
    </>
  );
};

export default PayBenReportGrid;
