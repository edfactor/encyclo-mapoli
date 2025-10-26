import { ColDef } from "ag-grid-community";
import { createCountColumn, createCurrencyColumn, createPointsColumn } from "../../../utils/gridColumnFactory";

export const GetQPAY600GridColumns = (): ColDef[] => {
  return [
    createPointsColumn({
      headerName: "Years of Service",
      field: "yearsOfService",
      valueFormatter: (params) => {
        if (params.data?._isTotal) {
          return "TOTAL";
        }
        return params.value?.toString() || "";
      }
    }),
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
