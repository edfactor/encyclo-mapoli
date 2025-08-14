import { ColDef } from "ag-grid-community";
import { formatNumberWithComma } from "smart-ui-library";
import { createCurrencyColumn } from "../../utils/gridColumnFactory";

export const GetQPAY600GridColumns = (navFunction: (destination: string) => void): ColDef[] => {
  return [
    {
      headerName: "Years of Service",
      field: "yearsOfService",
      colId: "yearsOfService",
      minWidth: 150,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: true,
      valueFormatter: (params) => {
        if (params.data?._isTotal) {
          return "TOTAL";
        }
        return params.value?.toString() || "";
      }
    },
    {
      headerName: "Employees",
      field: "employees",
      colId: "employees",
      minWidth: 120,
      type: "rightAligned",
      resizable: true,
      sortable: true,
      valueFormatter: (params) => formatNumberWithComma(params.value)
    },
    createCurrencyColumn({
      headerName: "Total Weekly Pay",
      field: "totalWeeklyPay",
      minWidth: 160
    }),
    createCurrencyColumn({
      headerName: "Last Year's Wages",
      field: "lastYearWages",
      minWidth: 160
    })
  ];
};