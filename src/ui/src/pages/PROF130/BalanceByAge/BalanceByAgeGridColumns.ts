import { agGridNumberToCurrency } from "smart-ui-library";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";
import { ColDef } from "ag-grid-community";

export interface IForfeitureByAgeGridColumns {
  headerName: string;
  children?: ColDef[];
}

export const GetBalanceByAgeColumns = (  reportType: FrozenReportsByAgeRequestType): IForfeitureByAgeGridColumns => {
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
        headerName: "Count",
        field: "employeeCount",
        colId: "employeeCount",
        minWidth: 100,
        headerClass: "left-align",
        cellClass: "left-align",
        resizable: true
      },
      {
        headerName: "Balance",
        field: "currentBalance",
        colId: "currentBalance",
        minWidth: 150,
        headerClass: "left-align",
        cellClass: "left-align",
        resizable: true,
        valueFormatter: agGridNumberToCurrency
      },
      {
        headerName: "Vested",
        field: "vestedBalance",
        colId: "vestedBalance",
        minWidth: 150,
        headerClass: "left-align",
        cellClass: "left-align",
        resizable: true,
        valueFormatter: agGridNumberToCurrency
      }
    ]
  };
};
