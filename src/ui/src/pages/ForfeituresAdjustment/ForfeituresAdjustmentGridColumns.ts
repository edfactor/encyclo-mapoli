import { ColDef } from "ag-grid-community";
import { createBadgeColumn, createCurrencyColumn } from "../../utils/gridColumnFactory";

export const GetForfeituresAdjustmentColumns = (): ColDef[] => {
  const getColDefForForfeiture = (): ColDef[] => {
    return [
      createBadgeColumn({
        headerName: "Badge",
        minWidth: 150
      }),
      {
        ...createCurrencyColumn({
          headerName: "Starting Balance",
          field: "startingBalance"
        }),
        width: 180
      },
      {
        ...createCurrencyColumn({
          headerName: "Forfeiture Amount",
          field: "forfeitureAmount"
        }),
        width: 180
      },
      {
        ...createCurrencyColumn({
          headerName: "Net Balance",
          field: "netBalance"
        }),
        width: 180
      },
      {
        ...createCurrencyColumn({
          headerName: "Net Vested",
          field: "netVested"
        }),
        width: 180
      }
    ];
  };

  return getColDefForForfeiture();
};
