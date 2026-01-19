import { ColDef } from "ag-grid-community";
import { GRID_COLUMN_WIDTHS } from "../../../constants";
import { createBadgeColumn, createSSNColumn } from "../../../utils/gridColumnFactory";

export const GetNegativeEtvaForSSNsOnPayProfitColumns = (): ColDef[] => {
  return [
    createBadgeColumn({
      headerName: "Badge",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      alignment: "left"
    }),
    createSSNColumn({ alignment: "left" }),
    {
      headerName: "ETVA",
      field: "etvaValue",
      colId: "etvaValue",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    }
  ];
};
