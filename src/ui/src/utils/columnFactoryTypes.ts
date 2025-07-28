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