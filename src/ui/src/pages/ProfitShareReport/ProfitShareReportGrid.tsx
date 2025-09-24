import { useMemo } from "react";
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

  return (
    <>
      <DSMGrid
        preferenceKey={"ProfitShareReportGrid"}
        isLoading={isLoading}
        handleSortChanged={onSortChange}
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
