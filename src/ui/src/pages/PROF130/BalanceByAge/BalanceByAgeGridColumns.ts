import { ColDef, ColGroupDef } from "ag-grid-community";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";
import { createCurrencyColumn } from "../../../utils/gridColumnFactory";

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
        {
          headerName: "Count",
          field: "employeeCount",
          colId: "employeeCount",
          minWidth: 100,
          headerClass: "left-align",
          cellClass: "left-align",
          resizable: true
        },
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
