import { Typography, CircularProgress } from "@mui/material";
import { memo, useMemo } from "react";
import { DSMGrid, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import { GetForfeituresTransactionGridColumns } from "./ForfeituresTransactionGridColumns";

interface TransactionData {
  results: any[];
  total: number;
}

interface ForfeituresTransactionGridProps {
  transactionData?: TransactionData | null;
  isLoading?: boolean;
  gridPagination?: {
    pageNumber: number;
    pageSize: number;
    sortParams: any;
  };
  onPaginationChange?: (pageNumber: number, pageSize: number) => void;
  onSortChange?: (sortParams: any) => void;
}

const ForfeituresTransactionGrid: React.FC<ForfeituresTransactionGridProps> = memo(
  ({ transactionData, isLoading, gridPagination, onPaginationChange, onSortChange }) => {
    const columnDefs = useMemo(() => GetForfeituresTransactionGridColumns(), []);

    if (isLoading) {
      return <CircularProgress />;
    }


    const handlePaginationChange = (pageNumber: number, pageSize: number) => {
      if (onPaginationChange) {
        onPaginationChange(pageNumber, pageSize);
      }
    };

    const handleSortChange = (sortParams: any) => {
      if (onSortChange) {
        onSortChange(sortParams);
      }
    };

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
            preferenceKey={`${CAPTIONS.FORFEITURES_ADJUSTMENT}_TRANSACTIONS`}
            handleSortChanged={handleSortChange}
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
          {gridPagination && onPaginationChange && (
            <Pagination
              pageNumber={gridPagination.pageNumber}
              setPageNumber={(value: number) => {
                handlePaginationChange(value - 1, gridPagination.pageSize);
              }}
              pageSize={gridPagination.pageSize}
              setPageSize={(value: number) => {
                handlePaginationChange(0, value);
              }}
              recordCount={displayData.total}
            />
          )}
        </div>
      </>
    );
  },
  (prevProps, nextProps) => {
    return (
      prevProps.transactionData?.results === nextProps.transactionData?.results &&
      prevProps.transactionData?.total === nextProps.transactionData?.total &&
      prevProps.isLoading === nextProps.isLoading &&
      prevProps.gridPagination?.pageNumber === nextProps.gridPagination?.pageNumber &&
      prevProps.gridPagination?.pageSize === nextProps.gridPagination?.pageSize &&
      prevProps.gridPagination?.sortParams === nextProps.gridPagination?.sortParams &&
      prevProps.onPaginationChange === nextProps.onPaginationChange &&
      prevProps.onSortChange === nextProps.onSortChange
    );
  }
);

export default ForfeituresTransactionGrid;