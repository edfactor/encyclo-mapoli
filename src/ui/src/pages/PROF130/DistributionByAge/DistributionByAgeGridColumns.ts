import { ColDef, ColGroupDef } from "ag-grid-community";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";
import { createCurrencyColumn } from "../../../utils/gridColumnFactory";

export const GetDistributionsByAgeColumns = (reportType: FrozenReportsByAgeRequestType): (ColDef | ColGroupDef)[] => {
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
          headerName: "EMPS",
          field: "regularEmployeeCount",
          colId: "regularEmployeeCount",
          minWidth: 100,
          type: "rightAligned",
          resizable: true
        },
        createCurrencyColumn({
          headerName: "Amount",
          field: "regularAmount",
          minWidth: 150
        })
      ]
    }
  ];
  return columns ?? [];
};
