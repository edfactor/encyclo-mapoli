import { ColDef } from "ag-grid-community";
import { createCountColumn, createCurrencyColumn, createPointsColumn } from "../../../utils/gridColumnFactory";

export const GetQPAY600GridColumns = (): ColDef[] => {
  return [
    {
      ...createPointsColumn({
        headerName: "Years of Service",
        field: "yearsOfServiceLabel",
        valueFormatter: (params) => {
          if (params.data?._isTotal) {
            return "TOTAL";
          }
          return params.value?.toString() || "";
        }
      }),
      comparator: (_valueA: string, _valueB: string, nodeA: any, nodeB: any) => {
        // Use the numeric yearsOfService for sorting instead of the label
        const numA = nodeA.data?.yearsOfService ?? 0;
        const numB = nodeB.data?.yearsOfService ?? 0;
        return numA - numB;
      }
    } as ColDef,
    createCountColumn({
      headerName: "Employees",
      field: "employees"
    }),
    createCurrencyColumn({
      headerName: "Total Weekly Pay",
      field: "weeklyPay",
      minWidth: 160
    }),
    createCurrencyColumn({
      headerName: "Last Year's Wages",
      field: "yearsWages",
      minWidth: 160
    })
  ];
};
