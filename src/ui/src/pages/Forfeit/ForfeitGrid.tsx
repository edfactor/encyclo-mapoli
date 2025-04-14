import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo, useState } from "react";
import { Path, useNavigate } from "react-router";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetProfitShareForfeitColumns } from "./ForfeitGridColumns";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { useLazyGetForfeituresAndPointsQuery } from "reduxstore/api/YearsEndApi";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { CAPTIONS } from "../../constants";

interface ForfeitGridProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const totalsRow = {
  forfeitures: "0.00",
  forfeitPoints: 0,
  earningsPoints: 0
};

const ForfeitGrid: React.FC<ForfeitGridProps> = ({ initialSearchLoaded, setInitialSearchLoaded }) => {
  const navigate = useNavigate();
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });
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

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  
  return (
    <>
      {forfeituresAndPoints?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`${CAPTIONS.FORFEIT} (${forfeituresAndPoints?.response.total || 2} records)`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={CAPTIONS.FORFEIT}
            isLoading={false}
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
