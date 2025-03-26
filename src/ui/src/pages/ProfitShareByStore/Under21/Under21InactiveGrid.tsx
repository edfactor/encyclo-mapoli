import React from 'react';
import { Typography } from "@mui/material";
import { DSMGrid } from "smart-ui-library";
import Grid2 from '@mui/material/Grid2';
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { under21InactiveColumnDefs } from './GetUnder21BreakdownColumnDefs';

const Under21InactiveGrid: React.FC = () => {
  const under21Inactive = useSelector((state: RootState) => state.yearsEnd.under21Inactive);
  const isLoading = !under21Inactive;

  return (
    <Grid2 container direction="column" width="100%">
      <Grid2 paddingX="24px">
        <Typography
          variant="h6"
          sx={{ color: "#0258A5", marginBottom: "16px" }}>
          Inactive Under 21
        </Typography>
      </Grid2>
      <Grid2 width="100%">
        <DSMGrid
          preferenceKey="UNDER_21_INACTIVE_REPORT"
          isLoading={isLoading}
          handleSortChanged={(_params) => {}}
          providedOptions={{
            rowData: under21Inactive?.response?.results || [],
            columnDefs: under21InactiveColumnDefs
          }}
        />
      </Grid2>
    </Grid2>
  );
};

export default Under21InactiveGrid; 