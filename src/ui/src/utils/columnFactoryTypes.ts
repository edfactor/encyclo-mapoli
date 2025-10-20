import { ValueFormatterParams, ValueGetterParams } from "ag-grid-community";

// Base interface with common properties
export interface BaseColumnOptions {
  headerName?: string;
  field?: string;
  colId?: string;
  minWidth?: number;
  maxWidth?: number;
  sortable?: boolean;
  resizable?: boolean;
  tooltip?: string; // Static tooltip text
  tooltipField?: string; // Field name containing tooltip data
  tooltipValueGetter?: (params: ValueGetterParams) => string; // Dynamic tooltip generator
  headerTooltip?: string; // Header tooltip text
}

// Extended base interfaces
export interface AlignableColumnOptions extends BaseColumnOptions {
  alignment?: "left" | "center" | "right";
}

export interface LimitedAlignmentColumnOptions extends BaseColumnOptions {
  alignment?: "left" | "center";
}

export interface AddressColumnOptions extends AlignableColumnOptions {
  valueGetter?: (params: ValueGetterParams) => string;
}

export interface FormattableColumnOptions extends AlignableColumnOptions {
  valueFormatter?: (params: ValueFormatterParams) => string;
}

export interface DataAccessColumnOptions extends FormattableColumnOptions {
  valueGetter?: (params: ValueGetterParams) => string;
}

export interface StreetAddressColumnOptions extends DataAccessColumnOptions {
  field1?: string;
  field2?: string;
}

// Specialized column interfaces (only those that add new properties)
export interface SSNColumnOptions extends LimitedAlignmentColumnOptions {
  valueFormatter?: (params: ValueFormatterParams) => string;
}

export interface BadgeColumnOptions extends LimitedAlignmentColumnOptions {
  renderAsLink?: boolean;
  psnSuffix?: boolean;
  navigateFunction?: (path: string) => void;
}

export interface CurrencyColumnOptions extends BaseColumnOptions {
  field: string; // Required field
  valueGetter?: (params: ValueGetterParams) => string;
  valueFormatter?: (params: ValueFormatterParams) => string;
  cellStyle?: (params: ValueFormatterParams) => React.CSSProperties | null;
}

export interface DateColumnOptions extends LimitedAlignmentColumnOptions {
  field: string; // Required field
  valueFormatter?: (params: ValueFormatterParams) => string;
}

export interface YesOrNoColumnOptions extends FormattableColumnOptions {
  useWords?: boolean;
}

export interface NameColumnOptions extends LimitedAlignmentColumnOptions {
  valueFormatter?: (params: ValueFormatterParams) => string;
}

export interface StateColumnOptions extends DataAccessColumnOptions {
  alignment?: "left" | "center"; // Override to limit alignment
}

export interface CommentColumnOptions extends LimitedAlignmentColumnOptions {
  valueFormatter?: (params: ValueFormatterParams) => string;
}

export interface PercentageColumnOptions extends FormattableColumnOptions {
  field: string; // Required field
}

export interface HoursColumnOptions extends DataAccessColumnOptions {
  editable?: boolean;
}

export interface PointsColumnOptions extends FormattableColumnOptions {
  includeCommaFormatting?: boolean;
}

export interface CityColumnOptions extends DataAccessColumnOptions {
  nestedPath?: string;
}

export interface TaxCodeColumnOptions extends FormattableColumnOptions {
  hideZeroValues?: boolean;
  showBrackets?: boolean;
  idField?: string;
  nameField?: string;
}

export interface PSNColumnOptions extends FormattableColumnOptions {
  enableLinking?: boolean;
  navigateFunction?: (badgeNumber: string, psnSuffix?: string) => void;
  linkingStyle?: "simple" | "badge-psn"; // Different PSN linking patterns
}
