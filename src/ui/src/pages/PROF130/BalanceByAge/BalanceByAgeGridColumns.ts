import { ColDef } from "ag-grid-community";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";
import { createCurrencyColumn, createCountColumn, createAgeColumn } from "../../../utils/gridColumnFactory";

export const GetBalanceByAgeColumns = (_reportType: FrozenReportsByAgeRequestType): ColDef[] => {
  // Flattened columns (no header group) to avoid group header render issues
  return [
    {
      ...createAgeColumn({
        headerName: "Age",
        field: "age",
        minWidth: 80
      }),
      flex: 1
    } as ColDef,
    {
      ...createCountColumn({
        headerName: "EMPS",
        field: "employeeCount",
        minWidth: 100,
        alignment: "left"
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
