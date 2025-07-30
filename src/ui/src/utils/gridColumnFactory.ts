import { ColDef, ICellRendererParams } from "ag-grid-community";
import { agGridNumberToCurrency } from "smart-ui-library";
import { GRID_COLUMN_WIDTHS } from "../constants";
import { BadgeColumnOptions, SSNColumnOptions, CurrencyColumnOptions } from "./columnFactoryTypes";
import { viewBadgeLinkRenderer } from "./masterInquiryLink";

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

export const createBadgeColumn = (options: BadgeColumnOptions = {}): ColDef => {
  const {
    headerName = "Badge",
    minWidth = 120,
    maxWidth,
    alignment = "center",
    sortable = true,
    resizable = true,
    renderAsLink = true,
    psnSuffix = false,
    navigateFunction
  } = options;

  const alignmentClass = alignment === "center" ? "center-align" : "left-align";

  const column: ColDef = {
    headerName,
    field: "badgeNumber",
    colId: "badgeNumber",
    minWidth,
    headerClass: alignmentClass,
    cellClass: alignmentClass,
    resizable,
    sortable
  };

  if (maxWidth) {
    column.maxWidth = maxWidth;
  }

  if (renderAsLink) {
    column.cellRenderer = (params: ICellRendererParams) => {
      if (psnSuffix && params.data.psnSuffix) {
        return viewBadgeLinkRenderer(params.data.badgeNumber, params.data.psnSuffix, navigateFunction);
      }
      return viewBadgeLinkRenderer(params.data.badgeNumber, navigateFunction);
    };
  }

  return column;
};

export const createCurrencyColumn = (options: CurrencyColumnOptions): ColDef => {
  const {
    headerName,
    field,
    colId = field,
    minWidth = 120,
    maxWidth,
    sortable = true,
    resizable = true,
    valueFormatter = agGridNumberToCurrency
  } = options;

  const column: ColDef = {
    headerName,
    field,
    colId,
    minWidth,
    type: "rightAligned",
    resizable,
    sortable,
    valueFormatter
  };

  if (maxWidth) {
    column.maxWidth = maxWidth;
  }

  return column;
};
