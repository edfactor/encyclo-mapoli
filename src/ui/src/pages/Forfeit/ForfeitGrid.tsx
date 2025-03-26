import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo, useState } from "react";
import { Path, useNavigate } from "react-router";
import { DSMGrid, Pagination } from "smart-ui-library";
import { GetProfitShareForfeitColumns } from "./ForfeitGridColumns";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { useLazyGetForfeituresAndPointsQuery } from "reduxstore/api/YearsEndApi";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";

interface ForfeitGridProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const _sampleData = [
  {
    badgeNumber: 47425,
    employeeName: "BACHELDER, JAKE R",
    ssn: "***-**-7425",
    forfeitures: 5000.0,
    forfeitPoints: 565,
    earningsPoints: 317,
    benNumber: "12345"
  },
  {
    badgeNumber: 82424,
    employeeName: "BATISTA, STEVEN",
    ssn: "***-**-2424",
    forfeitures: 2500.0,
    forfeitPoints: 23,
    earningsPoints: 23,
    benNumber: "12346"
  }
];

const totalsRow = {
  forfeitures: "0.00",
  forfeitPoints: 0,
  earningsPoints: 0
};

const ForfeitGrid: React.FC<ForfeitGridProps> = ({ initialSearchLoaded, setInitialSearchLoaded }) => {
  const navigate = useNavigate();
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const { forfeituresAndPoints } = useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearch] = useLazyGetForfeituresAndPointsQuery();
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
        pagination: { skip: pageNumber * pageSize, take: pageSize }
      },
      false
    ).unwrap();
  }, [pageNumber, pageSize, triggerSearch, fiscalCloseProfitYear]);

  useEffect(() => {
    if (initialSearchLoaded) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, onSearch]);

  return (
    <>
      {forfeituresAndPoints?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`PROFIT SHARE FORFEIT [PAY443] (${forfeituresAndPoints?.response.total || 2} records)`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"PROFIT_SHARE_FORFEIT"}
            isLoading={false}
            handleSortChanged={(_params) => {}}
            providedOptions={{
              rowData: _sampleData,
              pinnedBottomRowData: [totalsRow],
              columnDefs: columnDefs
            }}
          />
          {(forfeituresAndPoints.response.results.length > 0 || !!_sampleData) && (
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
