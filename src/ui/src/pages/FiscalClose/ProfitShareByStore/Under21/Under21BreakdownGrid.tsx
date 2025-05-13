import React, { useMemo } from 'react';
import { Typography } from "@mui/material";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import Grid2 from '@mui/material/Grid2';
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { GetUnder21BreakdownColumnDefs } from './GetUnder21BreakdownColumnDefs';
import { useNavigate } from 'react-router-dom';

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
  const handleNavigation = (path: string) => {
    navigate(path);
  };

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
    <Grid2 container direction="column" width="100%">
      <Grid2 paddingX="24px">
        <Typography
          variant="h6"
          sx={{ color: "#0258A5", marginBottom: "16px" }}>
          Under 21 Breakdown
        </Typography>
      </Grid2>
      <Grid2 width="100%">
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
      </Grid2>
    </Grid2>
  );
};

export default Under21BreakdownGrid; 