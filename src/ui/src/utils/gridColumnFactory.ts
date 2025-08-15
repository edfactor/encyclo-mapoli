import { ColDef, ICellRendererParams } from "ag-grid-community";
import { agGridNumberToCurrency, formatNumberWithComma, yyyyMMDDToMMDDYYYY } from "smart-ui-library";
import { GRID_COLUMN_WIDTHS } from "../constants";
import {
  AgeColumnOptions,
  BadgeColumnOptions,
  CommentColumnOptions,
  CountColumnOptions,
  CurrencyColumnOptions,
  DateColumnOptions,
  HoursColumnOptions,
  NameColumnOptions,
  SSNColumnOptions,
  StatusColumnOptions,
  StoreColumnOptions,
  YearColumnOptions,
  ZipColumnOptions
} from "./columnFactoryTypes";
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
    field = "badgeNumber",
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
    field: field,
    colId: field,
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
      // We could get a PSN as a string
      const dataValue: number = field === "badgeNumber" ? params.data.badgeNumber : Number(params.data[field]);
      // If dataValue is NaN just return nothing
      if (isNaN(dataValue)) return;
      if (psnSuffix && params.data.psnSuffix) {
        return viewBadgeLinkRenderer(params.data.badgeNumber, params.data.psnSuffix, navigateFunction);
      }

      return viewBadgeLinkRenderer(dataValue, navigateFunction);
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

  if (valueFormatter) {
    column.valueFormatter = valueFormatter;
  }

  return column;
};

export const createAgeColumn = (options: AgeColumnOptions = {}): ColDef => {
  const {
    headerName = "Age",
    field = "age",
    colId = field,
    minWidth = 70,
    maxWidth,
    sortable = true,
    resizable = true
  } = options;

  const column: ColDef = {
    headerName,
    field,
    colId,
    minWidth,
    type: "rightAligned",
    resizable,
    sortable
  };

  if (maxWidth) {
    column.maxWidth = maxWidth;
  }

  return column;
};

export const createDateColumn = (options: DateColumnOptions): ColDef => {
  const {
    headerName,
    field,
    colId = field,
    minWidth = 120,
    maxWidth,
    alignment = "center",
    sortable = true,
    resizable = true,
    valueFormatter = (params) => (params.value ? yyyyMMDDToMMDDYYYY(params.value) : "")
  } = options;

  const alignmentClass = alignment === "center" ? "center-align" : "left-align";

  const column: ColDef = {
    headerName,
    field,
    colId,
    minWidth,
    headerClass: alignmentClass,
    cellClass: alignmentClass,
    resizable,
    sortable,
    valueFormatter
  };

  if (maxWidth) {
    column.maxWidth = maxWidth;
  }

  return column;
};

export const createStoreColumn = (options: StoreColumnOptions = {}): ColDef => {
  const {
    headerName = "Store",
    field = "storeNumber",
    colId = field,
    minWidth = 80,
    maxWidth,
    alignment = "right",
    sortable = true,
    resizable = true
  } = options;

  const column: ColDef = {
    headerName,
    field,
    colId,
    minWidth,
    resizable,
    sortable
  };

  if (alignment === "right") {
    column.type = "rightAligned";
  } else {
    const alignmentClass = alignment === "center" ? "center-align" : "left-align";
    column.headerClass = alignmentClass;
    column.cellClass = alignmentClass;
  }

  if (maxWidth) {
    column.maxWidth = maxWidth;
  }

  return column;
};

export const createNameColumn = (options: NameColumnOptions = {}): ColDef => {
  const {
    headerName = "Name",
    field = "employeeName",
    colId = field,
    minWidth = 180,
    maxWidth,
    alignment = "left",
    sortable = true,
    resizable = true,
    valueFormatter
  } = options;

  const alignmentClass = alignment === "center" ? "center-align" : "left-align";

  const column: ColDef = {
    headerName,
    field,
    colId,
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

export const createCommentColumn = (options: CommentColumnOptions = {}): ColDef => {
  const {
    headerName = "Comment",
    field = "comment",
    colId = field,
    minWidth = 200,
    maxWidth,
    alignment = "left",
    sortable = true,
    resizable = true,
    valueFormatter
  } = options;

  const alignmentClass = alignment === "center" ? "center-align" : "left-align";

  const column: ColDef = {
    headerName,
    field,
    colId,
    minWidth,
    headerClass: alignmentClass,
    cellClass: alignmentClass,
    resizable,
    sortable,
    valueFormatter
  };

  if (maxWidth) {
    column.maxWidth = maxWidth;
  }

  if (valueFormatter) {
    column.valueFormatter = valueFormatter;
  }

  return column;
};

export const createHoursColumn = (options: HoursColumnOptions = {}): ColDef => {
  const {
    headerName = "Hours",
    field = "hours",
    colId = field,
    minWidth = 100,
    maxWidth,
    sortable = true,
    resizable = true,
    alignment = "right",
    editable = false,
    valueGetter = (params) => {
      const value = params.data?.hours;
      return value == null || value === 0 ? null : value;
    },
    valueFormatter = (params) => {
      const value = params.value;
      if (value == null || value === "") return ""; // keep empty display consistent
      // If it's already a string (even if numeric-like), return as-is per requirement
      if (typeof value === "string") return value;
      // Only format when it's an actual number
      if (typeof value === "number" && !isNaN(value)) {
        return formatNumberWithComma(value);
      }
      // Fallback: attempt not to break – convert other types to string
      return String(value);
    }
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

  if (valueGetter) {
    column.valueGetter = valueGetter;
  }

  column.editable = editable;

  if (alignment === "right") {
    column.type = "rightAligned";
  } else {
    const alignmentClass = alignment === "center" ? "center-align" : "left-align";
    column.headerClass = alignmentClass;
    column.cellClass = alignmentClass;
  }

  return column;
};

export const createStatusColumn = (options: StatusColumnOptions = {}): ColDef => {
  const {
    headerName = "Status",
    field = "status",
    colId = field,
    minWidth = 120,
    maxWidth,
    alignment = "left",
    sortable = true,
    resizable = true,
    valueFormatter
  } = options;

  const column: ColDef = {
    headerName,
    field,
    colId,
    minWidth,
    resizable,
    sortable
  };

  if (alignment === "right") {
    column.type = "rightAligned";
  } else {
    const alignmentClass = alignment === "center" ? "center-align" : "left-align";
    column.headerClass = alignmentClass;
    column.cellClass = alignmentClass;
  }

  if (maxWidth) {
    column.maxWidth = maxWidth;
  }

  if (valueFormatter) {
    column.valueFormatter = valueFormatter;
  }

  return column;
};

export const createCountColumn = (options: CountColumnOptions = {}): ColDef => {
  const {
    headerName = "Count",
    field = "employeeCount",
    colId = field,
    minWidth = 100,
    maxWidth,
    alignment = "right",
    sortable = true,
    resizable = true
  } = options;

  const column: ColDef = {
    headerName,
    field,
    colId,
    minWidth,
    resizable,
    sortable
  };

  if (alignment === "right") {
    column.type = "rightAligned";
  } else {
    const alignmentClass = alignment === "center" ? "center-align" : "left-align";
    column.headerClass = alignmentClass;
    column.cellClass = alignmentClass;
  }

  if (maxWidth) {
    column.maxWidth = maxWidth;
  }

  return column;
};

export const createYearColumn = (options: YearColumnOptions = {}): ColDef => {
  const {
    headerName = "Year",
    field = "year",
    colId = field,
    minWidth = 100,
    maxWidth,
    alignment = "right",
    sortable = true,
    resizable = true
  } = options;

  const column: ColDef = {
    headerName,
    field,
    colId,
    minWidth,
    resizable,
    sortable
  };

  if (alignment === "right") {
    column.type = "rightAligned";
  } else {
    const alignmentClass = alignment === "center" ? "center-align" : "left-align";
    column.headerClass = alignmentClass;
    column.cellClass = alignmentClass;
  }

  if (maxWidth) {
    column.maxWidth = maxWidth;
  }

  return column;
};

export const createZipColumn = (options: ZipColumnOptions = {}): ColDef => {
  const {
    headerName = "Zip Code",
    field = "zipCode",
    colId = field,
    minWidth = 100,
    maxWidth,
    alignment = "left",
    sortable = true,
    resizable = true,
    valueFormatter = (params) => {
      if (params.value == null || params.value === "") return "";

      const zipStr = String(params.value);
      // If exactly 4 digits, add leading zero
      if (/^\d{4}$/.test(zipStr)) {
        return `0${zipStr}`;
      }
      return zipStr;
    }
  } = options;

  const column: ColDef = {
    headerName,
    field,
    colId,
    minWidth,
    resizable,
    sortable,
    valueFormatter
  };

  if (alignment === "right") {
    column.type = "rightAligned";
  } else {
    const alignmentClass = alignment === "center" ? "center-align" : "left-align";
    column.headerClass = alignmentClass;
    column.cellClass = alignmentClass;
  }

  if (maxWidth) {
    column.maxWidth = maxWidth;
  }

  return column;
};
