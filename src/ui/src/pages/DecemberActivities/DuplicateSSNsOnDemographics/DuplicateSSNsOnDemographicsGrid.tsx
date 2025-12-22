import { RefObject, useMemo } from "react";
import { DSMGrid, Pagination } from "smart-ui-library";
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
  onPaginationChange: (pageNumber: number, pageSize: number) => void;
  onSortChange: (sortParams: SortParams) => void;
}

const DuplicateSSNsOnDemographicsGrid = ({
  innerRef,
  data,
  isLoading,
  showData,
  hasResults,
  pagination,
  onPaginationChange,
  onSortChange
}: DuplicateSSNsOnDemographicsGridProps) => {
  const columnDefs = useMemo(() => GetDuplicateSSNsOnDemographicsColumns(), []);

  return (
    <>
      {showData && data?.response && (
        <div ref={innerRef}>
          <DSMGrid
            preferenceKey={GRID_KEYS.DUPLICATE_SSNS}
            isLoading={isLoading}
            handleSortChanged={onSortChange}
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

export default DuplicateSSNsOnDemographicsGrid;
