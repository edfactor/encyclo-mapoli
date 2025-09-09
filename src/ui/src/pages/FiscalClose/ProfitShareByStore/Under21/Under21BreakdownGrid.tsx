import { Grid } from "@mui/material";
import React, { useMemo } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import ReportSummary from "../../../../components/ReportSummary";
import { GetUnder21BreakdownColumnDefs } from "./GetUnder21BreakdownColumnDefs";

interface Under21BreakdownGridProps {
  isLoading?: boolean;
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  pageNumber: number;
  setPageNumber: (value: number) => void;
  pageSize: number;
  setPageSize: (value: number) => void;
  sortParams: ISortParams;
  setSortParams: (value: ISortParams) => void;
}

const Under21BreakdownGrid: React.FC<Under21BreakdownGridProps> = ({
  isLoading = false,
  initialSearchLoaded,
  setInitialSearchLoaded,
  pageNumber,
  setPageNumber,
  pageSize,
  setPageSize,
  sortParams,
  setSortParams
}) => {
  const under21Breakdown = useSelector((state: RootState) => state.yearsEnd.under21BreakdownByStore);
  const navigate = useNavigate();

  // Handle navigation for badge clicks
  const handleNavigation = React.useCallback(
    (path: string) => {
      navigate(path);
    },
    [navigate]
  );

  const sortEventHandler = (update: ISortParams) => {
    if (update.sortBy === "") {
      update.sortBy = "badgeNumber";
      update.isSortDescending = false;
    }
    setSortParams(update);
    setPageNumber(0);
    setInitialSearchLoaded(true);
  };

  const columnDefs = useMemo(() => GetUnder21BreakdownColumnDefs(handleNavigation), [handleNavigation]);

  return (
    <Grid
      container
      direction="column"
      width="100%">
      <ReportSummary report={under21Breakdown} />
      <Grid width="100%">
        <DSMGrid
          preferenceKey="UNDER_21_BREAKDOWN_REPORT"
          isLoading={isLoading}
          handleSortChanged={sortEventHandler}
          providedOptions={{
            rowData: under21Breakdown?.response?.results || [],
            columnDefs
          }}
        />
        {under21Breakdown?.response?.results && under21Breakdown.response.results.length > 0 && (
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
            recordCount={under21Breakdown.response.total || 0}
          />
        )}
      </Grid>
    </Grid>
  );
};

export default Under21BreakdownGrid;
