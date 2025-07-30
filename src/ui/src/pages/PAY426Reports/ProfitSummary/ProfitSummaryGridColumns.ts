import { ColDef } from "ag-grid-community";
import { formatNumberWithComma } from "smart-ui-library";
import { createCurrencyColumn } from "../../../utils/gridColumnFactory";

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
      type: "rightAligned",
      resizable: true,
      valueFormatter: (params) => formatNumberWithComma(params.value)
    },
    createCurrencyColumn({
      headerName: "Total Wages",
      field: "totalWages",
      minWidth: 180
    }),
    createCurrencyColumn({
      headerName: "Total Balance",
      field: "totalBalance",
      minWidth: 180
    })
  ];
};
