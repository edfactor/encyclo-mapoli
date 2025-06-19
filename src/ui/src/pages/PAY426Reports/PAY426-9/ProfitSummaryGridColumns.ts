import { agGridNumberToCurrency, formatNumberWithComma } from "smart-ui-library";
import { ColDef } from "ag-grid-community";

export const GetProfitSummaryGridColumns = (): ColDef[] => {
  return [
    {
      headerName: "Line Item",
      field: "lineItemTitle",
      colId: "lineItemTitle",
      minWidth: 400,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: (params) => {
        return params.data.lineItemPrefix ? `${params.data.lineItemPrefix}. ${params.value}` : params.value;
      }
    },
    {
      headerName: "EMPS",
      field: "numberOfMembers",
      colId: "numberOfMembers",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: (params) => formatNumberWithComma(params.value)
    },
    {
      headerName: "Total Wages",
      field: "totalWages",
      colId: "totalWages",
      minWidth: 180,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Total Balance",
      field: "totalBalance",
      colId: "totalBalance",
      minWidth: 180,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    }
  ];
}; 