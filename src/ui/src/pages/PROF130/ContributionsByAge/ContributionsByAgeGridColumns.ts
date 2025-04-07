import { agGridNumberToCurrency } from "smart-ui-library";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";
import { ColDef, ColGroupDef } from "ag-grid-community";

export const GetContributionsByAgeColumns = (reportType: FrozenReportsByAgeRequestType): ColDef[] => {
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
          field: "employeeCount",
          colId: "employeeCount",
          minWidth: 100,
          headerClass: "left-align",
          cellClass: "left-align",
          resizable: true
        },
        {
          headerName: "Amount",
          field: "amount",
          colId: "amount",
          minWidth: 150,
          headerClass: "left-align",
          cellClass: "left-align",
          resizable: true,
          valueFormatter: agGridNumberToCurrency
        }
      ]
    }
  ];
  return columns;
};
