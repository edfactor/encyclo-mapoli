import { ColDef, ValueFormatterParams } from "ag-grid-community";
import { GRID_COLUMN_WIDTHS } from "../constants";

export interface SSNColumnOptions {
  headerName?: string;
  minWidth?: number;
  maxWidth?: number;
  alignment?: "left" | "center";
  sortable?: boolean;
  resizable?: boolean;
  valueFormatter?: (params: ValueFormatterParams) => string;
}

export const createSSNColumn = (options: SSNColumnOptions = {}): ColDef => {
  const {
    headerName = "SSN",
    minWidth = GRID_COLUMN_WIDTHS.SSN,
    maxWidth,
    alignment = "center",
    sortable = true,
    resizable = true,
    valueFormatter
  } = options;

  const alignmentClass = alignment === "center" ? "center-align" : "left-align";

  const column: ColDef = {
    headerName,
    field: "ssn",
    colId: "ssn",
    minWidth,
    headerClass: alignmentClass,
    cellClass: alignmentClass,
    resizable,
    sortable
  };

  if (maxWidth) {
    column.maxWidth = maxWidth;
  }

  if (valueFormatter) {
    column.valueFormatter = valueFormatter;
  }

  return column;
};