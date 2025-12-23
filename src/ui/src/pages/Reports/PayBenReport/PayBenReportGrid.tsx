import { RefObject, useCallback, useMemo } from "react";
import { ISortParams } from "smart-ui-library";
import { DSMPaginatedGrid } from "../../../components/DSMPaginatedGrid/DSMPaginatedGrid";
import { GRID_KEYS } from "../../../constants";
import { useContentAwareGridHeight } from "../../../hooks/useContentAwareGridHeight";
import { GridPaginationActions, GridPaginationState, SortParams } from "../../../hooks/useGridPagination";
import { PayBenReportResponse } from "../../../types";
import { PayBenReportGridColumn } from "./PayBenReportGridColumns";

interface PayBenReportGridProps {
  innerRef: RefObject<HTMLDivElement | null>;
  data: PayBenReportResponse | null;
  isLoading: boolean;
  showData: boolean;
  hasResults: boolean;
  pagination: GridPaginationState & GridPaginationActions;
  onPaginationChange: (pageNumber: number, pageSize: number) => void;
  onSortChange: (sortParams: SortParams) => void;
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

  // Use content-aware grid height utility hook
  const gridMaxHeight = useContentAwareGridHeight({
    rowCount: data?.results?.length ?? 0
  });

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

  const paginationProps = {
    pageNumber: pagination.pageNumber,
    pageSize: pagination.pageSize,
    sortParams: pagination.sortParams,
    handlePageNumberChange: (value: number) => onPaginationChange(value, pagination.pageSize),
    handlePageSizeChange: (value: number) => onPaginationChange(0, value),
    handleSortChange: onSortChange
  };

  if (!showData || !data?.results) {
    return null;
  }

  return (
    <div
      className="relative"
      ref={innerRef}>
      <DSMPaginatedGrid
        preferenceKey={GRID_KEYS.PAY_BEN_REPORT}
        data={data.results}
        columnDefs={columnDefs}
        totalRecords={data.total || 0}
        isLoading={isLoading}
        pagination={paginationProps}
        onSortChange={handleSortChanged}
        heightConfig={{
          mode: "content-aware",
          maxHeight: gridMaxHeight
        }}
        gridOptions={{
          suppressMultiSort: true
        }}
        showPagination={hasResults}
      />
    </div>
  );
};

export default PayBenReportGrid;
