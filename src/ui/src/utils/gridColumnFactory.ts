import { ColDef } from "ag-grid-community";
import { GRID_COLUMN_WIDTHS } from "../constants";
import { SSNColumnOptions } from "./columnFactoryTypes";

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