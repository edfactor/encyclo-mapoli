import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo } from "react";
import { useSelector } from "react-redux";
import { useLazyGetGrossWagesReportQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { GrossWagesReportDto } from "reduxstore/types";
import { TotalsGrid, numberToCurrency } from "smart-ui-library";
import { DSMPaginatedGrid } from "../../../components/DSMPaginatedGrid/DSMPaginatedGrid";
import { GRID_KEYS } from "../../../constants";
import { useContentAwareGridHeight } from "../../../hooks/useContentAwareGridHeight";
import { SortParams, useGridPagination } from "../../../hooks/useGridPagination";
import { GetProfitShareGrossReportColumns } from "./ProfitShareGrossReportColumns";

interface ProfitShareGrossReportGridProps {
  pageNumberReset: boolean;
  setPageNumberReset: (reset: boolean) => void;
}

const ProfitShareGrossReportGrid: React.FC<ProfitShareGrossReportGridProps> = ({
  pageNumberReset,
  setPageNumberReset
}) => {
  const { grossWagesReport, grossWagesReportQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearch, { isFetching }] = useLazyGetGrossWagesReportQuery();

  // Use content-aware grid height utility hook
  const gridMaxHeight = useContentAwareGridHeight({
    rowCount: grossWagesReport?.response?.results?.length ?? 0
  });

  const { pageNumber, pageSize, handlePageNumberChange, handlePageSizeChange, handleSortChange, resetPagination } =
    useGridPagination({
      initialPageSize: 25,
      initialSortBy: "BadgeNumber",
      initialSortDescending: false,
      persistenceKey: GRID_KEYS.PROFIT_SHARE_GROSS_REPORT,
      onPaginationChange: useCallback(
        async (pageNum: number, pageSz: number, sortPrms: SortParams) => {
          // Match the behavior of Duplicate Names & Birthdays: once search params exist,
          // page/sort changes trigger a server call.
          if (!grossWagesReportQueryParams?.profitYear) {
            return;
          }

          const request: GrossWagesReportDto = {
            profitYear: grossWagesReportQueryParams.profitYear,
            pagination: {
              skip: pageNum * pageSz,
              take: pageSz,
              sortBy: sortPrms.sortBy,
              isSortDescending: sortPrms.isSortDescending
            },
            minGrossAmount: grossWagesReportQueryParams.minGrossAmount ?? 0
          };

          await triggerSearch(request, false);
        },
        [grossWagesReportQueryParams?.profitYear, grossWagesReportQueryParams?.minGrossAmount, triggerSearch]
      )
    });

  const columnDefs = useMemo(() => GetProfitShareGrossReportColumns(), []);

  useEffect(() => {
    if (pageNumberReset) {
      resetPagination();
      setPageNumberReset(false);
    }
  }, [pageNumberReset, setPageNumberReset, resetPagination]);

  return (
    <div className="relative">
      {!!grossWagesReport && (
        <>
          <div className="px-6">
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`PROFIT SHARE GROSS REPORT (QPAY501) (${(grossWagesReport?.response.total || 0).toLocaleString()} ${grossWagesReport?.response.total === 1 ? "Record" : "Records"})`}
            </Typography>
          </div>
          {grossWagesReport && (
            <TotalsGrid
              breakpoints={{ xs: 12, sm: 6, md: 3, lg: 3, xl: 3 }}
              tablePadding="0px"
              displayData={[
                [numberToCurrency(grossWagesReport?.totalGrossWages || 0)],
                [numberToCurrency(grossWagesReport?.totalProfitSharingAmount || 0)],
                [numberToCurrency(grossWagesReport?.totalLoans || 0)],
                [numberToCurrency(grossWagesReport?.totalForfeitures || 0)]
              ]}
              leftColumnHeaders={["Gross Wages", "Profit Sharing", "Loans", "Forfeitures"]}
              topRowHeaders={["Totals"]}
            />
          )}
          <DSMPaginatedGrid
            preferenceKey={GRID_KEYS.PROFIT_SHARE_GROSS_REPORT}
            data={grossWagesReport?.response.results ?? []}
            columnDefs={columnDefs}
            totalRecords={grossWagesReport?.response.total ?? 0}
            isLoading={isFetching}
            onSortChange={handleSortChange}
            heightConfig={{ maxHeight: gridMaxHeight }}
            pagination={{
              pageNumber,
              pageSize,
              sortParams: { sortBy: "", isSortDescending: false },
              handlePageNumberChange,
              handlePageSizeChange,
              handleSortChange: () => {}
            }}
            showPagination={!!grossWagesReport && grossWagesReport.response.results.length > 0}
            gridOptions={{
              suppressHorizontalScroll: true,
              suppressColumnVirtualisation: true
            }}
          />
        </>
      )}
    </div>
  );
};

export default ProfitShareGrossReportGrid;
