import { Typography } from "@mui/material";
import { useCallback, useMemo } from "react";
import { DSMGrid, Pagination } from "smart-ui-library";
import { useContentAwareGridHeight } from "../../../hooks/useContentAwareGridHeight";
import { GRID_KEYS } from "../../../constants";
import { SortParams, useGridPagination } from "../../../hooks/useGridPagination";
import { DistributionSearchResponse } from "../../../types";
import { GetDisbursementGridColumns } from "./DisbursementGridColumns";

interface DisbursementGridProps {
  title: string;
  data: DistributionSearchResponse[] | null;
  totalRecords: number;
  isLoading: boolean;
  initialPageSize?: number;
  rowsPerPageOptions?: number[];
  onPaginationChange: (pageNumber: number, pageSize: number, sortParams: SortParams) => void;
}

const DisbursementGrid: React.FC<DisbursementGridProps> = ({
  title,
  data,
  totalRecords,
  isLoading,
  initialPageSize = 25,
  rowsPerPageOptions = [10, 25, 50, 100],
  onPaginationChange
}) => {
  const { pageNumber, pageSize, handlePageNumberChange, handlePageSizeChange, handleSortChange } = useGridPagination({
    initialPageSize,
    initialSortBy: "paymentSequence",
    initialSortDescending: false,
    persistenceKey: `${GRID_KEYS.DISBURSEMENT_PREFIX}${title}`,
    onPaginationChange: useCallback(
      async (pageNum: number, pageSz: number, sortPrms: SortParams) => {
        onPaginationChange(pageNum, pageSz, sortPrms);
      },
      [onPaginationChange]
    )
  });

  const columnDefs = useMemo(() => GetDisbursementGridColumns(), []);

  // Use content-aware grid height utility hook
  const gridMaxHeight = useContentAwareGridHeight({
    rowCount: data?.length ?? 0
  });

  return (
    <>
      <div style={{ padding: "0px 24px 0px 24px" }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5", marginBottom: "8px" }}>
          {`${title} (${totalRecords} ${totalRecords === 1 ? "Record" : "Records"})`}
        </Typography>
      </div>

      <DSMGrid
        preferenceKey={`${GRID_KEYS.DISBURSEMENT_PREFIX}${title}`}
        isLoading={isLoading}
        handleSortChanged={handleSortChange}
        maxHeight={gridMaxHeight}
        providedOptions={{
          rowData: data,
          columnDefs: columnDefs,
          suppressMultiSort: true
        }}
      />

      {data && data.length > 0 && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => {
            handlePageNumberChange(value - 1);
          }}
          pageSize={pageSize}
          setPageSize={handlePageSizeChange}
          rowsPerPageOptions={rowsPerPageOptions}
          recordCount={totalRecords}
        />
      )}
    </>
  );
};

export default DisbursementGrid;
