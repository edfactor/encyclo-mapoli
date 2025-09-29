import { useCallback, useMemo } from "react";
import { DSMGrid, Pagination } from "smart-ui-library";
import { useGridPagination } from "../../hooks/useGridPagination";
import { GetProfitShareReportColumns } from "./ProfitShareReportGridColumns";

interface ProfitShareReportGridProps {
  data: any[];
  isLoading: boolean;
  recordCount: number;
  onPageChange?: (pageNum: number, pageSz: number, sortPrms: any) => void;
  onSortChange?: (update: any) => void;
}

const ProfitShareReportGrid: React.FC<ProfitShareReportGridProps> = ({
  data,
  isLoading,
  recordCount,
  onPageChange,
  onSortChange
}) => {
  const columnDefs = useMemo(() => GetProfitShareReportColumns(), []);

  const { pageNumber, pageSize, handlePaginationChange, handleSortChange } = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "badgeNumber",
    initialSortDescending: false,
    onPaginationChange: useCallback((pageNum: number, pageSz: number, sortPrms: any) => {
      if (onPageChange) {
        onPageChange(pageNum, pageSz, sortPrms);
      }
    }, [onPageChange])
  });

  const handleSortChanged = useCallback(
    (update: any) => {
      // Handle empty sortBy case - set default to badgeNumber
      if (update.sortBy === "") {
        update.sortBy = "badgeNumber";
        update.isSortDescending = false;
      }

      handleSortChange(update);
      if (onSortChange) {
        onSortChange(update);
      }
    },
    [handleSortChange, onSortChange]
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
            handlePaginationChange(value - 1, pageSize);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            handlePaginationChange(0, value);
          }}
          recordCount={recordCount}
        />
      )}
    </>
  );
};

export default ProfitShareReportGrid;
