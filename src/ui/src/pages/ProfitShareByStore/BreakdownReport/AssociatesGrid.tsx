import { Typography } from "@mui/material";
import { DSMGrid, ISortParams, Pagination, agGridNumberToCurrency } from "smart-ui-library";
import { useMemo, useEffect, useState, useCallback } from "react";
import Grid2 from "@mui/material/Grid2";
import { useLazyGetBreakdownByStoreQuery } from "reduxstore/api/YearsEndApi";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { ICellRendererParams } from "ag-grid-community";
import { viewBadgeLinkRenderer } from "../../../utils/masterInquiryLink";
import { useNavigate } from "react-router-dom";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";

interface AssociatesGridProps {
  store: number;
}

const AssociatesGrid: React.FC<AssociatesGridProps> = ({ store }) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });

  const [fetchBreakdownByStore, { isFetching }] = useLazyGetBreakdownByStoreQuery();
  const breakdownByStore = useSelector((state: RootState) => state.yearsEnd.breakdownByStore);
  const queryParams = useSelector((state: RootState) => state.yearsEnd.breakdownByStoreQueryParams);
  const navigate = useNavigate();
  const profitYear = useDecemberFlowProfitYear();

  const handleNavigation = useCallback(
    (path: string) => {
      navigate(path);
    },
    [navigate]
  );

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  
  const fetchData = useCallback(() => {
    const params = {
      profitYear: queryParams?.profitYear || profitYear,
      storeNumber: store,
      storeManagement: false,
      pagination: {
        skip: pageNumber * pageSize,
        take: pageSize,
        sortBy: sortParams.sortBy,
        isSortDescending: sortParams.isSortDescending
      }
    };
    fetchBreakdownByStore(params);
  }, [fetchBreakdownByStore, pageNumber, pageSize, profitYear, queryParams?.profitYear, sortParams.isSortDescending, sortParams.sortBy, store]);

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
        field: "payClassificationName",
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
        field: "forfeitures",
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
          Associates
        </Typography>
      </Grid2>
      <Grid2 width="100%">
        <DSMGrid
          preferenceKey={`BREAKDOWN_REPORT_ASSOCIATES_STORE_${store}`}
          isLoading={isFetching}
          handleSortChanged={sortEventHandler}
          providedOptions={{
            rowData: breakdownByStore?.response?.results || [],
            columnDefs: columnDefs
          }}
        />
        {breakdownByStore?.response?.results && breakdownByStore.response.results.length > 0 && (
          <Pagination
            pageNumber={pageNumber}
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
