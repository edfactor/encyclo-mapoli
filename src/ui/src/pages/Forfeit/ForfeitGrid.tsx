import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetForfeituresAndPointsQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, numberToCurrency, Pagination } from "smart-ui-library";
import ReportSummary from "../../components/ReportSummary";
import { CAPTIONS } from "../../constants";
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
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });
  const { forfeituresAndPoints } = useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearch, { isFetching }] = useLazyGetForfeituresAndPointsQuery();
  const fiscalCloseProfitYear = useFiscalCloseProfitYear();

  const columnDefs = useMemo(() => GetProfitShareForfeitColumns(), []);

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
  }, [initialSearchLoaded, pageNumber, pageSize, onSearch]);

  useEffect(() => {
    if (pageNumberReset) {
      setPageNumber(0);
      setPageNumberReset(false);
    }
  }, [pageNumberReset, setPageNumberReset]);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  const totalForfeitures = forfeituresAndPoints?.totalForfeitures ?? 0;
  const totalForfeitPoints = forfeituresAndPoints?.totalForfeitPoints ?? 0;
  const totalEarningPoints = forfeituresAndPoints?.totalEarningPoints ?? 0;

  const totalsRow = {
    forfeitures: totalForfeitures.toFixed(2) ?? "0.00",
    contForfeitPoints: totalForfeitPoints ?? 0,
    earningPoints: totalEarningPoints ?? 0
  };

  return (
    <>
      {forfeituresAndPoints?.response && (
        <>
          <table>
            <tbody>
              <tr>
                <td>Total Profit Sharing Balance&nbsp;</td>
                <td align="right">{numberToCurrency(forfeituresAndPoints?.totalProfitSharingBalance)}</td>
              </tr>
              <tr>
                <td>Distribution Total</td>
                <td align="right">{numberToCurrency(forfeituresAndPoints?.distributionTotals)} </td>
              </tr>
              <tr>
                <td>Allocation To Total</td>
                <td align="right">{numberToCurrency(forfeituresAndPoints?.allocationToTotals)}</td>
              </tr>
              <tr>
                <td>Allocations From Total</td>
                <td align="right">{numberToCurrency(forfeituresAndPoints?.allocationsFromTotals)}</td>
              </tr>
            </tbody>
          </table>
          <br />

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
                setPageNumber(value - 1);
                setInitialSearchLoaded(true);
              }}
              pageSize={pageSize}
              setPageSize={(value: number) => {
                setPageSize(value);
                setPageNumber(1);
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
