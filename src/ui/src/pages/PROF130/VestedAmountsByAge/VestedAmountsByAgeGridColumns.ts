import { agGridNumberToCurrency } from "smart-ui-library";
import { ColDef, ColGroupDef } from "ag-grid-community";

export const GetVestedAmountsByAgeColumns = (countColName: string, amountColName: string): (ColDef | ColGroupDef)[] => {
  return [
    {
      headerName: "Age",
      field: "age",
      minWidth: 80,
      type: "rightAligned",
      resizable: true,
      sort: "asc",
      cellDataType: "text"
    },
    {
      headerName: "Count",
      field: countColName,
      colId: countColName,
      minWidth: 120,
      type: "rightAligned",
      resizable: true
    },
    {
      headerName: "Amount",
      field: amountColName,
      colId: amountColName,
      minWidth: 120,
      type: "rightAligned",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    }
  ];
};
