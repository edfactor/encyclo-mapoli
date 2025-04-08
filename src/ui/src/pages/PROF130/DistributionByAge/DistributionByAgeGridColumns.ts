import { agGridNumberToCurrency } from "smart-ui-library";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";
import { ColDef, ColGroupDef } from "ag-grid-community";

export const GetDistributionsByAgeColumns = (reportType: FrozenReportsByAgeRequestType): ColDef[] => {
  const columns: (ColDef | ColGroupDef)[] = [
    {
      headerName: reportType,
      children: [
        {
          headerName: "Age",
          field: "age",
          colId: "age",
          minWidth: 80,
          headerClass: "right-align",
          cellClass: "right-align",
          resizable: true,
          sort: "asc",
          cellDataType: "text"
        },
        {
          headerName: "EMPS",
          field: "regularEmployeeCount",
          colId: "regularEmployeeCount",
          minWidth: 100,
          headerClass: "right-align",
          cellClass: "right-align",
          resizable: true
        },
        {
          headerName: "Amount",
          field: "regularAmount",
          colId: "regularAmount",
          minWidth: 150,
          headerClass: "right-align",
          cellClass: "right-align",
          resizable: true,
          valueFormatter: agGridNumberToCurrency
        }
      ]
    }
  ];
  return columns;
};
