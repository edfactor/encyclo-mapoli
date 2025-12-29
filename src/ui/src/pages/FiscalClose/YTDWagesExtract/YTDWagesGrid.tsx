import FullscreenIcon from "@mui/icons-material/Fullscreen";
import FullscreenExitIcon from "@mui/icons-material/FullscreenExit";
import { Box, IconButton, Typography } from "@mui/material";
import { RowClassParams } from "ag-grid-community";
import { RefObject, useMemo } from "react";
import { numberToCurrency } from "smart-ui-library";
import DSMPaginatedGrid from "../../../components/DSMPaginatedGrid/DSMPaginatedGrid";
import ReportSummary from "../../../components/ReportSummary";
import { GRID_KEYS } from "../../../constants";
import { useContentAwareGridHeight } from "../../../hooks/useContentAwareGridHeight";
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

  // Calculate totals for pinned top row - use API totals if available, otherwise calculate
  const totalsRow = useMemo(() => {
    if (!clonedData?.response?.results) return null;

    // Check if API provided totals
    const totalHours =
      clonedData.totalHoursCurrentYearWages ??
      clonedData.response.results.reduce((sum, row) => sum + (row.hoursCurrentYear || 0), 0);

    const totalIncome =
      clonedData.totalIncomeCurrentYearWages ??
      clonedData.response.results.reduce((sum, row) => sum + (row.incomeCurrentYear || 0), 0);

    return {
      badgeNumber: "TOTALS",
      hoursCurrentYear: totalHours,
      incomeCurrentYear: totalIncome,
      isExecutive: false,
      storeNumber: 0
    };
  }, [clonedData?.response?.results, clonedData?.totalHoursCurrentYearWages, clonedData?.totalIncomeCurrentYearWages]);

  // Use content-aware grid height utility hook
  const gridMaxHeight = useContentAwareGridHeight({
    rowCount: clonedData?.response?.results?.length ?? 0,
    heightPercentage: isGridExpanded ? 0.85 : 0.5
  });

  // Header content with title and totals (when not expanded)
  const headerContent = useMemo(
    () => (
      <Box
        display="flex"
        alignItems="center"
        gap={3}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          {clonedData?.reportName || "YTD Wages Extract"}
        </Typography>
        {!isGridExpanded && (
          <>
            <Box
              display="flex"
              alignItems="center"
              gap={1}>
              <Typography
                variant="body2"
                fontWeight="semibold">
                Total Hours:
              </Typography>
              <Typography variant="body2">
                {clonedData?.totalHoursCurrentYearWages?.toFixed(2) ?? totalsRow?.hoursCurrentYear.toFixed(2) ?? "0.00"}
              </Typography>
            </Box>
            <Box
              display="flex"
              alignItems="center"
              gap={1}>
              <Typography
                variant="body2"
                fontWeight="semibold">
                Total Income:
              </Typography>
              <Typography variant="body2">
                {numberToCurrency(clonedData?.totalIncomeCurrentYearWages ?? totalsRow?.incomeCurrentYear ?? 0)}
              </Typography>
            </Box>
          </>
        )}
      </Box>
    ),
    [
      clonedData?.reportName,
      clonedData?.totalHoursCurrentYearWages,
      clonedData?.totalIncomeCurrentYearWages,
      isGridExpanded,
      totalsRow
    ]
  );

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
        showPagination={!isGridExpanded && hasResults}
        onSortChange={onSortChange}
        heightConfig={{ maxHeight: gridMaxHeight }}
        header={headerContent}
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
        beforeGrid={!isGridExpanded ? <ReportSummary report={clonedData} /> : undefined}
        gridOptions={{
          pinnedTopRowData: totalsRow ? [totalsRow] : [],
          getRowStyle: (params: RowClassParams) => {
            if (params.node.rowPinned) {
              return { background: "#f0f0f0", fontWeight: "bold" };
            }
            return undefined;
          }
        }}
      />
    </div>
  );
};

export default YTDWagesGrid;
