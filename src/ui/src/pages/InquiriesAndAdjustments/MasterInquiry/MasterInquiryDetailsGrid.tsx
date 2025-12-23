import FullscreenIcon from "@mui/icons-material/Fullscreen";
import FullscreenExitIcon from "@mui/icons-material/FullscreenExit";
import { Box, IconButton, Typography } from "@mui/material";
import { memo, useMemo } from "react";
import { ISortParams } from "smart-ui-library";
import { DSMPaginatedGrid } from "../../../components/DSMPaginatedGrid/DSMPaginatedGrid";
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
    handlePageNumberChange: (pageNumber: number) => void;
    handlePageSizeChange: (pageSize: number) => void;
  };
  onSortChange?: (sortParams: SortParams) => void;
  isGridExpanded?: boolean;
  onToggleExpand?: () => void;
}

const MasterInquiryGrid: React.FC<MasterInquiryGridProps> = memo(
  ({ profitData, isLoading, profitGridPagination, onSortChange, isGridExpanded = false, onToggleExpand }) => {
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
          <DSMPaginatedGrid
            preferenceKey={GRID_KEYS.MASTER_INQUIRY}
            data={profitData.results}
            columnDefs={columnDefs}
            totalRecords={profitData.total}
            isLoading={!!isLoading}
            pagination={{
              pageNumber: profitGridPagination?.pageNumber ?? 0,
              pageSize: profitGridPagination?.pageSize ?? 50,
              sortParams: profitGridPagination?.sortParams ?? { sortBy: "", isSortDescending: false },
              handlePageNumberChange: (value: number) => profitGridPagination?.handlePageNumberChange(value - 1),
              handlePageSizeChange: profitGridPagination?.handlePageSizeChange ?? (() => {}),
              handleSortChange: onSortChange ?? (() => {})
            }}
            onSortChange={handleSortChange}
            heightConfig={{
              mode: "content-aware",
              maxHeight: gridMaxHeight
            }}
            gridOptions={{
              suppressMultiSort: true,
              rowSelection: {
                mode: "multiRow",
                checkboxes: false,
                headerCheckbox: false,
                enableClickSelection: false
              }
            }}
            showPagination={!!profitGridPagination && profitData.total > 0}
          />
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
      prevProps.onSortChange === nextProps.onSortChange &&
      prevProps.onToggleExpand === nextProps.onToggleExpand
    );
  }
);

export default MasterInquiryGrid;
