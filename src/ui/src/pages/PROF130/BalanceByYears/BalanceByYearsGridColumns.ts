import { ColDef } from "ag-grid-community";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";
import { createCurrencyColumn, createCountColumn } from "../../../utils/gridColumnFactory";

export const GetBalanceByYearsGridColumns = (_: FrozenReportsByAgeRequestType): ColDef[] => {
  // Flattened columns (no header group) to avoid group header render issues
  return [
    {
      ...createCountColumn({
        headerName: "Years",
        field: "years",
        minWidth: 80
      }),
      flex: 1
    } as ColDef,
    {
      ...createCountColumn({
        headerName: "EMPS",
        field: "employeeCount",
        minWidth: 100
      }),
      flex: 1
    } as ColDef,
    {
      ...createCurrencyColumn({
        headerName: "Balance",
        field: "currentBalance",
        minWidth: 150
      }),
      flex: 2
    } as ColDef,
    {
      ...createCurrencyColumn({
        headerName: "Vested",
        field: "vestedBalance",
        minWidth: 150
      }),
      flex: 2
    } as ColDef
  ];
};
