import { ValueFormatterParams } from "ag-grid-community";

export interface SSNColumnOptions {
  headerName?: string;
  minWidth?: number;
  maxWidth?: number;
  alignment?: "left" | "center";
  sortable?: boolean;
  resizable?: boolean;
  valueFormatter?: (params: ValueFormatterParams) => string;
}

export interface BadgeColumnOptions {
  headerName?: string;
  minWidth?: number;
  maxWidth?: number;
  alignment?: "left" | "center";
  sortable?: boolean;
  resizable?: boolean;
  renderAsLink?: boolean;
  psnSuffix?: boolean;
  navigateFunction?: (path: string) => void;
}

export interface CurrencyColumnOptions {
  headerName?: string;
  field: string;
  colId?: string;
  minWidth?: number;
  maxWidth?: number;
  sortable?: boolean;
  resizable?: boolean;
  valueFormatter?: (params: ValueFormatterParams) => string;
}
