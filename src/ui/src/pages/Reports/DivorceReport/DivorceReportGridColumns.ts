import { ColDef } from "ag-grid-community";
import {
  createBadgeColumn,
  createCurrencyColumn,
  createNameColumn,
  createSSNColumn
} from "../../../utils/gridColumnFactory";

export const GetDivorceReportColumns = (): ColDef[] => {
  return [
    createBadgeColumn({}),
    createNameColumn({
      field: "fullName"
    }),
    createSSNColumn({ minWidth: 170, sortable: false }),
    {
      headerName: "Year",
      field: "profitYear",
      colId: "profitYear",
      headerClass: "left-align",
      cellClass: "left-align",
      minWidth: 80,
      sortable: true,
      resizable: true
    },
    createCurrencyColumn({
      headerName: "Contributions",
      field: "contributions",
      minWidth: 120
    }),
    createCurrencyColumn({
      headerName: "Earnings/(Loss)",
      field: "earnings",
      minWidth: 130
    }),
    createCurrencyColumn({
      headerName: "Forfeitures",
      field: "forfeitures",
      minWidth: 120
    }),
    createCurrencyColumn({
      headerName: "Withdrawals",
      field: "withdrawals",
      minWidth: 120
    }),
    createCurrencyColumn({
      headerName: "Ending Balance",
      field: "endingBalance",
      minWidth: 130
    })
  ];
};
