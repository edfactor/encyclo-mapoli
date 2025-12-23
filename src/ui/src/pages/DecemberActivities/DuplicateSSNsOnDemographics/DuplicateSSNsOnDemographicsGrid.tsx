import { RefObject, useMemo } from "react";
import { DSMPaginatedGrid } from "../../../components/DSMPaginatedGrid";
import { GRID_KEYS } from "../../../constants";
import { GridPaginationActions, GridPaginationState, SortParams } from "../../../hooks/useGridPagination";
import { DuplicateSSNDetail, PagedReportResponse } from "../../../types";
import { GetDuplicateSSNsOnDemographicsColumns } from "./DuplicateSSNsOnDemographicsGridColumns";

interface DuplicateSSNsOnDemographicsGridProps {
  innerRef: RefObject<HTMLDivElement | null>;
  data: PagedReportResponse<DuplicateSSNDetail> | null;
  isLoading: boolean;
  showData: boolean;
  hasResults: boolean;
  pagination: GridPaginationState & GridPaginationActions;
  onSortChange: (sortParams: SortParams) => void;
}

const DuplicateSSNsOnDemographicsGrid = ({
  innerRef,
  data,
  isLoading,
  showData,
  hasResults,
  pagination,
  onSortChange
}: DuplicateSSNsOnDemographicsGridProps) => {
  const columnDefs = useMemo(() => GetDuplicateSSNsOnDemographicsColumns(), []);

  if (!showData || !data?.response) {
    return null;
  }

  return (
    <DSMPaginatedGrid<DuplicateSSNDetail>
      innerRef={innerRef}
      preferenceKey={GRID_KEYS.DUPLICATE_SSNS}
      data={data.response.results}
      columnDefs={columnDefs}
      totalRecords={data.response.total || 0}
      isLoading={isLoading}
      pagination={pagination}
      onSortChange={onSortChange}
      showPagination={hasResults}
    />
  );
};

export default DuplicateSSNsOnDemographicsGrid;
