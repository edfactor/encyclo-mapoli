import { Typography } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { useCallback, useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { useLazyGetBreakdownByStoreQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import { GetStoreManagementGridColumns } from "./StoreManagementGridColumns";

interface StoreManagementGridProps {
  store: number;
  pageNumberReset: boolean;
  setPageNumberReset: (reset: boolean) => void;
}

const StoreManagementGrid: React.FC<StoreManagementGridProps> = ({ store, pageNumberReset, setPageNumberReset }) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });

  const [fetchStoreManagement, { isFetching }] = useLazyGetBreakdownByStoreQuery();
  const storeManagement = useSelector((state: RootState) => state.yearsEnd.storeManagementBreakdown);
  const queryParams = useSelector((state: RootState) => state.yearsEnd.breakdownByStoreQueryParams);
  const navigate = useNavigate();
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);

  const handleNavigation = useCallback(
    (path: string) => {
      navigate(path);
    },
    [navigate]
  );

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const profitYear = useDecemberFlowProfitYear();

  const fetchData = useCallback(() => {
    const params = {
      profitYear: queryParams?.profitYear || profitYear,
      storeNumber: store,
      storeManagement: true,
      badgeNumber: queryParams?.badgeNumber,
      employeeName: queryParams?.employeeName,
      pagination: {
        skip: pageNumber * pageSize,
        take: pageSize,
        sortBy: sortParams.sortBy,
        isSortDescending: sortParams.isSortDescending
      }
    };
    if (hasToken) {
      fetchStoreManagement(params);
    }
  }, [
    fetchStoreManagement,
    hasToken,
    pageNumber,
    pageSize,
    profitYear,
    queryParams?.profitYear,
    sortParams.isSortDescending,
    sortParams.sortBy,
    store
  ]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  useEffect(() => {
    if (pageNumberReset) {
      setPageNumber(0);
      setPageNumberReset(false);
    }
  }, [pageNumberReset, setPageNumberReset]);

  const columnDefs = useCallback(() => GetStoreManagementGridColumns(handleNavigation), [handleNavigation])();

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
          isLoading={isFetching}
          handleSortChanged={sortEventHandler}
          providedOptions={{
            rowData: storeManagement?.response?.results || [],
            columnDefs: columnDefs
          }}
        />
        {storeManagement?.response?.results && storeManagement.response.results.length > 0 && (
          <Pagination
            pageNumber={pageNumber}
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
