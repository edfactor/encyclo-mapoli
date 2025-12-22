import { useCallback, useMemo } from "react";
import { Path, useNavigate } from "react-router";
import { DSMGrid, ISortParams, numberToCurrency, Pagination, TotalsGrid } from "smart-ui-library";
import ReportSummary from "../../../components/ReportSummary";
import { GRID_KEYS } from "../../../constants";
import { useContentAwareGridHeight } from "../../../hooks/useContentAwareGridHeight";
import { useGridPagination } from "../../../hooks/useGridPagination";
import { ForfeituresAndPointsResponse } from "../../../types";
import { GetProfitShareForfeitColumns } from "./ForfeitGridColumns";

interface ForfeitGridProps {
  searchResults: ForfeituresAndPointsResponse | null;
  pagination: ReturnType<typeof useGridPagination>;
  isSearching: boolean;
}

const ForfeitGrid: React.FC<ForfeitGridProps> = ({ searchResults, pagination, isSearching }) => {
  const navigate = useNavigate();

  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  const columnDefs = useMemo(
    () => GetProfitShareForfeitColumns(handleNavigationForButton),
    [handleNavigationForButton]
  );

  // Use content-aware grid height utility hook
  const gridMaxHeight = useContentAwareGridHeight({
    rowCount: searchResults?.response?.results?.length ?? 0
  });

  const sortEventHandler = (update: ISortParams) => {
    // if field is badgeOrPsn, we need to make sortBy equal to badgeNumber,beneficiaryPsn
    // to get a compound sort
    if (update.sortBy === "badgeOrPsn") {
      const newUpdate = {
        ...update,
        sortBy: update.isSortDescending ? "beneficiaryPsn" : "badgeNumber"
      };
      pagination.handleSortChange(newUpdate);
      return;
    }
    pagination.handleSortChange(update);
  };

  // Some API responses may return numeric totals as strings; coerce safely before formatting.
  const safeNumber = (val: unknown) => {
    const n = typeof val === "number" ? val : parseFloat(val as string);
    return Number.isFinite(n) ? n : 0;
  };
  const totalForfeituresRaw = safeNumber(searchResults?.totalForfeitures);
  const totalForfeitPoints = safeNumber(searchResults?.totalForfeitPoints);
  const totalEarningPoints = safeNumber(searchResults?.totalEarningPoints);

  const totalsRow = {
    forfeitures: totalForfeituresRaw.toFixed(2),
    contForfeitPoints: totalForfeitPoints,
    earningPoints: totalEarningPoints
  };

  if (!searchResults?.response) return null;

  return (
    <div className="relative">
      {searchResults?.response && (
        <>
          <div className="sticky top-0 z-10 flex bg-white">
            <TotalsGrid
              displayData={[[numberToCurrency(searchResults.totalProfitSharingBalance || 0)]]}
              leftColumnHeaders={["Profit Sharing Amount"]}
              topRowHeaders={[]}></TotalsGrid>
            <TotalsGrid
              displayData={[[numberToCurrency(searchResults.distributionTotals || 0)]]}
              leftColumnHeaders={["Distribution Amount"]}
              topRowHeaders={[]}></TotalsGrid>
            <TotalsGrid
              displayData={[[numberToCurrency(searchResults.allocationToTotals || 0)]]}
              leftColumnHeaders={["Allocations To"]}
              topRowHeaders={[]}></TotalsGrid>
            <TotalsGrid
              displayData={[[numberToCurrency(searchResults.allocationsFromTotals || 0)]]}
              leftColumnHeaders={["Allocations From"]}
              topRowHeaders={[]}></TotalsGrid>
          </div>

          <ReportSummary report={searchResults} />
          <DSMGrid
            preferenceKey={GRID_KEYS.FORFEIT}
            isLoading={isSearching}
            maxHeight={gridMaxHeight}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: searchResults.response.results,
              pinnedTopRowData: [totalsRow],
              columnDefs: columnDefs
            }}
          />
          {searchResults.response.results.length > 0 && (
            <Pagination
              pageNumber={pagination.pageNumber}
              setPageNumber={(value: number) => pagination.handlePageNumberChange(value - 1)}
              pageSize={pagination.pageSize}
              setPageSize={pagination.handlePageSizeChange}
              recordCount={searchResults.response.total}
            />
          )}
        </>
      )}
    </div>
  );
};

export default ForfeitGrid;
