import { ColDef } from "ag-grid-community";
import { createCountColumn, createCurrencyColumn } from "../../utils/gridColumnFactory";

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
    createCountColumn({
      headerName: "Employees",
      field: "employees"
    }),
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
