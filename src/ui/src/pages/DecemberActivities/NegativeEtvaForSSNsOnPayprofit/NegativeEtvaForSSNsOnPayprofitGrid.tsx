import { RefObject, useCallback, useMemo } from "react";
import { Path, useNavigate } from "react-router-dom";
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
  onSortChange: (sortParams: SortParams) => void;
}

const NegativeEtvaForSSNsOnPayprofitGrid = ({
  innerRef,
  data,
  isLoading,
  showData,
  hasResults,
  pagination,
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
      pagination={pagination}
      onSortChange={onSortChange}
      showPagination={hasResults}
    />
  );
};

export default NegativeEtvaForSSNsOnPayprofitGrid;
