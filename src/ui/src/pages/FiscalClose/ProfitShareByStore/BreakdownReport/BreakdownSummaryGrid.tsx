import { CircularProgress, Grid, Typography } from "@mui/material";
import { ColDef, RowClassParams, ValueFormatterParams } from "ag-grid-community";
import { useMemo } from "react";
import { DSMGrid } from "smart-ui-library";
import { useBreakdownGrandTotals } from "../../../../hooks/useBreakdownGrandTotals";

/** Shared value formatter for numeric columns */
const numberValueFormatter = (params: ValueFormatterParams): string => {
  if (params.value === null || params.value === undefined) return "0";
  return Math.round(Number(params.value)).toLocaleString("en-US");
};

/** Column definitions for the breakdown summary grid */
const BREAKDOWN_COLUMN_DEFS: ColDef[] = [
  {
    headerName: "Category",
    field: "category",
    flex: 1,
    minWidth: 150
  },
  {
    headerName: "STE 1-140",
    field: "ste1",
    flex: 1,
    type: "rightAligned",
    valueFormatter: numberValueFormatter
  },
  {
    headerName: "700",
    field: "700",
    flex: 1,
    type: "rightAligned",
    valueFormatter: numberValueFormatter
  },
  {
    headerName: "701",
    field: "701",
    flex: 1,
    type: "rightAligned",
    valueFormatter: numberValueFormatter
  },
  {
    headerName: "800",
    field: "800",
    flex: 1,
    type: "rightAligned",
    valueFormatter: numberValueFormatter
  },
  {
    headerName: "801",
    field: "801",
    flex: 1,
    type: "rightAligned",
    valueFormatter: numberValueFormatter
  },
  {
    headerName: "802",
    field: "802",
    flex: 1,
    type: "rightAligned",
    valueFormatter: numberValueFormatter
  },
  {
    headerName: "900",
    field: "900",
    flex: 1,
    type: "rightAligned",
    valueFormatter: numberValueFormatter
  },
  {
    headerName: "Total",
    field: "total",
    flex: 1,
    type: "rightAligned",
    valueFormatter: numberValueFormatter
  }
];

/** Grid options for styling pinned rows */
const GRID_OPTIONS = {
  getRowStyle: (params: RowClassParams) => {
    if (params.node.rowPinned) {
      return { background: "#f0f0f0", fontWeight: "bold" };
    }
    return undefined;
  }
};

export interface BreakdownSummaryGridProps {
  /** Title to display above the grid */
  title: string;
  /** Grid preference key for saving column state */
  preferenceKey: string;
  /** Whether to filter for under 21 participants only */
  under21Participants?: boolean;
  /** Error message to display on fetch failure */
  errorMessage?: string;
}

/**
 * Reusable grid component for displaying breakdown summary data by vesting category.
 * Used for both "All Employees" and "Under 21 Employees" views.
 * 
 * @example
 * ```tsx
 * <BreakdownSummaryGrid
 *   title="All Employees (By report section)"
 *   preferenceKey={GRID_KEYS.BREAKDOWN_REPORT_SUMMARY}
 * />
 * 
 * <BreakdownSummaryGrid
 *   title="Under 21 Employees (By report section)"
 *   preferenceKey={GRID_KEYS.UNDER_21_BREAKDOWN_REPORT}
 *   under21Participants
 *   errorMessage="Failed to load Under 21 employee data."
 * />
 * ```
 */
const BreakdownSummaryGrid: React.FC<BreakdownSummaryGridProps> = ({
  title,
  preferenceKey,
  under21Participants = false,
  errorMessage
}) => {
  const { rowData, grandTotal, isLoading, error } = useBreakdownGrandTotals({
    under21Participants,
    errorMessage
  });

  // Memoize provided options to prevent unnecessary re-renders
  const providedOptions = useMemo(
    () => ({
      rowData,
      columnDefs: BREAKDOWN_COLUMN_DEFS,
      domLayout: "autoHeight" as const,
      pinnedBottomRowData: [grandTotal],
      ...GRID_OPTIONS
    }),
    [rowData, grandTotal]
  );

  return (
    <Grid container direction="column" width="100%">
      <Grid paddingX="24px">
        <Typography variant="h2" sx={{ color: "#0258A5", marginBottom: "16px" }}>
          {title}
        </Typography>
      </Grid>
      <Grid width="100%">
        {isLoading ? (
          <Grid sx={{ display: "flex", justifyContent: "center", padding: 4 }}>
            <CircularProgress />
          </Grid>
        ) : error ? (
          <Grid sx={{ display: "flex", justifyContent: "center", padding: 4 }}>
            <Typography color="error">{error}</Typography>
          </Grid>
        ) : (
          <DSMGrid
            preferenceKey={preferenceKey}
            isLoading={false}
            handleSortChanged={() => {}}
            providedOptions={providedOptions}
          />
        )}
      </Grid>
    </Grid>
  );
};

export default BreakdownSummaryGrid;
