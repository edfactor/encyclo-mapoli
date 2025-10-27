import { ColDef } from "ag-grid-community";
import { FrozenReportsByAgeRequestType } from "../../../../reduxstore/types";
import { createAgeColumn, createCountColumn, createCurrencyColumn } from "../../../../utils/gridColumnFactory";

export const GetForfeituresByAgeColumns = (_reportType: FrozenReportsByAgeRequestType): ColDef[] => {
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
        minWidth: 100
      }),
      flex: 1
    } as ColDef,
    {
      ...createCurrencyColumn({
        headerName: "Amount",
        field: "amount",
        minWidth: 150
      }),
      flex: 2
    } as ColDef
  ];
};
