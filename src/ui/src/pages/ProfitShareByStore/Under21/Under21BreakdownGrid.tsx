import React from 'react';
import { Typography } from "@mui/material";
import { DSMGrid } from "smart-ui-library";
import Grid2 from '@mui/material/Grid2';
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { GetUnder21BreakdownColumnDefs } from './GetUnder21BreakdownColumnDefs';

const Under21BreakdownGrid: React.FC = () => {
  const under21Breakdown = useSelector((state: RootState) => state.yearsEnd.under21BreakdownByStore);
  const isLoading = !under21Breakdown;

  const columnDefs = GetUnder21BreakdownColumnDefs();
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
          handleSortChanged={(_params) => {}}
          providedOptions={{
            rowData: under21Breakdown?.response?.results || [],
            columnDefs
          }}
        />
      </Grid2>
    </Grid2>
  );
};

export default Under21BreakdownGrid; 