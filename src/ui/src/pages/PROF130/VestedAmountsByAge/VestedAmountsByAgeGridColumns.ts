import { ColDef, ColGroupDef } from "ag-grid-community";
import { createAgeColumn, createCountColumn, createCurrencyColumn } from "../../../utils/gridColumnFactory";

export const GetVestedAmountsByAgeColumns = (countColName: string, amountColName: string): (ColDef | ColGroupDef)[] => {
  return [
    createAgeColumn({}),
    createCountColumn({
      field: countColName
    }),
    createCurrencyColumn({
      headerName: "Amount",
      field: amountColName
    })
  ];
};
