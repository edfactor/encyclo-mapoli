import { ColDef, ICellRendererParams } from "ag-grid-community";
import { GRID_COLUMN_WIDTHS } from "../../../constants";
import { createSSNColumn, createBadgeColumn } from "../../../utils/gridColumnFactory";

export const GetNegativeEtvaForSSNsOnPayProfitColumns = (navFunction: (badgeNumber: string) => void): ColDef[] => {
  return [
    createBadgeColumn({ 
      headerName: "Badge",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      alignment: "left",
      navigateFunction: navFunction
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
