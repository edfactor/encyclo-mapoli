import { useCallback, useMemo } from "react";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetProfitShareReportColumns } from "./ProfitShareReportGridColumns";

interface ProfitShareReportGridProps {
  data: any[];
  isLoading: boolean;
  pageNumber: number;
  pageSize: number;
  recordCount: number;
  onPageChange: (value: number) => void;
  onPageSizeChange: (value: number) => void;
  onSortChange: (update: ISortParams) => void;
}

const ProfitShareReportGrid: React.FC<ProfitShareReportGridProps> = ({
  data,
  isLoading,
  pageNumber,
  pageSize,
  recordCount,
  onPageChange,
  onPageSizeChange,
  onSortChange
}) => {
  const columnDefs = useMemo(() => GetProfitShareReportColumns(), []);

  const handleSortChanged = useCallback(
    (update: ISortParams) => {
      // Handle empty sortBy case - set default to badgeNumber
      if (update.sortBy === "") {
        update.sortBy = "badgeNumber";
        update.isSortDescending = false;
      }

      // Reset to page 0 when sorting changes
      onPageChange(0);
      onSortChange(update);
    },
    [onPageChange, onSortChange]
  );

  return (
    <>
      <DSMGrid
        preferenceKey={"ProfitShareReportGrid"}
        isLoading={isLoading}
        handleSortChanged={handleSortChanged}
        providedOptions={{
          rowData: data,
          columnDefs: columnDefs
        }}
      />
      {data.length > 0 && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => {
            onPageChange(value - 1);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            onPageSizeChange(value);
          }}
          recordCount={recordCount}
        />
      )}
    </>
  );
};

export default ProfitShareReportGrid;
