import { ColDef, ICellRendererParams, ValueFormatterParams } from "ag-grid-community";
import { formatNumberWithComma, numberToCurrency, yyyyMMDDToMMDDYYYY } from "smart-ui-library";
import {
  AgeColumnOptions,
  AlignableColumnOptions,
  BadgeColumnOptions,
  BadgeOrPSNOptions,
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
  StreetAddressColumnOptions,
  TaxCodeColumnOptions,
  YesOrNoColumnOptions
} from "./columnFactoryTypes";
import { viewBadgeLinkRenderer } from "./masterInquiryLink";

export const createAddressColumn = (options: StreetAddressColumnOptions = {}): ColDef => {
  const {
    headerName = "Address",
    field = "address",
    colId = field,
    field1,
    field2,
    minWidth = 200,
    maxWidth,
    alignment = "left",
    sortable = true,
    resizable = true,
    valueGetter
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

  if (valueGetter) {
    column.valueGetter = valueGetter;
  } else {
    if (field1 && field2) {
      column.valueGetter = (params) => {
        const address1 = params.data.address[field1] || "";
        const address2 = params.data.address[field2] || "";
        return address2 && address2.trim() ? `${address1}, ${address2}` : address1;
      };
    } else {
      column.valueGetter = (params) => {
        const address1 = params.data.address || "";
        const address2 = params.data.address2 || "";
        return address2 && address2.trim() ? `${address1}, ${address2}` : address1;
      };
    }
  }

  if (maxWidth) {
    column.maxWidth = maxWidth;
  }

  return column;
};

export const createStateColumn = (options: StateColumnOptions = {}): ColDef => {
  const {
    headerName = "State",
    field = "state",
    colId = field,
    minWidth = 80,
    maxWidth,
    alignment = "left",
    sortable = true,
    resizable = true,
    valueFormatter,
    valueGetter,
    tooltip,
    tooltipField,
    tooltipValueGetter,
    headerTooltip
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

  // Add tooltip support
  if (tooltip) {
    column.tooltipValueGetter = () => tooltip;
  } else if (tooltipField) {
    column.tooltipField = tooltipField;
  } else if (tooltipValueGetter) {
    column.tooltipValueGetter = tooltipValueGetter;
  }

  // Add header tooltip support
  if (headerTooltip) {
    column.headerTooltip = headerTooltip;
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
    cellRenderer
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
    cellRenderer
  };

  if (!cellRenderer) {
    if (useWords) {
      column.cellRenderer = (params: ICellRendererParams) => (params.value ? "Yes" : "No");
    } else {
      column.cellRenderer = (params: ICellRendererParams) => (params.value ? "Y" : "N");
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
    minWidth = 80,
    maxWidth,
    alignment = "center",
    sortable = true,
    resizable = true,
    valueFormatter,
    tooltip,
    tooltipField,
    tooltipValueGetter,
    headerTooltip
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

  // Add tooltip support
  if (tooltip) {
    column.tooltipValueGetter = () => tooltip;
  } else if (tooltipField) {
    column.tooltipField = tooltipField;
  } else if (tooltipValueGetter) {
    column.tooltipValueGetter = tooltipValueGetter;
  }

  // Add header tooltip support
  if (headerTooltip) {
    column.headerTooltip = headerTooltip;
  }

  return column;
};

export const createBadgeColumn = (options: BadgeColumnOptions = {}): ColDef => {
  const {
    headerName = "Badge",
    field = "badgeNumber",
    minWidth = 120,
    maxWidth = 125,
    alignment = "left",
    sortable = true,
    resizable = true,
    renderAsLink = true,
    psnSuffix = false,
    navigateFunction,
    tooltip,
    tooltipField,
    tooltipValueGetter,
    headerTooltip
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
      if (dataValue === 0) return;
      if (psnSuffix && params.data.psnSuffix) {
        return viewBadgeLinkRenderer(params.data.badgeNumber, params.data.psnSuffix, navigateFunction);
      }

      return viewBadgeLinkRenderer(dataValue, navigateFunction);
    };
  } else if (psnSuffix) {
    // PS-2258: When not rendering as link but psnSuffix is enabled, show full PSN (badge + suffix)
    column.valueFormatter = (params: ValueFormatterParams) => {
      const badgeNumber = params.data?.badgeNumber;
      const suffix = params.data?.psnSuffix;
      if (badgeNumber == null || badgeNumber === 0) return "";
      if (suffix != null && suffix > 0) {
        return `${badgeNumber}${String(suffix).padStart(4, "0")}`;
      }
      return String(badgeNumber);
    };
  }

  // Add tooltip support
  if (tooltip) {
    column.tooltipValueGetter = () => tooltip;
  } else if (tooltipField) {
    column.tooltipField = tooltipField;
  } else if (tooltipValueGetter) {
    column.tooltipValueGetter = tooltipValueGetter;
  }

  // Add header tooltip support
  if (headerTooltip) {
    column.headerTooltip = headerTooltip;
  }

  return column;
};

export const createCurrencyColumn = (options: CurrencyColumnOptions): ColDef => {
  const defaultValueFormatter = (params: ValueFormatterParams) => {
    const value = params.value;
    // Need to log value and type of value

    if (value == null || value === "") return ""; // keep empty display consistent
    // If it's already a string (even if numeric-like), return as-is per requirement
    if (typeof value === "string") return value;
    // Only format when it's an actual number
    if (typeof value === "number" && !isNaN(value)) {
      return numberToCurrency(value);
    }
    // Fallback: attempt not to break – convert other types to string
    return String(value);
  };

  const {
    headerName,
    field,
    colId = field,
    minWidth = 80,
    maxWidth,
    sortable = true,
    resizable = true,
    valueFormatter = defaultValueFormatter,
    valueGetter,
    cellStyle,
    tooltip,
    tooltipField,
    tooltipValueGetter,
    headerTooltip
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

  if (cellStyle) {
    // Ensure cellStyle is a plain object or function returning a plain object with string keys/values
    // cellStyle can be either a plain object or a function; typing here is loose to
    // match ag-grid expected types. Use an explicit any cast and disable the linter
    // for this line to avoid noisy typing conflicts.
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    column.cellStyle = cellStyle as any;
  }

  // Add tooltip support
  if (tooltip) {
    column.tooltipValueGetter = () => tooltip;
  } else if (tooltipField) {
    column.tooltipField = tooltipField;
  } else if (tooltipValueGetter) {
    column.tooltipValueGetter = tooltipValueGetter;
  }

  // Add header tooltip support
  if (headerTooltip) {
    column.headerTooltip = headerTooltip;
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
    resizable = true,
    tooltip,
    tooltipField,
    tooltipValueGetter,
    headerTooltip,
    valueGetter
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

  // Add tooltip support
  if (tooltip) {
    column.tooltipValueGetter = () => tooltip;
  } else if (tooltipField) {
    column.tooltipField = tooltipField;
  } else if (tooltipValueGetter) {
    column.tooltipValueGetter = tooltipValueGetter;
  }

  // Add header tooltip support
  if (headerTooltip) {
    column.headerTooltip = headerTooltip;
  }

  if (valueGetter) {
    column.valueGetter = valueGetter;
  } else {
    column.valueGetter = (params) => {
      if (params.data?.[field] && typeof params.data?.[field] === "number" && params.data?.[field] == 0) {
        return "N/A";
      } else if (params.data?.[field] && typeof params.data?.[field] === "string") {
        const age = params.data?.[field];
        // If age is >= 125 (default Oracle date 1/1/1900), display empty string
        const ageNumber = typeof age === "string" ? parseInt(age, 10) : age;
        if (!isNaN(ageNumber) && ageNumber >= 125) {
          return "";
        }
      } else {
        // This will be used for masked values also
        return params.data?.[field];
      }
    };
  }

  return column;
};

export const createDateColumn = (options: DateColumnOptions): ColDef => {
  const {
    headerName,
    field,
    colId = field,
    minWidth = 80,
    maxWidth,
    alignment = "center",
    sortable = true,
    resizable = true,
    valueFormatter = (params) => {
      const value = params.value;
      // Only format if value is a valid date string (yyyyMMdd or yyyy-MM-dd)
      if (!value) return "";
      // Accepts both string and Date
      if (typeof value === "string" && /^\d{8}$/.test(value)) {
        return yyyyMMDDToMMDDYYYY(value);
      }
      if (typeof value === "string" && /^\d{4}-\d{2}-\d{2}$/.test(value)) {
        return yyyyMMDDToMMDDYYYY(value);
      }
      // Handle masked dates (e.g., "XXXX-XX-XX")
      if (typeof value === "string" && /^[X\d]{4}-[X\d]{2}-[X\d]{2}$/.test(value)) {
        return value;
      }
      if (value instanceof Date && !isNaN(value.getTime())) {
        // Format as MM/DD/YYYY
        return value.toLocaleDateString("en-US");
      }
      return "";
    },
    tooltip,
    tooltipField,
    tooltipValueGetter,
    headerTooltip
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

  // Add tooltip support
  if (tooltip) {
    column.tooltipValueGetter = () => tooltip;
  } else if (tooltipField) {
    column.tooltipField = tooltipField;
  } else if (tooltipValueGetter) {
    column.tooltipValueGetter = tooltipValueGetter;
  }

  // Add header tooltip support
  if (headerTooltip) {
    column.headerTooltip = headerTooltip;
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
    field = "fullName",
    colId = field,
    minWidth = 180,
    maxWidth,
    alignment = "left",
    sortable = true,
    resizable = true,
    valueFormatter,
    tooltip,
    tooltipField,
    tooltipValueGetter,
    headerTooltip
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

  // Add tooltip support
  if (tooltip) {
    column.tooltipValueGetter = () => tooltip;
  } else if (tooltipField) {
    column.tooltipField = tooltipField;
  } else if (tooltipValueGetter) {
    column.tooltipValueGetter = tooltipValueGetter;
  }

  // Add header tooltip support
  if (headerTooltip) {
    column.headerTooltip = headerTooltip;
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
    minWidth = 70,
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
    alignment = "right",
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
    column.valueFormatter = (params) => {
      const value = params.value;

      // Handle masked values (security rule: points may be masked as "X")
      if (value === "X" || value === "x") {
        return value.toUpperCase();
      }

      // Handle numeric values (excluding NaN)
      if (typeof value === "number") {
        if (isNaN(value)) {
          return "";
        }
        return formatNumberWithComma(value);
      }

      // Return as-is for other string values or null/undefined
      return value != null ? String(value) : "";
    };
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
    minWidth = 60,
    maxWidth,
    alignment = "left",
    sortable = true,
    resizable = true,
    hideZeroValues = true,
    showBrackets = true,
    idField,
    nameField,
    valueFormatter,
    tooltip,
    tooltipField,
    tooltipValueGetter,
    headerTooltip
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
  // If we do not have these three, it's just a default render of the string
  if ((idField && nameField) || valueFormatter) {
    if (valueFormatter) {
      column.valueFormatter = valueFormatter;
    } else {
      column.valueFormatter = (params) => {
        const id = params.data?.["taxCodeId"];
        const name = params.data?.["taxCodeName"];

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
  }

  // Add tooltip support
  if (tooltip) {
    column.tooltipValueGetter = () => tooltip;
  } else if (tooltipField) {
    column.tooltipField = tooltipField;
  } else if (tooltipValueGetter) {
    column.tooltipValueGetter = tooltipValueGetter;
  }

  // Add header tooltip support
  if (headerTooltip) {
    column.headerTooltip = headerTooltip;
  }

  return column;
};

export const createPhoneColumn = (options: FormattableColumnOptions = {}): ColDef => {
  const {
    headerName = "Phone Number",
    field = "phoneNumber",
    colId = field,
    minWidth = 80,
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
    minWidth = 80,
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

export const createBadgeOrPSNColumn = (
  options: BadgeOrPSNOptions = {
    badgeField: "",
    psnField: ""
  }
): ColDef => {
  const {
    headerName = "Badge/PSN",
    field = "psn",
    colId = field,
    badgeField = "badgeNumber",
    psnField = "psn",
    minWidth = 80,
    maxWidth,
    alignment = "center",
    sortable = true,
    resizable = true,
    enableLinking = false,
    navigateFunction,
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
      if (params.data[badgeField]) {
        return viewBadgeLinkRenderer(params.data[badgeField]);
      } else {
        return viewBadgeLinkRenderer(params.data[psnField]);
      }
    };
  } else if (valueFormatter) {
    column.valueFormatter = valueFormatter;
  }

  return column;
};

export const createColumnFrom = (initialSet: ColDef, additionalOptions: ColDef): ColDef => {
  return { ...initialSet, ...additionalOptions };
};
