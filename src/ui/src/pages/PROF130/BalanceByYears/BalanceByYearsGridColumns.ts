import { ColDef } from "ag-grid-community";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";
import { createCurrencyColumn, createCountColumn } from "../../../utils/gridColumnFactory";

export const GetBalanceByYearsGridColumns = (_: FrozenReportsByAgeRequestType): ColDef[] => {
  // Flattened columns (no header group) to avoid group header render issues
  return [
    createCountColumn({
      headerName: "Years",
      field: "years",
      minWidth: 80,
      sortable: false
    }),
    createCountColumn({
      headerName: "EMPS",
      field: "employeeCount",
      minWidth: 100,
      sortable: false
    }),
    createCurrencyColumn({
      headerName: "Balance",
      field: "currentBalance",
      minWidth: 150,
      sortable: false
    }),
    createCurrencyColumn({
      headerName: "Vested",
      field: "vestedBalance",
      minWidth: 150,
      sortable: false
    })
  ];
};
