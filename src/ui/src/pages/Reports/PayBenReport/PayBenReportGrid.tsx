import { RefObject, useMemo } from "react";
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
  onSortChange: (sortParams: SortParams) => void;
}

const PayBenReportGrid = ({
  innerRef,
  data,
  isLoading,
  showData,
  hasResults,
  pagination,
  onSortChange
}: PayBenReportGridProps) => {
  const columnDefs = useMemo(() => PayBenReportGridColumn(), []);

  // Use content-aware grid height utility hook
  const gridMaxHeight = useContentAwareGridHeight({
    rowCount: data?.results?.length ?? 0
  });

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
        pagination={pagination}
        onSortChange={onSortChange}
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
