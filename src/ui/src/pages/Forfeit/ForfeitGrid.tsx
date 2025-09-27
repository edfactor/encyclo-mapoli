import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useCallback, useEffect, useMemo } from "react";
import { useSelector } from "react-redux";
import { useLazyGetForfeituresAndPointsQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, numberToCurrency, Pagination, TotalsGrid } from "smart-ui-library";
import ReportSummary from "../../components/ReportSummary";
import { CAPTIONS } from "../../constants";
import { useGridPagination } from "../../hooks/useGridPagination";
import { GetProfitShareForfeitColumns } from "./ForfeitGridColumns";

interface ForfeitGridProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  pageNumberReset: boolean;
  setPageNumberReset: (reset: boolean) => void;
}

const ForfeitGrid: React.FC<ForfeitGridProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
  pageNumberReset,
  setPageNumberReset
}) => {
  const { forfeituresAndPoints } = useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearch, { isFetching }] = useLazyGetForfeituresAndPointsQuery();
  const fiscalCloseProfitYear = useFiscalCloseProfitYear();
  const columnDefs = useMemo(() => GetProfitShareForfeitColumns(), []);

  const { pageNumber, pageSize, sortParams, handlePaginationChange, handleSortChange, resetPagination } = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "badgeNumber",
    initialSortDescending: false,
    onPaginationChange: useCallback(async (pageNum: number, pageSz: number, sortPrms: any) => {
      if (initialSearchLoaded) {
        await triggerSearch(
          {
            profitYear: fiscalCloseProfitYear,
            useFrozenData: true,
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
    }, [initialSearchLoaded, fiscalCloseProfitYear, triggerSearch])
  });

  const onSearch = useCallback(async () => {
    await triggerSearch(
      {
        profitYear: fiscalCloseProfitYear,
        useFrozenData: true,
        pagination: {
          skip: pageNumber * pageSize,
          take: pageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        }
      },
      false
    ).unwrap();
  }, [pageNumber, pageSize, sortParams, triggerSearch, fiscalCloseProfitYear]);

  useEffect(() => {
    if (initialSearchLoaded) {
      onSearch();
    }
  }, [initialSearchLoaded, onSearch]);

  useEffect(() => {
    if (pageNumberReset) {
      resetPagination();
      setPageNumberReset(false);
    }
  }, [pageNumberReset, setPageNumberReset, resetPagination]);

  const sortEventHandler = (update: any) => handleSortChange(update);

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
    <>
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
    </>
  );
};

export default ForfeitGrid;
