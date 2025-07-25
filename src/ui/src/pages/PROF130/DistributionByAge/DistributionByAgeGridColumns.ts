import { ColDef, ColGroupDef } from "ag-grid-community";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";
import { agGridNumberToCurrency } from "smart-ui-library";

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
        {
          headerName: "Amount",
          field: "regularAmount",
          colId: "regularAmount",
          minWidth: 150,
          type: "rightAligned",
          resizable: true,
          valueFormatter: agGridNumberToCurrency
        }
      ]
    }
  ];
  return columns ?? [];
};
