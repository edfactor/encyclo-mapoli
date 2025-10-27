import { CircularProgress, Typography } from "@mui/material";
import { memo, useCallback, useMemo } from "react";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { SortParams, useGridPagination } from "../../../hooks/useGridPagination";
import { GetForfeituresTransactionGridColumns } from "./ForfeituresTransactionGridColumns";

interface TransactionData {
  results: unknown[];
  total: number;
}

interface ForfeituresTransactionGridProps {
  transactionData?: TransactionData | null;
  isLoading?: boolean;
  onPaginationChange?: (pageNumber: number, pageSize: number, sortParams: SortParams) => void;
  onSortChange?: (sortParams: SortParams) => void;
}

const ForfeituresTransactionGrid: React.FC<ForfeituresTransactionGridProps> = memo(
  ({ transactionData, isLoading, onPaginationChange, onSortChange }) => {
    const columnDefs = useMemo(() => GetForfeituresTransactionGridColumns(), []);

    const { pageNumber, pageSize, handlePaginationChange, handleSortChange } = useGridPagination({
      initialPageSize: 25,
      initialSortBy: "transactionDate",
      initialSortDescending: true,
      onPaginationChange: useCallback(
        (pageNum: number, pageSz: number, sortPrms: SortParams) => {
          if (onPaginationChange) {
            onPaginationChange(pageNum, pageSz, sortPrms);
          }
        },
        [onPaginationChange]
      )
    });

    const handleSortChangeInternal = useCallback(
      (sortParams: ISortParams) => {
        handleSortChange(sortParams);
        if (onSortChange) {
          onSortChange(sortParams);
        }
      },
      [handleSortChange, onSortChange]
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
            preferenceKey={`${CAPTIONS.FORFEITURES_ADJUSTMENT}_TRANSACTIONS`}
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
          {onPaginationChange && (
            <Pagination
              pageNumber={pageNumber}
              setPageNumber={(value: number) => {
                handlePaginationChange(value - 1, pageSize);
              }}
              pageSize={pageSize}
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
      prevProps.onPaginationChange === nextProps.onPaginationChange &&
      prevProps.onSortChange === nextProps.onSortChange
    );
  }
);

export default ForfeituresTransactionGrid;
