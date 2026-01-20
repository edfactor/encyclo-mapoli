import FullscreenIcon from "@mui/icons-material/Fullscreen";
import FullscreenExitIcon from "@mui/icons-material/FullscreenExit";
import { IconButton, Typography } from "@mui/material";
import { RefObject, useMemo } from "react";
import { numberToCurrency } from "smart-ui-library";
import DSMPaginatedGrid from "../../../components/DSMPaginatedGrid/DSMPaginatedGrid";
import ReportSummary from "../../../components/ReportSummary";
import { GRID_KEYS } from "../../../constants";
import { GridPaginationActions, GridPaginationState, SortParams } from "../../../hooks/useGridPagination";
import { EmployeeWagesForYearResponse } from "../../../reduxstore/types";
import { GetYTDWagesColumns } from "./YTDWagesGridColumns";

interface YTDWagesGridProps {
  innerRef: RefObject<HTMLDivElement | null>;
  data: EmployeeWagesForYearResponse | null;
  isLoading: boolean;
  showData: boolean;
  hasResults: boolean;
  pagination: GridPaginationState & GridPaginationActions;
  onSortChange: (sortParams: SortParams) => void;
  isGridExpanded?: boolean;
  onToggleExpand?: () => void;
}

const YTDWagesGrid = ({
  innerRef,
  data,
  isLoading,
  showData,
  hasResults,
  pagination,
  onSortChange,
  isGridExpanded = false,
  onToggleExpand
}: YTDWagesGridProps) => {
  // I need to clone the data object, but alter the reportName. I need to keep the year from the end
  // of the existing report name, but add "YTD Wages Extract " to the front.
  const clonedData = data ? ({ ...data } as EmployeeWagesForYearResponse) : null;
  const year = clonedData?.reportName?.match(/\d{4}$/)?.[0];
  if (clonedData && year) {
    clonedData.reportName = `YTD Wages Extract ${year}`;
  }

  const columnDefs = useMemo(() => GetYTDWagesColumns(), []);

  // Calculate totals - use API totals if available, otherwise calculate from results
  const totalHours = useMemo(() => {
    if (!clonedData?.response?.results) return 0;
    return (
      clonedData.totalHoursCurrentYearWages ??
      clonedData.response.results.reduce((sum, row) => sum + (row.hoursCurrentYear || 0), 0)
    );
  }, [clonedData?.response?.results, clonedData?.totalHoursCurrentYearWages]);

  const totalIncome = useMemo(() => {
    if (!clonedData?.response?.results) return 0;
    return (
      clonedData.totalIncomeCurrentYearWages ??
      clonedData.response.results.reduce((sum, row) => sum + (row.incomeCurrentYear || 0), 0)
    );
  }, [clonedData?.response?.results, clonedData?.totalIncomeCurrentYearWages]);

  const formatHoursTotal = (value: number | string) => {
    if (typeof value === "number" && Number.isFinite(value)) {
      return value.toLocaleString(undefined, {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
      });
    }

    return value;
  };

  const formatIncomeTotal = (value: number | string) => {
    if (typeof value === "number" && Number.isFinite(value)) {
      return numberToCurrency(value);
    }

    return value;
  };

  if (!showData || !clonedData?.response) {
    return null;
  }

  return (
    <div ref={innerRef}>
      <DSMPaginatedGrid
        preferenceKey={GRID_KEYS.YTD_WAGES}
        data={clonedData.response.results}
        columnDefs={columnDefs}
        isLoading={isLoading}
        pagination={pagination}
        totalRecords={hasResults ? (data?.response?.total ?? 0) : 0}
        showPagination={hasResults}
        onSortChange={onSortChange}
        heightConfig={{
          mode: "content-aware",
          heightPercentage: isGridExpanded ? 0.85 : 0.5
        }}
        headerActions={
          onToggleExpand ? (
            <IconButton
              onClick={onToggleExpand}
              sx={{ zIndex: 1 }}
              aria-label={isGridExpanded ? "Exit fullscreen" : "Enter fullscreen"}>
              {isGridExpanded ? <FullscreenExitIcon /> : <FullscreenIcon />}
            </IconButton>
          ) : null
        }
        beforeGrid={
          !isGridExpanded ? (
            <>
              <div style={{ marginTop: "-18px" }}>
                <ReportSummary report={clonedData} />
              </div>
              <div style={{ marginTop: "4px", marginBottom: "12px" }}>
                <Typography
                  variant="h6"
                  sx={{ mb: 1, px: 3 }}>
                  Totals
                </Typography>
                {/* One-off inline styles for this simple table. If we reuse this pattern, move to a shared CSS module. */}
                <style>{`
                  .ytd-wages-totals-table {
                    width: 100%;
                    border-collapse: collapse;
                  }
                  .ytd-wages-totals-table thead tr {
                    background-color: #E8E8E8;
                  }
                  .ytd-wages-totals-table th,
                  .ytd-wages-totals-table td {
                    padding: 0.5rem 1rem;
                    text-align: right;
                    font-size: 0.875rem;
                  }
                  .ytd-wages-totals-table th {
                    font-weight: 500;
                  }
                `}</style>
                <div className="mx-3 rounded border border-gray-300">
                  <table className="ytd-wages-totals-table">
                    <thead>
                      <tr>
                        <th>Total Income</th>
                        <th>Total Hours</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr>
                        <td>{formatIncomeTotal(totalIncome)}</td>
                        <td>{formatHoursTotal(totalHours)}</td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
            </>
          ) : undefined
        }
      />
    </div>
  );
};

export default YTDWagesGrid;
