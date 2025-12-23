import { RefObject, useCallback, useMemo } from "react";
import { Path, useNavigate } from "react-router";
import { DSMPaginatedGrid } from "../../../components/DSMPaginatedGrid";
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

  if (!showData || !data?.response) {
    return null;
  }

  return (
    <DSMPaginatedGrid<NegativeEtvaForSSNsOnPayProfit>
      innerRef={innerRef}
      preferenceKey={GRID_KEYS.NEGATIVE_ETVA}
      data={data.response.results}
      columnDefs={columnDefs}
      totalRecords={data.response.total || 0}
      isLoading={isLoading}
      pagination={{
        ...pagination,
        handlePageNumberChange: (pageNum: number) => onPaginationChange(pageNum, pagination.pageSize),
        handlePageSizeChange: (pageSz: number) => onPaginationChange(0, pageSz)
      }}
      onSortChange={onSortChange}
      showPagination={hasResults}
    />
  );
};

export default NegativeEtvaForSSNsOnPayprofitGrid;
