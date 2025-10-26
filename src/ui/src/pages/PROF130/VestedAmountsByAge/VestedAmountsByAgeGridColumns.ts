import { ColDef, ColGroupDef } from "ag-grid-community";
import { createAgeColumn, createCountColumn, createCurrencyColumn } from "../../../utils/gridColumnFactory";

export const GetVestedAmountsByAgeColumns = (countColName: string, amountColName: string): (ColDef | ColGroupDef)[] => {
  return [
    {
      ...createAgeColumn({}),
      flex: 1
    } as ColDef,
    {
      ...createCountColumn({
        field: countColName,
        headerName: "Count"
      }),
      flex: 1
    } as ColDef,
    {
      ...createCurrencyColumn({
        headerName: "Amount",
        field: amountColName
      }),
      flex: 2
    } as ColDef
  ];
};
