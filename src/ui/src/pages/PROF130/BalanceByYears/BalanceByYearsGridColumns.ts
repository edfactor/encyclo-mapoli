import { ColDef, ColGroupDef } from "ag-grid-community";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";
import { createCurrencyColumn, createCountColumn } from "../../../utils/gridColumnFactory";

export const GetBalanceByYearsGridColumns = (reportType: FrozenReportsByAgeRequestType): (ColDef | ColGroupDef)[] => {
  const columns: (ColDef | ColGroupDef)[] = [
    {
      headerName: reportType,
      children: [
        createCountColumn({
          headerName: "Years",
          field: "years",
          minWidth: 80
        }),
        createCountColumn({
          field: "employeeCount",
          minWidth: 100
        }),
        createCurrencyColumn({
          headerName: "Balance",
          field: "currentBalance",
          minWidth: 150
        }),
        createCurrencyColumn({
          headerName: "Vested",
          field: "vestedBalance",
          minWidth: 150
        })
      ]
    }
  ];
  return columns;
};
