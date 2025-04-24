import { Typography } from "@mui/material";
import { useCallback, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useSelector } from "react-redux";
import { DSMGrid, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import { GetForfeituresAdjustmentColumns } from "./ForfeituresAdjustmentGridColumns";
import { RootState } from "reduxstore/store";

interface ISortParams {
  sortBy: string;
  isSortDescending: boolean;
}

interface ForfeituresAdjustmentGridProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const ForfeituresAdjustmentGrid: React.FC<ForfeituresAdjustmentGridProps> = ({ 
  initialSearchLoaded, 
  setInitialSearchLoaded 
}) => {
  const navigate = useNavigate();
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "clientNumber",
    isSortDescending: false
  });

  const { forfeitureAdjustmentData } = useSelector((state: RootState) => state.forfeituresAdjustment);
  const results = forfeitureAdjustmentData?.response?.results || [];
  const totalRecords = forfeitureAdjustmentData?.response?.total || 0;
  
  const totalsRow = forfeitureAdjustmentData ? {
    netBalance: forfeitureAdjustmentData.totatNetBalance,
    netVested: forfeitureAdjustmentData.totatNetVested
  } : null;

  // Wrapper to pass react function to non-react class
  const handleNavigationForButton = useCallback(
    (badgeNumber: string) => {
      navigate(`/master-inquiry/${badgeNumber}`);
    },
    [navigate]
  );

  const columnDefs = useMemo(
    () => GetForfeituresAdjustmentColumns(handleNavigationForButton),
    [handleNavigationForButton]
  );

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  return (
    <>
      {initialSearchLoaded && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`${forfeitureAdjustmentData?.reportName || "Forfeiture Adjustments"} (${totalRecords} records)`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={CAPTIONS.FORFEITURES_ADJUSTMENT}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: results,
              pinnedTopRowData: totalsRow ? [totalsRow] : [],
              columnDefs: columnDefs
            }}
          />
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
            recordCount={totalRecords}
          />
        </>
      )}
    </>
  );
};

export default ForfeituresAdjustmentGrid; 