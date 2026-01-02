import { ColDef, ITooltipParams } from "ag-grid-community";
import type { ProfitSharingAdjustmentRowDto } from "../../../reduxstore/types";
import { createCurrencyColumn, createTaxCodeColumn, createYearColumn } from "../../../utils/gridColumnFactory";
import { AdjustmentStatusCellRenderer } from "./AdjustmentStatusCellRenderer";

/**
 * Helper to parse string to number or keep old value
 */
const toNumberOrOld = (params: { newValue: string | number; oldValue: number }): number => {
  if (typeof params.newValue === "number") {
    return params.newValue;
  }
  const parsed = Number.parseFloat(params.newValue);
  return Number.isFinite(parsed) ? parsed : params.oldValue;
};

/**
 * Helper to determine if a row is a draft insert row (editable)
 */
const isDraftInsertRow = (data: ProfitSharingAdjustmentRowDto | undefined): boolean => {
  return data?.profitDetailId === undefined || data.profitDetailId === null;
};

/**
 * Tooltip for reversed rows - shows explanation when hovering
 */
const reversedRowTooltip = (params: ITooltipParams): string | undefined => {
  const data = params.data as ProfitSharingAdjustmentRowDto | undefined;
  if (data?.hasBeenReversed) {
    return "This row has already been reversed and cannot be adjusted again";
  }
  return undefined;
};

/**
 * Returns column definitions for the Profit Sharing Adjustments grid.
 * Formatted to match Master Inquiry profit details grid standards.
 */
export const GetProfitSharingAdjustmentsGridColumns = (): ColDef[] => {
  return [
    {
      headerName: "",
      field: "status",
      colId: "status",
      width: 50,
      minWidth: 50,
      maxWidth: 50,
      pinned: "left",
      cellRenderer: AdjustmentStatusCellRenderer,
      suppressHeaderMenuButton: true,
      sortable: false,
      resizable: false
    },
    {
      headerName: "Row",
      valueGetter: "node.rowIndex + 1",
      sortable: false,
      filter: false,
      editable: false,
      width: 70,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: false,
      tooltipValueGetter: reversedRowTooltip
    },
    {
      ...createYearColumn({
        headerName: "Profit Year",
        field: "profitYear"
      }),
      sortable: false,
      filter: false,
      editable: false,
      minWidth: 115,
      tooltipValueGetter: reversedRowTooltip,
      valueFormatter: (params) => {
        const year = params.data.profitYear;
        const iter = params.data.profitYearIteration;
        if (year === undefined || year === null) return "";
        if (iter === undefined || iter === null) return `${year}`;
        return `${year}.${iter}`;
      }
    },
    {
      headerName: "Profit Code",
      field: "profitCodeId",
      sortable: false,
      filter: false,
      editable: false,
      minWidth: 250,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: (params) => {
        const id = params.data.profitCodeId;
        const name = params.data.profitCodeName;
        if (id === undefined || id === null) return "";
        if (!name) return `[${id}]`;
        return `[${id}] ${name}`;
      },
      tooltipValueGetter: (params) => {
        const name = params.data.profitCodeName;
        return name || "";
      }
    },
    {
      ...createCurrencyColumn({
        headerName: "Contribution",
        field: "contribution"
      }),
      editable: (params) => isDraftInsertRow(params.data as ProfitSharingAdjustmentRowDto | undefined),
      valueParser: toNumberOrOld,
      tooltipValueGetter: reversedRowTooltip
    },
    {
      ...createCurrencyColumn({
        headerName: "Earnings",
        field: "earnings"
      }),
      editable: (params) => isDraftInsertRow(params.data as ProfitSharingAdjustmentRowDto | undefined),
      valueParser: toNumberOrOld,
      tooltipValueGetter: reversedRowTooltip
    },
    {
      ...createCurrencyColumn({
        headerName: "Forfeiture",
        field: "forfeiture"
      }),
      editable: (params) => isDraftInsertRow(params.data as ProfitSharingAdjustmentRowDto | undefined),
      valueParser: toNumberOrOld,
      tooltipValueGetter: reversedRowTooltip
    },
    {
      headerName: "Activity Date",
      field: "activityDate",
      sortable: false,
      filter: false,
      editable: false,
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      tooltipValueGetter: reversedRowTooltip
    },
    {
      ...createCurrencyColumn({
        headerName: "Payment",
        field: "payment"
      }),
      editable: false,
      tooltipValueGetter: reversedRowTooltip
    },
    {
      ...createCurrencyColumn({
        headerName: "Federal Tax",
        field: "federalTaxes"
      }),
      minWidth: 120,
      editable: false,
      tooltipValueGetter: reversedRowTooltip
    },
    {
      ...createCurrencyColumn({
        headerName: "State Tax",
        field: "stateTaxes"
      }),
      minWidth: 120,
      editable: false,
      tooltipValueGetter: reversedRowTooltip
    },
    {
      ...createTaxCodeColumn(),
      editable: false,
      tooltipValueGetter: reversedRowTooltip
    },
    {
      headerName: "Comment",
      field: "comment",
      sortable: false,
      filter: false,
      editable: false,
      flex: 1,
      minWidth: 160,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      tooltipValueGetter: reversedRowTooltip
    }
  ];
};
