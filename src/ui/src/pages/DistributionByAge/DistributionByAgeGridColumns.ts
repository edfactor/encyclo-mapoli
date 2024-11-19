import { formattingUtils } from "smart-ui-library";
import { ProfitSharingDistributionsByAge } from "../../reduxstore/types";
import { ColDef } from "ag-grid-community";

export interface IDistributionByAgeGridColumns {
  headerName: string;
  pinnedRowDataTotal?: ColDef[];
  children?: ColDef[];
}

export const GetDistributionsByAgeColumns = (
  response: ProfitSharingDistributionsByAge
): IDistributionByAgeGridColumns => {
  return {
    headerName: response.reportType,
    pinnedRowDataTotal: [
      {
        headerName: "Employee Count",
        field: "hardshipTotalEmployees",
        colId: "distributionTotalAmount"
      }
    ],
    children: [
      {
        headerName: "Age",
        field: "age",
        colId: "age",
        minWidth: 80,
        headerClass: "right-align",
        cellClass: "right-align",
        resizable: true,
        sort: "asc"
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
