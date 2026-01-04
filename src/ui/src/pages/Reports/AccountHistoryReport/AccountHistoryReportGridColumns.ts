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
    {
      headerName: "Vesting %",
      field: "vestingPercent",
      minWidth: 110,
      maxWidth: 110,
      sortable: true,
      filter: false,
      valueFormatter: (params) => {
        const value = params.value;
        if (value == null) return "N/A";
        return `${(value * 100).toFixed(0)}%`;
      },
      cellStyle: { textAlign: "right" }
    },
    {
      headerName: "Years",
      field: "yearsInPlan",
      minWidth: 80,
      maxWidth: 80,
      sortable: true,
      filter: false,
      valueFormatter: (params) => {
        const value = params.value;
        return value != null ? value.toString() : "N/A";
      },
      cellStyle: { textAlign: "right" }
    },
    createCurrencyColumn({
      headerName: "Withdrawals",
      field: "withdrawals",
      minWidth: 120
    }),
    createCurrencyColumn({
      headerName: "Vested Balance",
      field: "vestedBalance",
      minWidth: 130
    }),
    createCurrencyColumn({
      headerName: "Ending Balance",
      field: "endingBalance",
      minWidth: 130
    })
  ];
};
