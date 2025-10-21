import { ColDef } from "ag-grid-community";

/**
 * Default column definition for action columns (buttons, icons, etc.)
 */
export const ActionColumnDefault: ColDef = {
  headerName: "Action",
  colId: "action",
  pinned: true,
  sortable: false,
  resizable: false,
  lockPosition: true,
  suppressMovable: true,
  suppressAutoSize: true,
  suppressSizeToFit: true,
  cellStyle: { textAlign: "center" },
  width: 100
};
