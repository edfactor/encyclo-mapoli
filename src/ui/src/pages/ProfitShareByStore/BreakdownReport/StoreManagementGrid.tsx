import { Typography } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { ICellRendererParams } from "ag-grid-community";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { useLazyGetBreakdownByStoreQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination, agGridNumberToCurrency } from "smart-ui-library";
import { viewBadgeLinkRenderer } from "../../../utils/masterInquiryLink";

interface StoreManagementGridProps {
  store: string;
}

const StoreManagementGrid: React.FC<StoreManagementGridProps> = ({ store }) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });

  const [fetchStoreManagement, { isLoading }] = useLazyGetBreakdownByStoreQuery();
  const storeManagement = useSelector((state: RootState) => state.yearsEnd.breakdownByStore);
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
      under21Only: false,
      pagination: { skip: pageNumber * pageSize, take: pageSize, sortBy: sortParams.sortBy, isSortDescending: sortParams.isSortDescending }
    };
    fetchStoreManagement(params);
  }, [fetchStoreManagement, pageNumber, pageSize, queryParams?.profitYear, sortParams, store]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  const columnDefs = useMemo(
    () => [
      {
        headerName: "Badge",
        field: "badgeNumber",
        width: 100,
        cellRenderer: (params: ICellRendererParams) => viewBadgeLinkRenderer(params.data.badgeNumber, handleNavigation)
      },
      {
        headerName: "Name",
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
        headerName: "Contributions",
        field: "contributions",
        width: 120,
        valueFormatter: agGridNumberToCurrency
      },
      {
        headerName: "Forfeiture",
        field: "forfeiture",
        width: 120,
        valueFormatter: agGridNumberToCurrency
      },
      {
        headerName: "Distributions",
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
    ],
    [handleNavigation]
  );

  return (
    <Grid2
      container
      direction="column"
      width="100%">
      <Grid2 paddingX="24px">
        <Typography
          variant="h6"
          sx={{ color: "#0258A5", marginBottom: "16px" }}>
          Store Management
        </Typography>
      </Grid2>
      <Grid2 width="100%">
        <DSMGrid
          preferenceKey={`STORE_MANAGEMENT_STORE_${store}`}
          isLoading={isLoading}
          handleSortChanged={sortEventHandler}
          providedOptions={{
            rowData: storeManagement?.response?.results || [],
            columnDefs: columnDefs
          }}
        />
        {storeManagement?.response?.results && storeManagement.response.results.length > 0 && (
          <Pagination
            pageNumber={pageNumber + 1}
            setPageNumber={(value: number) => setPageNumber(value - 1)}
            pageSize={pageSize}
            setPageSize={setPageSize}
            recordCount={storeManagement.response.total || 0}
          />
        )}
      </Grid2>
    </Grid2>
  );
};

export default StoreManagementGrid;
