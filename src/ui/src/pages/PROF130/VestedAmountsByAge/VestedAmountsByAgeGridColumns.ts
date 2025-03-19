import { agGridNumberToCurrency } from "smart-ui-library";
import { ColDef } from "ag-grid-community";

export const GetVestedAmountsByAgeColumns = (countColName: string, amountColName: string): ColDef[] => {
  return [
    {
      headerName: "Age",
      field: "age",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sort: "asc",
      cellDataType: "text"
    },
    {
      headerName: "Count",
      field: countColName,
      colId: countColName,
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Amount",
      field: amountColName,
      colId: amountColName,
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    }
  ];
};
