import { ColDef, IRowNode } from "ag-grid-community";
import { createCountColumn, createCurrencyColumn } from "../../../utils/gridColumnFactory";
import { PayServicesDto } from "../../../types/reports/payservices";

export function GetAdhocProfLetter73GridColumns(): ColDef<PayServicesDto>[] {
  return [
    {
      headerName: "Years of Service",
      field: "yearsOfServiceLabel",
      sortable: true,
      filter: "agTextColumnFilter",
      comparator: (
        _valueA: string,
        _valueB: string,
        nodeA: IRowNode<PayServicesDto>,
        nodeB: IRowNode<PayServicesDto>
      ) => {
        // Custom comparator to sort by numeric yearsOfService instead of string label
        const numA = nodeA.data?.yearsOfService ?? 0;
        const numB = nodeB.data?.yearsOfService ?? 0;
        return numA - numB;
      }
    },
    createCountColumn({
      headerName: "Employees",
      field: "employees"
    }),
    createCurrencyColumn({
      headerName: "Total Weekly Pay",
      field: "weeklyPay"
    }),
    createCurrencyColumn({
      headerName: "Last Year's Wages",
      field: "yearsWages"
    })
  ];
}
