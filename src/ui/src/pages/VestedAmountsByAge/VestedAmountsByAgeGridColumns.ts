import { agGridNumberToCurrency } from "smart-ui-library";
import { ColDef } from "ag-grid-community";

export interface IVestingAmountsByAgeGridColumns {
  children?: ColDef[];
}

export const GetVestedAmountsByAgeColumns = (): IVestingAmountsByAgeGridColumns => {
  return {
    children: [
      {
        headerName: "Vested Balance",
        field: "vestedBalance",
        valueFormatter: agGridNumberToCurrency,
        resizable: true
      },
      {
        headerName: "Total Count",
        field: "employeeCount",
        resizable: true
      }
    ]
  };
};
