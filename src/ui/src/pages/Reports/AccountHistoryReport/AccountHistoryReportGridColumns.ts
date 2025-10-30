import { ColDef } from "ag-grid-community";
import {
  createBadgeColumn,
  createCurrencyColumn,
  createNameColumn,
  createSSNColumn,
  createYearColumn
} from "../../../utils/gridColumnFactory";

export const GetAccountHistoryReportColumns = (): ColDef[] => {
  return [
    createBadgeColumn({}),
    createNameColumn({
      field: "fullName"
    }),
    createSSNColumn({ minWidth: 170, sortable: false }),
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
    })
  ];
};
