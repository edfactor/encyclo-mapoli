import { ColDef, ColGroupDef } from "ag-grid-community";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";
import { createCurrencyColumn, createCountColumn } from "../../../utils/gridColumnFactory";

export const GetBalanceByAgeColumns = (reportType: FrozenReportsByAgeRequestType): (ColDef | ColGroupDef)[] => {
  const columns: (ColDef | ColGroupDef)[] = [
    {
      headerName: reportType,
      children: [
        {
          headerName: "Age",
          field: "age",
          colId: "age",
          minWidth: 80,
          type: "rightAligned",
          resizable: true,
          sort: "asc",
          cellDataType: "text"
        },
        createCountColumn({
          field: "employeeCount",
          minWidth: 100,
          alignment: "left"
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
