import { ColDef } from "ag-grid-community";
import { GRID_COLUMN_WIDTHS } from "../../constants";
import { createBadgeColumn, createCurrencyColumn } from "../../utils/gridColumnFactory";

export const GetYTDWagesColumns = (): ColDef[] => {
  const columns: ColDef[] = [
    createBadgeColumn({
      headerName: "Badge",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      alignment: "left"
    }),
    {
      headerName: "Hours Current Year",
      field: "hoursCurrentYear",
      colId: "hoursCurrentYear",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    createCurrencyColumn({
      headerName: "Dollars Current Year",
      field: "incomeCurrentYear",
      minWidth: 150
    }),
    {
      headerName: "Store",
      field: "storeNumber",
      colId: "storeNumber",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    }
  ];
  return columns;
};
