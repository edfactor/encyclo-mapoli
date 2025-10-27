import { ColDef } from "ag-grid-community";
import {
  createBadgeColumn,
  createCurrencyColumn,
  createDateColumn,
  createHoursColumn,
  createNameColumn,
  createPercentageColumn,
  createStoreColumn
} from "../../../utils/gridColumnFactory";

export const GetQPAY066BGridColumns = (): ColDef[] => {
  return [
    createStoreColumn({}),
    createBadgeColumn({}),
    createNameColumn({
      field: "fullName",
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
    createPercentageColumn({
      headerName: "Vested %",
      field: "vestedPercent"
    }),
    createDateColumn({
      headerName: "Termination Date",
      field: "terminationDate"
    }),
    createHoursColumn({
      headerName: "Profit Share Hours",
      field: "profitShareHours",
      minWidth: 130
    })
  ];
};
