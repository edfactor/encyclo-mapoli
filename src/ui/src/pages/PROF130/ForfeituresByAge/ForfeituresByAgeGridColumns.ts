import { ColDef, ColGroupDef } from "ag-grid-community";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";
import { createCurrencyColumn, createCountColumn } from "../../../utils/gridColumnFactory";

export const GetForfeituresByAgeColumns = (reportType: FrozenReportsByAgeRequestType): (ColDef | ColGroupDef)[] => {
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
          headerName: "EMPS",
          field: "employeeCount",
          minWidth: 100
        }),
        createCurrencyColumn({
          headerName: "Amount",
          field: "amount",
          minWidth: 150
        })
      ]
    }
  ];
  return columns;
};
