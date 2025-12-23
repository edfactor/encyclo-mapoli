import React, { useMemo } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMPaginatedGrid } from "../../../../components/DSMPaginatedGrid";
import ReportSummary from "../../../../components/ReportSummary";
import { GRID_KEYS } from "../../../../constants";
import { SortParams, useGridPagination } from "../../../../hooks/useGridPagination";
import { GetUnder21BreakdownColumnDefs } from "./GetUnder21BreakdownGridColumns";

interface Under21BreakdownGridProps {
  isLoading?: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  pageNumber: number;
  pageSize: number;
  onPageChange: (page: number) => void;
}

const Under21BreakdownGrid: React.FC<Under21BreakdownGridProps> = ({
  isLoading = false,
  setInitialSearchLoaded,
  pageNumber,
  pageSize,
  onPageChange
}) => {
  const under21Breakdown = useSelector((state: RootState) => state.yearsEnd.under21BreakdownByStore);

  const { sortParams, handleSortChange } = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "badgeNumber",
    initialSortDescending: false,
    persistenceKey: GRID_KEYS.UNDER_21_BREAKDOWN_REPORT,
    onPaginationChange: () => {
      // This component doesn't trigger API calls on pagination change
      // The data is already loaded from the parent component
    }
  });

  const columnDefs = useMemo(() => GetUnder21BreakdownColumnDefs(), []);

  // Create pagination object for DSMPaginatedGrid
  const pagination = useMemo(
    () => ({
      pageNumber,
      pageSize,
      sortParams,
      handlePageNumberChange: (newPageNumber: number) => {
        onPageChange(newPageNumber);
        setInitialSearchLoaded(true);
      },
      handlePageSizeChange: (_newPageSize: number) => {
        onPageChange(0);
        setInitialSearchLoaded(true);
      },
      handleSortChange: (update: SortParams) => {
        const normalizedUpdate = {
          ...update,
          sortBy: update.sortBy === "" ? "badgeNumber" : update.sortBy,
          isSortDescending: update.sortBy === "" ? false : update.isSortDescending
        };
        handleSortChange(normalizedUpdate);
        setInitialSearchLoaded(true);
      }
    }),
    [pageNumber, pageSize, sortParams, onPageChange, handleSortChange, setInitialSearchLoaded]
  );

  if (!under21Breakdown?.response) {
    return null;
  }

  return (
    <DSMPaginatedGrid
      preferenceKey={GRID_KEYS.UNDER_21_BREAKDOWN_REPORT}
      data={under21Breakdown.response.results || []}
      columnDefs={columnDefs}
      totalRecords={under21Breakdown.response.total || 0}
      isLoading={isLoading}
      pagination={pagination}
      beforeGrid={<ReportSummary report={under21Breakdown} />}
    />
  );
};

export default Under21BreakdownGrid;
