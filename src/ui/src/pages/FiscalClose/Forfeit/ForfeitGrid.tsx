import { useCallback, useMemo } from "react";
import { Path, useNavigate } from "react-router";
import { numberToCurrency, TotalsGrid } from "smart-ui-library";
import DSMPaginatedGrid from "../../../components/DSMPaginatedGrid/DSMPaginatedGrid";
import ReportSummary from "../../../components/ReportSummary";
import { GRID_KEYS } from "../../../constants";
import { SortParams, useGridPagination } from "../../../hooks/useGridPagination";
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

  // Custom sort handler for compound sort on badgeOrPsn column
  const handleSortChange = useCallback(
    (update: SortParams) => {
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
    },
    [pagination]
  );

  // Some API responses may return numeric totals as strings; coerce safely before formatting.
  const safeNumber = (val: unknown) => {
    const n = typeof val === "number" ? val : parseFloat(val as string);
    return Number.isFinite(n) ? n : 0;
  };
  const totalForfeituresRaw = safeNumber(searchResults?.totalForfeitures);
  const totalForfeitPoints = safeNumber(searchResults?.totalForfeitPoints);
  const totalEarningPoints = safeNumber(searchResults?.totalEarningPoints);

  const totalsRow = useMemo(
    () => ({
      forfeitures: totalForfeituresRaw.toFixed(2),
      contForfeitPoints: totalForfeitPoints,
      earningPoints: totalEarningPoints
    }),
    [totalForfeituresRaw, totalForfeitPoints, totalEarningPoints]
  );

  if (!searchResults?.response) return null;

  return (
    <DSMPaginatedGrid
      preferenceKey={GRID_KEYS.FORFEIT}
      data={searchResults.response.results}
      columnDefs={columnDefs}
      totalRecords={searchResults.response.total}
      isLoading={isSearching}
      pagination={pagination}
      onSortChange={handleSortChange}
      beforeGrid={
        <>
          <div className="sticky top-0 z-10 flex bg-white">
            <TotalsGrid
              displayData={[[numberToCurrency(searchResults.totalProfitSharingBalance || 0)]]}
              leftColumnHeaders={["Profit Sharing Amount"]}
              topRowHeaders={[]}
            />
            <TotalsGrid
              displayData={[[numberToCurrency(searchResults.distributionTotals || 0)]]}
              leftColumnHeaders={["Distribution Amount"]}
              topRowHeaders={[]}
            />
            <TotalsGrid
              displayData={[[numberToCurrency(searchResults.allocationToTotals || 0)]]}
              leftColumnHeaders={["Allocations To"]}
              topRowHeaders={[]}
            />
            <TotalsGrid
              displayData={[[numberToCurrency(searchResults.allocationsFromTotals || 0)]]}
              leftColumnHeaders={["Allocations From"]}
              topRowHeaders={[]}
            />
          </div>
          <ReportSummary report={searchResults} />
        </>
      }
      gridOptions={{
        pinnedTopRowData: [totalsRow]
      }}
      className="relative"
    />
  );
};

export default ForfeitGrid;
