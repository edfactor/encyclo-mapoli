import { ColDef, ColGroupDef } from "ag-grid-community";
import { createCurrencyColumn, createCountColumn } from "../../../utils/gridColumnFactory";

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
    createCountColumn({
      field: countColName,
      minWidth: 120
    }),
    createCurrencyColumn({
      headerName: "Amount",
      field: amountColName,
      colId: amountColName,
      minWidth: 120
    })
  ];
};
