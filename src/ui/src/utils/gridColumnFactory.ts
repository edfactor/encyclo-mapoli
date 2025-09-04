import { ColDef, ICellRendererParams } from "ag-grid-community";
import { formatNumberWithComma, numberToCurrency, yyyyMMDDToMMDDYYYY } from "smart-ui-library";
import { GRID_COLUMN_WIDTHS } from "../constants";
import {
  AlignableColumnOptions,
  BadgeColumnOptions,
  BaseColumnOptions,
  CityColumnOptions,
  CommentColumnOptions,
  CurrencyColumnOptions,
  DateColumnOptions,
  FormattableColumnOptions,
  HoursColumnOptions,
  NameColumnOptions,
  PercentageColumnOptions,
  PointsColumnOptions,
  PSNColumnOptions,
  SSNColumnOptions,
  StateColumnOptions,
  TaxCodeColumnOptions,
  YesOrNoColumnOptions
} from "./columnFactoryTypes";
import { viewBadgeLinkRenderer } from "./masterInquiryLink";

export const createStateColumn = (options: StateColumnOptions = {}): ColDef => {
  const {
    headerName = "State",
    field = "state",
    colId = field,
    minWidth = 100,
    maxWidth,
    alignment = "left",
    sortable = true,
    resizable = true,
    valueFormatter,
    valueGetter
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

  if (valueFormatter) {
    column.valueFormatter = valueFormatter;
  }

  if (valueGetter) {
    column.valueGetter = valueGetter;
  }

  if (maxWidth) {
    column.maxWidth = maxWidth;
  }

  return column;
};

export const createYesOrNoColumn = (options: YesOrNoColumnOptions): ColDef => {
  const {
    headerName = "Yes/No",
    field,
    colId = field,
    minWidth = 90,
    maxWidth,
    alignment = "center",
    sortable = true,
    resizable = true,
    useWords = false,
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

  if (!valueFormatter) {
    if (useWords) {
      column.valueFormatter = (params) => (params.value ? "Yes" : "No");
    } else {
      column.valueFormatter = (params) => (params.value ? "Y" : "N");
    }
  }

  if (maxWidth) {
    column.maxWidth = maxWidth;
  }

  return column;
};

export const createPercentageColumn = (options: PercentageColumnOptions): ColDef => {
  const {
    headerName = "Percentage",
    field,
    colId = field,
    minWidth = 90,
    maxWidth,
    alignment = "center",
    sortable = true,
    resizable = true,
    valueFormatter = (params) => `${params.value != null ? params.value : 0}%`
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
    maxWidth = 125,
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
    valueFormatter = (params) => numberToCurrency(params.value),
    valueGetter
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

  if (valueGetter) {
    column.valueGetter = valueGetter;
  }

  return column;
};

export const createAgeColumn = (options: BaseColumnOptions = {}): ColDef => {
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

export const createStoreColumn = (options: AlignableColumnOptions = {}): ColDef => {
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
      const value = params.data?.[field];
      return value == null || value === 0 ? null : value;
    },
    valueFormatter = (params) => {
      const value = params.value;
      // Need to log value and type of value

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

export const createStatusColumn = (options: FormattableColumnOptions = {}): ColDef => {
  const {
    headerName = "Status",
    field = "status",
    colId = field,
    minWidth = 80,
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

export const createCountColumn = (options: AlignableColumnOptions = {}): ColDef => {
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

export const createYearColumn = (options: FormattableColumnOptions = {}): ColDef => {
  const {
    headerName = "Year",
    field = "year",
    colId = field,
    minWidth = 100,
    maxWidth,
    alignment = "right",
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

export const createZipColumn = (options: FormattableColumnOptions = {}): ColDef => {
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

export const createPointsColumn = (options: PointsColumnOptions = {}): ColDef => {
  const {
    headerName = "Points",
    field = "points",
    colId = field,
    minWidth = 100,
    maxWidth,
    alignment = "right",
    sortable = true,
    resizable = true,
    includeCommaFormatting = true,
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
  } else if (includeCommaFormatting) {
    column.valueFormatter = (params) => formatNumberWithComma(params.value);
  }

  return column;
};

export const createCityColumn = (options: CityColumnOptions = {}): ColDef => {
  const {
    headerName = "City",
    field = "city",
    colId = field,
    minWidth = 120,
    maxWidth,
    alignment = "left",
    sortable = true,
    resizable = true,
    nestedPath,
    valueGetter,
    valueFormatter
  } = options;

  const alignmentClass = alignment === "center" ? "center-align" : alignment === "right" ? "right-align" : "left-align";

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

  if (valueGetter) {
    column.valueGetter = valueGetter;
  } else if (nestedPath) {
    column.valueGetter = (params) => {
      const pathParts = nestedPath.split(".");
      let value = params.data;
      for (const part of pathParts) {
        value = value?.[part];
        if (value === undefined || value === null) break;
      }
      return value || "";
    };
  }

  if (valueFormatter) {
    column.valueFormatter = valueFormatter;
  }

  return column;
};

export const createTaxCodeColumn = (options: TaxCodeColumnOptions = {}): ColDef => {
  const {
    headerName = "Tax Code",
    field = "taxCodeId",
    colId = field,
    minWidth = 100,
    maxWidth,
    alignment = "left",
    sortable = true,
    resizable = true,
    hideZeroValues = true,
    showBrackets = true,
    idField = "taxCodeId",
    nameField = "taxCodeName",
    valueFormatter
  } = options;

  const alignmentClass = alignment === "center" ? "center-align" : alignment === "right" ? "right-align" : "left-align";

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
  } else {
    column.valueFormatter = (params) => {
      const id = params.data?.[idField];
      const name = params.data?.[nameField];

      if (hideZeroValues && (id == 0 || id == null)) {
        return "";
      }

      if (showBrackets) {
        return `[${id}] ${name || ""}`;
      } else {
        return `${id} - ${name || ""}`;
      }
    };
  }

  return column;
};

export const createPhoneColumn = (options: FormattableColumnOptions = {}): ColDef => {
  const {
    headerName = "Phone Number",
    field = "phoneNumber",
    colId = field,
    minWidth = 130,
    maxWidth,
    alignment = "center",
    sortable = true,
    resizable = true,

    valueFormatter
  } = options;

  const alignmentClass = alignment === "center" ? "center-align" : alignment === "right" ? "right-align" : "left-align";

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
  } else {
    column.valueFormatter = (params) => params.data[field] || "";
  }

  return column;
};

export const createPSNColumn = (options: PSNColumnOptions = {}): ColDef => {
  const {
    headerName = "PSN",
    field = "psn",
    colId = field,
    minWidth = 120,
    maxWidth,
    alignment = "center",
    sortable = true,
    resizable = true,
    enableLinking = false,
    navigateFunction,
    linkingStyle = "simple",
    valueFormatter
  } = options;

  const alignmentClass = alignment === "center" ? "center-align" : alignment === "right" ? "right-align" : "left-align";

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

  if (enableLinking && navigateFunction) {
    column.cellRenderer = (params: ICellRendererParams) => {
      if (linkingStyle === "badge-psn") {
        // For complex badge-psn linking (like in PayBenReport)
        return viewBadgeLinkRenderer(params.data.badgeNumber, parseInt(params.data[field].slice(-4)));
      } else if (field === "psnSuffix") {
        // For psnSuffix fields (like in BeneficiaryInquiry)
        return viewBadgeLinkRenderer(params.data.badgeNumber, params.data.psnSuffix);
      } else {
        // Simple PSN linking
        return viewBadgeLinkRenderer(params.data[field]);
      }
    };
  } else if (valueFormatter) {
    column.valueFormatter = valueFormatter;
  }

  return column;
};
