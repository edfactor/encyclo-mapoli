import { RefObject, useCallback, useMemo } from "react";
import { Path, useNavigate } from "react-router";
import { DSMGrid, Pagination } from "smart-ui-library";
import { GRID_KEYS } from "../../../constants";
import { GridPaginationActions, GridPaginationState, SortParams } from "../../../hooks/useGridPagination";
import { NegativeEtvaForSSNsOnPayProfit, PagedReportResponse } from "../../../types";
import { GetNegativeEtvaForSSNsOnPayProfitColumns } from "./NegativeEtvaForSSNsOnPayprofitGridColumns";

interface NegativeEtvaForSSNsOnPayprofitGridProps {
  innerRef: RefObject<HTMLDivElement | null>;
  data: PagedReportResponse<NegativeEtvaForSSNsOnPayProfit> | null;
  isLoading: boolean;
  showData: boolean;
  hasResults: boolean;
  pagination: GridPaginationState & GridPaginationActions;
  onPaginationChange: (pageNumber: number, pageSize: number) => void;
  onSortChange: (sortParams: SortParams) => void;
}

const NegativeEtvaForSSNsOnPayprofitGrid = ({
  innerRef,
  data,
  isLoading,
  showData,
  hasResults,
  pagination,
  onPaginationChange,
  onSortChange
}: NegativeEtvaForSSNsOnPayprofitGridProps) => {
  const navigate = useNavigate();

  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  const columnDefs = useMemo(
    () => GetNegativeEtvaForSSNsOnPayProfitColumns(handleNavigationForButton),
    [handleNavigationForButton]
  );

  return (
    <>
      {showData && data?.response && (
        <div ref={innerRef}>
          <DSMGrid
            preferenceKey={GRID_KEYS.NEGATIVE_ETVA}
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

export default NegativeEtvaForSSNsOnPayprofitGrid;
