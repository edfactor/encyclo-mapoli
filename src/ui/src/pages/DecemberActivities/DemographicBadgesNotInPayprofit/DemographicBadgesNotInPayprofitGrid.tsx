import { RefObject, useCallback, useMemo } from "react";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
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

  return (
    <>
      {showData && data?.response && (
        <div ref={innerRef}>
          <DSMGrid
            preferenceKey={"DEMOGRAPHIC_BADGES"}
            isLoading={isLoading}
            handleSortChanged={handleSortChanged}
            providedOptions={{
              rowData: data.response.results,
              columnDefs: columnDefs
            }}
          />
        </div>
      )}
      {hasResults && data?.response && (
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

export default DemographicBadgesNotInPayprofitGrid;
