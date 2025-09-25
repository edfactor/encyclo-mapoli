import { Typography } from "@mui/material";
import { DSMGrid, Pagination } from "smart-ui-library";
import { Path, useNavigate } from "react-router";
import { GetProfitShareGrossReportColumns } from "./ProfitShareGrossReportColumns";
import { useLazyGetGrossWagesReportQuery } from "reduxstore/api/YearsEndApi";
import { useCallback, useEffect, useMemo } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { GrossWagesReportDto } from "reduxstore/types";
import { useGridPagination } from "../../hooks/useGridPagination";

const totalsRow = {
  psWages: 0,
  psAmount: 0,
  loans: 0,
  forfeitures: 0
};

interface ProfitShareGrossReportGridProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  pageNumberReset: boolean;
  setPageNumberReset: (reset: boolean) => void;
}

const ProfitShareGrossReportGrid: React.FC<ProfitShareGrossReportGridProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
  pageNumberReset,
  setPageNumberReset
}) => {
  const { grossWagesReport, grossWagesReportQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearch, { isFetching }] = useLazyGetGrossWagesReportQuery();
  const navigate = useNavigate();

  const { pageNumber, pageSize, sortParams, handlePaginationChange, handleSortChange, resetPagination } = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "badgeNumber",
    initialSortDescending: false,
    onPaginationChange: useCallback(async (pageNum: number, pageSz: number, sortPrms: any) => {
      if (initialSearchLoaded) {
        const request: GrossWagesReportDto = {
          profitYear: grossWagesReportQueryParams?.profitYear ?? 0,
          pagination: {
            skip: pageNum * pageSz,
            take: pageSz,
            sortBy: sortPrms.sortBy,
            isSortDescending: sortPrms.isSortDescending
          },
          minGrossAmount: grossWagesReportQueryParams?.minGrossAmount ?? 0
        };
        await triggerSearch(request, false);
      }
    }, [initialSearchLoaded, grossWagesReportQueryParams?.profitYear, grossWagesReportQueryParams?.minGrossAmount, triggerSearch])
  });

  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  const columnDefs = useMemo(
    () => GetProfitShareGrossReportColumns(handleNavigationForButton),
    [handleNavigationForButton]
  );

  const onSearch = useCallback(async () => {
    const request: GrossWagesReportDto = {
      profitYear: grossWagesReportQueryParams?.profitYear ?? 0,
      pagination: {
        skip: pageNumber * pageSize,
        take: pageSize,
        sortBy: sortParams.sortBy,
        isSortDescending: sortParams.isSortDescending
      },
      minGrossAmount: grossWagesReportQueryParams?.minGrossAmount ?? 0
    };

    await triggerSearch(request, false);
  }, [
    pageNumber,
    pageSize,
    triggerSearch,
    grossWagesReportQueryParams?.profitYear,
    grossWagesReportQueryParams?.minGrossAmount,
    sortParams
  ]);

  useEffect(() => {
    if (initialSearchLoaded) {
      onSearch();
    }
  }, [initialSearchLoaded, onSearch]);

  useEffect(() => {
    if (pageNumberReset) {
      resetPagination();
      setPageNumberReset(false);
    }
  }, [pageNumberReset, setPageNumberReset, resetPagination]);

  return (
    <>
      {!!grossWagesReport && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`PROFIT SHARE GROSS REPORT (QPAY501) (${grossWagesReport?.response.total || 0} ${grossWagesReport?.response.total === 1 ? "Record" : "Records"})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"PROFIT_SHARE_GROSS_REPORT"}
            isLoading={isFetching}
            handleSortChanged={handleSortChange}
            providedOptions={{
              rowData: grossWagesReport?.response.results,
              pinnedTopRowData: [
                {
                  grossWages: grossWagesReport?.totalGrossWages,
                  profitSharingAmount: grossWagesReport?.totalProfitSharingAmount,
                  loans: grossWagesReport?.totalLoans,
                  forfeitures: grossWagesReport?.totalForfeitures
                }
              ],
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {!!grossWagesReport && grossWagesReport.response.results.length && (
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
          recordCount={grossWagesReport.response.total}
        />
      )}
    </>
  );
};

export default ProfitShareGrossReportGrid;
