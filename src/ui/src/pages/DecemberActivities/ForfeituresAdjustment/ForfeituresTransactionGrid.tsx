import { CircularProgress, Typography } from "@mui/material";
import { memo, useCallback, useMemo } from "react";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GRID_KEYS } from "../../../constants";
import { SortParams, useGridPagination } from "../../../hooks/useGridPagination";
import { GetForfeituresTransactionGridColumns } from "./ForfeituresTransactionGridColumns";

interface TransactionData {
  results: unknown[];
  total: number;
}

interface ForfeituresTransactionGridProps {
  transactionData: TransactionData | null;
  isLoading: boolean;
  pagination: ReturnType<typeof useGridPagination>;
  onPaginationChange: (pageNumber: number, pageSize: number, sortParams: SortParams) => void;
  onSortChange: (sortParams: SortParams) => void;
}

const ForfeituresTransactionGrid: React.FC<ForfeituresTransactionGridProps> = memo(
  ({ transactionData, isLoading, pagination, onPaginationChange, onSortChange }) => {
    const columnDefs = useMemo(() => GetForfeituresTransactionGridColumns(), []);

    const handleSortChangeInternal = useCallback(
      (sortParams: ISortParams) => {
        onSortChange(sortParams);
      },
      [onSortChange]
    );

    if (isLoading) {
      return <CircularProgress />;
    }

    const displayData = transactionData || { results: [], total: 0 };

    return (
      <>
        <div style={{ height: "400px", width: "100%" }}>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`Forfeit/Unforfeit Transactions (${displayData.total} ${displayData.total === 1 ? "Record" : "Records"})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={GRID_KEYS.FORFEITURES_ADJUSTMENT}
            handleSortChanged={handleSortChangeInternal}
            isLoading={!!isLoading}
            providedOptions={{
              rowData: displayData.results,
              columnDefs: columnDefs,
              suppressMultiSort: true,
              rowSelection: {
                mode: "multiRow",
                checkboxes: false,
                headerCheckbox: false,
                enableClickSelection: false
              }
            }}
          />
          <Pagination
            pageNumber={pagination.pageNumber}
            setPageNumber={(value: number) => {
              onPaginationChange(value - 1, pagination.pageSize, pagination.sortParams);
            }}
            pageSize={pagination.pageSize}
            setPageSize={(value: number) => {
              onPaginationChange(0, value, pagination.sortParams);
            }}
            recordCount={displayData.total}
          />
        </div>
      </>
    );
  },
  (prevProps, nextProps) => {
    return (
      prevProps.transactionData?.results === nextProps.transactionData?.results &&
      prevProps.transactionData?.total === nextProps.transactionData?.total &&
      prevProps.isLoading === nextProps.isLoading &&
      prevProps.pagination.pageNumber === nextProps.pagination.pageNumber &&
      prevProps.pagination.pageSize === nextProps.pagination.pageSize &&
      prevProps.pagination.sortParams === nextProps.pagination.sortParams &&
      prevProps.onPaginationChange === nextProps.onPaginationChange &&
      prevProps.onSortChange === nextProps.onSortChange
    );
  }
);

export default ForfeituresTransactionGrid;
