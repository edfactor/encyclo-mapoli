import { agGridNumberToCurrency } from "smart-ui-library";
import { FrozenReportsByAgeRequestType } from "../../reduxstore/types";
import { ColDef } from "ag-grid-community";

export interface IDistributionByAgeGridColumns {
  headerName: string;
  children?: ColDef[];
}

export const GetDistributionsByAgeColumns = (
  reportType: FrozenReportsByAgeRequestType
): IDistributionByAgeGridColumns => {
  return {
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
  };
};
