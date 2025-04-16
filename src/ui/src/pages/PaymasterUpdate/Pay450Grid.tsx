import { Typography } from "@mui/material";
import { useCallback, useMemo, useState, useEffect } from "react";
import { useSelector } from "react-redux"; 
import { Path } from "react-router";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetPay450GridColumns } from "./Pay450GridColumns";
import { YearsEndApi } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";

interface Pay450GridProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  profitYear: number;
}

const Pay450Grid: React.FC<Pay450GridProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
  profitYear
}) => {
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "name",
    isSortDescending: false
  });
  
  const { updateSummary } = useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearch, { isLoading }] = YearsEndApi.endpoints.getUpdateSummary.useLazyQuery();

  const onSearch = useCallback(async () => {
    try {
      await triggerSearch({
        profitYear,
        pagination: {
          skip: pageNumber * pageSize,
          take: pageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        }
      });
    } catch (error) {
      console.error("API call failed:", error);
    }
  }, [pageNumber, pageSize, sortParams, triggerSearch, profitYear]);

  useEffect(() => {
    if (initialSearchLoaded && hasToken) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, sortParams, onSearch, hasToken]);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  
  // Mock function to handle navigation (needed for GetPay450GridColumns)
  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      console.log("Navigation to", destination);
    },
    []
  );
  
  const columnDefs = useMemo(() => GetPay450GridColumns(handleNavigationForButton), [handleNavigationForButton]);

  const getSummaryRow = useCallback(() => {
    if (!updateSummary) return [];
    
    return [
      {
        psAmountOriginal: updateSummary.totalBeforeProfitSharingAmount,
        psVestedOriginal: updateSummary.totalBeforeVestedAmount,
        psAmountUpdated: updateSummary.totalAfterProfitSharingAmount,
        psVestedUpdated: updateSummary.totalAfterVestedAmount
      }
    ];
  }, [updateSummary]);

  const gridData = useMemo(() => {
    if (!updateSummary?.response?.results) return [];
    
    return updateSummary.response.results.map(employee => ({
      badge: employee.badgeNumber,
      employeeName: employee.name,
      store: employee.storeNumber,
      psAmountOriginal: employee.before.profitSharingAmount,
      psVestedOriginal: employee.before.vestedProfitSharingAmount,
      yearsOriginal: employee.before.yearsInPlan,
      enrollOriginal: employee.before.enrollmentId,
      psAmountUpdated: employee.after.profitSharingAmount,
      psVestedUpdated: employee.after.vestedProfitSharingAmount,
      yearsUpdated: employee.after.yearsInPlan,
      enrollUpdated: employee.after.enrollmentId
    }));
  }, [updateSummary]);

  return (
    <>
      {updateSummary?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`UPDATE SUMMARY FOR PROFIT SHARING (${updateSummary.response.total || 0} records)`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"ELIGIBLE_EMPLOYEES"}
            isLoading={isLoading}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: gridData,
              pinnedTopRowData: getSummaryRow(),
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {!!updateSummary && updateSummary.response.results.length > 0 && (
        <Pagination
          pageNumber={pageNumber + 1}
          setPageNumber={(value: number) => {
            setPageNumber(value - 1);
            setInitialSearchLoaded(true);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            setPageSize(value);
            setPageNumber(0);
            setInitialSearchLoaded(true);
          }}
          recordCount={updateSummary.response.total}
        />
      )}
    </>
  );
};

export default Pay450Grid;
