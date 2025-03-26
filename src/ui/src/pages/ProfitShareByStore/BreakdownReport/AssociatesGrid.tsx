import { Typography } from "@mui/material";
import { DSMGrid } from "smart-ui-library";
import { useMemo, useEffect } from "react";
import Grid2 from '@mui/material/Grid2';
import { useLazyGetBreakdownByStoreQuery } from "reduxstore/api/YearsEndApi";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";

interface AssociatesGridProps {
  store: string;
}

const AssociatesGrid: React.FC<AssociatesGridProps> = ({ store }) => {
  const [fetchBreakdownByStore, { isLoading }] = useLazyGetBreakdownByStoreQuery();
  const breakdownByStore = useSelector((state: RootState) => state.yearsEnd.breakdownByStore);
  const queryParams = useSelector((state: RootState) => state.yearsEnd.breakdownByStoreQueryParams);

  useEffect(() => {
    if (queryParams) {
      fetchBreakdownByStore(queryParams);
    } else {
      fetchBreakdownByStore({
        profitYear: 2024,
        storeNumber: store,
        under21Only: true,
        isSortDescending: true,
        pagination: {
          take: 255,
          skip: 0
        }
      });
    }
  }, [fetchBreakdownByStore, store, queryParams]);

  const columnDefs = useMemo(() => [
    {
      headerName: "Badge",
      field: "badgeNumber",
      width: 100
    },
    {
      headerName: "Employee Name",
      field: "fullName",
      width: 200
    },
    {
      headerName: "Beginning Balance",
      field: "beginningBalance",
      width: 150,
      valueFormatter: (params: any) => {
        return new Intl.NumberFormat('en-US', {
          style: 'currency',
          currency: 'USD'
        }).format(params.value);
      }
    },
    {
      headerName: "Earnings",
      field: "earnings",
      width: 120,
      valueFormatter: (params: any) => {
        return new Intl.NumberFormat('en-US', {
          style: 'currency',
          currency: 'USD'
        }).format(params.value);
      }
    },
    {
      headerName: "Cont",
      field: "contributions",
      width: 120,
      valueFormatter: (params: any) => {
        return new Intl.NumberFormat('en-US', {
          style: 'currency',
          currency: 'USD'
        }).format(params.value);
      }
    },
    {
      headerName: "Forf",
      field: "forfeiture",
      width: 120,
      valueFormatter: (params: any) => {
        return new Intl.NumberFormat('en-US', {
          style: 'currency',
          currency: 'USD'
        }).format(params.value);
      }
    },
    {
      headerName: "Dist",
      field: "distributions",
      width: 120,
      valueFormatter: (params: any) => {
        return new Intl.NumberFormat('en-US', {
          style: 'currency',
          currency: 'USD'
        }).format(params.value);
      }
    },
    {
      headerName: "Ending Balance",
      field: "endingBalance",
      width: 150,
      valueFormatter: (params: any) => {
        return new Intl.NumberFormat('en-US', {
          style: 'currency',
          currency: 'USD'
        }).format(params.value);
      }
    },
    {
      headerName: "Ending Balance",
      field: "endingBalance",
      width: 150,
      valueFormatter: (params: any) => {
        return new Intl.NumberFormat('en-US', {
          style: 'currency',
          currency: 'USD'
        }).format(params.value);
      }
    },
    {
      headerName: "Vested Amount",
      field: "vestedAmount",
      width: 150,
      valueFormatter: (params: any) => {
        return new Intl.NumberFormat('en-US', {
          style: 'currency',
          currency: 'USD'
        }).format(params.value);
      }
    }
  ], []);

  return (
    <Grid2 container direction="column" width="100%">
      <Grid2 paddingX="24px">
        <Typography
          variant="h6"
          sx={{ color: "#0258A5", marginBottom: "16px" }}>
          Associates
        </Typography>
      </Grid2>
      <Grid2 width="100%">
        <DSMGrid
          preferenceKey={`BREAKDOWN_REPORT_ASSOCIATES_STORE_${store}`}
          isLoading={isLoading}
          handleSortChanged={(_params) => {}}
          providedOptions={{
            rowData: breakdownByStore?.response?.results || [],
            columnDefs: columnDefs
          }}
        />
      </Grid2>
    </Grid2>
  );
};

export default AssociatesGrid; 