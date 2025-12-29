import React, { useMemo } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { RootState } from "reduxstore/store";
import { DSMPaginatedGrid } from "../../../components/DSMPaginatedGrid";
import ReportSummary from "../../../components/ReportSummary";
import { GRID_KEYS } from "../../../constants";
import { SortParams, useGridPagination } from "../../../hooks/useGridPagination";
import { GetPostFrozenUnder21ReportColumnDefs } from "./PostFrozenUnder21ReportGridColumns";

interface PostFrozenUnder21ReportGridProps {
  isLoading?: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  gridPagination: ReturnType<typeof useGridPagination>;
}

const PostFrozenUnder21ReportGrid: React.FC<PostFrozenUnder21ReportGridProps> = ({
  isLoading = false,
  setInitialSearchLoaded,
  gridPagination
}) => {
  const profitSharingUnder21Report = useSelector((state: RootState) => state.yearsEnd.profitSharingUnder21Report);
  const navigate = useNavigate();
  const { pageNumber, pageSize, sortParams, handlePageNumberChange, handlePageSizeChange, handleSortChange } =
    gridPagination;

  // Handle navigation for badge clicks
  const handleNavigation = React.useCallback(
    (path: string) => {
      navigate(path);
    },
    [navigate]
  );

  const columnDefs = useMemo(() => GetPostFrozenUnder21ReportColumnDefs(handleNavigation), [handleNavigation]);

  // Create pagination object for DSMPaginatedGrid with proper sort normalization
  const pagination = useMemo(
    () => ({
      pageNumber,
      pageSize,
      sortParams,
      handlePageNumberChange: (newPageNumber: number) => {
        handlePageNumberChange(newPageNumber);
        setInitialSearchLoaded(true);
      },
      handlePageSizeChange: (newPageSize: number) => {
        handlePageSizeChange(newPageSize);
        setInitialSearchLoaded(true);
      },
      handleSortChange: (update: SortParams) => {
        const normalizedUpdate = {
          ...update,
          sortBy: update.sortBy === "" ? "badgeNumber" : update.sortBy,
          isSortDescending: update.sortBy === "" ? false : update.isSortDescending
        };
        handleSortChange(normalizedUpdate);
      }
    }),
    [
      pageNumber,
      pageSize,
      sortParams,
      handlePageNumberChange,
      handlePageSizeChange,
      handleSortChange,
      setInitialSearchLoaded
    ]
  );

  if (!profitSharingUnder21Report?.response) {
    return null;
  }

  return (
    <DSMPaginatedGrid
      preferenceKey={GRID_KEYS.POST_FROZEN_UNDER_21_REPORT}
      data={profitSharingUnder21Report.response.results || []}
      columnDefs={columnDefs}
      totalRecords={profitSharingUnder21Report.response.total || 0}
      isLoading={isLoading}
      pagination={pagination}
      beforeGrid={<ReportSummary report={profitSharingUnder21Report} />}
    />
  );
};

export default PostFrozenUnder21ReportGrid;
