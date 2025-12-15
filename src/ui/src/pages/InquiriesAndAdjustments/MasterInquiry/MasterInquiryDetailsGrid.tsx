import { Box, IconButton, Typography } from "@mui/material";
import FullscreenIcon from "@mui/icons-material/Fullscreen";
import FullscreenExitIcon from "@mui/icons-material/FullscreenExit";
import { memo, useMemo } from "react";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GRID_KEYS } from "../../../constants";
import { useContentAwareGridHeight } from "../../../hooks/useContentAwareGridHeight";
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
  isGridExpanded?: boolean;
  onToggleExpand?: () => void;
}

const MasterInquiryGrid: React.FC<MasterInquiryGridProps> = memo(
  ({
    profitData,
    isLoading,
    profitGridPagination,
    onPaginationChange,
    onSortChange,
    isGridExpanded = false,
    onToggleExpand
  }) => {
    const columnDefs = useMemo(() => GetMasterInquiryGridColumns(), []);
    const gridMaxHeight = useContentAwareGridHeight({
      rowCount: profitData?.results?.length ?? 0,
      heightPercentage: isGridExpanded ? 0.85 : 0.5
    });

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
          <Box
            sx={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              padding: "0 24px",
              marginBottom: "8px"
            }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`Profit Details (${profitData.total} ${profitData.total === 1 ? "Record" : "Records"})`}
            </Typography>
            <IconButton
              onClick={onToggleExpand}
              sx={{ zIndex: 1 }}
              aria-label={isGridExpanded ? "Exit fullscreen" : "Enter fullscreen"}>
              {isGridExpanded ? <FullscreenExitIcon /> : <FullscreenIcon />}
            </IconButton>
          </Box>
          <DSMGrid
            preferenceKey={GRID_KEYS.MASTER_INQUIRY}
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
      prevProps.isGridExpanded === nextProps.isGridExpanded &&
      prevProps.profitGridPagination?.pageNumber === nextProps.profitGridPagination?.pageNumber &&
      prevProps.profitGridPagination?.pageSize === nextProps.profitGridPagination?.pageSize &&
      prevProps.profitGridPagination?.sortParams === nextProps.profitGridPagination?.sortParams &&
      prevProps.onPaginationChange === nextProps.onPaginationChange &&
      prevProps.onSortChange === nextProps.onSortChange &&
      prevProps.onToggleExpand === nextProps.onToggleExpand
    );
  }
);

export default MasterInquiryGrid;
