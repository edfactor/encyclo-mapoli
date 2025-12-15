import { RefObject, useMemo } from "react";
import { Grid, IconButton, Typography } from "@mui/material";
import FullscreenIcon from "@mui/icons-material/Fullscreen";
import FullscreenExitIcon from "@mui/icons-material/FullscreenExit";
import { DSMGrid, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { useContentAwareGridHeight } from "../../../hooks/useContentAwareGridHeight";
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

  // Use content-aware grid height utility hook
  const gridMaxHeight = useContentAwareGridHeight({
    rowCount: data?.response?.results?.length ?? 0,
    heightPercentage: isGridExpanded ? 0.85 : 0.5
  });

  return (
    <>
      {showData && data?.response && (
        <div ref={innerRef}>
          <Grid
            container
            justifyContent="space-between"
            alignItems="center"
            marginBottom={2}>
            <Grid>
              <Typography
                variant="h2"
                sx={{ color: "#0258A5" }}>
                {`${CAPTIONS.DUPLICATE_NAMES} (${data.response.total || 0} records)`}
              </Typography>
            </Grid>
            <Grid>
              {onToggleExpand && (
                <IconButton
                  onClick={onToggleExpand}
                  sx={{ zIndex: 1 }}
                  aria-label={isGridExpanded ? "Exit fullscreen" : "Enter fullscreen"}>
                  {isGridExpanded ? <FullscreenExitIcon /> : <FullscreenIcon />}
                </IconButton>
              )}
            </Grid>
          </Grid>
          <DSMGrid
            preferenceKey={CAPTIONS.DUPLICATE_NAMES}
            isLoading={isLoading}
            maxHeight={gridMaxHeight}
            handleSortChanged={onSortChange}
            providedOptions={{
              rowData: data.response.results,
              columnDefs: columnDefs
            }}
          />
        </div>
      )}
      {!isGridExpanded && hasResults && data?.response && (
        <Pagination
          pageNumber={pagination.pageNumber}
          setPageNumber={(value: number) => {
            onPaginationChange(value - 1, pagination.pageSize);
          }}
          pageSize={pagination.pageSize}
          setPageSize={(value: number) => {
            onPaginationChange(0, value);
          }}
          recordCount={data.response.total || 0}
        />
      )}
    </>
  );
};

export default DuplicateNamesAndBirthdaysGrid;
