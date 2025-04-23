import { ColDef, ICellRendererParams } from "ag-grid-community";
import { agGridNumberToCurrency } from "smart-ui-library";

export const GetForfeituresAdjustmentColumns = (handleNavigation: (badgeNumber: string) => void): ColDef[] => {
  const getColDefForForfeiture = (): ColDef[] => {
    return [
      {
        headerName: "Client #",
        field: "clientNumber",
        width: 150
      },
      {
        headerName: "Badge #",
        field: "badgeNumber",
        width: 150
      },
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