import { formattingUtils } from "smart-ui-library";
import { FrozenReportsByAgeRequestType } from "../../reduxstore/types";
import { ColDef } from "ag-grid-community";

export interface IContributionByAgeGridColumns {
  headerName: string;
  children?: ColDef[];
}

export const GetContributionsByAgeColumns = (  reportType: FrozenReportsByAgeRequestType): IContributionByAgeGridColumns => {
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
        valueFormatter: formattingUtils.agGridNumberToCurrency
      }
    ]
  };
};
