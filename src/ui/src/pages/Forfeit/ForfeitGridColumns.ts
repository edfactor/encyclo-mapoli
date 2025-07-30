import { ColDef } from "ag-grid-community";
import { GRID_COLUMN_WIDTHS } from "../../constants";
import { createBadgeColumn, createCurrencyColumn, createSSNColumn } from "../../utils/gridColumnFactory";

export const GetProfitShareForfeitColumns = (navFunction: (badgeNumber: string) => void): ColDef[] => {
  return [
    createBadgeColumn({
      headerName: "Badge",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      alignment: "left",
      navigateFunction: navFunction
    }),
    {
      headerName: "Name",
      field: "employeeName",
      colId: "employeeName",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    createSSNColumn({
      alignment: "left"
    }),
    createCurrencyColumn({
      headerName: "Forfeitures",
      field: "forfeitures",
      minWidth: 120
    }),
    {
      headerName: "Cont/Forfeit Points",
      field: "contForfeitPoints",
      colId: "contForfeitPoints",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Earnings Points",
      field: "earningPoints",
      colId: "earningPoints",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Ben #",
      field: "beneficiaryPsn",
      colId: "beneficiaryPsn",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    }
  ];
};
