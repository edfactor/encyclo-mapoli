import { ColDef, ICellRendererParams } from "ag-grid-community";
import { agGridNumberToCurrency } from "smart-ui-library";
import { createBadgeColumn } from "../../utils/gridColumnFactory";

export const GetForfeituresAdjustmentColumns = (): ColDef[] => {
  const getColDefForForfeiture = (): ColDef[] => {
    return [
      createBadgeColumn({ 
        headerName: "Badge",
        minWidth: 150
      }),
      {
        headerName: "Starting Balance",
        field: "startingBalance",
        width: 180,
        valueFormatter: agGridNumberToCurrency
      },
      {
        headerName: "Forfeiture Amount",
        field: "forfeitureAmount",
        width: 180,
        valueFormatter: agGridNumberToCurrency
      },
      {
        headerName: "Net Balance",
        field: "netBalance",
        width: 180,
        valueFormatter: agGridNumberToCurrency
      },
      {
        headerName: "Net Vested",
        field: "netVested",
        width: 180,
        valueFormatter: agGridNumberToCurrency
      }
    ];
  };

  return getColDefForForfeiture();
};
