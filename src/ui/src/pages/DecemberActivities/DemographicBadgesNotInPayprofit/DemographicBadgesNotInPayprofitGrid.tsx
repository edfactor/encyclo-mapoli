import { RefObject, useMemo } from "react";
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
  onSortChange: (sortParams: SortParams) => void;
}

const DemographicBadgesNotInPayprofitGrid = ({
  innerRef,
  data,
  isLoading,
  showData,
  hasResults,
  pagination,
  onSortChange
}: DemographicBadgesNotInPayprofitGridProps) => {
  const columnDefs = useMemo(() => GetDemographicBadgesNotInPayprofitColumns(), []);



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
        pagination={pagination}
        onSortChange={onSortChange}
        showPagination={hasResults}
      />
    </div>
  );
};

export default DemographicBadgesNotInPayprofitGrid;
