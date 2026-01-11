import { ColDef } from "ag-grid-community";
import { createBadgeColumn, createCurrencyColumn, createYearColumn } from "../../../utils/gridColumnFactory";

export const GetAccountHistoryReportColumns = (): ColDef[] => {
  return [
    createBadgeColumn({}),
    createYearColumn({
      headerName: "Profit Year",
      field: "profitYear",
      alignment: "right",
      sortable: true,
      minWidth: 100,
      maxWidth: 120
    }),
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
    }),
    {
      headerName: "",
      field: "",
      sortable: false,
      filter: false,
      resizable: false,
      flex: 1
    }
  ];
};
