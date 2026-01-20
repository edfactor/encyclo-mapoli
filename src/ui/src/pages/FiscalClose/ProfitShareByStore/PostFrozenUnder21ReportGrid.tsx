import React, { useEffect, useMemo, useRef } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMPaginatedGrid } from "../../../components/DSMPaginatedGrid";
import ReportSummary from "../../../components/ReportSummary";
import { GRID_KEYS } from "../../../constants";
import { SortParams, useGridPagination } from "../../../hooks/useGridPagination";
import { ProfitSharingUnder21ReportResponse } from "../../../types/reports/qpay";
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
  const { pageNumber, pageSize, sortParams, handlePageNumberChange, handlePageSizeChange, handleSortChange } =
    gridPagination;

  const columnDefs = useMemo(() => GetPostFrozenUnder21ReportColumnDefs(), []);

  // Keep a ref to the last successful report so we can continue showing the
  // previous page while a new page is being fetched (avoids content clearing).
  const lastReportRef = useRef<ProfitSharingUnder21ReportResponse | null>(profitSharingUnder21Report ?? null);

  useEffect(() => {
    if (profitSharingUnder21Report && profitSharingUnder21Report.response) {
      lastReportRef.current = profitSharingUnder21Report;
    }
  }, [profitSharingUnder21Report]);

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

  // Use either the current report or the last-known report when rendering.
  const displayReport = profitSharingUnder21Report ?? lastReportRef.current;

  if (!displayReport?.response) {
    // No data available yet (initial load) — keep original behavior.
    return null;
  }

  return (
    <DSMPaginatedGrid
      preferenceKey={GRID_KEYS.POST_FROZEN_UNDER_21_REPORT}
      data={displayReport.response.results || []}
      columnDefs={columnDefs}
      totalRecords={displayReport.response.total || 0}
      isLoading={isLoading}
      pagination={pagination}
      beforeGrid={<ReportSummary report={displayReport} />}
    />
  );
};

export default PostFrozenUnder21ReportGrid;
