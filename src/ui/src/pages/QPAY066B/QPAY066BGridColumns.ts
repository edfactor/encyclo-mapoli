import { ColDef } from "ag-grid-community";
import { formatNumberWithComma } from "smart-ui-library";
import { GRID_COLUMN_WIDTHS } from "../../constants";
import {
  createBadgeColumn,
  createCurrencyColumn,
  createNameColumn,
  createHoursColumn
} from "../../utils/gridColumnFactory";

export const GetQPAY066BGridColumns = (navFunction: (badgeNumber: string) => void): ColDef[] => {
  return [
    {
      headerName: "Store",
      field: "storeNumber",
      colId: "storeNumber",
      minWidth: 80,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: true
    },
    createBadgeColumn({
      headerName: "Badge",
      field: "badgeNumber",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      alignment: "center",
      navigateFunction: navFunction
    }),
    createNameColumn({
      field: "fullName",
      minWidth: 180,
      sortable: true
    }),
    {
      headerName: "Pay Classification",
      field: "payClassificationName",
      colId: "payClassificationName",
      minWidth: 160,
      resizable: true,
      sortable: true
    },
    createCurrencyColumn({
      headerName: "Beginning Balance",
      field: "beginningBalance",
      minWidth: 140
    }),
    createCurrencyColumn({
      headerName: "Earnings",
      field: "earnings",
      minWidth: 100
    }),
    createCurrencyColumn({
      headerName: "Contributions",
      field: "contributions",
      minWidth: 120
    }),
    createCurrencyColumn({
      headerName: "Forfeitures",
      field: "forfeitures",
      minWidth: 110
    }),
    createCurrencyColumn({
      headerName: "Distributions",
      field: "distributions",
      minWidth: 120
    }),
    createCurrencyColumn({
      headerName: "Ending Balance",
      field: "endingBalance",
      minWidth: 130
    }),
    createCurrencyColumn({
      headerName: "Vested Amount",
      field: "vestedAmount",
      minWidth: 130
    }),
    {
      headerName: "Vested %",
      field: "vestedPercent",
      colId: "vestedPercent",
      minWidth: 90,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: true,
      valueFormatter: (params) => params.value ? `${params.value}%` : "0%"
    },
    {
      headerName: "Termination Date",
      field: "terminationDate",
      colId: "terminationDate",
      minWidth: 130,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: true,
      valueFormatter: (params) => {
        if (params.value) {
          const date = new Date(params.value);
          return date.toLocaleDateString();
        }
        return "";
      }
    },
    createHoursColumn({
      headerName: "Profit Share Hours",
      field: "profitShareHours",
      minWidth: 130
    })
  ];
};