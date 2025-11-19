import { Typography } from "@mui/material";
import { memo, useMemo } from "react";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { useDynamicGridHeight } from "../../../hooks/useDynamicGridHeight";
import { SortParams } from "../../../hooks/useGridPagination";
import { GetMasterInquiryGridColumns } from "./MasterInquiryGridColumns";

interface ProfitData {
  results: unknown[];
  total: number;
}

interface MasterInquiryGridProps {
  profitData?: ProfitData | null;
  isLoading?: boolean;
  profitGridPagination?: {
    pageNumber: number;
    pageSize: number;
    sortParams: SortParams;
  };
  onPaginationChange?: (pageNumber: number, pageSize: number) => void;
  onSortChange?: (sortParams: SortParams) => void;
}

const MasterInquiryGrid: React.FC<MasterInquiryGridProps> = memo(
  ({ profitData, isLoading, profitGridPagination, onPaginationChange, onSortChange }) => {
    const columnDefs = useMemo(() => GetMasterInquiryGridColumns(), []);
    const gridMaxHeight = useDynamicGridHeight();

    if (isLoading) {
      return <Typography>Loading profit details...</Typography>;
    }

    if (!profitData) {
      return <Typography>No profit details found.</Typography>;
    }

    const handlePaginationChange = (pageNumber: number, pageSize: number) => {
      if (onPaginationChange) {
        onPaginationChange(pageNumber, pageSize);
      }
    };

    const handleSortChange = (sortParams: ISortParams) => {
      if (onSortChange) {
        onSortChange(sortParams);
      }
    };

    return (
      <>
        <div className="w-full">
          <div className="px-6">
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`Profit Details (${profitData.total} ${profitData.total === 1 ? "Record" : "Records"})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={CAPTIONS.MASTER_INQUIRY}
            handleSortChanged={handleSortChange}
            isLoading={!!isLoading}
            maxHeight={gridMaxHeight}
            providedOptions={{
              rowData: profitData.results,
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
          {profitGridPagination && onPaginationChange && (
            <Pagination
              pageNumber={profitGridPagination.pageNumber}
              setPageNumber={(value: number) => {
                handlePaginationChange(value - 1, profitGridPagination.pageSize);
              }}
              pageSize={profitGridPagination.pageSize}
              setPageSize={(value: number) => {
                handlePaginationChange(0, value);
              }}
              recordCount={profitData.total}
            />
          )}
        </div>
      </>
    );
  },
  (prevProps, nextProps) => {
    // Custom comparison function
    // Only re-render if incoming props are different
    return (
      prevProps.profitData?.results === nextProps.profitData?.results &&
      prevProps.profitData?.total === nextProps.profitData?.total &&
      prevProps.isLoading === nextProps.isLoading &&
      prevProps.profitGridPagination?.pageNumber === nextProps.profitGridPagination?.pageNumber &&
      prevProps.profitGridPagination?.pageSize === nextProps.profitGridPagination?.pageSize &&
      prevProps.profitGridPagination?.sortParams === nextProps.profitGridPagination?.sortParams &&
      prevProps.onPaginationChange === nextProps.onPaginationChange &&
      prevProps.onSortChange === nextProps.onSortChange
    );
  }
);

export default MasterInquiryGrid;
