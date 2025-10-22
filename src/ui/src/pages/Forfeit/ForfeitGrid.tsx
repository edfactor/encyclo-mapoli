import { useCallback, useEffect, useMemo } from "react";
import { useSelector } from "react-redux";
import { DSMGrid, numberToCurrency, Pagination, TotalsGrid, ISortParams } from "smart-ui-library";
import ReportSummary from "../../components/ReportSummary";
import { CAPTIONS } from "../../constants";
import { useDynamicGridHeight } from "../../hooks/useDynamicGridHeight";
import useFiscalCloseProfitYear from "../../hooks/useFiscalCloseProfitYear";
import { useGridPagination, SortParams } from "../../hooks/useGridPagination";
import { useLazyGetForfeituresAndPointsQuery } from "../../reduxstore/api/YearsEndApi";
import { RootState } from "../../reduxstore/store";
import { GetProfitShareForfeitColumns } from "./ForfeitGridColumns";

interface ForfeitGridProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  pageNumberReset: boolean;
  setPageNumberReset: (reset: boolean) => void;
  shouldArchive: boolean;
}

const ForfeitGrid: React.FC<ForfeitGridProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
  pageNumberReset,
  setPageNumberReset,
  shouldArchive
}) => {
  const { forfeituresAndPoints } = useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearch, { isFetching }] = useLazyGetForfeituresAndPointsQuery();
  const fiscalCloseProfitYear = useFiscalCloseProfitYear();
  const columnDefs = useMemo(() => GetProfitShareForfeitColumns(), []);

  // Use dynamic grid height utility hook
  const gridMaxHeight = useDynamicGridHeight();

  const { pageNumber, pageSize, sortParams, handlePaginationChange, handleSortChange, resetPagination } =
    useGridPagination({
      initialPageSize: 25,
      initialSortBy: "badgeNumber",
      initialSortDescending: false,
      onPaginationChange: useCallback(
        async (pageNum: number, pageSz: number, sortPrms: SortParams) => {
          if (initialSearchLoaded) {
            await triggerSearch(
              {
                profitYear: fiscalCloseProfitYear,
                useFrozenData: true,
                archive: false, // Always use archive=false for normal pagination
                pagination: {
                  skip: pageNum * pageSz,
                  take: pageSz,
                  sortBy: sortPrms.sortBy,
                  isSortDescending: sortPrms.isSortDescending
                }
              },
              false
            ).unwrap();
          }
        },
        [initialSearchLoaded, fiscalCloseProfitYear, triggerSearch]
      )
    });

  // Separate useEffect to handle archive=true ONLY when status changes to Complete
  useEffect(() => {
    if (shouldArchive && initialSearchLoaded) {
      triggerSearch(
        {
          profitYear: fiscalCloseProfitYear,
          useFrozenData: true,
          archive: true, // ONLY set to true when Complete status selected
          pagination: {
            skip: pageNumber * pageSize,
            take: pageSize,
            sortBy: sortParams.sortBy,
            isSortDescending: sortParams.isSortDescending
          }
        },
        false
      );
      // Note: shouldArchive will be reset by parent component (Forfeit.tsx)
    }
  }, [shouldArchive, initialSearchLoaded, fiscalCloseProfitYear, pageNumber, pageSize, sortParams, triggerSearch]);

  useEffect(() => {
    if (pageNumberReset) {
      resetPagination();
      setPageNumberReset(false);
    }
  }, [pageNumberReset, setPageNumberReset, resetPagination]);

  const sortEventHandler = (update: ISortParams) => handleSortChange(update);

  // Some API responses may return numeric totals as strings; coerce safely before formatting.
  const safeNumber = (val: unknown) => {
    const n = typeof val === "number" ? val : parseFloat(val as string);
    return Number.isFinite(n) ? n : 0;
  };
  const totalForfeituresRaw = safeNumber(forfeituresAndPoints?.totalForfeitures);
  const totalForfeitPoints = safeNumber(forfeituresAndPoints?.totalForfeitPoints);
  const totalEarningPoints = safeNumber(forfeituresAndPoints?.totalEarningPoints);

  const totalsRow = {
    forfeitures: totalForfeituresRaw.toFixed(2),
    contForfeitPoints: totalForfeitPoints,
    earningPoints: totalEarningPoints
  };

  return (
    <div className="relative">
      {forfeituresAndPoints?.response && (
        <>
          <div className="sticky top-0 z-10 flex bg-white">
            <TotalsGrid
              displayData={[[numberToCurrency(forfeituresAndPoints.totalProfitSharingBalance || 0)]]}
              leftColumnHeaders={["Amount in Profit Sharing"]}
              topRowHeaders={[]}></TotalsGrid>
            <TotalsGrid
              displayData={[[numberToCurrency(forfeituresAndPoints.distributionTotals || 0)]]}
              leftColumnHeaders={["Amount in Profit Sharing"]}
              topRowHeaders={[]}></TotalsGrid>
            <TotalsGrid
              displayData={[[numberToCurrency(forfeituresAndPoints.allocationToTotals || 0)]]}
              leftColumnHeaders={["Amount in Profit Sharing"]}
              topRowHeaders={[]}></TotalsGrid>
            <TotalsGrid
              displayData={[[numberToCurrency(forfeituresAndPoints.allocationsFromTotals || 0)]]}
              leftColumnHeaders={["Amount in Profit Sharing"]}
              topRowHeaders={[]}></TotalsGrid>
          </div>

          <ReportSummary report={forfeituresAndPoints} />
          <DSMGrid
            preferenceKey={CAPTIONS.FORFEIT}
            isLoading={isFetching}
            maxHeight={gridMaxHeight}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: forfeituresAndPoints.response.results,
              pinnedTopRowData: [totalsRow],
              columnDefs: columnDefs
            }}
          />
          {forfeituresAndPoints.response.results.length > 0 && (
            <Pagination
              pageNumber={pageNumber}
              setPageNumber={(value: number) => {
                handlePaginationChange(value - 1, pageSize);
                setInitialSearchLoaded(true);
              }}
              pageSize={pageSize}
              setPageSize={(value: number) => {
                handlePaginationChange(0, value);
                setInitialSearchLoaded(true);
              }}
              recordCount={forfeituresAndPoints.response.total}
            />
          )}
        </>
      )}
    </div>
  );
};

export default ForfeitGrid;
