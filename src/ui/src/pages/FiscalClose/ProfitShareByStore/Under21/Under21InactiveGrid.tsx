import React, { useMemo } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { ISortParams } from "smart-ui-library";
import { DSMPaginatedGrid } from "../../../../components/DSMPaginatedGrid";
import ReportSummary from "../../../../components/ReportSummary";
import { GRID_KEYS } from "../../../../constants";
import { SortParams } from "../../../../hooks/useGridPagination";
import { under21InactiveColumnDefs } from "./GetUnder21BreakdownGridColumns";

interface Under21InactiveGridProps {
  isLoading?: boolean;
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  pageNumber: number;
  setPageNumber: (value: number) => void;
  pageSize: number;
  setPageSize: (value: number) => void;
  sortParams: ISortParams;
  setSortParams: (value: ISortParams) => void;
}

const Under21InactiveGrid: React.FC<Under21InactiveGridProps> = ({
  isLoading = false,
  setInitialSearchLoaded,
  pageNumber,
  setPageNumber,
  pageSize,
  setPageSize,
  sortParams,
  setSortParams
}) => {
  const under21Inactive = useSelector((state: RootState) => state.yearsEnd.under21Inactive);

  const columnDefs = useMemo(() => under21InactiveColumnDefs(), []);

  // Create pagination object for DSMPaginatedGrid
  const pagination = useMemo(
    () => ({
      pageNumber,
      pageSize,
      sortParams: sortParams as SortParams,
      handlePageNumberChange: (newPageNumber: number) => {
        setPageNumber(newPageNumber);
        setInitialSearchLoaded(true);
      },
      handlePageSizeChange: (newPageSize: number) => {
        setPageSize(newPageSize);
        setPageNumber(0);
        setInitialSearchLoaded(true);
      },
      handleSortChange: (update: SortParams) => {
        const normalizedUpdate = {
          ...update,
          sortBy: update.sortBy === "" ? "badgeNumber" : update.sortBy,
          isSortDescending: update.sortBy === "" ? false : update.isSortDescending
        };
        setSortParams(normalizedUpdate);
        setPageNumber(0);
        setInitialSearchLoaded(true);
      }
    }),
    [pageNumber, pageSize, sortParams, setPageNumber, setPageSize, setSortParams, setInitialSearchLoaded]
  );

  if (!under21Inactive?.response) {
    return null;
  }

  return (
    <DSMPaginatedGrid
      preferenceKey={GRID_KEYS.UNDER_21_INACTIVE_REPORT}
      data={under21Inactive.response.results || []}
      columnDefs={columnDefs}
      totalRecords={under21Inactive.response.total || 0}
      isLoading={isLoading}
      pagination={pagination}
      beforeGrid={<ReportSummary report={under21Inactive} />}
    />
  );
};

export default Under21InactiveGrid;
