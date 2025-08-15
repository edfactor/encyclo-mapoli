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
  field?: string;
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

export interface AgeColumnOptions {
  headerName?: string;
  field?: string;
  colId?: string;
  minWidth?: number;
  maxWidth?: number;
  sortable?: boolean;
  resizable?: boolean;
}

export interface DateColumnOptions {
  headerName?: string;
  field: string;
  colId?: string;
  minWidth?: number;
  maxWidth?: number;
  alignment?: "left" | "center";
  sortable?: boolean;
  resizable?: boolean;
  valueFormatter?: (params: ValueFormatterParams) => string;
}

export interface StoreColumnOptions {
  headerName?: string;
  field?: string;
  colId?: string;
  minWidth?: number;
  maxWidth?: number;
  alignment?: "left" | "center" | "right";
  sortable?: boolean;
  resizable?: boolean;
}

export interface NameColumnOptions {
  headerName?: string;
  field?: string;
  colId?: string;
  minWidth?: number;
  maxWidth?: number;
  alignment?: "left" | "center";
  sortable?: boolean;
  resizable?: boolean;
  valueFormatter?: (params: ValueFormatterParams) => string;
}

export interface CommentColumnOptions {
  headerName?: string;
  field?: string;
  colId?: string;
  minWidth?: number;
  maxWidth?: number;
  alignment?: "left" | "center";
  sortable?: boolean;
  resizable?: boolean;
  valueFormatter?: (params: ValueFormatterParams) => string;
}

export interface HoursColumnOptions {
  headerName?: string;
  field?: string;
  colId?: string;
  minWidth?: number;
  maxWidth?: number;
  sortable?: boolean;
  resizable?: boolean;
  alignment?: "left" | "center" | "right";
  editable?: boolean;
  valueFormatter?: (params: ValueFormatterParams) => string;
}

export interface StatusColumnOptions {
  headerName?: string;
  field?: string;
  colId?: string;
  minWidth?: number;
  maxWidth?: number;
  alignment?: "left" | "center" | "right";
  sortable?: boolean;
  resizable?: boolean;
  valueFormatter?: (params: ValueFormatterParams) => string;
}

export interface CountColumnOptions {
  headerName?: string;
  field?: string;
  colId?: string;
  minWidth?: number;
  maxWidth?: number;
  alignment?: "left" | "center" | "right";
  sortable?: boolean;
  resizable?: boolean;
}

export interface YearColumnOptions {
  headerName?: string;
  field?: string;
  colId?: string;
  minWidth?: number;
  maxWidth?: number;
  alignment?: "left" | "center" | "right";
  sortable?: boolean;
  resizable?: boolean;
}

export interface ZipColumnOptions {
  headerName?: string;
  field?: string;
  colId?: string;
  minWidth?: number;
  maxWidth?: number;
  alignment?: "left" | "center" | "right";
  sortable?: boolean;
  resizable?: boolean;
  valueFormatter?: (params: ValueFormatterParams) => string;
}
