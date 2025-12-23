import { RefObject, useMemo } from "react";
import { IconButton, Typography } from "@mui/material";
import FullscreenIcon from "@mui/icons-material/Fullscreen";
import FullscreenExitIcon from "@mui/icons-material/FullscreenExit";
import { DSMPaginatedGrid } from "../../../components/DSMPaginatedGrid";
import { CAPTIONS, GRID_KEYS } from "../../../constants";
import { DuplicateNameAndBirthday, PagedReportResponse } from "../../../types";
import { GetDuplicateNamesAndBirthdayColumns } from "./DuplicateNamesAndBirthdaysGridColumns";
import { GridPaginationState, GridPaginationActions, SortParams } from "../../../hooks/useGridPagination";

interface DuplicateNamesAndBirthdaysGridProps {
  innerRef: RefObject<HTMLDivElement | null>;
  data: PagedReportResponse<DuplicateNameAndBirthday> | null;
  isLoading: boolean;
  showData: boolean;
  hasResults: boolean;
  pagination: GridPaginationState & GridPaginationActions;
  onPaginationChange: (pageNumber: number, pageSize: number) => void;
  onSortChange: (sortParams: SortParams) => void;
  isGridExpanded?: boolean;
  onToggleExpand?: () => void;
}

const DuplicateNamesAndBirthdaysGrid = ({
  innerRef,
  data,
  isLoading,
  showData,
  hasResults,
  pagination,
  onPaginationChange,
  onSortChange,
  isGridExpanded = false,
  onToggleExpand
}: DuplicateNamesAndBirthdaysGridProps) => {
  const columnDefs = useMemo(() => GetDuplicateNamesAndBirthdayColumns(), []);

  if (!showData || !data?.response) {
    return null;
  }

  return (
    <DSMPaginatedGrid<DuplicateNameAndBirthday>
      innerRef={innerRef}
      preferenceKey={GRID_KEYS.DUPLICATE_NAMES}
      data={data.response.results}
      columnDefs={columnDefs}
      totalRecords={data.response.total || 0}
      isLoading={isLoading}
      pagination={{
        ...pagination,
        handlePageNumberChange: (pageNum: number) => onPaginationChange(pageNum, pagination.pageSize),
        handlePageSizeChange: (pageSz: number) => onPaginationChange(0, pageSz)
      }}
      onSortChange={onSortChange}
      showPagination={!isGridExpanded && hasResults}
      heightConfig={{
        heightPercentage: isGridExpanded ? 0.85 : 0.5
      }}
      header={
        <Typography variant="h2" sx={{ color: "#0258A5" }}>
          {`${CAPTIONS.DUPLICATE_NAMES} (${data.response.total || 0} records)`}
        </Typography>
      }
      headerActions={
        onToggleExpand && (
          <IconButton
            onClick={onToggleExpand}
            sx={{ zIndex: 1 }}
            aria-label={isGridExpanded ? "Exit fullscreen" : "Enter fullscreen"}
          >
            {isGridExpanded ? <FullscreenExitIcon /> : <FullscreenIcon />}
          </IconButton>
        )
      }
    />
  );
};

export default DuplicateNamesAndBirthdaysGrid;
