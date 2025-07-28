import { ColDef, ICellRendererParams } from "ag-grid-community";
import { agGridNumberToCurrency } from "smart-ui-library";
import { GRID_COLUMN_WIDTHS } from "../../constants";
import { createSSNColumn, createBadgeColumn } from "../../utils/gridColumnFactory";

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
    {
      headerName: "Forfeitures",
      field: "forfeitures",
      colId: "forfeitures",
      minWidth: 120,
      type: "rightAligned",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
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
