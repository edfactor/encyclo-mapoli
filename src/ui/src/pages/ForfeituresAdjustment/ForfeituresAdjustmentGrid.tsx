import { AddOutlined } from "@mui/icons-material";
import { Button } from "@mui/material";
import React, { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { useLazyGetForfeitureAdjustmentsQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, Pagination } from "smart-ui-library";
import ReportSummary from "../../components/ReportSummary";
import { CAPTIONS } from "../../constants";
import { GetForfeituresAdjustmentColumns } from "./ForfeituresAdjustmentGridColumns";

interface ISortParams {
  sortBy: string;
  isSortDescending: boolean;
}

interface ForfeituresAdjustmentGridProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  onAddForfeiture?: () => void;
  pageNumberReset: boolean;
  setPageNumberReset: (reset: boolean) => void;
}

const ForfeituresAdjustmentGrid: React.FC<ForfeituresAdjustmentGridProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
  onAddForfeiture,
  pageNumberReset,
  setPageNumberReset
}) => {
  const navigate = useNavigate();
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "clientNumber",
    isSortDescending: false
  });

  const [_, { isFetching }] = useLazyGetForfeitureAdjustmentsQuery();

  const { forfeitureAdjustmentData } = useSelector((state: RootState) => state.forfeituresAdjustment);
  const results = forfeitureAdjustmentData?.response?.results || [];
  const totalRecords = forfeitureAdjustmentData?.response?.total || 0;

  const totalsRow = forfeitureAdjustmentData
    ? {
        netBalance: forfeitureAdjustmentData.totatNetBalance,
        netVested: forfeitureAdjustmentData.totatNetVested
      }
    : null;

  // Wrapper to pass react function to non-react class
  const handleNavigationForButton = useCallback(
    (badgeNumber: string) => {
      navigate(`/master-inquiry/${badgeNumber}`);
    },
    [navigate]
  );

  const columnDefs = useMemo(() => GetForfeituresAdjustmentColumns(), [handleNavigationForButton]);

  useEffect(() => {
    if (pageNumberReset) {
      setPageNumber(0);
      setPageNumberReset(false);
    }
  }, [pageNumberReset, setPageNumberReset]);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  return (
    <>
      {initialSearchLoaded && (
        <>
          <div
            style={{
              padding: "0 24px 24px 24px",
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center"
            }}>
            {onAddForfeiture && (
              <Button
                onClick={onAddForfeiture}
                variant="contained"
                startIcon={<AddOutlined />}
                color="primary">
                ADD FORFEITURE
              </Button>
            )}
          </div>
          {forfeitureAdjustmentData && <ReportSummary report={forfeitureAdjustmentData} />}
          <DSMGrid
            preferenceKey={CAPTIONS.FORFEITURES_ADJUSTMENT}
            isLoading={isFetching}
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
