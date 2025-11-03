import { ColDef, ICellRendererParams } from "ag-grid-community";
import { formatNumberWithComma, numberToCurrency } from "smart-ui-library";
import { createCurrencyColumn, createHoursColumn, createPointsColumn } from "../../../../utils/gridColumnFactory";

export const GetProfitSummaryGridColumns = (): ColDef[] => {
  return [
    {
      headerName: "Line Item",
      field: "lineItemTitle",
      colId: "lineItemTitle",
      minWidth: 400,
      headerClass: "left-align",
      cellClass: "left-align h-5 normal-case underline decoration-blue-600 !outline-none !border-none focus:outline-none focus:border-none",
      resizable: true,
      suppressCellFocus: true,
      valueFormatter: (params) => {
        return params.data.lineItemPrefix ? `${params.data.lineItemPrefix}. ${params.value}` : params.value;
      }
    },
    createPointsColumn({
      headerName: "EMPS",
      field: "numberOfMembers",
      valueFormatter: (params) => formatNumberWithComma(params.value)
    }),
    createCurrencyColumn({
      headerName: "Total Wages",
      field: "totalWages",
      minWidth: 180,
      valueFormatter: (params) => {
        if (typeof params.value === "string" && params.value.includes("X")) {
          return params.value;
        }
        return numberToCurrency(params.value);
      }
    }),
    createCurrencyColumn({
      headerName: "Total Balance",
      field: "totalBalance",
      minWidth: 180,
      valueFormatter: (params) => {
        if (typeof params.value === "string" && params.value.includes("X")) {
          return params.value;
        }
        return numberToCurrency(params.value);
      }
    }),
    createHoursColumn({
      headerName: "Hours",
      field: "totalHours",
      minWidth: 80,
      valueFormatter: (params) => formatNumberWithComma(params.value)
    }),
    createPointsColumn({
      headerName: "Points",
      field: "totalPoints",
      minWidth: 100,
      valueFormatter: (params) => formatNumberWithComma(params.value)
    })
  ];
};
