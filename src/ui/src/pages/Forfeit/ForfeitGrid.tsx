import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { Path, useNavigate } from "react-router";
import {DSMGrid, ISortParams, numberToCurrency, Pagination} from "smart-ui-library";
import { GetProfitShareForfeitColumns } from "./ForfeitGridColumns";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { useLazyGetForfeituresAndPointsQuery } from "reduxstore/api/YearsEndApi";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { CAPTIONS } from "../../constants";
import ReportSummary from "../../components/ReportSummary";

interface ForfeitGridProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const ForfeitGrid: React.FC<ForfeitGridProps> = ({ initialSearchLoaded, setInitialSearchLoaded }) => {
  const navigate = useNavigate();
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });
  const { forfeituresAndPoints } = useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearch, { isFetching }] = useLazyGetForfeituresAndPointsQuery();
  const fiscalCloseProfitYear = useFiscalCloseProfitYear();

  // Wrapper to pass react function to non-react class
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

  const onSearch = useCallback(async () => {
    await triggerSearch(
      {
        profitYear: fiscalCloseProfitYear,
        useFrozenData: true,
        pagination: { skip: pageNumber * pageSize, take: pageSize, sortBy: sortParams.sortBy, isSortDescending: sortParams.isSortDescending }
      },
      false
    ).unwrap();
  }, [pageNumber, pageSize, sortParams, triggerSearch, fiscalCloseProfitYear]);

  useEffect(() => {
    if (initialSearchLoaded) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, onSearch]);

  // Need a useEffect on a change in forfeituresAndPoints to reset the page number
  const prevForfeituresAndPoints = useRef<any>(null);
  useEffect(() => {
    if (forfeituresAndPoints?.response?.results && forfeituresAndPoints.response.results.length > 0 &&
        (prevForfeituresAndPoints.current === null || 
         forfeituresAndPoints.response.results.length !== prevForfeituresAndPoints.current.response?.results.length)) {
      setPageNumber(0);
    }
    prevForfeituresAndPoints.current = forfeituresAndPoints;
  }, [forfeituresAndPoints]);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  const totalForfeitures = forfeituresAndPoints?.totalForfeitures ?? 0;
  const totalForfeitPoints = forfeituresAndPoints?.totalForfeitPoints ?? 0;
  const totalEarningPoints = forfeituresAndPoints?.totalEarningPoints ?? 0;

  const totalsRow = {
    forfeitures: totalForfeitures.toFixed(2) ?? "0.00",
    forfeitPoints: totalForfeitPoints ?? 0,
    earningPoints: totalEarningPoints ?? 0
  };
  
  return (
    <>
      {forfeituresAndPoints?.response && (
        <>
          <table>
            <tr><td>Total Profit Sharing Balance&nbsp;</td><td align="right">{numberToCurrency(forfeituresAndPoints?.totalProfitSharingBalance)}</td></tr>
            <tr><td>Distribution Total</td><td align="right">{numberToCurrency(forfeituresAndPoints?.distributionTotals)} </td></tr>
            <tr><td>Allocation To Total</td><td align="right">{numberToCurrency(forfeituresAndPoints?.allocationToTotals)}</td></tr>
            <tr><td>Allocations From Total</td><td align="right">{numberToCurrency(forfeituresAndPoints?.allocationsFromTotals)}</td></tr>
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
          {(forfeituresAndPoints.response.results.length > 0) && (
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
