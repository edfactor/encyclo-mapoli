import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo } from "react";
import { useSelector } from "react-redux";
import { Path } from "react-router";
import { YearsEndApi } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { Pagination } from "smart-ui-library";
import { DSMGrid } from "../../components/DSMGrid/DSMGrid";
import { SortParams, useGridPagination } from "../../../hooks/useGridPagination";
import { GetPayMasterUpdateGridColumns } from "./PayMasterUpdateGridColumns";

interface PayMasterUpdateGridProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  profitYear: number;
  pageNumberReset: boolean;
  setPageNumberReset: (reset: boolean) => void;
}

const PayMasterUpdateGrid: React.FC<PayMasterUpdateGridProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
  profitYear,
  pageNumberReset,
  setPageNumberReset
}) => {
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const { updateSummary } = useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearch, { isFetching }] = YearsEndApi.endpoints.getUpdateSummary.useLazyQuery();

  const { pageNumber, pageSize, sortParams, handlePaginationChange, handleSortChange, resetPagination } =
    useGridPagination({
      initialPageSize: 25,
      initialSortBy: "name",
      initialSortDescending: false,
      onPaginationChange: useCallback(
        async (pageNum: number, pageSz: number, sortPrms: SortParams) => {
          if (initialSearchLoaded && hasToken) {
            try {
              await triggerSearch({
                profitYear,
                pagination: {
                  skip: pageNum * pageSz,
                  take: pageSz,
                  sortBy: sortPrms.sortBy,
                  isSortDescending: sortPrms.isSortDescending
                }
              });
            } catch (error) {
              console.error("API call failed:", error);
            }
          }
        },
        [initialSearchLoaded, hasToken, profitYear, triggerSearch]
      )
    });

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
  }, [initialSearchLoaded, onSearch, hasToken]);

  useEffect(() => {
    if (pageNumberReset) {
      resetPagination();
      setPageNumberReset(false);
    }
  }, [pageNumberReset, setPageNumberReset, resetPagination]);

  // Mock function to handle navigation (needed for GetPayMasterUpdateGridColumns)
  const handleNavigationForButton = useCallback((destination: string | Partial<Path>) => {
    console.log("Navigation to", destination);
  }, []);

  const columnDefs = useMemo(
    () => GetPayMasterUpdateGridColumns(handleNavigationForButton),
    [handleNavigationForButton]
  );

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

    return updateSummary.response.results.map((employee) => ({
      badgeNumber: employee.badgeNumber,
      employeeName: employee.name,
      storeNumber: employee.storeNumber === 0 ? "-" : employee.storeNumber,
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
            isLoading={isFetching}
            handleSortChanged={handleSortChange}
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
          pageNumber={pageNumber}
          setPageNumber={(value: number) => {
            handlePaginationChange(value - 1, pageSize);
            setInitialSearchLoaded(true);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            handlePaginationChange(0, value);
            setInitialSearchLoaded(true);
          }}
          recordCount={updateSummary.response.total}
        />
      )}
    </>
  );
};

export default PayMasterUpdateGrid;
