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
      headerName: "Profit Year",
      field: "profitYear",
      colId: "profitYear",
      headerClass: "left-align",
      cellClass: "left-align",
      minWidth: 100,
      sortable: true,
      resizable: true
    },
    createCurrencyColumn({
      headerName: "Total Contributions",
      field: "totalContributions",
      minWidth: 150
    }),
    createCurrencyColumn({
      headerName: "Total Withdrawals",
      field: "totalWithdrawals",
      minWidth: 150
    }),
    createCurrencyColumn({
      headerName: "Total Distributions",
      field: "totalDistributions",
      minWidth: 150
    }),
    createCurrencyColumn({
      headerName: "Total Dividends",
      field: "totalDividends",
      minWidth: 150
    }),
    createCurrencyColumn({
      headerName: "Total Forfeitures",
      field: "totalForfeitures",
      minWidth: 150
    }),
    createCurrencyColumn({
      headerName: "Ending Balance",
      field: "endingBalance",
      minWidth: 150
    }),
    createCurrencyColumn({
      headerName: "Cumulative Contributions",
      field: "cumulativeContributions",
      minWidth: 180
    }),
    createCurrencyColumn({
      headerName: "Cumulative Withdrawals",
      field: "cumulativeWithdrawals",
      minWidth: 180
    }),
    createCurrencyColumn({
      headerName: "Cumulative Distributions",
      field: "cumulativeDistributions",
      minWidth: 180
    })
  ];
};
