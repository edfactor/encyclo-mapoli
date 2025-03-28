import { Typography } from "@mui/material";
import { DSMGrid, ISortParams, Pagination, agGridNumberToCurrency } from "smart-ui-library";
import { useMemo, useEffect, useState, useCallback } from "react";
import Grid2 from '@mui/material/Grid2';
import { useLazyGetBreakdownByStoreQuery } from "reduxstore/api/YearsEndApi";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { ICellRendererParams } from "ag-grid-community";
import { viewBadgeLinkRenderer } from "../../../utils/masterInquiryLink";
import { useNavigate } from "react-router-dom";

interface AssociatesGridProps {
  store: string;
}

const AssociatesGrid: React.FC<AssociatesGridProps> = ({ store }) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });

  const [fetchBreakdownByStore, { isLoading }] = useLazyGetBreakdownByStoreQuery();
  const breakdownByStore = useSelector((state: RootState) => state.yearsEnd.breakdownByStore);
  const queryParams = useSelector((state: RootState) => state.yearsEnd.breakdownByStoreQueryParams);
  const navigate = useNavigate();

  const handleNavigation = (path: string) => {
    navigate(path);
  };

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  const fetchData = useCallback(() => {
    const params = {
      profitYear: queryParams?.profitYear || 2024,
      storeNumber: store,
      under21Only: true,
      isSortDescending: sortParams.isSortDescending,
      pagination: {
        take: pageSize,
        skip: pageNumber * pageSize
      }
    };
    fetchBreakdownByStore(params);
  }, [fetchBreakdownByStore, pageNumber, pageSize, queryParams?.profitYear, sortParams.isSortDescending, store]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  const columnDefs = useMemo(() => [
    {
      headerName: "Badge",
      field: "badgeNumber",
      width: 100,
      cellRenderer: (params: ICellRendererParams) => viewBadgeLinkRenderer(params.data.badgeNumber, handleNavigation)
    },
    {
      headerName: "Employee Name",
      field: "fullName",
      width: 200
    },
    {
      headerName: "Position",
      field: "position",
      width: 120
    },
    {
      headerName: "Beginning Balance",
      field: "beginningBalance",
      width: 150,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Earnings",
      field: "earnings",
      width: 120,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Cont",
      field: "contributions",
      width: 120,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Forf",
      field: "forfeiture",
      width: 120,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Dist",
      field: "distributions",
      width: 120,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Ending Balance",
      field: "endingBalance",
      width: 150,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Vested Amount",
      field: "vestedAmount",
      width: 150,
      valueFormatter: agGridNumberToCurrency
    }
  ], [handleNavigation]);

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
          handleSortChanged={sortEventHandler}
          providedOptions={{
            rowData: breakdownByStore?.response?.results || [],
            columnDefs: columnDefs
          }}
        />
        {breakdownByStore?.response?.results && breakdownByStore.response.results.length > 0 && (
          <Pagination
            pageNumber={pageNumber + 1}
            setPageNumber={(value: number) => setPageNumber(value - 1)}
            pageSize={pageSize}
            setPageSize={setPageSize}
            recordCount={breakdownByStore.response.total || 0}
          />
        )}
      </Grid2>
    </Grid2>
  );
};

export default AssociatesGrid; 