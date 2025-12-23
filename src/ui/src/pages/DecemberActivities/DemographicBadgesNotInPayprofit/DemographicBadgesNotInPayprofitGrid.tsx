import { RefObject, useCallback, useMemo } from "react";
import { ISortParams } from "smart-ui-library";
import { DSMPaginatedGrid } from "../../../components/DSMPaginatedGrid/DSMPaginatedGrid";
import { GRID_KEYS } from "../../../constants";
import { GridPaginationActions, GridPaginationState, SortParams } from "../../../hooks/useGridPagination";
import { DemographicBadgesNotInPayprofit, PagedReportResponse } from "../../../types";
import { GetDemographicBadgesNotInPayprofitColumns } from "./DemographicBadgesNotInPayprofitGridColumns";

interface DemographicBadgesNotInPayprofitGridProps {
  innerRef: RefObject<HTMLDivElement | null>;
  data: PagedReportResponse<DemographicBadgesNotInPayprofit> | null;
  isLoading: boolean;
  showData: boolean;
  hasResults: boolean;
  pagination: GridPaginationState & GridPaginationActions;
  onPaginationChange: (pageNumber: number, pageSize: number) => void;
  onSortChange: (sortParams: SortParams) => void;
}

const DemographicBadgesNotInPayprofitGrid = ({
  innerRef,
  data,
  isLoading,
  showData,
  hasResults,
  pagination,
  onPaginationChange,
  onSortChange
}: DemographicBadgesNotInPayprofitGridProps) => {
  const columnDefs = useMemo(() => GetDemographicBadgesNotInPayprofitColumns(), []);

  const handleSortChanged = useCallback(
    (update: ISortParams) => {
      // Handle empty sortBy case - set default (preserving original logic)
      if (update.sortBy === "") {
        update.sortBy = "badgeNumber";
        update.isSortDescending = true;
      }

      // Reset to page 0 when sorting changes (preserving original logic)
      onPaginationChange(0, pagination.pageSize);
      onSortChange(update);
    },
    [onPaginationChange, onSortChange, pagination.pageSize]
  );

  const paginationProps = {
    pageNumber: pagination.pageNumber,
    pageSize: pagination.pageSize,
    sortParams: pagination.sortParams,
    handlePageNumberChange: (value: number) => onPaginationChange(value, pagination.pageSize),
    handlePageSizeChange: (value: number) => onPaginationChange(0, value),
    handleSortChange: onSortChange
  };

  if (!showData || !data?.response) {
    return null;
  }

  return (
    <div ref={innerRef}>
      <DSMPaginatedGrid
        preferenceKey={GRID_KEYS.DEMOGRAPHIC_BADGES}
        data={data.response.results}
        columnDefs={columnDefs}
        totalRecords={data.response.total || 0}
        isLoading={isLoading}
        pagination={paginationProps}
        onSortChange={handleSortChanged}
        showPagination={hasResults}
      />
    </div>
  );
};

export default DemographicBadgesNotInPayprofitGrid;
